using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using MoreMountains.NiceVibrations;
namespace HungwX
{
    public enum SpaceDimension
    {
        XY,
        XZ,
        YZ
    }
    public delegate void EventPress(int i);

    /// <summary>
    /// GuidePointManager can only be used while this tranform.rotattion is 0,0,0
    /// </summary>
    public class GuidePointManager : MonoBehaviour
    {
        [SerializeField] private GameObject guidePointImage = null;
        [SerializeField] private float moveSpeed = 186, radiusTouch = 86;
        public SpaceDimension spaceDimension = SpaceDimension.XY;
        [SerializeField]
        public List<GuidePoint> guidePoints;
        private Transform parentOfGuidePoints;
        [NonSerialized] public List<GameObject> guidePointImages;
        private Camera mainCamera;
        private int targetIndexPressed;
        private MobileInputManager mobileInputManager;
        private int countVibration = 0;
        public EventPress OnMouthLeftGuidePointPressed,
            OnMouthRightGuidePointPressed,
            OnEyeLeftGuidePointPressed,
            OnEyeRightGuidePointPressed,
            OnPointerUpAction,
            OnBackRightGuidePointPressed,
            OnBackLeftGuidePointPressed;

        IEnumerator Start()
        {
            for (int i = 0; i < guidePoints.Count; i++)
            {
                Destroy(guidePoints[i].guidePointPos.GetComponent<BoxCollider>());
                guidePoints[i].winZone = guidePoints[i].guidePointPos.gameObject.GetComponent<WinZone>(); ;
            }
            mobileInputManager = MobileInputManager.Instance;
            this.parentOfGuidePoints = mobileInputManager.transform;
            mainCamera = Camera.main;
            yield return new WaitForEndOfFrame();
            CreateGuidePointImages();
            mobileInputManager.OnPointerMoveAction.AddListener(OnPointerMove);
            mobileInputManager.OnPointerDownAction.AddListener(OnPointerDown);
            mobileInputManager.OnPointerUpAction.AddListener(OnPointerUp);
            GameManager.Instance.SetMobileInput(true);
        }

        void OnDestroy()
        {
            ClearGuidePointImages();
        }

        public void ClearGuidePointImages()
        {
            foreach (GameObject guidePoint in guidePointImages)
            {
                Destroy(guidePoint);
            }
            guidePointImages.Clear();
        }

        private void CreateGuidePointImages()
        {
            guidePointImages = new List<GameObject>();
            for (int i = 0; i < guidePoints.Count; i++)
            {
                GameObject guidePoint = Instantiate(guidePointImage, parentOfGuidePoints);
                guidePointImages.Add(guidePoint);
                guidePointImages[i].transform.position = mainCamera.WorldToScreenPoint(guidePoints[i].guidePointPos.position);
                HandleGuidePointMove(i);
            }
        }

        public void SetGuidePointPosition(Vector3 position, int index)
        {
            guidePoints[index].guidePointPos.position = position;
            guidePointImages[index].transform.position = mainCamera.WorldToScreenPoint(guidePoints[index].guidePointPos.position);
            HandleGuidePointMove(index);
        }

        private void OnPointerUp()
        {
            if (targetIndexPressed != -1)
                OnPointerUpAction?.Invoke(targetIndexPressed);
            mobileInputManager.isPointerDown = false;
        }

        private void OnPointerDown()
        {
            float minDistance = float.MaxValue;
            targetIndexPressed = -1;
            for (int i = 0; i < guidePointImages.Count; i++)
            {
                float distance = Vector2.Distance(mobileInputManager.touchPosition, mainCamera.WorldToScreenPoint(guidePoints[i].guidePointPos.position));

                if (minDistance > distance && guidePoints[i].isMoveToWinPoint == false && guidePoints[i].CheckScreenPointIsInWinZone(mobileInputManager.touchPosition, radiusTouch))
                {
                    minDistance = distance;
                    targetIndexPressed = i;
                }
            }
            if (targetIndexPressed == -1) return;
            Animator targetAnimator = guidePointImages[targetIndexPressed].GetComponent<Animator>();
            targetAnimator.speed = 1;
            targetAnimator.Play("Pressed");
        }

