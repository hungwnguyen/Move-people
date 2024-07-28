namespace HungwX
{
    public class MouthStyling : AlternateStyling
    {
        public override float CalculateStylingScore(int i)
        {
            index = i;
            guidePoints[index].score = winZones[index].GetVerticalScore(guidePoints[index].guidePointPos.position, winThreshold);
            return base.CalculateStylingScore(i);
        }

        public void AddEventMouthLeftGuidePointPressed()
        {
            guidePointManager.OnMouthLeftGuidePointPressed += OnMouthGuidePointPressed;
        }

        public void AddEventMouthRightGuidePointPressed()
        {
            guidePointManager.OnMouthRightGuidePointPressed += OnMouthGuidePointPressed;
        }

        protected void OnMouthGuidePointPressed(int i)
        {
            BodyPart bodyPart = guidePoints[i].winZone.bodyPart;
            switch (guidePoints[i].targetDirectionX)
            {
                case TargetDirectionX.Left:
                    blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthLeft ? LeftIndex[1] : RightIndex[1], 0);
                    blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthLeft ? LeftIndex[0] : RightIndex[0], guidePoints[i].perCentWeightInWorld.x);
                    break;
                case TargetDirectionX.Right:
                    blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthLeft ? LeftIndex[0] : RightIndex[0], 0);
                    blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthLeft ? LeftIndex[1] : RightIndex[1], guidePoints[i].perCentWeightInWorld.x);
                    break;
            }
            switch (guidePoints[i].targetDirectionY)
            {
                case TargetDirectionY.Up:
                    blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthLeft ? LeftIndex[3] : RightIndex[3], 0);
                    blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthLeft ? LeftIndex[2] : RightIndex[2], guidePoints[i].perCentWeightInWorld.y);
                    break;
                case TargetDirectionY.Down:
                    blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthLeft ? LeftIndex[2] : RightIndex[2], 0);
                    blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.MouthLeft ? LeftIndex[3] : RightIndex[3], guidePoints[i].perCentWeightInWorld.y);
                    break;
            }
        }
    }
}
