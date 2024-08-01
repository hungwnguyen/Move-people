using RootMotion.Dynamics;
using System;
using System.Collections;
using UnityEngine;

namespace HungwX
{
    public class CharController : MonoBehaviour
    {
        [SerializeField] private PuppetMaster puppetMaster = default;
        [SerializeField, Range(0, 1)] private float pinWeightDrag = 0.5f;
        [SerializeField, Range(0, 1)] private float pinWeightDrop = 0.38f;
        [SerializeField] private GameObject charModel = default;
        [SerializeField] private Transform charStartPosition = default, puppetStartPos = default, currentPosition = default, puppetPos = default;
        public static CharController Instance;
        public Action OnCharPosReset;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            GameManager.Instance.OnLevelReplay += ResetCharPos;
            MobileInputManager.Instance.OnPointerDownAction.AddListener(() => puppetMaster.pinWeight = pinWeightDrag);
            MobileInputManager.Instance.OnPointerUpAction.AddListener(() => puppetMaster.pinWeight = pinWeightDrop);
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnLevelReplay -= ResetCharPos;
        }

        private void ResetCharPos()
        {
            StartCoroutine(ResetCharPosCourountine());
        }

        IEnumerator ResetCharPosCourountine()
        {
            for (int i = 0; i < 4; i++)
            {
                charModel.SetActive(false);
                currentPosition.position = charStartPosition.position;
                currentPosition.rotation = charStartPosition.rotation;
                puppetPos.position = puppetStartPos.position;
                puppetPos.rotation = puppetStartPos.rotation;
                yield return new WaitForEndOfFrame();
                charModel.SetActive(true);
                puppetMaster.pinWeight = 1;
            }
        }
    }
}
