using SuperPack;
using System.Collections;
using UnityEngine;

namespace HungwX
{
    public class BackStyling : AlternateStyling
    {
        float time;
        protected override void Start()
        {
            base.Start();
            /*guidePointManager.OnGuidePointDown += PlayAnimationPress;
            guidePointManager.OnGuidePointUp += PlayAnimationUp;*/
        }

        /*private void PlayAnimationUp(int i)
        {
            StartCoroutine(PlayAnimationPressCoroutine(i, true));
        }

        private void PlayAnimationPress(int i)
        {
            StartCoroutine(PlayAnimationPressCoroutine(i));
        }*/

        IEnumerator PlayAnimationPressCoroutine(int i, bool isReverse = false)
        {
            float time = 0.15f + (isReverse ? -0.15f : 0.15f);
            BodyPart bodyPart = guidePoints[i].winZone.bodyPart;
            while (time < 0.3)
            {
                time += Time.deltaTime * (isReverse ? -1 : 1);
                blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.BackLeft ? LeftIndex[i] : RightIndex[i], time / 0.3f);
                yield return new WaitForSeconds(0.01f);
            }
        }

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
                    ResetBlendShapeWeight(BodyPart.BackLeft);
                    ResetBlendShapeWeight(BodyPart.BackRight);
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
        public void AddEventBackLeftGuidePointPressed()
        {
            guidePointManager.OnBackLeftGuidePointPressed += OnBackGuidePointPressed;
        }

        public void AddEventBackRightGuidePointPressed()
        {
            guidePointManager.OnBackRightGuidePointPressed += OnBackGuidePointPressed;
        }

        protected override void ResetGuidPointImage(int i)
        {
            base.ResetGuidPointImage(i);
            time = 0;
        }

        protected virtual void OnBackGuidePointPressed(int i)
        {
            BodyPart bodyPart = guidePoints[i].winZone.bodyPart;
            ResetBlendShapeWeight(bodyPart);
            float perCent = guidePoints[i].perCentWeightInWorld.z;
            int index = (int)perCent;
            if (perCent == 0) return;
            if (index == 0)
            {
                blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.BackLeft ? LeftIndex[index] : RightIndex[index], perCent - index);
            }
            else if (index == 9)
            {
                blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.BackLeft ? LeftIndex[index] : RightIndex[index], 1 - perCent + index);
            }
            else
            {
                blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.BackLeft ? LeftIndex[index + 1] : RightIndex[index + 1], perCent - index);
                blendShapeHandle.SetBlendShapeWeight(bodyPart == BodyPart.BackLeft ? LeftIndex[index] : RightIndex[index], 1 - perCent + index);
            }
        }

        private void ResetBlendShapeWeight(BodyPart bodyPart)
        {
            if (bodyPart == BodyPart.BackLeft)
            {
                for(int index = 0; index < 10; index++)
                {
                    blendShapeHandle.SetBlendShapeWeight(LeftIndex[index], 0);
                }
            } else
            {
                for (int index = 0; index < 10; index++)
                {
                    blendShapeHandle.SetBlendShapeWeight(RightIndex[index], 0);
                }
            }
        }
    }
}
