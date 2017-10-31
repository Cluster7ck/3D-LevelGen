using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGen3D : MonoBehaviour {

    public int Seed;
    public string StartRoom;
    public int MinimumAmountRooms;

    Transform roomParentTransform;
    RoomCollection roomCollection;
    RoomFactory roomFactory;

    List<RoomData> allRooms = new List<RoomData>();
    Queue<RoomData> openRoomQueue = new Queue<RoomData>();
    Dictionary<RoomIndex, RoomData> indexMap = new Dictionary<RoomIndex, RoomData>();

    //Open doors in world coordinates. Here the index of the door lies within the room it belongs to
    Dictionary<RoomIndex, Door> northDoors = new Dictionary<RoomIndex, Door>();
    Dictionary<RoomIndex, Door> eastDoors = new Dictionary<RoomIndex, Door>();
    Dictionary<RoomIndex, Door> southDoors = new Dictionary<RoomIndex, Door>();
    Dictionary<RoomIndex, Door> westDoors = new Dictionary<RoomIndex, Door>();
    Dictionary<RoomIndex, Door> upDoors = new Dictionary<RoomIndex, Door>();
    Dictionary<RoomIndex, Door> downDoors = new Dictionary<RoomIndex, Door>();

    private bool running = false;

    void Clear()
    {
        allRooms.Clear();
        openRoomQueue.Clear();
        indexMap.Clear();

        northDoors.Clear();
        eastDoors.Clear();
        southDoors.Clear();
        westDoors.Clear();
        upDoors.Clear();
        downDoors.Clear();
    }

    void LoadPreRequisites()
    {
        roomParentTransform = new GameObject("Level").transform;
        roomCollection = RoomCollection.Instance;
        roomCollection.Load(Application.dataPath + "/StreamingAssets/XML/room_collection.xml");
        RoomData.BaseSize = roomCollection.BaseSize;
        roomFactory = new RoomFactory();
    }

    void Start()
    {
        InitGenerate();
        StartCoroutine(generateLoop());
    }

    public void InitGenerate()
    {
        Clear();
        LoadPreRequisites();
        Random.InitState(Seed);

        RoomData startRoom = new RoomData(roomCollection.rooms.Find(x => x.Name == StartRoom));

        Vector3 startIndex = new Vector3(0, 0, 0);
        int startRotation = Random.Range(0, 4);
        startRoom.rotateData_90Deg(startRotation);

        addRoom(startRoom, startIndex);

        running = true;
    }

    public void GenerateLevel()
    {
        Clear();
        LoadPreRequisites();
        Random.InitState(Seed);

        RoomData startRoom = new RoomData(roomCollection.rooms.Find(x => x.Name == StartRoom));

        Vector3 startIndex = new Vector3(0 ,0, 0);
        int startRotation = Random.Range(0, 4);
        startRoom.rotateData_90Deg(startRotation);

        addRoom(startRoom, startIndex);
        //Add 4 or 3 opening room
        while (openRoomQueue.Any() && allRooms.Count < MinimumAmountRooms)
        {
            //|| allRooms.Count() + numOpenDoors() >= MinimumAmountRooms
            if (allRooms.Count() >= MinimumAmountRooms )
            {
                 
            }
            else
            {
                cycleOverRoom();
            }
        }
    }

    IEnumerator generateLoop()
    {
        while (openRoomQueue.Any() && allRooms.Count < MinimumAmountRooms)
        {
            yield return StartCoroutine(cycleOverRoomCo());
        }
        yield return null;
    }

    IEnumerator cycleOverRoomCo()
    {
        RoomData currentRoom = openRoomQueue.Dequeue();

        while (currentRoom.doors.Count == 0 && openRoomQueue.Any())
        {
            currentRoom = openRoomQueue.Dequeue();
        }

        List<Door> doorsToIterate = currentRoom.doors.Where(x => x.ConnectedRoom == null).ToList();
        yield return StartCoroutine(addRoomLoop(currentRoom, doorsToIterate));
    }

    IEnumerator addRoomLoop(RoomData currentRoom, List<Door> doorsToIterate)
    {
        foreach (Door door in doorsToIterate)
        {
            if (allRooms.Count() == 28)
            {
                bool plisBreak = true;
            }
            List<RoomData> roomsToChooseFrom = roomsByType(door.RoomType);
            RoomData nextRoom = randomWeightedRoom(roomsToChooseFrom);
            Vector3 nextRoomIndex = Vector3.zero;
            Door nextDoor = null;
            bool roomFits = false;
            while (!roomFits)
            {
                //Roate RoomData until there is a door on the corresponding side
                int rotations = 0;

                bool fits = false;
                while (!fits && rotations < 4)
                {
                    if (checkRotationFit(nextRoom, door, ref nextDoor))
                    {
                        nextRoomIndex = placeRoom(currentRoom, nextRoom, door, nextDoor);
                        if (nextRoomIndex != Vector3.zero)
                        {
                            //Debug.Log("Room: "+ door.Room+" Door:"+ door.Direction+ " rotation fits and placed at "+nextRoomIndex);
                            fits = checkNeighbors(nextRoom, nextRoomIndex);
                        }
                    }

                    if (!fits)
                    {
                        rotations++;
                        nextRoom.rotateData_90Deg();
                    }
                }

                if (fits)
                {
                    addRoom(nextRoom, nextRoomIndex);

                    roomFits = true;
                }
                else
                {
                    roomsToChooseFrom.Remove(roomsToChooseFrom.Where(x => x.Name == nextRoom.Name).First());
                    if (roomsToChooseFrom.Count == 0)
                    {
                        break;
                    }
                    nextRoom = randomWeightedRoom(roomsToChooseFrom);
                }
            }
            yield return null;
        }
    }

    void cycleOverRoom()
    {
        RoomData currentRoom = openRoomQueue.Dequeue();
        
        while (currentRoom.doors.Count == 0 && openRoomQueue.Any())
        {
            currentRoom = openRoomQueue.Dequeue();
        }

        List<Door> doorsToIterate = currentRoom.doors.Where(x => x.ConnectedRoom == null).ToList();
        foreach (Door door in doorsToIterate)
        {
            if(allRooms.Count() == 28)
            {
                bool plisBreak = true;
            }
            List<RoomData> roomsToChooseFrom = roomsByType(door.RoomType);
            RoomData nextRoom = randomWeightedRoom(roomsToChooseFrom);
            Vector3 nextRoomIndex = Vector3.zero;
            Door nextDoor = null;
            bool roomFits = false;
            while (!roomFits)
            {
                //Roate RoomData until there is a door on the corresponding side
                int rotations = 0;

                bool fits = false;
                while (!fits && rotations<4)
                {
                    if (checkRotationFit(nextRoom,door, ref nextDoor))
                    {
                        nextRoomIndex = placeRoom(currentRoom, nextRoom, door, nextDoor);
                        if (nextRoomIndex != Vector3.zero)
                        {
                            //Debug.Log("Room: "+ door.Room+" Door:"+ door.Direction+ " rotation fits and placed at "+nextRoomIndex);
                            fits = checkNeighbors(nextRoom, nextRoomIndex);
                        }
                    }

                    if(!fits)
                    {
                        rotations++;
                        nextRoom.rotateData_90Deg();
                    }
                }

                if (fits)
                {
                    addRoom(nextRoom, nextRoomIndex);

                    roomFits = true;
                }
                else
                {
                    roomsToChooseFrom.Remove(roomsToChooseFrom.Where(x => x.Name == nextRoom.Name).First());
                    if(roomsToChooseFrom.Count == 0)
                    {
                        break;
                    }
                    nextRoom = randomWeightedRoom(roomsToChooseFrom);
                }
            }
        }
    }

    void addRoom(RoomData room, Vector3 index)
    {
        room.Index = index;
        Debug.Log("Added Room: " + room + " at " + index);
        openRoomQueue.Enqueue(room);
        addDoorsToOpenDoors(room);
        addRoomToDict(room);
        allRooms.Add(room);
        roomFactory.CreateRoomObject(room,roomParentTransform, allRooms.Count);
    }

    void addRoomToDict(RoomData room)
    {
        //Block all Indices
        for (int x = room.Index.x; x < room.Index.x + room.Dimensions.x; x++)
        {
            for (int y = room.Index.y; y < room.Index.y + room.Dimensions.y; y++)
            {
                for (int z = room.Index.z; z < room.Index.z + room.Dimensions.z; z++)
                {
                    indexMap.Add(new Vector3(x, y, z), room);
                }
            }
        }
    }

    void addDoorsToOpenDoors(RoomData room)
    {
        foreach(Door door in room.doors)
        {
            Vector3 doorWorldIndex = door.WorldIndex;
            if(door.ConnectedRoom == null)
            {
                switch (door.Direction)
                {
                    case DoorDirection.NORTH:
                        doorWorldIndex.z -= 1;
                        break;
                    case DoorDirection.EAST:
                        doorWorldIndex.x -= 1;
                        break;
                    case DoorDirection.SOUTH:
                        doorWorldIndex.z += 1;
                        break;
                    case DoorDirection.WEST:
                        doorWorldIndex.x += 1;
                        break;
                    case DoorDirection.UP:
                        doorWorldIndex.y -= 1;
                        break;
                    case DoorDirection.DOWN:
                        doorWorldIndex.y += 1;
                        break;
                    default:
                        break;
                }
                getDoorDictionary(door.Direction).Add(doorWorldIndex, door);
            }
        }
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

    RoomIndex placeRoom(RoomData prevRoom, RoomData nextRoom, Door prevRoomDoor, Door nextRoomDoor)
    {
        bool fits = true;
        RoomIndex roomPos = prevRoomDoor.WorldIndex;

        switch (prevRoomDoor.Direction)
        {
            case DoorDirection.NORTH:
                roomPos.x -= nextRoomDoor.RelativeIndex.x;
                roomPos.y -= nextRoomDoor.RelativeIndex.y;
                break;
            case DoorDirection.EAST:
                roomPos.y -= nextRoomDoor.RelativeIndex.y;
                roomPos.z -= nextRoomDoor.RelativeIndex.z;
                break;
            case DoorDirection.SOUTH:
                roomPos.x -= nextRoomDoor.RelativeIndex.x;
                roomPos.y -= nextRoomDoor.RelativeIndex.y;
                roomPos.z -= nextRoom.UpperBounds.z;
                break;
            case DoorDirection.WEST:
                roomPos.x -= nextRoom.UpperBounds.x;
                roomPos.y -= nextRoomDoor.RelativeIndex.y;
                roomPos.z -= nextRoomDoor.RelativeIndex.z;
                break;
            default:
                break;
        }


        for (int x = roomPos.x; x < roomPos.x + nextRoom.Dimensions.x; x++)
        {
            for (int y = roomPos.y; y < roomPos.y + nextRoom.Dimensions.y; y++)
            {
                for (int z = roomPos.z; z < roomPos.z + nextRoom.Dimensions.z; z++)
                {
                    RoomData tryRoom;
                    if (indexMap.TryGetValue(new Vector3(x, y, z), out tryRoom))
                    {
                        fits = false;
                    }
                }
            }
        }
        nextRoom.Index = roomPos;
        foreach(Door door in nextRoom.doors)
        {
            RoomData tryRoom;
            if (indexMap.TryGetValue(door.WorldIndex, out tryRoom))
            {
                Door tryDoor;
                if(!getDoorDictionary(door.Direction.opposite()).TryGetValue(door.WorldIndex, out tryDoor))
                {
                    fits = false;
                }
            }
        }

        if (fits)
        {
            return roomPos;
        }
        nextRoom.Index = Vector3.zero;
        return Vector3.zero;
    }

    bool checkNeighbors(RoomData nextRoom, RoomIndex startIndex)
    {
        bool fits = true;
        int startX = startIndex.x - 1;
        int startY = startIndex.y - 1;
        int startZ = startIndex.z - 1;
        int endX = startIndex.x + nextRoom.Dimensions.x;
        int endY = startIndex.y + nextRoom.Dimensions.y;
        int endZ = startIndex.z + nextRoom.Dimensions.z;
        List<DoorHelperClass> adjacentDoors = new List<DoorHelperClass>();
        //List<Door> ownDoors = new List<Door>();
        Dictionary<Door, Door> ownDoors = new Dictionary<Door, Door>();
        nextRoom.Index = startIndex;

        foreach (Door door in nextRoom.doors)
        {
            Door tryDoor;
            DoorDirection dir = door.Direction.opposite();
            if (getDoorDictionary(dir).TryGetValue(door.WorldIndex, out tryDoor))
            {
                //remember the doors so we can close them later
                //temporarily remove from doorDictionarys
                adjacentDoors.Add(new DoorHelperClass(tryDoor, door.WorldIndex));
                getDoorDictionary(tryDoor.Direction).Remove(door.WorldIndex);
                ownDoors.Add(door,tryDoor);
            }
        }

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                for(int z = startZ; z <= endZ; z++)
                {
                    if ( (x == startX || x == endX) && (y != startY && y != endY) && (z != startZ && z != endZ) )
                    {
                        if(x == startX)
                        {
                            Vector3 neighborDoor = new Vector3(x, y, z);
                            Door doorAtIndex;
                            if (eastDoors.TryGetValue(neighborDoor, out doorAtIndex))
                            {
                                fits = false;
                            }
                        }
                        else if(x == endX)
                        {
                            Vector3 neighborDoor = new Vector3(x, y, z);
                            Door doorAtIndex;
                            if (westDoors.TryGetValue(neighborDoor, out doorAtIndex))
                            {
                                fits = false;
                            }
                        }
                    }
                    else if ( (y == startY || y == endY) && (x != startX && x != endX) && (z != startZ && z != endZ) )
                    {
                        if (y == startY)
                        {
                            Vector3 neighborDoor = new Vector3(x, y, z);
                            Door doorAtIndex;
                            if (upDoors.TryGetValue(neighborDoor, out doorAtIndex))
                            {
                                fits = false;
                            }
                        }
                        else if (y == endY)
                        {
                            Vector3 neighborDoor = new Vector3(x, y, z);
                            Door doorAtIndex;
                            if (downDoors.TryGetValue(neighborDoor, out doorAtIndex))
                            {
                                fits = false;
                            }
                        }
                    }
                    else if ( (z == startZ || z == endZ) && (y != startY && y != endY) && (x != startX && x != endX) )
                    {
                        if (z == startZ)
                        {
                            Vector3 neighborDoor = new Vector3(x, y, z);
                            Door doorAtIndex;
                            if (northDoors.TryGetValue(neighborDoor, out doorAtIndex))
                            {
                                fits = false;
                            }
                        }
                        else if (z == endZ)
                        {
                            Vector3 neighborDoor = new Vector3(x, y, z);
                            Door doorAtIndex;
                            if (southDoors.TryGetValue(neighborDoor, out doorAtIndex))
                            {
                                fits = false;
                            }
                        }
                    }
                }
            }
        }

        if (!fits)
        {
            nextRoom.Index = null;
            foreach (DoorHelperClass dh in adjacentDoors)
            {
                getDoorDictionary(dh.door.Direction).Add(dh.doorIndex, dh.door);
            }
        }
        else
        {
            foreach(KeyValuePair<Door,Door> entry in ownDoors)
            {
                entry.Key.ConnectedRoom = entry.Value.Room;
                entry.Value.ConnectedRoom = entry.Key.Room;
            }
        }

        return fits;
    }
    
    bool checkIndexForDoors()
    {


        return false;
    }

    bool checkRoomSide(RoomData nextRoom, RoomIndex doorIndex, DoorDirection direction, Dictionary<RoomIndex, Door> doorDictionary , ref List<DoorHelperClass> doorsToClose)
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

    Dictionary<RoomIndex, Door> getDoorDictionary(DoorDirection direction)
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
                return new Dictionary<RoomIndex, Door>();
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