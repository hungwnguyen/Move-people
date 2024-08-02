using UnityEditor;
using UnityEngine;

namespace HungwX
{
    [CustomEditor(typeof(ObstacleTool))]
    public class ObstacleDataEditor : Editor
    {
        private int selectedTab = 0;
        private string[] tabTitles = { "Editor", "Setting" };
        private GameObject selectedGameObject;
        private ObstacleTool obstacleTool;
        private string folderPath = "";

        public override void OnInspectorGUI()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, tabTitles);
            obstacleTool = (ObstacleTool)target;
            GUILayout.Space(10);
            base.OnInspectorGUI();
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
                selectedGameObject = null;
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
                    Debug.Log($"<color=green>Saved level {PlayerPrefs.GetInt("LevelEditor", 1)} successfully</color>");
                    PlayerPrefs.SetInt("LevelEditor", PlayerPrefs.GetInt("LevelEditor", 1) + 1);
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError($"Cannot save level {PlayerPrefs.GetInt("LevelEditor", 1)}, please change the data path!");
                }
            }
            GUI.color = Color.white;
            GUILayout.Space(10);
            
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
            PlayerPrefs.SetString("DataPath", folderPath);
            EditorGUILayout.EndHorizontal();
            FillLevelName();
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

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUICustom;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUICustom;
        }

        private void OnSceneGUICustom(SceneView sceneView)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    selectedGameObject = hit.collider.gameObject;
                    Repaint();
                    obstacleTool.UpdateObstacle(selectedGameObject);
                }
            }
        }
    }
}

