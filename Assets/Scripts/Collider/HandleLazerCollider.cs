using UnityEngine;

namespace HungwX
{
    public class HandleLazerCollider : MonoBehaviour
    {
        [SerializeField] LayerMask targetLayer1 = default, targetLayer2 = default;
        private GameManager manager;

        private void Start()
        {
            manager = GameManager.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer) == targetLayer1 || (1 << other.gameObject.layer) == targetLayer2)
            {
                if (!manager.IsGameOver && !manager.IsWin)
                {
                    manager.OnLevelFail?.Invoke();
                    manager.IsGameOver = true;
                    SoundManager.CreatePlayFXSound(SoundManager.Instance.audioClip.Die);
                }
                LevelManager.instance.OnHitLazer(gameObject, other.gameObject.transform.position);
            }
        }
    }
}