using System.Collections.Generic;
using UnityEngine;

namespace HungwX
{
    public class ObstacleSpawner : MonoBehaviour, IObstacleSpawner
    {
        public List<GameObject> obstaclePrefabs = default;
        public List<Material> crateMaterials = default;
        private List<Obstacle> obstacles;
        public Dictionary<string, GameObject> pool;
        private Dictionary<string, int> typeMark;

        public static ObstacleSpawner Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(gameObject);
            }
            pool = new Dictionary<string, GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                pool["electric" + i] = this.transform.GetChild(i).gameObject;
            }
        }

        private void Start()
        {
            LevelController.Instance.OnLoadNextLevel += DestroyAll;
            GameManager.Instance.OnLevelReplay += SetTranform;
        }

        public void Spawn(List<Obstacle> value)
        {
            obstacles = value;
            SetTranform();
        }

        private void SetTranform()
        {
            if (obstacles == null) { return; }
            typeMark = new Dictionary<string, int>();
            foreach (Obstacle data in obstacles)
            {
                string key = data.Type;
                if (typeMark.ContainsKey(key))
                {
                    typeMark[key]++;
                }
                else
                {
                    typeMark[key] = 0;
                }
                string keyPool = key + typeMark[key];
                if (!pool.ContainsKey(keyPool))
                {
                    GameObject obj = Instantiate(GetPrefabByType(data.Type), transform);
                    pool[keyPool] = obj;
                    if (data.Type.Equals("Crate"))
                    {
                        obj.GetComponent<MeshRenderer>().material = crateMaterials[Random.Range(0, crateMaterials.Count)];
                    } 
                }
                GameObject target = pool[keyPool];
                target.transform.position = data.Position;
                target.transform.eulerAngles = data.Rotation;
                target.transform.localScale = data.Scale;
                if (data.Type.Equals("Diamond"))
                {
                    Transform diamond = target.transform.GetChild(0);
                    Transform prefabDiamond = diamond.transform.GetChild(0);
                    prefabDiamond.position = diamond.position;
                    prefabDiamond.rotation = diamond.rotation;
                }
                target.SetActive(true);
            }
        }

        public GameObject SpawnFromPool(GameObject go, string key)
        {
            if (typeMark.ContainsKey(key))
            {
                typeMark[key]++;
                if (typeMark[key] > 6)
                {
                    typeMark[key] = 0;
                }
            }
            else
            {
                typeMark[key] = 0;
            }
            string keyPool = key + typeMark[key];
            GameObject obj;
            if (!pool.ContainsKey(keyPool))
            {
                obj = Instantiate(go, transform);
                pool[keyPool] = obj;
            } else
            {
                obj = pool[keyPool];
                obj.SetActive(true);
            }
            return obj;
        }

        public void DestroyAll()
        {
            foreach (GameObject value in pool.Values)
            {
                value.SetActive(false);
            }
        }

        public GameObject GetPrefabByType(string type)
        {
            for (int i = 0; i < obstaclePrefabs.Count; i++)
            {
                if (obstaclePrefabs[i].tag == type)
                {
                    return obstaclePrefabs[i];
                }
            }
            return null;
        }
    }
}