using UnityEditor;
using UnityEngine;

namespace HungwX
{
    [CustomEditor(typeof(WinPosController))]
    public class WinPosEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WinPosController winPos = (WinPosController)target;
            GUILayout.Space(5);
            
            if (GUILayout.Button("Update Win Pos"))
            {
                winPos.UpdateWinPos();
            }
        }
    }
}
