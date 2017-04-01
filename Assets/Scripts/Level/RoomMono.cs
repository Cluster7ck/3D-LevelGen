using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMono : MonoBehaviour {
    static public float baseSize;

    public string roomName;
    public int rotation;
    public float relativeChance;

    public Vector3 dimensions;
    public Vector3 upperBound;
    public Vector3 lowerBound;

    public int pathLength = 0;
    public List<Door> doors = new List<Door>();

    private Vector3 rotateDoorIndices(Vector3 doorIndex, float sizeXR, float sizeZR)
    {
        float rotatedSizeX = sizeZR;
        float rotatedSizeZ = sizeXR;
        //Debug.Log("Before rotate: (" + doorIndex.x + ", " + doorIndex.z + ")");
        Vector3 rotatedDoorIndex = new Vector3();
        if (doorIndex.x == lowerBound.x-1)
        {
            rotatedDoorIndex.z = rotatedSizeZ;
            rotatedDoorIndex.x = doorIndex.z;
        }
        else if (doorIndex.x == sizeXR)
        {
            rotatedDoorIndex.z = -1;
            rotatedDoorIndex.x = doorIndex.z;
        }

        if (doorIndex.z == lowerBound.z - 1)
        {
            rotatedDoorIndex.x = -1;
            rotatedDoorIndex.z = (sizeXR - 1) - doorIndex.x;
        }
        else if (doorIndex.z == sizeZR)
        {
            rotatedDoorIndex.x = rotatedSizeX;
            rotatedDoorIndex.z = (sizeXR - 1) - doorIndex.x;
        }
        return rotatedDoorIndex;
    }
}
