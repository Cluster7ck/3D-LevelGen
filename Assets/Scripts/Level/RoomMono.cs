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

    public void rotate90DegClockwise(int rotations)
    {
        
        for (int i = 0; i < rotations; i++)
        {
            rotation = (rotation + 90) % 360;

            int rotatedSizeX = (int)dimensions.x;
            int rotatedSizeY = (int)dimensions.y;
            List<Vector3> rotatedDoors = new List<Vector3>();
            foreach (Door door in doors)
            {
                //Set door index dor.setRellativeIndex()
                rotatedDoors.Add(rotateDoorIndex(door.getRelativeDoorIndex()));
            }

            dimensions.x = rotatedSizeX;
            dimensions.y = rotatedSizeY;
        }
        
    }

    private Vector3 rotateDoorIndex(Vector3 doorIndex)
    {
        //Translate Mid of room to 0,0,0
        //rotate
        //put bottom left at 0,0,0 ^in rotate90DegClockwise
        float halfDiagonal = (Mathf.Sqrt(2) * baseSize) / 2;
        Vector3 halfBaseSizeVec = new Vector3(halfDiagonal, 0, halfDiagonal);
        Vector3 transVector = (doorIndex + halfBaseSizeVec) - new Vector3(upperBound.x, 0, upperBound.z) / 2;
        Vector3 newVector = Quaternion.AngleAxis(90, Vector3.up) * transVector;

        /*
        float rotatedSizeX = dimensions.x;
        float rotatedSizeZ = dimensions.z;
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
        return rotatedDoorIndex;*/
        return new Vector3();
        
    }
}
