using UnityEngine;

namespace HungwX
{
    public class HandleWinCollider : MonoBehaviour
    {
        [SerializeField] LayerMask targetLayer = default;
        private GameManager manager;

        private void Start()
        {
            manager = GameManager.Instance;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((1 << collision.gameObject.layer) == targetLayer)
            {
                if (!manager.IsWin && !manager.IsGameOver)
                {
                    manager.ClearAlternateStyling();
                    manager.progressController.UpdatePosition(1);
                    manager.OnLevelComplete?.Invoke();
                    manager.IsWin = true;
                }
            }
        }
    }
}
