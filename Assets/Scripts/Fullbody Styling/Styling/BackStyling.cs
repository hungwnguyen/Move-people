using SuperPack;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace HungwX
{
    public class BackStyling : AlternateStyling
    {
        float time;
        bool isPress = false;
        bool isResset = false;
        BodyPart bodyPart;
        protected override void Start()
        {
            base.Start();
            guidePointManager.OnGuidePointDown += PlayAnimationPress;
            guidePointManager.OnGuidePointUp += PlayAnimationUp;
        }

        private void PlayAnimationUp(int i)
        {
            StopAllCoroutines();
            StartCoroutine(PlayAnimationResetWeightCoroutine(i));
        }

        private void PlayAnimationPress(int i)
        {
            isPress = true;
            isResset = true;
            guidePointManager.HandleGuidePointMove(i);
        }

        IEnumerator PlayAnimationResetWeightCoroutine(int i)
        {
            float time = 0.3f;
            bodyPart = guidePoints[i].winZone.bodyPart;
            if (bodyPart == BodyPart.BackLeft)
            {
                for (int index = 0; index < 10; index++)
                {
                    float weight = blendShapeHandle.GetBlendShapeWeight(LeftIndex[index]);
                    while(weight > 0)
                    {
                        weight -= Time.deltaTime / time;
                        blendShapeHandle.SetBlendShapeWeight(LeftIndex[index], weight);
                        yield return new WaitForEndOfFrame();
                    }
                    blendShapeHandle.SetBlendShapeWeight(LeftIndex[index], 0);
                }
            }
            else
            {
                for (int index = 0; index < 10; index++)
                {
                    float weight = blendShapeHandle.GetBlendShapeWeight(RightIndex[index]);
                    while (weight > 0)
                    {
                        weight -= Time.deltaTime / time;
                        blendShapeHandle.SetBlendShapeWeight(RightIndex[index], weight);
                        yield return new WaitForEndOfFrame();
                    }
                    blendShapeHandle.SetBlendShapeWeight(RightIndex[index], 0);
                }
            }
        }

        IEnumerator PlayAnimationPressCoroutine(int[] arr, int index, float weight)
        {
            float startWeight = blendShapeHandle.GetBlendShapeWeight(arr[index]);
            while(startWeight < weight)
            {
                startWeight += Time.deltaTime / 0.3f;
                blendShapeHandle.SetBlendShapeWeight(arr[index], startWeight);
                yield return new WaitForEndOfFrame();
            }
            blendShapeHandle.SetBlendShapeWeight(arr[index], weight);
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
            }
            else
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
            bodyPart = guidePoints[i].winZone.bodyPart;
            ResetBlendShapeWeight(bodyPart);
            float perCent = guidePoints[i].perCentWeightInWorld.z;
            int index = (int)perCent;
            if (perCent == 0) return;
            if (isPress)
            {
                isPress = false;
                if (index == 0)
                {
                    StartCoroutine(PlayAnimationPressCoroutine(bodyPart == BodyPart.BackLeft ? LeftIndex : RightIndex, index, perCent - index));
                }
                else if (index == 9)
                {
                    StartCoroutine(PlayAnimationPressCoroutine(bodyPart == BodyPart.BackLeft ? LeftIndex : RightIndex, index, 1 - perCent + index));
                }
                else
                {
                    StartCoroutine(PlayAnimationPressCoroutine(bodyPart == BodyPart.BackLeft ? LeftIndex : RightIndex, index + 1, perCent - index));
                    StartCoroutine(PlayAnimationPressCoroutine(bodyPart == BodyPart.BackLeft ? LeftIndex : RightIndex, index, 1 - perCent + index));
                }
            } else
            {
                if (isResset)
                {
                    isResset = false;
                    StopAllCoroutines();
                }
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
            
        }

        private void ResetBlendShapeWeight(BodyPart bodyPart)
        {
            if (bodyPart == BodyPart.BackLeft)
            {
                for (int index = 0; index < 10; index++)
                {
                    blendShapeHandle.SetBlendShapeWeight(LeftIndex[index], 0);
                }
            }
            else
            {
                for (int index = 0; index < 10; index++)
                {
                    blendShapeHandle.SetBlendShapeWeight(RightIndex[index], 0);
                }
            }
        }
    }
}
