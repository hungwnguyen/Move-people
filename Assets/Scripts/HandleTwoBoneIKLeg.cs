using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace HungwX
{
    [RequireComponent(typeof(TwoBoneIKConstraint))]
    [RequireComponent(typeof(GravityController))]
    public class HandleTwoBoneIKLeg : MonoBehaviour
    {
        private Transform root = default, tip = default;
        private TwoBoneIKConstraint twoBoneIK;
        [SerializeField] private float rotateSpeed = 100;
        private float startRangeY;
        private Vector3 startPos;

        private void Start()
        {
            twoBoneIK = GetComponent<TwoBoneIKConstraint>();
            root = twoBoneIK.data.root;
            tip = twoBoneIK.data.tip;
            MobileInputManager.Instance.OnPointerDownAction.AddListener(OnBonePress);
            startRangeY = tip.position.y - root.position.y;
        }

        private void OnEnable()
        {
            MobileInputManager.Instance.OnPointerMoveAction.AddListener(UpdateBonePosAndRot);
        }

        void OnBonePress()
        {
            startPos = this.transform.position;
        }

        void UpdateBonePosAndRot()
        {
            float perCentY = tip.position.y - root.position.y;
            int direction = tip.position.z > root.position.z ? 1 : -1;
            if (perCentY > 0)
            {
            }
            else if (direction == -1)
            {
                this.transform.localRotation = Quaternion.Euler((startRangeY - perCentY) * rotateSpeed, 0, 0);
            }
            else
            {
                this.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }

        }
    }
}
