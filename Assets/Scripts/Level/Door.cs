using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlType("Door")]
public class Door
{
    [XmlElement("DoorDirection")]
    private DoorDirection direction;
    [XmlElement("LocalIndex")]
    private SerializableVector3 relativeIndex;
    [XmlElement("WorldIndex")]
    private SerializableVector3 worldIndex;
    
    [XmlIgnoreAttribute]
    private Room room;
    [XmlIgnoreAttribute]
    private Room connectedRoom;

    [XmlIgnoreAttribute]
    private GameObject physicalDoor;
    [XmlIgnoreAttribute]
    public GameObject leftDoor { get; set; }
    [XmlIgnoreAttribute]
    public GameObject rightDoor { get; set; }
    [XmlIgnoreAttribute]
    public bool isClosed { get; set; }

    public Door()
    {

    }

    public Door(Vector3 relativeIndex, DoorDirection direction)
    {
        this.relativeIndex = relativeIndex;
        this.direction = direction;
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
