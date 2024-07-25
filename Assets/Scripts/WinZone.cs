using System;
using HungwX;
using UnityEngine;
/// <summary>
/// winZone can only be used while this tranform.rotattion is 0,0,0
/// </summary>
public class WinZone : MonoBehaviour
{
    [SerializeField] public Vector3 worldTopRight, worldBottomLeft, winPos;
    [SerializeField] private float centerX, centerY, centerZ, maxY, minY, maxX, minX, maxZ, minZ;
    public BodyPart bodyPart;
    public GuidePointManager guidePointManager;

    public float GetScore(Vector3 point)
    {
        if (point.x < minX || point.x > maxX || point.y < minY || point.y > maxY || point.z < minZ || point.z > maxZ)
        {
            return 0f;
        }

        float normalizedX = ((point.x > centerX ? maxX - centerX : centerX - minX) - Math.Abs(point.x - centerX)) / (point.x > centerX ? maxX - centerX : centerX - minX);
        float normalizedY = ((point.y > centerY ? maxY - centerY : centerY - minY) - Math.Abs(point.y - centerY)) / (point.y > centerY ? maxY - centerY : centerY - minY);
        float normalizedZ = ((point.z > centerZ ? maxZ - centerZ : centerZ - minZ) - Math.Abs(point.z - centerZ)) / (point.z > centerZ ? maxZ - centerZ : centerZ - minZ);
        float averageNormalized = (normalizedX + normalizedY + normalizedZ) / 3f;
        return averageNormalized;
    }

    public float GetHorizontalScore(Vector3 point, float upValue = 1, float downValue = 1)
    {
        if (point.x < minX || point.x > maxX)
        {
            return 0f;
        }

        float normalizedX = ((point.x > centerX ? (maxX - centerX) * upValue : (centerX - minX) * downValue) - Math.Abs(point.x - centerX)) / (point.x > centerX ? maxX - centerX : centerX - minX);
        return guidePointManager.Clamp(normalizedX, 0, 1);
    }

    public float GetVerticalScore(Vector3 point, float upValue = 1, float downValue = 1)
    {
        if (point.y < minY || point.y > maxY)
        {
            return 0f;
        }

        float normalizedY = ((point.y > centerY ? (maxY - centerY) * upValue : (centerY - minY) * downValue) - Math.Abs(point.y - centerY)) / (point.y > centerY ? maxY - centerY : centerY - minY);
        return guidePointManager.Clamp(normalizedY, 0, 1);
    }

    public void SetUpWinZone()
    {
        worldTopRight = new Vector3(maxX, maxY, maxZ);
        worldBottomLeft = new Vector3(minX, minY, minZ);
        winPos = new Vector3(centerX, centerY, centerZ);
    }

    public void FindWinZone()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("WinZone must have a BoxCollider component");
            return;
        }

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
            worldCorners[i] = this.transform.TransformPoint(boxCollider.center + Vector3.Scale(boxCollider.size, localCorners[i]));
        }

        winPos = this.transform.TransformPoint(boxCollider.center);
        centerX = winPos.x;
        centerY = winPos.y;
        centerZ = winPos.z;
        maxX = worldCorners[2].x > worldCorners[5].x ? worldCorners[2].x : worldCorners[5].x;
        minX = worldCorners[2].x > worldCorners[5].x ? worldCorners[5].x : worldCorners[2].x;
        maxY = worldCorners[2].y > worldCorners[5].y ? worldCorners[2].y : worldCorners[5].y;
        minY = worldCorners[2].y > worldCorners[5].y ? worldCorners[5].y : worldCorners[2].y;
        maxZ = worldCorners[2].z > worldCorners[5].z ? worldCorners[2].z : worldCorners[5].z;
        minZ = worldCorners[2].z > worldCorners[5].z ? worldCorners[5].z : worldCorners[2].z;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(worldBottomLeft, 0.2f);
        Gizmos.DrawWireSphere(worldTopRight, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(winPos, 0.2f);
    }
}