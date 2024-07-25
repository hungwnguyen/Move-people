using SuperPack;
using UnityEngine;

namespace HungwX
{
    public class HeadStyling : AlternateStyling
    {
        public override float CalculateStylingScore(int i)
        {
            //gameManager.Score += 1f;
            gameManager.Score += 0.016f;
            SuperAnimator.SetRuntimeAnimationKeyFrame(guidePointManager.guidePointImages[i].GetComponent<Animator>(), "GuidePoint", gameManager.Score / 2);
            if (CheckScoreTarget(1))
            {
                StylingComplete();
                gameManager.OnLevelCompleteEvent?.Invoke();

            }
            return gameManager.Score;
        }
    }
}