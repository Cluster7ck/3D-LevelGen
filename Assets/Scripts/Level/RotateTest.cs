using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTest : MonoBehaviour {

    public float baseSize;
    public Vector3 upperBound;
    public Vector3 lowerBound;
    public RoomMono room;

    public void rotateMe()
    {
        Vector3 rotatedPosition = this.transform.position - upperBound/2;

        rotatedPosition = Quaternion.AngleAxis(90, Vector3.up) * rotatedPosition;
        upperBound = new Vector3(upperBound.z, upperBound.y, upperBound.x);
        this.transform.position = rotatedPosition + (upperBound/2);
    }

    public void rotateMeForrealz()
    {
        this.transform.Rotate(Vector3.up, 90, Space.World);
    }

    public void rotate90()
    {
        this.transform.Rotate(Vector3.up, 90, Space.Self);

        foreach(Transform child in transform)
        {
            Debug.Log(child.name + ": " + child.transform.position);
        }
    }

    public void rotateRoom()
    {
        room.rotate90Deg(1);
    }
}
