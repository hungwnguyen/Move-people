using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace HungwX
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; set; }
        private List<AlternateStyling> alternateStylings = new List<AlternateStyling>();
        public ProgressController progressController = null;
        public UnityEvent OnLevelCompleteEvent, OnLevelLoadCompleteEvent;
        public Action OnLevelReplay;
        public float Score { get; set; }
        public int NumberOfCompletedPoints { get; set; }
        private int numberOfStylings;
        [SerializeField] private Text currentLevel = default, nextLevel = default;
        private LevelController levelController;
        [SerializeField] private EventHandler popupHandler = default;
        [SerializeField]
        private LocalizeStringEvent message = default;
        [SerializeField] private EventTrigger eventTrigger = default;

        void Awake()
        {
            Application.targetFrameRate = 60;
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            OnLevelCompleteEvent.AddListener(PlayMusicWin);
        }

        void Start()
        {
            levelController = LevelController.Instance;
            if (levelController == null)
            {
                Debug.LogWarning("LevelController is missing in the scene");
                return;
            }
            levelController.OnLevelLoadComplete = OnLevelLoadComplete;
            SetCurrentLevel();
        }

        public void SendMessagenger(string value = "null")
        {
            message.SetEntry(value);
            StartCoroutine(ResetMessage());
        }

        IEnumerator ResetMessage()
        {
            yield return new WaitForSeconds(2);
            message.SetEntry("null");
        }

        public void ResetGame()
        {
            levelController.LoadLevel(1);
            Score = 0;
            OnLevelLoadCompleteEvent?.Invoke();
        }

        public void SetMobileInput(bool value)
        {
            eventTrigger.enabled = value;
        }

        public void PlayUISound()
        {
            SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.click);
        }

        public void SetCurrentLevel()
        {
            currentLevel.text = levelController.CurrentLevelID.ToString();
            nextLevel.text = (levelController.CurrentLevelID + 1).ToString();
        }

        public void AddNewAlternateStyling(AlternateStyling alternateStyling)
        {
            alternateStylings.Add(alternateStyling);
        }

        public void ClearAlternateStyling()
        {
            alternateStylings.Clear();
        }

        private void OnLevelLoadComplete()
        {
            OnLevelLoadCompleteEvent?.Invoke();
            NumberOfCompletedPoints = 0;
            alternateStylings = new List<AlternateStyling>();
            Score = 0;
            progressController.UpdatePosition(0);
        }

        public void UpdateNumberOfCompletedPoints()
        {
            NumberOfCompletedPoints++;
            if (NumberOfCompletedPoints == numberOfStylings)
            {
                OnLevelCompleteEvent?.Invoke();
            }
        }

        private void PlayMusicWin()
        {
            SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.Levelcomplete2);

            switch (levelController.CurrentLevelID)
            {
                case 1:
                    SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.Happy1);
                    break;
                case 2:
                    SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.Happy2);
                    break;
                default:
                    SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.Happy4);
                    break;
            }
        }

        public void UpdateScore(int index)
        {
            float score = 0;
            int count = 0;
            foreach (var styling in alternateStylings)
            {
                if (!styling.isStylingComplete)
                {
                    score = styling.CalculateStylingScore(index);
                    count = styling.winZones.Count;
                }
            }
            numberOfStylings = count;
            score /= numberOfStylings;
            progressController.UpdatePosition(score);
        }
        public void ResetLevel()
        {
            Score = 0;
            popupHandler.OnAnimationFinished = ResetCurrentLevel;
            eventTrigger.enabled = true;
        }

        public void RePlayLevel()
        {
            Score = 0;
            popupHandler.OnAnimationFinished = levelController.LoadCurrentLevel;
            eventTrigger.enabled = true;
            OnLevelLoadCompleteEvent?.Invoke();
        }

        public void ResetCurrentLevel()
        {
            NumberOfCompletedPoints = 0;
            OnLevelReplay?.Invoke();
            StartCoroutine(Reset());
            foreach (var styling in alternateStylings)
            {
                if (!styling.isStylingComplete)
                {
                    styling.ResetAlternateStyling();
                }
            }
        }

        IEnumerator Reset()
        {
            yield return new WaitForSeconds(0.2f);
            OnLevelLoadCompleteEvent?.Invoke();
        }

        public void LoadNextLevel()
        {
            Score = 0;
            popupHandler.OnAnimationFinished = levelController.LoadNextLevel;
        }
    }
}
