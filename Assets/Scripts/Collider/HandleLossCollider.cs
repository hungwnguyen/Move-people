using UnityEngine;

namespace HungwX
{
    public class HandleLossCollider : MonoBehaviour
    {
        [SerializeField] LayerMask targetLayer = default;
        private GameManager manager;

        private void Start()
        {
            manager = GameManager.Instance;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((1 << collision.gameObject.layer) != targetLayer)
            {
                if (!manager.IsGameOver && !manager.IsWin)
                {
                    manager.OnLevelFail?.Invoke();
                    manager.IsGameOver = true;
                }
            }
        }
    }
}
