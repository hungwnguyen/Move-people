using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HungwX
{
    public class MobileInputManager : MonoBehaviour
    {
        public static MobileInputManager Instance { get; private set; }
        public UnityEvent OnPointerDownAction, OnPointerUpAction, OnPointerMoveAction;
        [NonSerialized] public Vector2 touchPosition = Vector2.zero, touchStartPosition = Vector2.zero;
        [NonSerialized] public bool isPointerDown = false;

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
            OnPointerDownAction.AddListener(GetTouchPosition);
        }

        public void ResetMobileInput()
        {
            isPointerDown = false;
            OnPointerDownAction.RemoveAllListeners();
            OnPointerUpAction.RemoveAllListeners();
            OnPointerMoveAction.RemoveAllListeners();
            OnPointerDownAction.AddListener(GetTouchPosition);
        }

        private void GetTouchPosition()
        {
            if (!isPointerDown)
            {
                isPointerDown = true;
                touchStartPosition = GetTouchPositionFormInput();
                StartCoroutine(GetTouchPositionCoroutine());
            }
        }

        Vector2 GetTouchPositionFormInput()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                int touchCount = Input.touchCount;
                if (touchCount > 0)
                {
                    touchPosition = Input.GetTouch(0).position;
                }
#else
            touchPosition = Input.mousePosition;
#endif
            return touchPosition;
        }

        IEnumerator GetTouchPositionCoroutine()
        {
            while (isPointerDown)
            {
                if (GetTouchPositionFormInput() != touchStartPosition)
                {
                    OnPointerMoveAction?.Invoke();
                    touchStartPosition = touchPosition;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        public void OnPointerDown()
        {
            if (!isPointerDown)
            {
                OnPointerDownAction?.Invoke();
            }
        }

        public void OnPointerUp()
        {
            OnPointerUpAction?.Invoke();
        }
    }
}
