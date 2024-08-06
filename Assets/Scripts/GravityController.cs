using System;
using System.Collections;
using UnityEngine;

namespace HungwX
{
    public class GravityController : MonoBehaviour
    {
        Vector3 groundPos;
        [SerializeField] GuidePointManager pointManager = default;
        private int index;
        [SerializeField] float speed = 1f;
        private float distanceY;
        public Action OnGravityUp;

        void Start()
        {
            index = this.transform.GetSiblingIndex();
            pointManager.OnGuidePointUp += UseGravity;
            pointManager.OnGuidePointDown += ResetDO;
            FindGroundPos();
            distanceY = transform.position.y - groundPos.y;
        }

        private void ResetDO(int i)
        {
            StopAllCoroutines();
        }

        private void UseGravity(int i)
        {
            if (i == index)
                StartCoroutine(MoveToGround(i));
        }

        IEnumerator MoveToGround(int i)
        {
            FindGroundPos();
            while (transform.position.y > groundPos.y)
            {
                this.transform.position = Vector3.MoveTowards(transform.position, groundPos, Time.deltaTime * speed);
                OnGravityUp?.Invoke();
                yield return new WaitForEndOfFrame();
            }
        }

        private void FindGroundPos()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                groundPos = hit.point + Vector3.up * distanceY;
            }
        }

        private void OnDrawGizmosSelected()
        {
            FindGroundPos();
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundPos, 0.1f);
        }

    }
}
