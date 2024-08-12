using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Localization.Components;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System;
using System.Collections;

namespace HungwX
{
    public class LevelController : MonoBehaviour, ILevelController
    {
        [SerializeField] private List<LevelData> _levelDatas;
        [SerializeField] private LocalizeStringEvent titleContent = default;
        public List<LevelData> LevelDatas { get => _levelDatas; set => _levelDatas = value; }
        public GameObject InstanceReference { get; set; }
        public AssetReferenceGameObject prefabOfLevel { get; set; }
        public static LevelController Instance { get; set; }
        public Action OnLevelLoadComplete { get; set; }
        /// <summary>
        /// Call before OnLevelLoadComplete invoke
        /// </summary>
        public Action OnLoadNextLevel { get; set; }
        public int CurrentLevelID { get => _currentLevelID; set => _currentLevelID = value; }
        [SerializeField] private int _currentLevelID = -1;
        public LevelType levelType;
        private GameManager gameManager;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            if (CurrentLevelID == -1)
                CurrentLevelID = PlayerPrefs.GetInt("CurrentLevel", 1);
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            LoadLevel(CurrentLevelID);
        }

        public void LoadNextLevel()
        {
            CurrentLevelID++;
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevelID);
            LoadCurrentLevel();
        }

        public void LoadCurrentLevel()
        {
            SoundManager.DisableAllMusic();
            ReleaseLevel();
            gameManager.SetCurrentLevel();
            Camera.main.backgroundColor = _levelDatas[CurrentLevelID - 1 > 2 ? 2 : CurrentLevelID - 1].levelCameraColor;
            OnLoadNextLevel?.Invoke();
            if (levelType == LevelType.Regular)
            {
                prefabOfLevel = _levelDatas[CurrentLevelID - 1 > 2 ? 2 : CurrentLevelID - 1].prefabOfLevel;
                prefabOfLevel.InstantiateAsync().Completed += OnAddressableLoadComplete;
                titleContent.SetEntry(_levelDatas[CurrentLevelID - 1 > 2 ? 2 : CurrentLevelID - 1].titleContent);
            }
        }

        public void SetTitleContent(string value)
        {
            titleContent.SetEntry(value);
        }

        public void LoadLevel(int levelID)
        {
            CurrentLevelID = levelID;
            PlayerPrefs.SetInt("CurrentLevel", levelID);
            LoadCurrentLevel();
        }

        public void ReleaseLevel()
        {
            if (InstanceReference != null)
                prefabOfLevel.ReleaseInstance(InstanceReference);
        }

        public void OnAddressableLoadComplete(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                InstanceReference = handle.Result;
                OnLevelLoadComplete?.Invoke();
                PlayLevelMusic();
            }
            else
            {
                Debug.LogError("Failed to load level");
            }
        }

        public void PlayLevelMusic()
        {
            if (CurrentLevelID < 3)
            {
                SoundManager.CreatePlayFXLoop(SoundManager.Instance.audioClip.BgRegularMusic[CurrentLevelID - 1]);
            }
            else
            {
                SoundManager.CreatePlayFXLoop(SoundManager.Instance.audioClip.BgRegularMusic[2]);
            }
        }
    }
}
