using UnityEditor;
using UnityEngine;

namespace HungwX
{
    [CustomEditor(typeof(ObstacleTool))]
    public class ObstacleDataEditor : Editor
    {
        private int selectedTab = 0;
        private string[] tabTitles = { "Editor", "Setting" };
        private ObstacleTool obstacleTool;
        private string folderPath = "";

        public override void OnInspectorGUI()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, tabTitles);
            obstacleTool = (ObstacleTool)target;
            GUILayout.Space(10);
            switch (selectedTab)
            {
                case 0:
                    DrawEditorTab();
                    break;
                case 1:
                    DrawSettingTab();
                    break;
            }
        }

        private void DrawSettingTab()
        {
            if (GUILayout.Button("Reset"))
            {
                obstacleTool.Reset();
            }
        }

        private void DrawEditorTab()
        {
            GUILayout.Space(10);
            GUI.color = Color.yellow;
            if (GUILayout.Button("Save current level data"))
            {
                if (!string.IsNullOrEmpty(folderPath) && folderPath != "Null")
                {
                    obstacleTool.SaveCurrentLevelData(PlayerPrefs.GetInt("LevelEditor", 1), folderPath);
                    Debug.Log($"<color=yellow>Saved level {PlayerPrefs.GetInt("LevelEditor", 1)} successfully</color>");
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError($"Cannot save level {PlayerPrefs.GetInt("LevelEditor", 1)}, please change the data path!");
                }
            }
            GUI.color = Color.green;
            if (GUILayout.Button("Load current level data"))
            {
                if (!string.IsNullOrEmpty(folderPath) && folderPath != "Null")
                {
                    obstacleTool.LoadCurrentLevelData(PlayerPrefs.GetInt("LevelEditor", 1), folderPath);
                }
                else
                {
                    Debug.LogError($"Cannot load level {PlayerPrefs.GetInt("LevelEditor", 1)}, please change the data path!");
                }
            }   
            GUI.color = Color.white;
            GUILayout.Space(10);
            FillLevelName();
            DrawFolderPath();
        }

        private void DrawFolderPath()
        {
            Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drop Folder Here", EditorStyles.helpBox);

            Event currentEvent = Event.current;
            if (dropArea.Contains(currentEvent.mousePosition))
            {
                if (currentEvent.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    currentEvent.Use();
                }
                else if (currentEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    string[] paths = DragAndDrop.paths;
                    if (paths.Length > 0)
                    {
                        folderPath = paths[0];
                        PlayerPrefs.SetString("DataPath", paths[0]);
                    }
                    else
                    {
                        folderPath = "Null";
                    }
                    currentEvent.Use();
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Folder Path:");
            folderPath = EditorGUILayout.TextField(PlayerPrefs.GetString("DataPath", folderPath));
            EditorGUILayout.EndHorizontal();
        }

        private void FillLevelName()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Level:");
            int level = PlayerPrefs.GetInt("LevelEditor", 1);
            level = EditorGUILayout.IntField(level);
            PlayerPrefs.SetInt("LevelEditor", level);
            if (GUILayout.Button("-"))
            {
                PlayerPrefs.SetInt("LevelEditor", level - 1);
            }
            if (GUILayout.Button("+"))
            {
                PlayerPrefs.SetInt("LevelEditor", level + 1);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}

