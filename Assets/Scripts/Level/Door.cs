using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

public enum DoorDirection
{
    NORTH,
    EAST,
    SOUTH,
    WEST,
    UP,
    DOWN,
}

[System.Serializable]
public class Door
{
    public RoomType RoomType { get; set; }
    public DoorDirection Direction { get; set; }
    public SerializableVector3 RelativeIndex { get; set; }
    [XmlIgnoreAttribute]
    public Vector3 WorldIndex {
        get { return Room.WorldPosition + RelativeIndex; }
    }

    [XmlIgnoreAttribute]
    public RoomData Room { get; set; }
    [XmlIgnoreAttribute]
    public RoomData ConnectedRoom { get; set; }

    public Door()
    {
        ConnectedRoom = null;
    }

    public Door(Vector3 relativeIndex, DoorDirection direction)
    {
        this.RelativeIndex = relativeIndex;
        this.Direction = direction;
        ConnectedRoom = null;
    }

    public Door(RoomData room, Vector3 relativeIndex)
    {
        this.Room = room;
        this.RelativeIndex = relativeIndex;
        ConnectedRoom = null;
    }

    public void initDoor(RoomData connectedRoom, DoorDirection direction, Vector3 worldIndex)
    {
        this.ConnectedRoom = connectedRoom;
        this.Direction = direction;
    }

    public Door(Door other)
    {
        this.Room = other.Room;
        this.ConnectedRoom = other.ConnectedRoom;
        this.Direction = other.Direction;
        this.RelativeIndex = other.RelativeIndex;
    }

    public void rotateDoorIndex(Vector3 tempBound)
    {
        Vector3 rotatedPosition = RelativeIndex - tempBound / 2;

        rotatedPosition = Quaternion.AngleAxis(90, Vector3.up) * rotatedPosition;
        tempBound = new Vector3(tempBound.z, tempBound.y, tempBound.x);
        rotatedPosition = rotatedPosition + (tempBound / 2);
        RelativeIndex = rotatedPosition;
        if(Direction != DoorDirection.UP && Direction != DoorDirection.DOWN)
        {
            Direction = Direction.next();
        }
    }

    public bool isDoorOpposite(Door door)
    {
        return door.Direction == Direction.opposite();
    }
}