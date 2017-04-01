using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class Door
{
    private Room room;
    private Room connectedRoom;
    private DoorDirection direction;
    private Vector3 relativeIndex;
    private Vector3 worldIndex;

    private GameObject physicalDoor;
    public GameObject leftDoor { get; set; }
    public GameObject rightDoor { get; set; }

    public bool isClosed { get; set; }

    public Door(Vector3 relativeIndex)
    {
        this.relativeIndex = relativeIndex;
    }

    public Door(Room room, Vector3 relativeIndex)
    {
        this.room = room;
        this.relativeIndex = relativeIndex;
    }

    public void initDoor(Room connectedRoom, DoorDirection direction, Vector3 worldIndex)
    {
        this.connectedRoom = connectedRoom;
        this.direction = direction;
        this.worldIndex = worldIndex;
        isClosed = true;
    }

    public Door(Door other)
    {
        this.connectedRoom = other.getConnectedRoom();
        this.direction = other.getDirection();
        this.relativeIndex = other.getRelativeDoorIndex();
        this.worldIndex = other.getWorldDoorIndex();
        this.physicalDoor = other.getPhysicalDoor();
    }

    public Room getConnectedRoom()
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
    }
}
