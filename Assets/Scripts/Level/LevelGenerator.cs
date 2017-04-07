using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    /*
    public string RoomDefinition;
    public int Seed { get; set; }
    public int MinimumAmountRooms { get; set; }
    public Room endRoom;
    //public int PathLength { get; set; }

    Transform roomParentTransform;
    RoomCollection collection;
    List<Room> roomsToChooseFrom = new List<Room>();
    Dictionary<RoomIndex, Room> modules = new Dictionary<RoomIndex, Room>();
    List<Room> allRooms = new List<Room>();

    Dictionary<string, GameObject> roomPrefabs = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> minimapPrefabs = new Dictionary<string, GameObject>();

    Queue<Room> openRoomQueue = new Queue<Room>();

    //Open doors in world coordinates. The Index lies within the corresponding room
    Dictionary<RoomIndex, Room> northDoors = new Dictionary<RoomIndex, Room>();
    Dictionary<RoomIndex, Room> eastDoors = new Dictionary<RoomIndex, Room>();
    Dictionary<RoomIndex, Room> southDoors = new Dictionary<RoomIndex, Room>();
    Dictionary<RoomIndex, Room> westDoors = new Dictionary<RoomIndex, Room>();

    RoomIndex lowerBounds = new RoomIndex(0, 0);
    RoomIndex upperBounds = new RoomIndex(0, 0);
    bool roomsForClosingLevel = false;
    bool firstRoundClosingLevel = false;
    

    void LoadPreRequisites()
    {
        roomParentTransform = GameObject.Find("Rooms").transform;
        this.collection = RoomCollection.Load(Path.Combine(Application.streamingAssetsPath, "Xml/"+RoomDefinition));
        collection.PostDeserialization();
        Room.setBaseSize(collection.baseSize);

        foreach (Room room in collection.rooms)
        {
            roomsToChooseFrom.Add(new Room(room));
        }

        LoadPrefabs();
    }

    void LoadPrefabs()
    {
        if (collection.rooms != null)
        {
            for (int i = 0; i < collection.rooms.Length; i++)
            {
                roomPrefabs.Add(collection.rooms[i].name, Resources.Load("LevelElements/Rooms/" + collection.rooms[i].name) as GameObject);
                string substring = "Basic/";
                minimapPrefabs.Add(collection.rooms[i].name, Resources.Load("LevelElements/Rooms/Minimap/" + collection.rooms[i].name.Substring(substring.Length)) as GameObject);
            }
        }
    }

    public void GenerateLevel()
    {
        LoadPreRequisites();
        Random.InitState(Seed);
        //collection.rooms.Single(r => r.name == "StartRoom");
        //REPLACE x => x.doorIndices.Count == 1 with x => x.name == "StartRoom"
        Room startRoom = new Room(roomsToChooseFrom.Find(x => x.name == "Basic/StartRoom"));
        roomsToChooseFrom.Remove(roomsToChooseFrom.Single(x => x.name == "Basic/StartRoom"));
        roomsToChooseFrom.Remove(roomsToChooseFrom.Single(x => x.name == "Basic/EndRoom"));
        RoomIndex startIndex = new RoomIndex(0, 0);
        int startRotation = Random.Range(0, 4);
        startRoom.rotate90DegClockwise(startRotation);

        addRoom(startRoom, null, startIndex);
        //Add 4 or 3 opening room
        while (openRoomQueue.Any())
        {
            if ((allRooms.Count() >= MinimumAmountRooms || allRooms.Count() + openRoomQueue.Count() >= MinimumAmountRooms) && roomsForClosingLevel == false)
            {
                roomsForClosingLevel = true;
                firstRoundClosingLevel = true;
                roomsToChooseFrom.Clear();
                foreach (Room room in collection.rooms)
                {
                    if ((room.sizeX == 1 && room.sizeY == 1 || room.doorIndices.Count == 1) && (room.name != "Basic/StartRoom" && room.name != "Basic/EndRoom"))
                    {
                        roomsToChooseFrom.Add(new Room(room));
                    }
                }

                foreach (Room room in roomsToChooseFrom)
                {
                    if (room.doorIndices.Count == 1)
                    {
                        room.floatChance = 1000;
                    }
                    else
                    {
                        room.floatChance = 0.01f;
                    }
                }
            }
            cycleOverRoom();
        }

        //placeEndRoom();
        addDoorAdjacency();
    }

    void cycleOverRoom()
    {
        bool reroll = false;
        Room currentRoom = openRoomQueue.Dequeue();
        
        while(currentRoom.doorIndices.Count == 0 && openRoomQueue.Any())
        {
            currentRoom = openRoomQueue.Dequeue();
        }

        for (int i = currentRoom.doorIndices.Count - 1; i >= 0; i--)
        {
            bool roomFits = false;
            //Get door orientation and set startIndex of room
            DoorDirection doorDirection = getDoorDirection(currentRoom.doorIndices[i], currentRoom);
            Room nextRoom ;
            List<Room> chooseFromRoom = getRoomsFullList();
            //prevent 1 door rooms from spawning as long as the level isn't supposed to close off the remaining openings
            /*if (!closingOffLevel)
            {
                chooseFromRoom.Remove(chooseFromRoom.Find(x => x.doorIndices.Count == 1));
            }
            */
            //Force first room after startRoom to have 1x1 dimensions and 4 doors
            /*if (allRooms.Count == 1)
            {
                nextRoom = new Room(chooseFromRoom.Find(x => x.doorIndices.Count == 4 && x.sizeX == 1));
            }
            else
            {
                if (!firstRoundClosingLevel) {
                    nextRoom = randomWeightedRoom(chooseFromRoom);
                } 
                else {
                    firstRoundClosingLevel = false;
                    nextRoom = collection.rooms.First(x => x.name == "Basic/EndRoom");
                    endRoom = nextRoom;
                }
                
            }

            while (!roomFits)
            {
                bool hasFittingDoor = false;
                RoomIndex connectDoor = new RoomIndex(0, 0);

                int triedRotations = 0;
                //Door check
                while (triedRotations < 4 && hasFittingDoor == false)
                {
                    hasFittingDoor = checkRotationFit(ref nextRoom, doorDirection, ref connectDoor);

                    if (hasFittingDoor == false)
                    {
                        triedRotations++;
                        if (triedRotations < 4)
                            nextRoom.rotate90DegClockwise(1);
                    }

                }

                if (hasFittingDoor)
                {
                    //see if dimensions fit
                    RoomIndex nextRoomIndex = placeRoom(currentRoom, nextRoom, currentRoom.doorIndices[i], connectDoor, doorDirection);

                    if (!(nextRoomIndex.x == 0 && nextRoomIndex.y == 0))
                    {
                        //Check Neighbors
                        bool neighborDoorsFit = false;
                        //take prior rotations into account
                        int rotations = triedRotations;

                        while (!neighborDoorsFit && rotations < 4)
                        {
                            neighborDoorsFit = checkNeighborDoors(nextRoom, nextRoomIndex, currentRoom);

                            if (neighborDoorsFit == false)
                            {
                                rotations++;
                                if (triedRotations < 4)
                                {
                                    nextRoom.rotate90DegClockwise(1);
                                    nextRoomIndex = placeRoom(currentRoom, nextRoom, currentRoom.doorIndices[i], connectDoor, doorDirection);
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
                    chooseFromRoom.Remove(nextRoom);
                    nextRoom = randomWeightedRoom(chooseFromRoom);
                }
            }
        }
    }

    void addRoom(Room room, Room prevRoom, RoomIndex index)
    {
        room.instantiate(index, roomPrefabs[room.name], prevRoom, roomParentTransform);
        room.gameObject.transform.parent = this.transform;
        openRoomQueue.Enqueue(room);
        addDoorsToOpenDoors(room);
        addRoomToDict(room);
        allRooms.Add(room);
    }

    void addRoomToDict(Room room)
    {
        //Block all Indices
        for (int x = room.index.x; x < (room.index.x + room.sizeX); x++)
        {
            for (int y = room.index.y; y < (room.index.y + room.sizeY); y++)
            {
                modules.Add(new RoomIndex(x, y), room);
                if (lowerBounds.x > x)
                    lowerBounds.x = x;
                if (lowerBounds.y > y)
                    lowerBounds.y = y;
                if (upperBounds.x < x)
                    upperBounds.x = x;
                if (upperBounds.y < y)
                    upperBounds.y = y;
            }
        }
    }

    bool checkRotationFit(ref Room room, DoorDirection correspondingDirection, ref RoomIndex connectDoor)
    {
        bool hasFittingDoor = false;

        foreach (RoomIndex nextRoomDoor in room.doorIndices)
        {
            DoorDirection nextDoorDirection = getDoorDirection(nextRoomDoor, room);
            //Check if it is an opposing door
            if (Mathf.Abs((correspondingDirection - nextDoorDirection)) == 2)
            {
                hasFittingDoor = true;
                connectDoor = nextRoomDoor;
            }
        }

        return hasFittingDoor;
    }

    bool checkNeighborDoors(Room room, RoomIndex startIndex, Room currentRoom)
    {
        bool fits = true;
        int startX = startIndex.x - 1;
        int startY = startIndex.y - 1;
        int endX = startIndex.x + room.sizeX;
        int endY = startIndex.y + room.sizeY;
        room.index = startIndex;
        //Store connected doors, so we can remove the doors from the module if the room fits. 
        //Extra class to save the Modules the doors are attached too. Can't use a dictionary because there might be duplicate Indices
        List<DoorHelperClass> connectedDoorsNeighbor = new List<DoorHelperClass>();
        List<RoomIndex> connectedDoorsNextRoom = new List<RoomIndex>();

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                Room tryRoom;
                //check if x,y is on the outer circle
                if (
                        ((x == startX || x == endX) && (y != startY && y != endY)) ||
                        ((y == startY || y == endY) && (x != startX && x != endX))
                    )
                {
                    RoomIndex neighborDoor = new RoomIndex(x, y);
                    if (x == startX)
                    {
                        //Get Doors with direction == east
                        if (eastDoors.TryGetValue(neighborDoor, out tryRoom))
                        {
                            bool foundDoor = false;
                            foreach (RoomIndex door in room.doorIndices)
                            {
                                if (room.doorToWorldIndex(door) == neighborDoor)
                                {
                                    foundDoor = true;
                                    connectedDoorsNeighbor.Add(new DoorHelperClass(tryRoom, tryRoom.doorToLocalIndex(neighborDoor, DoorDirection.EAST)));
                                }
                            }
                            if (!foundDoor)
                            {
                                fits = false;
                                break;
                            }
                        }
                    }
                    else if (x == endX)
                    {
                        //Get Doors with direction == west
                        if (westDoors.TryGetValue(neighborDoor, out tryRoom))
                        {
                            bool foundDoor = false;
                            foreach (RoomIndex door in room.doorIndices)
                            {
                                //RoomIndex doorWorld = room.doorToWorldIndex(door);
                                if (room.doorToWorldIndex(door) == (neighborDoor))
                                {
                                    foundDoor = true;
                                    connectedDoorsNeighbor.Add(new DoorHelperClass(tryRoom, tryRoom.doorToLocalIndex(neighborDoor, DoorDirection.WEST)));
                                }
                            }
                            if (!foundDoor)
                            {
                                fits = false;
                                break;
                            }
                        }
                    }
                    else if (y == startY)
                    {
                        //Get Doors with direction == north
                        if (northDoors.TryGetValue(neighborDoor, out tryRoom))
                        {
                            bool foundDoor = false;
                            foreach (RoomIndex door in room.doorIndices)
                            {
                                if (room.doorToWorldIndex(door) == neighborDoor)
                                {
                                    foundDoor = true;
                                    connectedDoorsNeighbor.Add(new DoorHelperClass(tryRoom, tryRoom.doorToLocalIndex(neighborDoor, DoorDirection.NORTH)));
                                }
                            }
                            if (!foundDoor)
                            {
                                fits = false;
                                break;
                            }
                        }
                    }
                    else if (y == endY)
                    {
                        //Get Doors with direction == south
                        if (southDoors.TryGetValue(neighborDoor, out tryRoom))
                        {
                            bool foundDoor = false;
                            foreach (RoomIndex door in room.doorIndices)
                            {
                                if (room.doorToWorldIndex(door) == neighborDoor)
                                {
                                    foundDoor = true;
                                    connectedDoorsNeighbor.Add(new DoorHelperClass(tryRoom, tryRoom.doorToLocalIndex(neighborDoor, DoorDirection.SOUTH)));
                                }
                            }
                            if (!foundDoor)
                            {
                                fits = false;
                                break;
                            }
                        }
                    }
                }
            }
            if (!fits)
            {
                break;
            }
        }

        foreach (RoomIndex door in room.doorIndices)
        {
            Room tryRoom;
            RoomIndex neighborDoor = room.doorToWorldIndex(door);
            if (door.x == -1)
            {
                //West door

                if (!eastDoors.TryGetValue(neighborDoor, out tryRoom))
                {
                    if (modules.TryGetValue(neighborDoor, out tryRoom))
                    {
                        fits = false;
                    }
                }
                else
                {
                    connectedDoorsNextRoom.Add(door);
                }
            }
            else if (door.x == room.sizeX)
            {
                //East door
                if (!westDoors.TryGetValue(neighborDoor, out tryRoom))
                {
                    if (modules.TryGetValue(neighborDoor, out tryRoom))
                    {
                        fits = false;
                    }
                }
                else
                {
                    connectedDoorsNextRoom.Add(door);
                }
            }
            else if (door.y == -1)
            {
                //South door
                if (!northDoors.TryGetValue(neighborDoor, out tryRoom))
                {
                    if (modules.TryGetValue(neighborDoor, out tryRoom))
                    {
                        fits = false;
                    }
                }
                else
                {
                    connectedDoorsNextRoom.Add(door);
                }
            }
            else if (door.y == room.sizeY)
            {
                //North door
                if (!southDoors.TryGetValue(neighborDoor, out tryRoom))
                {
                    if (modules.TryGetValue(neighborDoor, out tryRoom))
                    {
                        fits = false;
                    }
                }
                else
                {
                    connectedDoorsNextRoom.Add(door);
                }
            }
        }

        if (fits)
        {
            //Remove all Doors that are now connected to another room
            foreach (DoorHelperClass door in connectedDoorsNeighbor)
            {
                foreach (Room openRoom in allRooms)
                {
                    if(openRoom.doorIndices.Count != 0)
                    {
                        if (openRoom == door.room)
                        {
                            //Add rotated and about to be removed doorIndex to Door class
                            openRoom.doors.Add(new Door(openRoom, door.doorIndex));
                            openRoom.doorIndices.Remove(door.doorIndex);
                        }
                    }
                }
            }

            foreach (RoomIndex removeDoor in connectedDoorsNextRoom)
            {
                //Add rotated and about to be removed doorIndex to Door class
                room.doors.Add(new Door(room, removeDoor));
                room.doorIndices.Remove(removeDoor);
            }
        }

        return fits;
    }

    RoomIndex placeRoom(Room prevRoom, Room nextRoom, RoomIndex prevRoomDoor, RoomIndex nextRoomDoor, DoorDirection doorDirection)
    {
        bool fits = true;
        int startX = 0;
        int startY = 0;

        int extendsX = 0;
        int extendsY = 0;

        if (doorDirection == DoorDirection.NORTH)
        {
            extendsY = 1;
            extendsX = 0;
        }
        else if (doorDirection == DoorDirection.EAST)
        {
            extendsY = 0;
            extendsX = 1;
        }
        else if (doorDirection == DoorDirection.SOUTH)
        {
            extendsY = -1;
            extendsX = 0;
        }
        else if (doorDirection == DoorDirection.WEST)
        {
            extendsY = 0;
            extendsX = -1;
        }

        RoomIndex worldDoorOpening = prevRoom.doorToWorldIndex(prevRoomDoor);
        if (extendsX == 1)
        {
            startX = worldDoorOpening.x;
            startY = worldDoorOpening.y - nextRoomDoor.y;
        }
        else if (extendsY == 1)
        {
            startY = worldDoorOpening.y;
            startX = worldDoorOpening.x - nextRoomDoor.x;
        }
        else if (extendsX == -1)
        {
            startX = worldDoorOpening.x - (nextRoom.sizeX - 1);
            startY = worldDoorOpening.y - (nextRoomDoor.y);
        }
        else if (extendsY == -1)
        {
            startX = worldDoorOpening.x - nextRoomDoor.x;
            startY = worldDoorOpening.y - (nextRoom.sizeY - 1);
        }
        RoomIndex roomIndex = new RoomIndex(startX, startY);

        for (int x = roomIndex.x; x < (roomIndex.x + nextRoom.sizeX); x++)
        {
            for (int y = roomIndex.y; y < (roomIndex.y + nextRoom.sizeY); y++)
            {
                Room tryRoom;
                if (modules.TryGetValue(new RoomIndex(x, y), out tryRoom))
                {
                    fits = false;
                }
            }
        }

        if (fits)
        {
            return roomIndex;
        }

        return new RoomIndex(0, 0);
    }

    DoorDirection getDoorDirection(RoomIndex door, Room room)
    {
        DoorDirection doorDirection = DoorDirection.NORTH;
        if (door.x == -1)
        {
            //West door
            doorDirection = DoorDirection.WEST;
        }
        else if (door.x == room.sizeX)
        {
            //East door
            doorDirection = DoorDirection.EAST;
        }
        else if (door.y == -1)
        {
            //South door
            doorDirection = DoorDirection.SOUTH;
        }
        else if (door.y == room.sizeY)
        {
            //North door
            doorDirection = DoorDirection.NORTH;
        }

        return doorDirection;
    }

    List<Room> getRoomsFullList()
    {
        List<Room> copyList = new List<Room>();

        foreach (Room room in roomsToChooseFrom)
        {
            copyList.Add(new Room(room));
        }
        return copyList;
    }

    Room randomWeightedRoom(List<Room> rooms)
    {
        //http://answers.unity3d.com/questions/190249/random-instantion-from-array-with-weighted-frequen.html
        float totalFreq = 0;
        foreach (Room room in rooms)
        {
            totalFreq += room.getChance();
        }
        float roll = Random.Range(0, totalFreq);
        int index = -1;
        for (int i = 0; i < rooms.Count; i++)
        {
            if (roll <= rooms[i].getChance())
            {
                index = i;
                break;
            }
            roll -= rooms[i].getChance();
        }
        // just in case we manage to roll 0.0001 past the highest:
        if (index == -1)
            index = rooms.Count - 1;

        return new Room(rooms[index]);
    }

    void addDoorsToOpenDoors(Room room)
    {
        foreach (RoomIndex door in room.doorIndices)
        {
            RoomIndex worldDoor = room.doorToWorldIndex(door);
            if (door.x == -1)
            {
                worldDoor.x += 1;
            }
            else if (door.x == room.sizeX)
            {
                worldDoor.x -= 1;
            }
            else if (door.y == -1)
            {
                worldDoor.y += 1;
            }
            else if (door.y == room.sizeY)
            {
                worldDoor.y -= 1;
            }
            DoorDirection direction = getDoorDirection(door, room);

            switch (direction)
            {
                case DoorDirection.NORTH:
                    northDoors.Add(worldDoor, room);
                    break;
                case DoorDirection.EAST:
                    eastDoors.Add(worldDoor, room);
                    break;
                case DoorDirection.SOUTH:
                    southDoors.Add(worldDoor, room);
                    break;
                case DoorDirection.WEST:
                    westDoors.Add(worldDoor, room);
                    break;
            }

        }
    }

    public void placeEndRoom(Room room) {*/
        /*
        Room connectedRoom = room.doors[0].getConnectedRoom();
        Door connectedDoor = connectedRoom.doors.Find(d => (int)d.getDirection() == ((int)room.doors[0].getDirection() + 2) % 4);
        connectedDoor.getWorldDoorIndex();
        removeRoomFromDict(room);
        Room endRoom = collection.rooms.First(x => x.name == "Basic/EndRoom");
        endRoom.rotate90DegClockwise(connectedRoom.rotation / 90);
        endRoom.doorIndices
        addRoom(endRoom, connectedRoom, connectedDoor.getWorldDoorIndex());
        */
        /*
        int maxPath = 0;
        List<Room> maxPathRooms = new List<Room>();
        foreach (Room room in allRooms) {
            if (room.pathLength >= maxPath) {
                if (room.pathLength > maxPath) {
                    maxPathRooms.Clear();
                }
                maxPathRooms.Add(room);
                maxPath = room.pathLength;
            }
        }
        Debug.Log("pathLength " + maxPath);
        int i = 0;
        foreach (Room room in maxPathRooms) {
            Debug.Log("MaxPathRoom " + i + ": " + room.index);
            i++;
        }

        //Room endRoom = collection.rooms.First(x => x.name == "Basic/EndRoom");
        Room replacedRoom = maxPathRooms[Random.Range(0, maxPathRooms.Count)];
        RoomIndex index = replacedRoom.index;
        */
        /*
    }

    void removeRoomFromDict(Room room) {
        List<KeyValuePair<RoomIndex, Room>> keyValueList = new List<KeyValuePair<RoomIndex, Room>>();
        foreach (KeyValuePair<RoomIndex, Room> entry in modules) {
            if(entry.Value == room) {
                keyValueList.Add(entry);
            }
        }

        foreach(KeyValuePair<RoomIndex, Room> entry in keyValueList) {
            modules.Remove(entry.Key);
        }
    }

    void addDoorAdjacency()
    {
        foreach (Room room in allRooms)
        {
            foreach(Door door in room.doors)
            {
                RoomIndex doorWorldIndex = room.doorToWorldIndex(door.getRelativeDoorIndex());
                Room adjacentRoom;
                if (!modules.TryGetValue(doorWorldIndex, out adjacentRoom))
                {
                    throw new System.Exception("addDoorAdjacency tried to get module at " + doorWorldIndex + " and failed");
                }
                door.initDoor(adjacentRoom, getDoorDirection(door.getRelativeDoorIndex(), room), doorWorldIndex);

				room.connectedRooms[getDoorDirection(door.getRelativeDoorIndex(), room)] = adjacentRoom;
            }
        }
    }

    public Vector3 IndexToWorldPos(RoomIndex index)
    {
        return new Vector3((index.x * Room.baseSize + (index.x + 1) * Room.baseSize) / 2.0f, 0, (index.y * Room.baseSize + (index.y + 1) * Room.baseSize) / 2.0f);
    }

    public RoomIndex WorldPosToIndex(Vector3 pos)
    {
        int x = (int)Mathf.Floor(pos.x / (Room.baseSize * 1.0f));
        int y = (int)Mathf.Floor(pos.z / (Room.baseSize * 1.0f));
        return new RoomIndex(x, y);
    }

    public Room getRoomByPosition(Vector3 pos)
    {
        Room posRoom;
        if (modules.TryGetValue(WorldPosToIndex(pos), out posRoom))
        {
            return posRoom;
        }

        return null;
    }

    public Vector3 getLowerBounds()
    {
        Vector3 lowerBound = IndexToWorldPos(lowerBounds);
        lowerBound.x -= Room.baseSize / 2.0f;
        lowerBound.z -= Room.baseSize / 2.0f;
        return lowerBound;
    }

    public Vector3 getUpperBound()
    {
        Vector3 upperBound = IndexToWorldPos(upperBounds);
        upperBound.x += Room.baseSize / 2.0f;
        upperBound.z += Room.baseSize / 2.0f;
        return upperBound;
    }

    public List<Room> getAllRooms()
    {
        return allRooms;
    }

    public Dictionary<string, GameObject> getMinimapPrefabs()
    {
        return minimapPrefabs;
    }
    */
}

