using System;
using UnityEngine;

namespace HungwX
{
    public class LazerPoint : MonoBehaviour
    {
        [NonSerialized]public Transform endPos;  
        public float initialSpeed; 
        public float acceleration; 

        private Vector3 velocity = default;  

        void OnEnable()
        {
            velocity = Vector3.zero;
        }
        private void Start()
        {
            GameManager.Instance.OnLevelReplay += () => this.gameObject.SetActive(false);
        }

        void Update()
        {
            if (endPos != null)
            {
                Move(endPos);
            }
        }

        public void Move(Transform endPos)
        {
            if (transform.position.x > endPos.position.x)
            {
                Vector3 direction = (endPos.position - transform.position).normalized;
                velocity += direction * acceleration * Time.deltaTime;
                if (velocity.magnitude > initialSpeed)
                {
                    velocity = velocity.normalized * initialSpeed;
                }
                transform.position += velocity * Time.deltaTime;
            } else
            {
                gameObject.SetActive(false);
            }
        }
    }
}