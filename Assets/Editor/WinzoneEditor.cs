using UnityEditor;
using UnityEngine;

namespace HungwX
{
    [CustomEditor(typeof(WinZone))]
    public class WinzoneEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WinZone winZone = (WinZone)target;
            GUILayout.Space(5);
            if (GUILayout.Button("Set Up WinZone"))
            {
                winZone.SetUpWinZone();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Find WinZone"))
            {
                winZone.FindWinZone();
            }
        }
    }
}
