using System.Collections;
using UnityEngine;

namespace HungwX
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] private Transform target = default;
        [SerializeField] private float smoothSpeed = 2; // Adjust this value for desired smoothness
        private Vector3 currentZPos, targetZPos, desiredZPos;
        [SerializeField] private GuidePointManager pointManager = default;
        private Transform targetObject;
        [SerializeField] private GameObject Cam2 = default;

        private void OnEnable()
        {
            if (targetObject != null)
            {
                this.transform.position = new Vector3(transform.position.x, targetObject.transform.position.y, targetObject.transform.position.z);
            }
            GameManager.Instance.OnLevelComplete += ChangeCamera;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnLevelComplete -= ChangeCamera;
        }

        private void ChangeCamera()
        {
            Cam2.transform.parent = this.transform.parent;
            this.gameObject.SetActive(false);
        }

        IEnumerator Start()
        {
            targetZPos = target.position;
            currentZPos = transform.position;
            yield return new WaitForEndOfFrame();
            targetObject = ObstacleSpawner.Instance.pool["Diamond0"].transform.GetChild(0);
            this.transform.position = new Vector3(transform.position.x, targetObject.position.y, targetObject.position.z);
        }

        void LateUpdate()
        {
            desiredZPos = target.position - targetZPos + currentZPos;
            if (desiredZPos.z != transform.position.z && desiredZPos.y != transform.position.y)
            {
                transform.position = new Vector3(transform.position.x,
                    Mathf.Lerp(transform.position.y, desiredZPos.y, smoothSpeed * Time.deltaTime),
                    Mathf.Lerp(transform.position.z, desiredZPos.z, smoothSpeed * Time.deltaTime));
                pointManager.UpdateAllGuidePointImage();
            }
        }

    }
}