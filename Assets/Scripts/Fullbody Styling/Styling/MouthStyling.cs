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
    }
}
