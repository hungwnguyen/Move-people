using System;
using UnityEngine;

namespace HungwX
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] private Transform target = default;
        [SerializeField] private float smoothSpeed = 2; // Adjust this value for desired smoothness
        private Vector3 currentZPos, targetZPos, desiredZPos;
        [SerializeField] private GuidePointManager pointManager = default;
        public static Action OnCameraUpdate;
        void Start()
        {
            targetZPos = target.position;
            currentZPos = transform.position;
        }

        void LateUpdate()
        {
            SmoothMove();
            pointManager.UpdateAllGuidePointImage();
            OnCameraUpdate?.Invoke();
        }

        void SmoothMove()
        {
            desiredZPos = target.position - targetZPos + currentZPos;
            if (desiredZPos.z != transform.position.z && desiredZPos.y != transform.position.y)
                transform.position = new Vector3(transform.position.x,
                    Mathf.Lerp(transform.position.y, desiredZPos.y, smoothSpeed * Time.deltaTime),
                    Mathf.Lerp(transform.position.z, desiredZPos.z, smoothSpeed * Time.deltaTime));
        }

    }
}