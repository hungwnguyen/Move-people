using System.Collections;
using UnityEngine;

namespace HungwX
{
    public class LegStyling : AlternateStyling
    {
        private Transform target;
        [SerializeField] private Transform current = default;
        private float maxDistance;

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(FindTarget());
        }

        IEnumerator FindTarget()
        {
            yield return new WaitForEndOfFrame();
            target = ObstacleSpawner.Instance.pool["Diamond0"].transform.GetChild(0);
            maxDistance = target.position.z - current.position.z;
        }

        public override float CalculateStylingScore(int i)
        {
            return (1 - (target.position.z - current.position.z) / maxDistance) * 2;
        }
    }
}
