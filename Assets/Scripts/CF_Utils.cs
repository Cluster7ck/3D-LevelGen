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