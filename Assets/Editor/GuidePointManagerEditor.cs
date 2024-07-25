using UnityEditor;
using UnityEngine;

namespace HungwX
{
    [CustomEditor(typeof(GuidePointManager))]
    public class GuidePointManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GuidePointManager guidePointManager = (GuidePointManager)target;
            GUILayout.Space(5);
            if (GUILayout.Button("Set Up GuidePoint"))
            {
                guidePointManager.SetUpGuidePoint();
            }
            GUILayout.Space(5);
            DrawDefaultInspector();
        }
    }
}
