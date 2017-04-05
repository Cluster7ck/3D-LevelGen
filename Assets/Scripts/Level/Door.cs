using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class Door
{
    public DoorDirection Direction { get; set; }
    public SerializableVector3 RelativeIndex { get; set; }
    public SerializableVector3 WorldIndex { get; set; }

    [XmlIgnoreAttribute]
    public RoomData Room { get; set; }
    [XmlIgnoreAttribute]
    public RoomData ConnectedRoom { get; set; }

    public Door()
    {

    }

    public Door(Vector3 relativeIndex, DoorDirection direction)
    {
        this.RelativeIndex = relativeIndex;
        this.Direction = direction;
    }

    public Door(RoomData room, Vector3 relativeIndex)
    {
        this.Room = room;
        this.RelativeIndex = relativeIndex;
    }

    public void initDoor(RoomData connectedRoom, DoorDirection direction, Vector3 worldIndex)
    {
        this.ConnectedRoom = connectedRoom;
        this.Direction = direction;
        this.WorldIndex = worldIndex;
    }

    public Door(Door other)
    {
        this.Room = other.Room;
        this.ConnectedRoom = other.ConnectedRoom;
        this.Direction = other.Direction;
        this.RelativeIndex = other.RelativeIndex;
        this.WorldIndex = other.WorldIndex;
    }

    public void rotateDoorIndex(Vector3 tempBound)
    {
        Vector3 rotatedPosition = RelativeIndex - tempBound / 2;

        rotatedPosition = Quaternion.AngleAxis(90, Vector3.up) * rotatedPosition;
        tempBound = new Vector3(tempBound.z, tempBound.y, tempBound.x);
        rotatedPosition = rotatedPosition + (tempBound / 2);
        RelativeIndex = rotatedPosition;
    }
}
