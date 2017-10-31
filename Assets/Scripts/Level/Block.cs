using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Block : MonoBehaviour {
    public Openings[] openings;
    public int arrayIndex;
    public RoomIndex relativePos;

    public void rotate(int rotation)
    {
        int rot90Deg = rotation/90;
        transform.Rotate(Vector3.up, rotation);

        for(int i = 0; i < openings.Length; i++)
        {
            if(openings[i].direction != DoorDirection.UP && openings[i].direction != DoorDirection.DOWN)
            {
                openings[i].direction =  openings[i].direction.rotate90Horizontal(rot90Deg);
            }
        }
    }

    [System.Serializable]
    public struct Openings{
        public DoorDirection direction;
        public float yChange;
        public RoomType type;
    }
}
