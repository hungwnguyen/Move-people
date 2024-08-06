using UnityEngine;
using Random = UnityEngine.Random;
using System;

namespace HungwX
{
    public class LevelIOHandle : MonoBehaviour
    {
        [NonSerialized] public ObstacleData obstacleData = default;
        [SerializeField, Header("<color=red>Not contain Asset/Resources/</color>")] private string path = default;

        protected void Awake()
        {
            LevelController.Instance.OnLevelLoadComplete += ReadData;
        }

        private void OnDestroy()
        {
            LevelController.Instance.OnLevelLoadComplete -= ReadData;
        }

        private void ReadData()
        {
            int currentLevel = LevelController.Instance.CurrentLevelID - 2;
            ReadCurrentLevelData(currentLevel);
        }

        private void ReadCurrentLevelData(int level)
        {
            obstacleData = new ObstacleData();

            TextAsset textAsset = Resources.Load<TextAsset>(path + "/" + level);
            if (textAsset != null)
            {
                obstacleData = JsonUtility.FromJson<ObstacleData>(textAsset.text);
                //print(textAsset.text);
                ObstacleSpawner.Instance.Spawn(obstacleData.Obstacles);
            }
            else
            {
                if (level > 99)
                {
                    ReadCurrentLevelData(Random.Range(50, 100));
                }
                else
                {
                    Debug.LogWarning($"Level {path + "/" + level}is not available");
                }
            }
        }
    }
}
