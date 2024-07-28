using UnityEngine;

namespace HungwX
{
    public class EyeStyling : AlternateStyling
    {
        [SerializeField] float threshold = 0.5f;

        protected void OnEyeGuidePointPressed(int i)
        {
            BodyPart bodyPart = guidePoints[i].winZone.bodyPart;
            float percent = guidePoints[i].perCentWeightInWorld.y;
            if (percent > threshold)
            {
                blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthRight ? RightIndex[3] : LeftIndex[3], 0);
                blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthRight ? RightIndex[2] : LeftIndex[2], percent - threshold);
            }
            else
            {
                blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthRight ? RightIndex[2] : LeftIndex[2], 0);
                blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthRight ? RightIndex[3] : LeftIndex[3], threshold - percent);
            }
        }

        public void AddEventMouthLeftGuidePointPressed()
        {
            guidePointManager.OnMouthLeftGuidePointPressed += OnEyeGuidePointPressed;
        }

        public void AddEventMouthRightGuidePointPressed()
        {
            guidePointManager.OnMouthRightGuidePointPressed += OnEyeGuidePointPressed;
        }
    }
}
