using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RoomBuilder))]
public class RoomBuilderEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RoomBuilder myScript = (RoomBuilder)target;
        if (GUILayout.Button("Build Room"))
        {
            myScript.buildRoom();
        }
    }
}
