using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using MoreMountains.NiceVibrations;
using System.Linq;

namespace HungwX
{
    public enum SpaceDimension
    {
        XY,
        XZ,
        YZ,
    }
    public delegate bool CheckScreenPointAction(int index);
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
        private MobileInputManager mobileInputManager;
        private int countVibration = 0, targetIndexPressed, countCheck = 0;
        //, countCheck = 0
        private float frequency = 4;
        private GameManager gameManager;
        [SerializeField] private bool isUpdateAllGuidePoint = false;
        public CheckScreenPointAction CheckScreenPoint;
        [NonSerialized] public int numberOfPointsSFX;
        public Action<int> OnMouthLeftGuidePointPressed,
            OnMouthRightGuidePointPressed,
            OnEyeLeftGuidePointPressed,
            OnEyeRightGuidePointPressed,
            OnBackRightGuidePointPressed,
            OnBackLeftGuidePointPressed,
            OnHeadGuidePointPressed,
            OnGuidePointUp,
            OnGuidePointDown,
            OnGuidePointMove;

        private void Awake()
        {
            CheckScreenPoint = DefaultCheck;
        }

        private bool DefaultCheck(int index)
        {
            return true;
        }
       
        private IEnumerator Start()
        {
            SetWinZones();
            numberOfPointsSFX = 0;
            yield return new WaitForEndOfFrame();
            mobileInputManager = MobileInputManager.Instance;
            AddEvent();
            this.parentOfGuidePoints = mobileInputManager.transform;
            mainCamera = Camera.main;
            CreateGuidePointImages();
            gameManager = GameManager.Instance;
            gameManager.SetMobileInput(true);
            gameManager.OnLevelComplete += ClearGuidePointImages;
        }

        private void OnDestroy()
        {
            gameManager.OnLevelComplete -= ClearGuidePointImages;
        }

        private void AddEvent()
        {
            mobileInputManager.OnPointerMoveAction.AddListener(OnPointerMove);
            mobileInputManager.OnPointerDownAction.AddListener(OnPointerDown);
            mobileInputManager.OnPointerUpAction.AddListener(OnPointerUp);
        }

        private void SetWinZones()
        {
            for (int i = 0; i < guidePoints.Count; i++)
            {
                guidePoints[i].guidePointPos.gameObject.TryGetComponent<WinZone>(out guidePoints[i].winZone);
            }
        }

        public List<WinZone> GetWinZones()
        {
            SetWinZones();
            return guidePoints.GetRange(0, guidePoints.Count).Select(guidePoint => guidePoint.winZone).ToList();
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

        public void OnPointerUp()
        {
            if (targetIndexPressed != -1)
            {
                OnGuidePointUp?.Invoke(targetIndexPressed);
                SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.bodyPartAudioClips[numberOfPointsSFX]);
                numberOfPointsSFX = numberOfPointsSFX + 1 >= SoundManager.Instance.audioClip.bodyPartAudioClips.Count ? 0 : numberOfPointsSFX + 1;
            }
            mobileInputManager.isPointerDown = false;
        }

        public void OnPointerDown()
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
            OnGuidePointDown?.Invoke(targetIndexPressed);
            Animator targetAnimator = guidePointImages[targetIndexPressed].GetComponent<Animator>();
            targetAnimator.speed = 1;
            targetAnimator.Play("Pressed");
            frequency = 4;
            countVibration = 0;
        }

        public void OnPointerMove()
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
                    YZAxisMove(newWorldPosition);
                    break;
            }
            if (!isUpdateAllGuidePoint)
            {
                guidePointImages[targetIndexPressed].transform.position = mainCamera.WorldToScreenPoint(guidePoints[targetIndexPressed].guidePointPos.position);
            }
            countVibration++;
            if (countVibration >= (int)frequency)
            {
                MMVibrationManager.Haptic(HapticTypes.Selection, false, true, this);
                countVibration = 0;
                frequency = frequency < 8 ? frequency + 0.125f : 8;
            }
            OnGuidePointMove?.Invoke(targetIndexPressed);
            HandleGuidePointMove(targetIndexPressed);
            gameManager.UpdateScore(targetIndexPressed);
        }

        public void UpdateAllGuidePointImage()
        {
            if (guidePointImages == null)
                return;
            for (int i = 0; i < guidePoints.Count; i++)
            {
                guidePointImages[i].transform.position = mainCamera.WorldToScreenPoint(guidePoints[i].guidePointPos.position);
            }
        }

        private void YZAxisMove(Vector3 newWorldPosition)
        {
            //guidePoints[targetIndexPressed].guidePointPos.position = new Vector3(guidePoints[targetIndexPressed].guidePointPos.position.x, newWorldPosition.y, newWorldPosition.z);
            if (CheckScreenPoint(targetIndexPressed) || countCheck == 1)
            {
                countCheck = 0;
                guidePoints[targetIndexPressed].guidePointPos.position = new Vector3(guidePoints[targetIndexPressed].guidePointPos.position.x, newWorldPosition.y, newWorldPosition.z);
            }
            else
            {
                countCheck = 1;
            }
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

        public void CalculateHeadPercentWeith(int index)
        {
            Vector3 currentGuidePointPos = guidePoints[index].guidePointPos.position;
            Vector3 worldTopRight = guidePoints[index].winZone.worldTopRight;
            Vector3 worldBottomLeft = guidePoints[index].winZone.worldBottomLeft;
            if (currentGuidePointPos.z > worldBottomLeft.z && currentGuidePointPos.z < worldTopRight.z)
            {
                guidePoints[index].perCentWeightInWorld.z = currentGuidePointPos.z - (worldTopRight.z - worldBottomLeft.z) / 2;
            }
            else if (currentGuidePointPos.z < worldBottomLeft.z)
            {
                guidePoints[index].perCentWeightInWorld.z = -(worldTopRight.z - worldBottomLeft.z) / 2;
            }
            else if (currentGuidePointPos.z > worldTopRight.z)
            {
                guidePoints[index].perCentWeightInWorld.z = (worldTopRight.z - worldBottomLeft.z) / 2;
            }

            if (currentGuidePointPos.x > worldBottomLeft.x && currentGuidePointPos.x < worldTopRight.x)
            {
                guidePoints[index].perCentWeightInWorld.x = currentGuidePointPos.x - (worldTopRight.x - worldBottomLeft.x) / 2;
            }
            else if (currentGuidePointPos.x < worldBottomLeft.x)
            {
                guidePoints[index].perCentWeightInWorld.x = -(worldTopRight.x - worldBottomLeft.x) / 2;
            }
            else if (currentGuidePointPos.x > worldTopRight.x)
            {
                guidePoints[index].perCentWeightInWorld.x = (worldTopRight.x - worldBottomLeft.x) / 2;
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