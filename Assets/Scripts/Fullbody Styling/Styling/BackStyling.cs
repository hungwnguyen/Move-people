using SuperPack;
using UnityEngine;

namespace HungwX
{
    public class BackStyling : AlternateStyling
    {
        float time;

        public override float CalculateStylingScore(int i)
        {
            if (CheckInWinZone(i))
            {
                //gameManager.Score += 1f;
                gameManager.Score += 0.016f;
                SuperAnimator.SetRuntimeAnimationKeyFrame(guidePointManager.guidePointImages[i].GetComponent<Animator>(), "GuidePoint", gameManager.Score / 2);
                if (time > 1.5f)
                {
                    time = 0;
                    gameManager.SendMessagenger("level2Complain");
                }
                if (CheckScoreTarget(2))
                {
                    StylingComplete();
                    ResetBlendShapeWeight();
                    OnLevelComplete?.Invoke();
                }
            }
            time += Time.deltaTime;
           
            if (time > 0.6f && guidePoints[i].winZone.worldBottomLeft.z > guidePoints[i].guidePointPos.position.z)
            {
                time = 0;
                gameManager.SendMessagenger("touchbutt");
            }
            return gameManager.Score;
        }

        private bool CheckInWinZone(int i)
        {
            if (guidePointManager.spaceDimension == SpaceDimension.XZ)
            {
                return guidePoints[i].winZone.worldBottomLeft.z < guidePoints[i].guidePointPos.position.z
                    && guidePoints[i].winZone.worldTopRight.z > guidePoints[i].guidePointPos.position.z;
            } else
            {
                return false;
            }
        }
       
        protected override void ResetGuidPointImage(int i)
        {
            base.ResetGuidPointImage(i);
            time = 0;
        }
    }
}
