using System;
using UnityEngine;

namespace HungwX
{
    public enum BodyPart
    {
        MouthLeft,
        MouthRight,
        EyeLeft,
        EyeRight,
        BackLeft,
        BackRight,
        Head,
        LegRight,
        LegLeft,
    }

    public enum TargetDirectionX
    {
        Right,
        Left,
    }
    public enum TargetDirectionY
    {
        Up,
        Down,
    }
    public enum TargetDirectionZ
    {
        Forward,
        Backward,
    }

    [Serializable]
    public class GuidePoint
    {
        public Transform guidePointPos;
        public Vector3 startPos, currentPos;
        public Vector3 startGuidePointImagePos;
        public float score;
        public Vector3 perCentWeightInWorld, centerPosInWorld, distanceInWorld, maxPosInWorld, minPosInWorld;
        [NonSerialized] public TargetDirectionX targetDirectionX;
        [NonSerialized] public TargetDirectionY targetDirectionY;
        [NonSerialized] public TargetDirectionZ targetDirectionZ;
        [NonSerialized] public bool isMoveToWinPoint = false;
        [NonSerialized] public WinZone winZone;

        public bool CheckScreenPointIsInWinZone(Vector2 position, float radiusTouch)
        {
            Camera cam = Camera.main;
            return Vector2.Distance(cam.WorldToScreenPoint(guidePointPos.position), position) < radiusTouch;
        }

        public BoxCollider SetUpGuidePoint()
        {
            BoxCollider boxCollider = guidePointPos.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                Vector3[] localCorners = new Vector3[8];
                localCorners[0] = new Vector3(-0.5f, 0.5f, -0.5f);
                localCorners[1] = new Vector3(0.5f, 0.5f, -0.5f);
                localCorners[2] = new Vector3(-0.5f, -0.5f, -0.5f);
                localCorners[3] = new Vector3(0.5f, -0.5f, -0.5f);
                localCorners[4] = new Vector3(-0.5f, 0.5f, 0.5f);
                localCorners[5] = new Vector3(0.5f, 0.5f, 0.5f);
                localCorners[6] = new Vector3(-0.5f, -0.5f, 0.5f);
                localCorners[7] = new Vector3(0.5f, -0.5f, 0.5f);

                // Transform corners to world space
                Vector3[] worldCorners = new Vector3[8];
                for (int i = 0; i < 8; i++)
                {
                    worldCorners[i] = guidePointPos.TransformPoint(boxCollider.center + Vector3.Scale(boxCollider.size, localCorners[i]));
                }

                startPos = guidePointPos.position;
                startGuidePointImagePos = Camera.main.WorldToScreenPoint(startPos);
                centerPosInWorld = guidePointPos.TransformPoint(boxCollider.center);
                float maxY, minY, maxX, minX, maxZ, minZ;
                maxX = worldCorners[2].x > worldCorners[5].x ? worldCorners[2].x : worldCorners[5].x;
                minX = worldCorners[2].x > worldCorners[5].x ? worldCorners[5].x : worldCorners[2].x;
                maxY = worldCorners[2].y > worldCorners[5].y ? worldCorners[2].y : worldCorners[5].y;
                minY = worldCorners[2].y > worldCorners[5].y ? worldCorners[5].y : worldCorners[2].y;
                maxZ = worldCorners[2].z > worldCorners[5].z ? worldCorners[2].z : worldCorners[5].z;
                minZ = worldCorners[2].z > worldCorners[5].z ? worldCorners[5].z : worldCorners[2].z;

                maxPosInWorld = new Vector3(maxX, maxY, maxZ);
                minPosInWorld = new Vector3(minX, minY, minZ);
                distanceInWorld = new Vector3((maxX - minX)/ 2, (maxY - minY) / 2, (maxZ - minZ)/ 2);
            }
            return boxCollider;
        }
    }
}