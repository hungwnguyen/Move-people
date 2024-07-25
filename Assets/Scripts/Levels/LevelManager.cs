using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HungwX
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onLevelComplete = default;
        [SerializeField] private UnityEvent OnLevelReplay = default;
        [SerializeField] private UnityEvent OnLevelLoadComplete = default;
        [SerializeField] private UnityEvent OnCallAffterSeconds = default;
        private GameManager gameManager;

        void Start()
        {
            gameManager = GameManager.Instance;
            gameManager.OnLevelCompleteEvent.AddListener(OnLevelComplete);
            gameManager.OnLevelReplay += OnLevelReplay.Invoke;
            OnLevelLoadComplete?.Invoke();
        }

        void OnDestroy()
        {
            gameManager.OnLevelCompleteEvent.RemoveListener(OnLevelComplete);
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
