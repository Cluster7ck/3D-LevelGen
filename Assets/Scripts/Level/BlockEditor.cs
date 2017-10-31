using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor
{

    protected virtual void OnSceneGUI()
    {
        /*if (Event.current.type == EventType.Repaint)
        {

            
        }*/
        Transform transform = ((Block)target).transform;
        Block block = ((Block)target);
        Handles.color = Color.green;
        Handles.Label(transform.position + Vector3.up*0.5f + Vector3.left*0.1f,
                        "Pos: "+ block.relativePos.ToString()+"\n"+
                        "ArrayIndex: "+ block.arrayIndex
                        );
    }
}
