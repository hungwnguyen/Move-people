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
        private bool isInBoneZone = false;
        private Vector3 startPos;
        [SerializeField] private GuidePointManager guidePointManager;

        private void Start()
        {
            twoBoneIK = GetComponent<TwoBoneIKConstraint>();
            root = twoBoneIK.data.root;
            tip = twoBoneIK.data.tip;
            startRangeY = tip.position.y - root.position.y;
            MobileInputManager.Instance.OnPointerMoveAction.AddListener(UpdateBoneRot);
            GameManager.Instance.OnLevelReplay += UpdateBoneRot;
            guidePointManager = GetComponentInParent<GuidePointManager>();
            guidePointManager.OnGuidePointDown += OnBonePress;
            startRangeY = tip.position.y - root.position.y;
        }
        private bool CheckBoneOutOfZone(int index)
        {
            return isInBoneZone;
        }

        void OnBonePress(int index)
        {
            if (index == transform.GetSiblingIndex())
            {
                guidePointManager.CheckScreenPoint = CheckBoneOutOfZone;
            }
            startPos = this.transform.position;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnLevelReplay -= UpdateBoneRot;
        }

        private void UpdateBoneRot()
        {
            float perCentY = tip.position.y - root.position.y;
            int direction = tip.position.z > root.position.z ? 1 : -1;
            if (perCentY > 0)
            {
                this.transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * 2);
                isInBoneZone = false;
            }
            else if (direction == -1)
            {
                this.transform.localRotation = Quaternion.Euler((startRangeY - perCentY) * rotateSpeed, 0, 0);
                isInBoneZone = true;
            }
            else
            {
                this.transform.localRotation = Quaternion.Euler(Vector3.zero);
                isInBoneZone = true;
            }
        }
    }
}
