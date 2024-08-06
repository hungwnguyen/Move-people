using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HungwX
{
    public class LevelManager : MonoBehaviour
    {
        [Space(5f),SerializeField, Tooltip("Call when level completed!"), Header("Call when level completed!"), Space(10f)] private UnityEvent _onLevelComplete = default;
        [SerializeField] private UnityEvent OnLevelReplay = default;
        [SerializeField] private UnityEvent OnLevelLoadComplete = default;
        [SerializeField] private UnityEvent OnCallAffterSeconds = default;
        public Action OnGameOverAction;
        public Action<GameObject, Vector3> OnHitLazer;
        private GameManager gameManager;
        public static LevelManager instance;

        void Start()
        {
            instance = this;
            gameManager = GameManager.Instance;
            gameManager.OnLevelComplete += OnLevelComplete;
            gameManager.OnLevelReplay += OnLevelReplay.Invoke;
            OnLevelLoadComplete?.Invoke();
        }

        void OnDestroy()
        {
            gameManager.OnLevelComplete -= OnLevelComplete;
            gameManager.OnLevelReplay -= OnLevelReplay.Invoke;
        }

        public void OnGameOver()
        {
            OnGameOverAction?.Invoke();
        }

        void OnLevelComplete()
        {
            _onLevelComplete?.Invoke();
        }

        public void CallAffterSeconds(float time)
        {
            StartCoroutine(CallAffterTwoSecondsCoroutine(time));
        }

        public void SetTitleContent(string value)
        {
            LevelController.Instance.SetTitleContent(value);
        }

        private IEnumerator CallAffterTwoSecondsCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            OnCallAffterSeconds?.Invoke();
        }

        public void SendMessagenger(string value)
        {
            gameManager.SendMessagenger(value);
        }
    }
}
