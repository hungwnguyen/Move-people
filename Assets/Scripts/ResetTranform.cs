using System.Collections;
using UnityEngine;

namespace HungwX
{
    public class ResetTranform : MonoBehaviour
    {
        public float timeDelay = 2;
        public void ResetTransform()
        {
            this.gameObject.SetActive(false);
            this.transform.position = Vector3.zero;
            this.transform.rotation = Quaternion.identity;
            this.gameObject.SetActive(true);
            StartCoroutine(ResetTransformCoroutine());
        }

        IEnumerator ResetTransformCoroutine()
        {
            yield return new WaitForSeconds(timeDelay);
            this.transform.position = Vector3.zero;
            this.transform.localEulerAngles = Vector3.zero;
        }
    }
}
