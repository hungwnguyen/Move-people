using UnityEngine;

namespace HungwX
{
    public class HandleLazerCollider : MonoBehaviour
    {
        [SerializeField] LayerMask targetLayer = default;
        private GameManager manager;

        private void Start()
        {
            manager = GameManager.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer) == targetLayer)
            {
                if (!manager.IsGameOver && !manager.IsWin)
                {
                    manager.OnLevelFail?.Invoke();
                    manager.IsGameOver = true;
                }
                LevelManager.instance.OnHitLazer(gameObject, other.gameObject.transform.position);
            }
        }
    }
}