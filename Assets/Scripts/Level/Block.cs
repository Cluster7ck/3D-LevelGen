using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    public Openings[] openings;

    public void rotate(int rotation)
    {
        int rot90Deg = rotation/90;
        transform.Rotate(Vector3.up, rotation);

        //DoorDirection[] rotatedOpenings = new DoorDirection[openings.Length];
        for(int i = 0; i < openings.Length; i++)
        {
            if(openings[i].direction != DoorDirection.UP && openings[i].direction != DoorDirection.DOWN)
            {
                openings[i].direction = (DoorDirection)(((int)openings[i].direction + rot90Deg) % 4);
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
