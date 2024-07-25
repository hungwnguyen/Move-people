using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HungwX
{
    public class LoadingGame : MonoBehaviour
    {
        [SerializeField] private UnityEvent onLoadingComplete = null;
        [SerializeField] private float delayTime = 2;
        [SerializeField] private ProgressController progressController = null;

        IEnumerator Start()
        {
            float time = 0.01f;
            while (time < delayTime)
            {
                time += Time.deltaTime;
                if (time > delayTime)
                    time = delayTime;
                progressController.UpdatePosition(time / delayTime);
                yield return new WaitForEndOfFrame();
            }
            onLoadingComplete.Invoke();
        }
    }
}
