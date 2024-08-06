using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HungwX
{
    [RequireComponent(typeof(ObstacleSpawner))]
    public class ObstacleTool : MonoBehaviour
    {
#if UNITY_EDITOR
        private List<GameObject> obstacleObjects;
        private ObstacleData obstacleData;
        private ObstacleSpawner obstacleSpawner;
        
        private void GetObstacleObject()
        {
            obstacleObjects = new List<GameObject>();
            print("Child count: " + this.transform.childCount);
            for (int i = 0; i < transform.childCount; i++)
            {
                obstacleObjects.Add(transform.GetChild(i).gameObject);
            }
        }

        private void ClearCurrentObject()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        private void SetListObstacle(List<GameObject> obstaclesGameObject, int level, string dataPath)
        {
            obstacleData= new ObstacleData();
            obstacleData.Obstacles = new List<Obstacle>();
            foreach (GameObject obstacle in obstaclesGameObject)
            {
                Obstacle _obstacle = new Obstacle
                {
                    Type = obstacle.tag,
                    Position = obstacle.transform.position,
                    Rotation = obstacle.transform.eulerAngles,
                    Scale = obstacle.transform.localScale
                };
                obstacleData.Obstacles.Add(_obstacle);
            }
            SaveLevelDataToJson(dataPath + $"/{level}.txt");
        }

        private void SaveLevelDataToJson(string filePath)
        {
            string json = JsonUtility.ToJson(obstacleData);
            Debug.Log($"Saving level data to JSON: {json}");

            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            File.WriteAllText(filePath, json);
        }

        public void SaveCurrentLevelData(int level, string dataPath)
        {
            GetObstacleObject();
            SetListObstacle(obstacleObjects, level, dataPath);
        }

        public void Spawn(List<Obstacle> database)
        {
            foreach (Obstacle data in database)
            {

                GameObject obj = PrefabUtility.InstantiatePrefab(obstacleSpawner.GetPrefabByType(data.Type)) as GameObject;
                obj.transform.SetParent(transform);
                if (data.Type.Equals("Crate"))
                {
                    obj.GetComponent<MeshRenderer>().material = obstacleSpawner.crateMaterials[Random.Range(0, obstacleSpawner.crateMaterials.Count)];
                }
                obj.transform.position = data.Position;
                obj.transform.eulerAngles = data.Rotation;
                obj.transform.localScale = data.Scale;
            }
        }

        public void LoadCurrentLevelData(int level, string dataPath)
        {
            obstacleData = new ObstacleData();
            obstacleSpawner = GetComponent<ObstacleSpawner>();
            ClearCurrentObject();
            string filePath = $"{dataPath}/{level}.txt";
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                obstacleData = JsonUtility.FromJson<ObstacleData>(json);
                Spawn(obstacleData.Obstacles);
                Debug.Log($"<color=green>Loaded level {PlayerPrefs.GetInt("LevelEditor", 1)} successfully</color>");
            }
            else
            {
                Debug.LogWarning($"Level data file '{filePath}' does not exist.");
            }
        }

        public void Reset()
        {
            ClearCurrentObject();
        }
#endif
    }
}