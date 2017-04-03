using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RotateTest))]
public class RotateTestEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RotateTest myScript = (RotateTest)target;
        if (GUILayout.Button("Translate door"))
        {
            myScript.rotateMe();
        }
        if (GUILayout.Button("Rotate90"))
        {
            myScript.rotate90();
        }
        if (GUILayout.Button("Rotate 90 lololo"))
        {
            myScript.rotateMeForrealz();
        }
        if (GUILayout.Button("Rotate Room 90"))
        {
            myScript.rotateRoom();
        }
    }
}
