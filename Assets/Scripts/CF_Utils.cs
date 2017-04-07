using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CF_Utils : MonoBehaviour {
    
    public static GameObject findChildWithTag(GameObject parent,string tag) {
        for (int i = 0; i < parent.transform.childCount; i++) {
            GameObject crnt = parent.transform.GetChild(i).gameObject;
            if (crnt.tag.Equals(tag)) {
                return crnt;
            }
        }
        return null;
    }

    public static List<GameObject> findChildsWithTag(GameObject parent, string tag) {
        List<GameObject> childList = new List<GameObject>();
        for (int i = 0; i < parent.transform.childCount; i++) {
            GameObject crnt = parent.transform.GetChild(i).gameObject;
            if (crnt.tag.Equals(tag)) {
                childList.Add(crnt);
            }
        }
        return childList;
    }

    public static void removeParentFromChild(GameObject child) {
        child.transform.parent = null;
    }
}

public static class Extensions
{

    public static Vector3 DirectionOffset(this DoorDirection direction)
    {
        Vector3 retVec = new Vector3();
        switch (direction)
        {
            case DoorDirection.NORTH:
                retVec = new Vector3(0, 0, 1);
                break;
            case DoorDirection.EAST:
                retVec = new Vector3(1, 0, 0);
                break;
            case DoorDirection.SOUTH:
                retVec = new Vector3(0, 0, -1);
                break;
            case DoorDirection.WEST:
                retVec = new Vector3(-1, 0, 0);
                break;
            default:
                break;
        }

        return retVec;
    }

    public static DoorDirection next(this DoorDirection direction)
    {
        return (DoorDirection)(((int)direction + 1) % 4);
    }
}