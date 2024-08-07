using System;
using System.Collections.Generic;
using System.Collections;
using SuperPack;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace HungwX
{
    public abstract class AlternateStyling : MonoBehaviour
    {
        [SerializeField] protected GuidePointManager guidePointManager;
        [SerializeField] protected BlendShapeHandle blendShapeHandle;
        [SerializeField, Space(5f), Tooltip("Element 0 is Left, 1 is Right, 2 is Up, 3 is Down"), Space(5f)]
        protected int[] LeftIndex = new int[4] { 0, 0, 0, 0 }, RightIndex = new int[4] { 0, 0, 0, 0 };
        [SerializeField, Range(0, 1)] protected float stylingTargetScore;
        [SerializeField, Tooltip("Speed pointer move")] float speedMove = 5;
        protected int index;
        protected List<GuidePoint> guidePoints;
        [NonSerialized] public List<WinZone> winZones = new List<WinZone>();
        [SerializeField, Tooltip("to make winzone larger or smaller")] protected float winThreshold = 1.5f;
        [SerializeField] protected UnityEvent OnStylingAwake = default, OnStylingStart = default;
        protected GameManager gameManager;
        [NonSerialized]
        public bool isStylingComplete = false;
        public UnityEvent OnLevelComplete;

        protected virtual void Awake()
        {
            guidePoints = guidePointManager.guidePoints;
            foreach (GuidePoint guidePoint in guidePoints)
            {
                winZones.Add(guidePoint.guidePointPos.GetComponent<WinZone>());
            }
            OnStylingAwake?.Invoke();
        }

        protected bool CheckScoreTarget(float value)
        {
            if (gameManager.Score >= value)
            {
                return true;
            }
            return false;
        }

        protected virtual void OnEnable()
        {
            StartCoroutine(StartStyling());
        }

        IEnumerator StartStyling()
        {
            yield return new WaitForEndOfFrame();
            gameManager = GameManager.Instance;
            OnStylingStart?.Invoke();
        }

        public void AddNewAlternateStyling()
        {
            gameManager.AddNewAlternateStyling(this);
        }

        public void AddEventCheckOnPointerUpAction()
        {
            guidePointManager.OnGuidePointUp += CheckStyling;
        }

        public void AddEventResetGuidPointOnPointerUp()
        {
            guidePointManager.OnGuidePointUp += ResetGuidPointImage;
        }

        public virtual void ResetAlternateStyling()
        {
            index = 0;
            guidePointManager.numberOfPointsSFX = 0;
            gameManager.progressController.UpdatePosition(0);

            foreach (GuidePoint guidePoint in guidePoints)
            {
                guidePoint.score = 0;
                guidePoint.isMoveToWinPoint = false;
                guidePoint.guidePointPos.position = guidePoint.startPos;
            }
            for (int i = 0; i < guidePoints.Count; i++)
            {
                ResetGuidPointImage(guidePointManager.guidePointImages[i]);
                guidePointManager.guidePointImages[i].transform.position = Camera.main.WorldToScreenPoint(guidePoints[i].guidePointPos.position);
                guidePointManager.HandleGuidePointMove(i);
            }
            StartCoroutine(ResetAnimation());
        }

        protected IEnumerator ResetAnimation()
        {
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;
                for (int i = 0; i < guidePoints.Count; i++)
                {
                    if (guidePointManager.guidePointImages == null)
                    {
                        yield return new WaitForEndOfFrame();
                        yield return new WaitForEndOfFrame();
                    }
                    /*print(guidePointManager.guidePointImages[i].transform.position);
                    print(guidePoints[i].guidePointPos.position);*/
                    guidePointManager.guidePointImages[i].transform.position = Camera.main.WorldToScreenPoint(guidePoints[i].guidePointPos.position);
                }
                if (time >= 2)
                {
                    yield break;
                }
                yield return new WaitForSeconds(0.01f);
            }
        }

        protected void ResetGuidPointImage(GameObject guidPointImage)
        {
            guidPointImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            guidPointImage.GetComponent<Animator>().speed = 1;
            guidPointImage.GetComponent<Animator>().Play("Normal");
        }

        protected void CheckStyling(int i)
        {
            if (guidePoints[i].isMoveToWinPoint)
                StartCoroutine(PlayAnimationMoveToWinPoint(i));
            else
            {
                ResetGuidPointImage(i);
            }
        }

        protected virtual void ResetGuidPointImage(int i)
        {
            ResetGuidPointImage(guidePointManager.guidePointImages[i]);
        }
       
        IEnumerator PlayAnimationMoveToWinPoint(int i)
        {
            Image image = guidePointManager.guidePointImages[i].GetComponent<Image>();
            float maxDistance = Vector3.Distance(guidePoints[i].guidePointPos.position, winZones[i].winPos);
            while (i != -1)
            {
                if (guidePoints[i == -1 ? 0 : i].guidePointPos.position == winZones[i == -1 ? 0 : i].winPos)
                {
                    break;
                }
                image.color = new Color(0, 1, 0, Vector3.Distance(guidePoints[i].guidePointPos.position, winZones[i].winPos) / maxDistance);
                guidePointManager.SetGuidePointPosition(Vector3.MoveTowards(guidePoints[i == -1 ? 0 : i].guidePointPos.position, winZones[i == -1 ? 0 : i].winPos, speedMove * Time.deltaTime), i);
                gameManager.UpdateScore(i);
                yield return new WaitForSeconds(0.01f);
            }
            guidePointManager.SetGuidePointPosition(winZones[i == -1 ? 0 : i].winPos, i);
            image.color = new Color(0, 1, 0, 0);
            gameManager.UpdateNumberOfCompletedPoints();
        }

        public virtual float CalculateStylingScore(int i)
        {
            if (guidePoints[i].score > stylingTargetScore)
            {
                guidePoints[i].isMoveToWinPoint = true;
            }
            else
            {
                guidePoints[i].isMoveToWinPoint = false;
            }
            SuperAnimator.SetRuntimeAnimationKeyFrame(guidePointManager.guidePointImages[i].GetComponent<Animator>(), "GuidePoint", guidePoints[i].score);
            float scrore = 0;
            foreach (GuidePoint guidePoint in guidePoints)
            {
                scrore += guidePoint.score;
            }
            return scrore;
        }

        protected void StylingComplete()
        {
            gameManager.SetMobileInput(false);
            isStylingComplete = true;
            guidePointManager.ClearGuidePointImages();
            gameManager.Score = 0;
            gameManager.progressController.UpdatePosition(0);
            MobileInputManager.Instance.ResetMobileInput();
            guidePointManager.numberOfPointsSFX = 0;
        }
    }
}
