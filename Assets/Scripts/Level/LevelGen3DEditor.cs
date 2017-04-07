using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelGen3D))]
public class LevelGen3DEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LevelGen3D myScript = (LevelGen3D)target;
        if (GUILayout.Button("Build Level"))
        {
            myScript.GenerateLevel();
        }
    }
}
