using UnityEngine;
using System.Collections;

public class RunGenerator : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        this.GetComponent<LevelGen3D>().GenerateLevel();
    }
}
