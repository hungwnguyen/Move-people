using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HungwX
{
    [Serializable]
    public class DataGame
    {
        public List<Obstacle> obstacleDatas = new List<Obstacle>();
    }
    public class ObstacleTool : MonoBehaviour
    {
#if UNITY_EDITOR
        private List<int> obstacleList = new List<int>();
        public List<GameObject> obstacleObjects = new List<GameObject>();
        private GameObject currentObstacle;
        private DataGame levelData;

        private void SetListObstacle(List<GameObject> obstaclesGameObject, int level, string dataPath)
        {
            foreach (GameObject obstacle in obstaclesGameObject)
            {
                Obstacle obstacleData = new Obstacle
                {
                    Type = obstacle.tag,
                    Position = obstacle.transform.position,
                    Rotation = obstacle.transform.eulerAngles,
                    Scale = obstacle.transform.localScale
                };

                Debug.Log($"Adding obstacle: Type={obstacleData.Type}, Position={obstacleData.Position}, Rotation={obstacleData.Rotation}, Scale={obstacleData.Scale}");
                levelData.obstacleDatas.Add(obstacleData);
            }
            SaveLevelDataToJson(dataPath + $"/{level}.txt");
        }

        private void SaveLevelDataToJson(string filePath)
        {
            string json = JsonUtility.ToJson(levelData);
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
            levelData = new DataGame();
            SetListObstacle(obstacleObjects, level, dataPath);
        }

        public void Reset()
        {
            obstacleList.Clear();
            obstacleObjects.Clear();
        }

        public void UpdateObstacle(GameObject obstacle)
        {
            currentObstacle = obstacle;
            if (obstacleList == null)
            {
                obstacleList = new List<int>();
            }
            if (!obstacleList.Contains(obstacle.GetHashCode()))
            {
                obstacleList.Add(obstacle.GetHashCode());
                SetNewObstacle();
                Debug.Log("Added new obstacle: " + obstacle.name);
            }
        }

        private void SetNewObstacle()
        {
            if (obstacleObjects == null)
            {
                obstacleObjects = new List<GameObject>();
            }
            obstacleObjects.Add(currentObstacle);
        }

#endif
    }
}