        private void OnPointerMove()
        {
            if (targetIndexPressed == -1) return;
            // Get the current and start positions in world space
            Vector3 startWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mobileInputManager.touchStartPosition.x, mobileInputManager.touchStartPosition.y, mainCamera.nearClipPlane));
            Vector3 currentWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mobileInputManager.touchPosition.x, mobileInputManager.touchPosition.y, mainCamera.nearClipPlane));

            // Calculate the displacement in world space
            Vector3 displacement = (currentWorldPosition - startWorldPosition) * moveSpeed;

            // Calculate the new position based on the displacement
            Vector3 newWorldPosition = guidePoints[targetIndexPressed].guidePointPos.position + displacement;

            switch (spaceDimension)
            {
                case SpaceDimension.XY:
                    XYAxisMove(newWorldPosition);
                    break;
                case SpaceDimension.XZ:
                    XZAxisMove(newWorldPosition);
                    break;
                case SpaceDimension.YZ:
                    break;
            }

            // Update the guide point image position to reflect the new position
            guidePointImages[targetIndexPressed].transform.position = mainCamera.WorldToScreenPoint(guidePoints[targetIndexPressed].guidePointPos.position);
            countVibration++;
            if (countVibration == 7)
            {
                MMVibrationManager.Haptic(HapticTypes.Selection, false, true, this);
                countVibration = 0;
            }
            HandleGuidePointMove(targetIndexPressed);
            GameManager.Instance.UpdateScore(targetIndexPressed);
        }

        private void XZAxisMove(Vector3 newWorldPosition)
        {
            // Clamp the new position within the allowed bounds
            float clampedX = Clamp(newWorldPosition.x, guidePoints[targetIndexPressed].minPosInWorld.x, guidePoints[targetIndexPressed].maxPosInWorld.x);
            float clampedZ = Clamp(newWorldPosition.z, guidePoints[targetIndexPressed].minPosInWorld.z, guidePoints[targetIndexPressed].maxPosInWorld.z);

            // Update the position of the guide point
            guidePoints[targetIndexPressed].guidePointPos.position = new Vector3(clampedX, guidePoints[targetIndexPressed].guidePointPos.position.y, clampedZ);
        }

        private void XYAxisMove(Vector3 newWorldPosition)
        {
            // Clamp the new position within the allowed bounds
            float clampedX = Clamp(newWorldPosition.x, guidePoints[targetIndexPressed].minPosInWorld.x, guidePoints[targetIndexPressed].maxPosInWorld.x);
            float clampedY = Clamp(newWorldPosition.y, guidePoints[targetIndexPressed].minPosInWorld.y, guidePoints[targetIndexPressed].maxPosInWorld.y);

            // Update the position of the guide point
            guidePoints[targetIndexPressed].guidePointPos.position = new Vector3(clampedX, clampedY, guidePoints[targetIndexPressed].guidePointPos.position.z);

        }

        public float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
            if (value > max)
                return max;
            return value;
        }

        public GuidePoint GetCurrentGuidePoint()
        {
            return guidePoints[targetIndexPressed];
        }

        private void CalculateMouthPercentWeith(int index)
        {
            Vector3 currentGuidePointPos = guidePoints[index].guidePointPos.position;
            if (currentGuidePointPos.x > guidePoints[index].centerPosInWorld.x)
            {
                guidePoints[index].targetDirectionX = TargetDirectionX.Right;
                guidePoints[index].perCentWeightInWorld.x = (currentGuidePointPos.x - guidePoints[index].centerPosInWorld.x) / guidePoints[index].distanceInWorld.x;
            }
            else
            {
                guidePoints[index].targetDirectionX = TargetDirectionX.Left;
                guidePoints[index].perCentWeightInWorld.x = (guidePoints[index].centerPosInWorld.x - currentGuidePointPos.x) / guidePoints[index].distanceInWorld.x;
            }
            if (currentGuidePointPos.y > guidePoints[index].centerPosInWorld.y)
            {
                guidePoints[index].targetDirectionY = TargetDirectionY.Up;
                guidePoints[index].perCentWeightInWorld.y = (currentGuidePointPos.y - guidePoints[index].centerPosInWorld.y) / guidePoints[index].distanceInWorld.y;
            }
            else
            {
                guidePoints[index].targetDirectionY = TargetDirectionY.Down;
                guidePoints[index].perCentWeightInWorld.y = (guidePoints[index].centerPosInWorld.y - currentGuidePointPos.y) / guidePoints[index].distanceInWorld.y;
            }
        }

        public void CalculateBackPercentWeith(int index)
        {
            Vector3 currentGuidePointPos = guidePoints[index].guidePointPos.position;
            Vector3 worldTopRight = guidePoints[index].winZone.worldTopRight;
            Vector3 worldBottomLeft = guidePoints[index].winZone.worldBottomLeft;
            if (currentGuidePointPos.z > worldBottomLeft.z && currentGuidePointPos.z < worldTopRight.z)
            {
                guidePoints[index].perCentWeightInWorld.z = (currentGuidePointPos.z - worldBottomLeft.z) / (worldTopRight.z - worldBottomLeft.z) * 10;
            }
            else
            {
                guidePoints[index].perCentWeightInWorld.z = 0;
            }
        }

        public void HandleGuidePointMove(int index)
        {
            switch (guidePoints[index].winZone.bodyPart)
            {
                case BodyPart.MouthLeft:
                    CalculateMouthPercentWeith(index);
                    OnMouthLeftGuidePointPressed?.Invoke(index);
                    break;
                case BodyPart.MouthRight:
                    CalculateMouthPercentWeith(index);
                    OnMouthRightGuidePointPressed?.Invoke(index);
                    break;
                case BodyPart.EyeLeft:
                    OnEyeLeftGuidePointPressed?.Invoke(index);
                    break;
                case BodyPart.EyeRight:
                    OnEyeRightGuidePointPressed?.Invoke(index);
                    break;
                case BodyPart.BackLeft:
                    CalculateBackPercentWeith(index);
                    OnBackLeftGuidePointPressed?.Invoke(index);
                    break;
                case BodyPart.BackRight:
                    CalculateBackPercentWeith(index);
                    OnBackRightGuidePointPressed?.Invoke(index);
                    break;
            }
        }

        public void SetUpGuidePoint()
        {
            this.gameObject.SetActive(!this.gameObject.activeSelf);
            foreach (GuidePoint guidePoint in guidePoints)
            {
                guidePoint.winZone = guidePoint.guidePointPos.GetComponent<WinZone>();
                guidePoint.winZone.SetUpWinZone();
                guidePoint.SetUpGuidePoint();
                guidePoint.winZone.guidePointManager = this;
            }
        }

        public void OnDrawGizmosSelected()
        {
            foreach (GuidePoint guidePoint in guidePoints)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(guidePoint.minPosInWorld, 0.1f);
                Gizmos.DrawSphere(guidePoint.maxPosInWorld, 0.1f);
            }
            foreach (GuidePoint guidePoint in guidePoints)
            {
                BoxCollider collider = guidePoint.guidePointPos.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.matrix = guidePoint.guidePointPos.localToWorldMatrix;
                    Gizmos.DrawWireCube(collider.center, collider.size);
                }
            }
        }
    }
}