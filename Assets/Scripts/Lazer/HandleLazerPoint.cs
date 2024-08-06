using RootMotion;
using UnityEngine;

namespace HungwX
{
    public class HandleLazerPoint : Singleton<HandleLazerPoint>
    {
        [SerializeField] private GameObject lazerPrefab = default;

        void Start()
        {
            LevelManager.instance.OnHitLazer += OnHitLazer;
        }

        public void OnHitLazer(GameObject lazerGameObject, Vector3 colliderPos)
        {
            Transform lazerPos = lazerGameObject.transform.GetChild(0);
            Vector3 position = new Vector3(colliderPos.x, lazerPos.position.y, lazerPos.position.z);
            LazerPoint electric = ObstacleSpawner.Instance.SpawnFromPool(lazerPrefab, "electric").GetComponent<LazerPoint>();
            electric.transform.position = position;
            electric.endPos = lazerPos;
        }

    }
}
