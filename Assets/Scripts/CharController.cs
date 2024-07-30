using RootMotion.Dynamics;
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
        [SerializeField] private Transform charStartPosition = default, currentPosition = default, puppetPos = default;
        void Start()
        {
            GameManager.Instance.OnLevelReplay += ResetCharPos;
            MobileInputManager.Instance.OnPointerDownAction.AddListener(() => puppetMaster.pinWeight = pinWeightDrag);
            MobileInputManager.Instance.OnPointerUpAction.AddListener(() => puppetMaster.pinWeight = pinWeightDrop);
        }

        private void ResetCharPos()
        {
            StartCoroutine(ResetCharPosCourountine());
        }
        
        IEnumerator ResetCharPosCourountine()
        {
            charModel.SetActive(false);
            currentPosition.position = charStartPosition.position;
            currentPosition.rotation = charStartPosition.rotation;
            yield return new WaitForEndOfFrame();
            puppetPos.position = charStartPosition.position;
            puppetPos.rotation = charStartPosition.rotation;
            charModel.SetActive(true);
            puppetMaster.pinWeight = 1;
            charModel.SetActive(false);
            currentPosition.position = charStartPosition.position;
            currentPosition.rotation = charStartPosition.rotation;
            yield return new WaitForEndOfFrame();
            puppetPos.position = charStartPosition.position;
            puppetPos.rotation = charStartPosition.rotation;
            charModel.SetActive(true);
            puppetMaster.pinWeight = 1;
        }
    }
}
