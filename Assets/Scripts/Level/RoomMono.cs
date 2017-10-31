using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMono : MonoBehaviour {
    public float baseSize;

    public string roomName;
    public RoomType type;
    public int rotation;
    public float relativeChance;

    public RoomIndex dimensions;
    public RoomIndex upperBound;
    public RoomIndex lowerBound;
    public RoomIndex Index;

    public int pathLength = 0;
    public List<Door> doors = new List<Door>();

    public void init(float baseSize, string roomName, float relativeChance, int rotation, Vector3 dimensions, Vector3 lowerBound, Vector3 upperBound, RoomIndex Index)
    {
        this.baseSize = baseSize;
        this.roomName = roomName;
        this.relativeChance = relativeChance;
        this.rotation = rotation;
        this.dimensions = dimensions;
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
        this.Index = Index;
        this.transform.position = new Vector3(Index.x, Index.y, Index.z);
        rotateGameObject();
    }

    public void rotate90Deg(int rotations)
    {
        
        for (int i = 0; i < rotations; i++)
        {
            rotation = (rotation + 90) % 360;

            foreach (Door door in doors)
            {
                door.rotateDoorIndex(upperBound);
            }

            upperBound = new Vector3(upperBound.z, upperBound.y, upperBound.x);
            dimensions = new Vector3(dimensions.z, dimensions.y, dimensions.x);
        }
        rotateGameObject();
    }

    private void rotateGameObject()
    {
        this.transform.rotation = new Quaternion();
        this.transform.Rotate(Vector3.up, rotation);

        Vector3 offset;
        switch (rotation)
        {
            case 0:
                offset = new Vector3(0, 0, 0);
                break;
            case 90:
                offset = new Vector3(0, 0, upperBound.z);
                break;
            case 180:
                offset = new Vector3(upperBound.x, 0, upperBound.z);
                break;
            case 270:
                offset = new Vector3(upperBound.x, 0, 0);
                break;
            default:
                offset = new Vector3(0, 0, 0);
                break;
        }
        this.transform.position = this.transform.position + offset;
    }

}
