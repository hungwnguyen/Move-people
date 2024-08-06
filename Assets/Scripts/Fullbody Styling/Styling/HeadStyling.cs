using SuperPack;
using UnityEngine;

namespace HungwX
{
    public class HeadStyling : AlternateStyling
    {
        Animator guidPoint;
       
        public override float CalculateStylingScore(int i)
        {
            if (guidPoint == null)
                guidPoint = guidePointManager.guidePointImages[i].GetComponent<Animator>();
            //gameManager.Score += 1f;
            gameManager.Score += 0.01f;
            SuperAnimator.SetRuntimeAnimationKeyFrame(guidPoint, "GuidePoint", gameManager.Score);
            if (CheckScoreTarget(1))
            {
                StylingComplete();
                gameManager.OnLevelComplete?.Invoke();
            }
            return gameManager.Score;
        }
    }
}