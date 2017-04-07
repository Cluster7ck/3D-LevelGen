using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGen3D : MonoBehaviour {

    public int Seed { get; set; }
    public string StartRoom { get; set;}
    public int MinimumAmountRooms { get; set; }

    Transform roomParentTransform;
    RoomCollection roomCollection;

    List<RoomData> allRooms = new List<RoomData>();
    Queue<RoomData> openRoomQueue = new Queue<RoomData>();
    Dictionary<Vector3, RoomData> indexMap = new Dictionary<Vector3, RoomData>();

    //Open doors in world coordinates. Here the index of the door lies within the room it belongs to
    Dictionary<Vector3, Door> northDoors = new Dictionary<Vector3, Door>();
    Dictionary<Vector3, Door> eastDoors = new Dictionary<Vector3, Door>();
    Dictionary<Vector3, Door> southDoors = new Dictionary<Vector3, Door>();
    Dictionary<Vector3, Door> westDoors = new Dictionary<Vector3, Door>();
    Dictionary<Vector3, Door> upDoors = new Dictionary<Vector3, Door>();
    Dictionary<Vector3, Door> downDoors = new Dictionary<Vector3, Door>();
    Dictionary<Vector3, List<Door>> openDoors = new Dictionary<Vector3, List<Door>>();

    void LoadPreRequisites()
    {
        roomParentTransform = new GameObject("Level").transform;
        roomCollection = RoomCollection.Instance;
        RoomData.BaseSize = roomCollection.BaseSize;
    }

    public void GenerateLevel()
    {
        LoadPreRequisites();
        Random.InitState(Seed);

        RoomData startRoom = new RoomData(roomCollection.rooms.Find(x => x.Name == StartRoom));

        Vector3 startIndex = new Vector3(0 ,0, 0);
        int startRotation = Random.Range(0, 4);
        startRoom.rotateData_90Deg(startRotation);

        addRoom(startRoom, null, startIndex);
        //Add 4 or 3 opening room
        while (openRoomQueue.Any())
        {
            if (allRooms.Count() >= MinimumAmountRooms || allRooms.Count() + numOpenDoors() >= MinimumAmountRooms)
            { 
            }
            cycleOverRoom();
        }
    }

    void cycleOverRoom()
    {
        bool reroll = false;
        RoomData currentRoom = openRoomQueue.Dequeue();
        
        while (currentRoom.doors.Count == 0 && openRoomQueue.Any())
        {
            currentRoom = openRoomQueue.Dequeue();
        }

        foreach(Door door in currentRoom.doors)
        {
            List<RoomData> roomsToChooseFrom = roomsByType(door.RoomType);
            RoomData nextRoom = randomWeightedRoom(roomsToChooseFrom);
            Door nextDoor = null;
            bool roomFits = false;
            while (!roomFits)
            {
                //Roate RoomData until there is a door on the corresponding side
                int rotations = 0;
                bool hasFittingDoor = false;
                while (!hasFittingDoor && rotations<4)
                {
                    if (checkRotationFit(nextRoom,door, ref nextDoor))
                    {
                        hasFittingDoor = true;
                    }
                    else
                    {
                        rotations++;
                        nextRoom.rotateData_90Deg(1);
                    }
                }

                if (hasFittingDoor && nextDoor != null)
                {
                    //see if dimensions fit
                    Vector3 nextRoomIndex = placeRoom(currentRoom, nextRoom, door, nextDoor);

                    if (nextRoomIndex != Vector3.zero)
                    {
                        //Check Neighbors
                        bool neighborDoorsFit = false;
                        //take prior rotations into account

                        while (!neighborDoorsFit && rotations < 4)
                        {
                            neighborDoorsFit = checkNeighbors(nextRoom, nextRoomIndex, currentRoom);

                            if (neighborDoorsFit == false)
                            {
                                rotations++;
                                if (rotations < 4)
                                {
                                    nextRoom.rotateData_90Deg(1);
                                    nextRoomIndex = placeRoom(currentRoom, nextRoom, door, nextDoor);
                                }

                            }
                        }


                        if (nextRoomIndex.x == 0 && nextRoomIndex.y == 0)
                        {
                            neighborDoorsFit = false;
                        }

                        if (neighborDoorsFit)
                        {
                            addRoom(nextRoom, currentRoom, nextRoomIndex);

                            roomFits = true;
                        }
                        else
                        {
                            reroll = true;
                        }

                    }
                    else
                    {
                        reroll = true;
                    }
                }
                else
                {
                    reroll = true;
                }
                if (reroll)
                {
                    roomsToChooseFrom.Remove(nextRoom);
                    nextRoom = randomWeightedRoom(roomsToChooseFrom);
                }
            }
        }
    }

    void addRoom(RoomData room, RoomData prevRoom, Vector3 index)
    {
        /*
        openRoomQueue.Enqueue(room);
        addDoorsToOpenDoors(room);
        addRoomToDict(room);
        allRooms.Add(room);
        */
    }

    RoomData randomWeightedRoom(List<RoomData> rooms)
    {
        //http://answers.unity3d.com/questions/190249/random-instantion-from-array-with-weighted-frequen.html
        float totalFreq = 0;
        foreach (RoomData room in rooms)
        {
            totalFreq += room.RelativeChance;
        }
        float roll = Random.Range(0, totalFreq);
        int index = -1;
        for (int i = 0; i < rooms.Count; i++)
        {
            if (roll <= rooms[i].RelativeChance)
            {
                index = i;
                break;
            }
            roll -= rooms[i].RelativeChance;
        }
        // just in case we manage to roll 0.0001 past the highest:
        if (index == -1)
            index = rooms.Count - 1;

        return new RoomData(rooms[index]);
    }

    Vector3 placeRoom(RoomData prevRoom, RoomData nextRoom, Door prevRoomDoor, Door nextRoomDoor)
    {
        bool fits = true;

        Vector3 roomPos = prevRoomDoor.WorldIndex;
        Vector3 extents = new Vector3(0, 0, 0);

        switch (prevRoomDoor.Direction)
        {
            case DoorDirection.NORTH:
                extents.z = 1;
                break;
            case DoorDirection.EAST:
                extents.x = 1;
                break;
            case DoorDirection.SOUTH:
                extents.z = -1;
                break;
            case DoorDirection.WEST:
                extents.x = -1;
                break;
            default:
                break;

        }
        
        if (extents.z == 1)
        {
            roomPos.x -= nextRoomDoor.RelativeIndex.x;
            roomPos.y -= nextRoomDoor.RelativeIndex.y;
        }
        else if (extents.x == 1)
        {
            roomPos.y -= nextRoomDoor.RelativeIndex.y;
            roomPos.z -= nextRoomDoor.RelativeIndex.z;
        }
        else if (extents.z == -1)
        {
            roomPos.x -= nextRoomDoor.RelativeIndex.x;
            roomPos.y -= nextRoomDoor.RelativeIndex.y;
            roomPos.z -= nextRoom.UpperBounds.z;
        }
        else if (extents.x  == -1)
        {
            roomPos.x -= nextRoom.UpperBounds.x;
            roomPos.y -= nextRoomDoor.RelativeIndex.y;
            roomPos.z -= nextRoomDoor.RelativeIndex.z;
        }


        for (int x = (int)roomPos.x; x <= (int)(roomPos.x + nextRoom.UpperBounds.x); x++)
        {
            for (int y = (int)roomPos.y; y <= (int)(roomPos.y + nextRoom.UpperBounds.y); y++)
            {
                for (int z = (int)roomPos.z; z <= (int)(roomPos.z + nextRoom.UpperBounds.z); z++)
                {
                    RoomData tryRoom;
                    if (indexMap.TryGetValue(new Vector3(x, y, z), out tryRoom))
                    {
                        fits = false;
                    }
                }
            }
        }

        if (fits)
        {
            return roomPos;
        }

        return Vector3.zero;
    }

    bool checkNeighbors(RoomData room, Vector3 startIndex, RoomData nextRoom)
    {
        bool fits = true;
        int startX = (int)startIndex.x - 1;
        int startY = (int)startIndex.y - 1;
        int startZ = (int)startIndex.z - 1;
        int endX = (int)startIndex.x + (int)room.Dimensions.x;
        int endY = (int)startIndex.y + (int)room.Dimensions.x;
        int endZ = (int)startIndex.z + (int)room.Dimensions.z;
        List<DoorHelperClass> doorsToClose = new List<DoorHelperClass>();

        foreach(Door door in nextRoom.doors)
        {
            Door tryDoor;
            if(getDoorDictionary(door.Direction).TryGetValue(door.WorldIndex, out tryDoor))
            {
                //remember the doors so we can close them later
                //temporarily remove from doorDictionarys
            }
        }

        //Do normal check. There should be no doors found (the ones that fit are already handled)

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                Vector3 neighborDoorNorth = new Vector3(x, y, startZ);
                Vector3 neighborDoorSouth = new Vector3(x, y, endZ);

                fits =  checkRoomSide(nextRoom, neighborDoorNorth, DoorDirection.SOUTH, northDoors, ref doorsToClose) &&
                        checkRoomSide(nextRoom, neighborDoorSouth, DoorDirection.NORTH, southDoors, ref doorsToClose);
            }
        }

        for (int z = startZ; z<= endZ; z++)
        {
            for (int y = startY; y <= endY; y++)
            {
                Vector3 neighborDoorEast = new Vector3(startX, y, z);
                Vector3 neighborDoorWest = new Vector3(endX, y, z);
                Door doorAtIndex;

                fits =  checkRoomSide(nextRoom, neighborDoorEast, DoorDirection.WEST, eastDoors, ref doorsToClose) &&
                        checkRoomSide(nextRoom, neighborDoorWest, DoorDirection.EAST, westDoors, ref doorsToClose);
            }
        }

        for (int x = startX; x <= endX; x++)
        {
            for (int z = startZ; z <= endZ; z++)
            {
                Vector3 neighborDoorUp = new Vector3(x, startY, z);
                Vector3 neighborDoorDown = new Vector3(x, endY, z);

                fits =  checkRoomSide(nextRoom, neighborDoorUp, DoorDirection.DOWN, upDoors, ref doorsToClose) &&
                        checkRoomSide(nextRoom, neighborDoorDown, DoorDirection.UP, downDoors, ref doorsToClose);
            }
        }

        if (fits)
        {
            foreach (DoorHelperClass dh in doorsToClose)
            {
                getDoorDictionary(dh.door.Direction).Remove(dh.doorIndex);
            }
        }

        return fits;
    }
    
    bool checkRoomSide(RoomData nextRoom, Vector3 doorIndex, DoorDirection direction, Dictionary<Vector3, Door> doorDictionary , ref List<DoorHelperClass> doorsToClose)
    {
        Door doorAtIndex;

        bool hasCorrespondingDoor = true;
        if (doorDictionary.TryGetValue(doorIndex, out doorAtIndex))
        {
            bool found = false;
            foreach (Door door in nextRoom.doors)
            {
                if (door.Direction == direction && door.WorldIndex == doorIndex)
                {
                    doorsToClose.Add(new DoorHelperClass(doorAtIndex, doorIndex));
                    found = true;
                }
            }
            if (!found)
            {
                hasCorrespondingDoor = false;
            }
        }

        return hasCorrespondingDoor;
    }

    bool checkRotationFit(RoomData nextRoom, Door currentDoor ,ref Door connectDoor)
    {
        bool hasFittingDoor = false;

        foreach (Door nextRoomDoor in nextRoom.doors)
        {
            //Check if it is an opposing door
            if (nextRoomDoor.isDoorOpposite(currentDoor))
            {
                hasFittingDoor = true;
                connectDoor = nextRoomDoor;
            }
        }

        return hasFittingDoor;
    }

    int numOpenDoors()
    {
        int count = 0;
        foreach(RoomData room in openRoomQueue)
        {
            count += room.doors.Count;
        }
        return count;
    }
    
    List<RoomData> roomsByType(params  RoomType[] types)
    {
        List<RoomData> rooms = new List<RoomData>();
        foreach(RoomData room in RoomCollection.Instance.rooms.Where(x => types.Contains(x.Type)).ToList())
        {
            rooms.Add(new RoomData(room));
        }

        return rooms;
    }

    List<RoomData> roomsByType(RoomType type)
    {
        List<RoomData> rooms = new List<RoomData>();
        List<RoomData> copyList = RoomCollection.Instance.rooms.Where(x => x.Type == type).ToList();
        foreach (RoomData room in copyList)
        {
            rooms.Add(new RoomData(room));
        }

        return rooms;
    }

    Dictionary<Vector3, Door> getDoorDictionary(DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.NORTH:
                return northDoors;
            case DoorDirection.EAST:
                return eastDoors;
            case DoorDirection.SOUTH:
                return southDoors;
            case DoorDirection.WEST:
                return westDoors;
            case DoorDirection.UP:
                return upDoors;
            case DoorDirection.DOWN:
                return downDoors;
            default:
                return new Dictionary<Vector3, Door>();
        }
    }

}

public class DoorHelperClass
{
    public Door door;
    public Vector3 doorIndex;
    public DoorHelperClass(Door door, Vector3 doorIndex)
    {
        this.door = door;
        this.doorIndex = doorIndex;
    }
}