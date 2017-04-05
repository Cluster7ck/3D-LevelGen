using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMono : MonoBehaviour {
    /*
    private DoorDirection direction;
    private SerializableVector3 relativeIndex;
    private SerializableVector3 worldIndex;
    
    private RoomData room;
    private RoomData connectedRoom;
    
    private GameObject physicalDoor;
    public GameObject leftDoor { get; set; }
    public GameObject rightDoor { get; set; }
    public bool isClosed { get; set; }

    public void initDoor(RoomData connectedRoom, DoorDirection direction, Vector3 worldIndex)
    {
        this.connectedRoom = connectedRoom;
        this.direction = direction;
        this.worldIndex = worldIndex;
        isClosed = true;
    }

    public RoomData getConnectedRoom()
    {
        return this.connectedRoom;
    }

    public DoorDirection getDirection()
    {
        return this.direction;
    }

    public Vector3 getRelativeDoorIndex()
    {
        return this.relativeIndex;
    }

    public Vector3 getWorldDoorIndex()
    {
        return this.worldIndex;
    }

    public void setPhysicalDoor(GameObject physDoor)
    {
        this.physicalDoor = physDoor;
    }

    public GameObject getPhysicalDoor()
    {
        return this.physicalDoor;
    }

    public void FastClose()
    {
        if (isClosed == false)
        {
            isClosed = true;
            Transform leftDoorTrans = leftDoor.transform;
            Transform rightDoorTrans = rightDoor.transform;
            Vector3 startPosLeft = leftDoorTrans.localPosition;
            Vector3 startPosRight = rightDoorTrans.localPosition;
            leftDoorTrans.localPosition = startPosLeft + Vector3.right * 1.1f;
            rightDoorTrans.localPosition = startPosRight + Vector3.right * -1.1f;

            Door connectedDoor = connectedRoom.doors.Find(d => (int)d.getDirection() == ((int)getDirection() + 2) % 4);
            connectedDoor.FastClose();
        }
    }

    public void FastOpen()
    {
        if (isClosed == true)
        {
            isClosed = false;
            Transform leftDoorTrans = leftDoor.transform;
            Transform rightDoorTrans = rightDoor.transform;
            Vector3 startPosLeft = leftDoorTrans.localPosition;
            Vector3 startPosRight = rightDoorTrans.localPosition;
            leftDoorTrans.localPosition = startPosLeft + Vector3.right * -1.1f;
            rightDoorTrans.localPosition = startPosRight + Vector3.right * 1.1f;

            Door connectedDoor = connectedRoom.doors.Find(d => (int)d.getDirection() == ((int)getDirection() + 2) % 4);
            connectedDoor.FastOpen();
        }
    }*/
}
