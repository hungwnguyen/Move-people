using RootMotion;
using RootMotion.Dynamics;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HungwX
{
    public class CharController : MonoBehaviour
    {
        [SerializeField] private PuppetMaster puppetMaster = default;
        [SerializeField, Range(0, 1)] private float pinWeightDrop = 0.32f;
        [SerializeField, Range(0, 1)] private float pinWeightDrag = 0.48f;
        [SerializeField] private Animator playerAnimator = default;
        private float speedPin = 6;
        [SerializeField] private GameObject charModel = default;
        [SerializeField] private Transform charStartPosition = default, puppetStartPos = default, currentPosition = default, puppetPos = default;
        public Action OnCharPosReset;
        private Coroutine pinWeightSmoothCoroutine;
        [NonSerialized] public Vector3 winPos;
        [SerializeField] UnityEvent winEvent = default, replayEvent = default;
        private GameManager manager;
        private UnityAction OnPointerDown, OnPointerUp;
        public static CharController instance;
        void OnEnable()
        {
            instance = this;
            manager = GameManager.Instance;
            manager.OnLevelFail += OnLevelFail;
            manager.OnLevelComplete += OnLevelComplete;
            manager.OnLevelReplay += ResetCharPos;
            manager.OnLevelReplay += AddEvent;
            AddEvent();
        }

        void OnDestroy()
        {
            manager.OnLevelFail -= OnLevelFail;
            manager.OnLevelComplete -= OnLevelComplete;
            manager.OnLevelReplay -= ResetCharPos;
            manager.OnLevelReplay -= AddEvent;
        }

        private void AddEvent()
        {
            replayEvent?.Invoke();
            OnPointerDown = OnPointerDownAction;
            OnPointerUp = OnPointerUpAction;
            MobileInputManager.Instance.OnPointerDownAction.AddListener(OnPointerDown);
            MobileInputManager.Instance.OnPointerUpAction.AddListener(OnPointerUp);
        }

        private void ResetEvent()
        {
            MobileInputManager.Instance.OnPointerDownAction.RemoveListener(OnPointerDown);
            MobileInputManager.Instance.OnPointerUpAction.RemoveListener(OnPointerUp);
            MobileInputManager.Instance.isPointerDown = false;
        }

        private void OnPointerDownAction()
        {
            pinWeightSmoothCoroutine = StartCoroutine(PinWeightSmooth());
        }

        private void OnPointerUpAction()
        {
            if (pinWeightSmoothCoroutine != null)
            {
                StopCoroutine(pinWeightSmoothCoroutine);
                puppetMaster.pinWeight = pinWeightDrop;
            }
        }

        private void OnLevelFail()
        {
            ResetEvent();
            puppetMaster.pinWeight = 0;
            playerAnimator.Play("Writhe");
            SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.Die);
        }

        private void OnLevelComplete()
        {
            MobileInputManager.Instance.ResetMobileInput();
            StopAllCoroutines();
            puppetMaster.pinWeight = 1;
            playerAnimator.Play("Victory");
            winEvent?.Invoke();
            puppetPos.position = winPos;
        }

        IEnumerator PinWeightSmooth()
        {
            puppetMaster.pinWeight = pinWeightDrop;
            while (puppetMaster.pinWeight < pinWeightDrag)
            {
                puppetMaster.pinWeight += Time.deltaTime / speedPin;
                yield return new WaitForEndOfFrame();
            }
            puppetMaster.pinWeight = pinWeightDrag;
        }

        public void ResetCharPos()
        {
            StartCoroutine(ResetCharPosCourountine());
        }

        IEnumerator ResetCharPosCourountine()
        {
            charModel.SetActive(false);
            puppetMaster.pinWeight = 1;
            currentPosition.position = charStartPosition.position;
            currentPosition.rotation = charStartPosition.rotation;
            puppetPos.position = puppetStartPos.position;
            puppetPos.rotation = puppetStartPos.rotation;
            yield return new WaitForEndOfFrame();
            charModel.SetActive(true);
            puppetMaster.pinWeight = pinWeightDrop;
        }
    }
}
