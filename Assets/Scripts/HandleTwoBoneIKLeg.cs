using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

namespace HungwX
{
    [RequireComponent(typeof(TwoBoneIKConstraint))]
    [RequireComponent(typeof(GravityController))]
    public class HandleTwoBoneIKLeg : MonoBehaviour
    {
        private Transform root = default, tip = default;
        private TwoBoneIKConstraint twoBoneIK;
        [SerializeField] private float rotateSpeed = 100, minZ = 0.2f;
        private float startRangeY;

        private Coroutine rotationCoroutine;

        private void Start()
        {
            twoBoneIK = GetComponent<TwoBoneIKConstraint>();
            root = twoBoneIK.data.root;
            tip = twoBoneIK.data.tip;
            startRangeY = tip.position.y - root.position.y;
            MobileInputManager.Instance.OnPointerMoveAction.AddListener(UpdateBoneRot);
            MobileInputManager.Instance.OnPointerUpAction.AddListener(OnPointerUP);
            GameManager.Instance.OnLevelReplay += UpdateBoneRot;
        }

        void OnPointerUP()
        {
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
            }
            rotationCoroutine = StartCoroutine(SmoothRotate(Quaternion.Euler(Vector3.zero)));
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnLevelReplay -= UpdateBoneRot;
        }

        private void UpdateBoneRot()
        {
            float perCentY = tip.position.y - root.position.y;
            float perCentZ = tip.position.z - root.position.z;
            int direction = tip.position.z > root.position.z ? 1 : -1;

            Quaternion targetRotation;

            if (perCentY > 0)
            {
                if (direction != -1)
                {
                    targetRotation = Quaternion.Euler(-90, 0, 0);
                }
                else
                {
                    if (perCentZ * -1 > minZ)
                    {
                        targetRotation = Quaternion.Euler(90, 0, 0);
                    }
                    else
                    {
                        targetRotation = Quaternion.Euler(180, 0, 0);
                    }
                }
            }
            else if (direction == -1)
            {
                targetRotation = Quaternion.Euler((startRangeY - perCentY) * rotateSpeed, 0, 0);
            }
            else
            {
                if (perCentZ > minZ)
                {
                    targetRotation = Quaternion.Euler(-90, 0, 0);
                }
                else
                {
                    targetRotation = Quaternion.Euler(Vector3.zero);
                }
            }

            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
            }
            rotationCoroutine = StartCoroutine(SmoothRotate(targetRotation));
        }

        private IEnumerator SmoothRotate(Quaternion targetRotation)
        {
            Quaternion initialRotation = transform.localRotation;
            float elapsedTime = 0f;
            float duration = 0.2f; // Adjust duration to control how smooth the rotation is

            while (elapsedTime < duration)
            {
                transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localRotation = targetRotation;
        }
    }
}
