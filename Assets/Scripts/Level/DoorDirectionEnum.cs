using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorDirection
{
    NORTH,
    EAST,
    SOUTH,
    WEST,
    UP,
    DOWN,
}

static class DoorDirectionMethods {

    public static Vector3 DirectionOffset(this DoorDirection direction)
    {
        Vector3 retVec = new Vector3();
        switch (direction)
        {
            case DoorDirection.NORTH:
                retVec = new Vector3(0, 0, 1);
                break;
            case DoorDirection.EAST:
                retVec = new Vector3(1, 0, 0);
                break;
            case DoorDirection.SOUTH:
                retVec = new Vector3(0, 0, -1);
                break;
            case DoorDirection.WEST:
                retVec = new Vector3(-1, 0, 0);
                break;
            case DoorDirection.UP:
                retVec = new Vector3(0, 1, 0);
                break;
            case DoorDirection.DOWN:
                retVec = new Vector3(0, -1, 0);
                break;
            default:
                break;
        }

        return retVec;
    }

    public static RoomIndex DirectionOffsetIndex(this DoorDirection direction)
    {
        RoomIndex retVec = new RoomIndex();
        switch (direction)
        {
            case DoorDirection.NORTH:
                retVec = new RoomIndex(0, 0, 1);
                break;
            case DoorDirection.EAST:
                retVec = new RoomIndex(1, 0, 0);
                break;
            case DoorDirection.SOUTH:
                retVec = new RoomIndex(0, 0, -1);
                break;
            case DoorDirection.WEST:
                retVec = new RoomIndex(-1, 0, 0);
                break;
            case DoorDirection.UP:
                retVec = new RoomIndex(0, 1, 0);
                break;
            case DoorDirection.DOWN:
                retVec = new RoomIndex(0, -1, 0);
                break;
            default:
                break;
        }

        return retVec;
    }

    public static DoorDirection next(this DoorDirection direction)
    {
        if(direction == DoorDirection.UP || direction == DoorDirection.DOWN)
        {
            return direction.opposite();
        }
        else
        {
            return (DoorDirection)(((int)direction + 1) % 4);
        }
    }

    public static DoorDirection rotate90Horizontal(this DoorDirection direction, int rotations = 1)
    {
        if(direction == DoorDirection.UP || direction == DoorDirection.DOWN)
        {
            return direction;
        }

        return (DoorDirection)(((int)direction + rotations) % 4);
    }

    public static DoorDirection opposite(this DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.NORTH:
                return DoorDirection.SOUTH;
            case DoorDirection.EAST:
                return DoorDirection.WEST;
            case DoorDirection.SOUTH:
                return DoorDirection.NORTH;
            case DoorDirection.WEST:
                return DoorDirection.EAST;
            case DoorDirection.UP:
                return DoorDirection.DOWN;
            case DoorDirection.DOWN:
                return DoorDirection.UP;
            default:
                return DoorDirection.NORTH;
        }
    }
}
