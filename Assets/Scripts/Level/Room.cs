using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;

[System.Serializable]
public class Room : System.IEquatable<Room>
{
    static public float baseSize;

    [XmlElement("Name")]
    public string name;

    [XmlElement("Rotation")]
    public int rotation;

    [XmlElement("chance")]
    public string chanceString;

    [XmlElement("Dimensions")]
    public SerializableVector3 dimensions;

    [XmlElement("UpperBound")]
    public SerializableVector3 upperBound;

    [XmlElement("LowerBound")]
    public SerializableVector3 lowerBound;

    [XmlArray("DoorIndices"), XmlArrayItem("Door")]
    public List<RoomIndex> doorIndices;

    [XmlIgnoreAttribute]
    public GameObject gameObject;

    [XmlIgnoreAttribute]
    public int pathLength = 0;

    [XmlIgnore]
    //New door gets added everytime a doorIndex is removed from doorIndices
    public List<Door> doors = new List<Door>();
    public Vector3 index;
    public Vector3 worldPosition;
    public float floatChance;
    private float startSizeX;
    private float startSizeY;

	[XmlIgnoreAttribute]
	public Dictionary<DoorDirection,Room> connectedRooms = new Dictionary<DoorDirection,Room>();

    public Room()
    {
        index = new Vector3();
        worldPosition = new Vector3(0, 0, 0);
    }

    public Room(string name, string chanceString, Vector3 dimensions, Vector3 lowerBound, Vector3 upperBound, List<Door> doors)
    {
        this.name = name;
        this.chanceString = chanceString;
        this.dimensions = dimensions;
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
        foreach (Door door in doors)
        {
            this.doors.Add(door);
        }
    }

    public void PostDeserialization()
    {
        floatChance = float.Parse(chanceString, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        startSizeX = (int)dimensions.x;
        startSizeY = (int)dimensions.y;
    }

    public Room(Room other)
    {
        this.name = other.name;
        this.rotation = other.rotation;
        this.dimensions = other.dimensions;
        this.startSizeX = other.startSizeX;
        this.startSizeY = other.startSizeY;
        this.floatChance = other.floatChance;
        this.doorIndices = new List<RoomIndex>();
        this.index = other.index;
        this.worldPosition = other.worldPosition;
        foreach (RoomIndex door in other.doorIndices)
        {
            this.doorIndices.Add(new RoomIndex(door));
        }
        foreach (Door door in other.doors)
        {
            this.doors.Add(new Door(door));
        }
    }

    public void instantiate(Vector3 index, GameObject prefab, Room prevRoom, Transform roomParent)
    {
        this.gameObject = (GameObject)GameObject.Instantiate(prefab, roomParent);
        /*
		this.lightController = this.gameObject.GetComponentInChildren<RoomLightController>();
		if(this.lightController != null){
			this.lightController.room = this;
		}
        */
        rotateGameObj();
        rotatePhysicalDoorIndices();
        assignPhysicalDoorsToDoors();

        float posX = (index.x * baseSize + (index.x + dimensions.x) * baseSize) / 2.0f;
        float posY = (index.y * baseSize + (index.y + dimensions.y) * baseSize) / 2.0f;

        worldPosition = new Vector3(posX, 0, posY);
        gameObject.transform.position = worldPosition;
        gameObject.name = index.ToString();
        this.index = index;

        if (prevRoom == null)
        {
            this.pathLength = 0;
        }
        else
        {
            this.pathLength = prevRoom.pathLength + 1;
        }
    }


    public void rotateGameObj()
    {
        gameObject.transform.Rotate(0, rotation, 0);
    }

    public void rotate90DegClockwise(int rotations)
    {
        /*
        for (int i = 0; i < rotations; i++)
        {
            rotation += 90;
            if (rotation > 270)
            {
                rotation = 0;
            }

            int rotatedSizeX = (int)dimensions.x;
            int rotatedSizeY = (int)dimensions.y;
            List<Vector3> rotatedDoors = new List<Vector3>();
            foreach (RoomIndex doorIndex in doorIndices)
            {
                rotatedDoors.Add(rotateIndex(doorIndex, dimensions.x, dimensions.y));
            }

            dimensions.x = rotatedSizeX;
            dimensions.y = rotatedSizeY;

            doorIndices.Clear();
            doorIndices = rotatedDoors;
        }
        */
    }

    private Vector3 rotateIndex(RoomIndex doorIndex, float sizeXR, float sizeYR)
    {
        float rotatedSizeX = sizeYR;
        float rotatedSizeY = sizeXR;
        //Debug.Log("Before rotate: (" + doorIndex.x + ", " + doorIndex.y + ")");
        Vector3 rotatedDoorIndex = new Vector3();
        if (doorIndex.x == -1)
        {
            rotatedDoorIndex.y = rotatedSizeY;
            rotatedDoorIndex.x = doorIndex.y;
        }
        else if (doorIndex.x == sizeXR)
        {
            rotatedDoorIndex.y = -1;
            rotatedDoorIndex.x = doorIndex.y;
        }

        if (doorIndex.y == -1)
        {
            rotatedDoorIndex.x = -1;
            rotatedDoorIndex.y = (sizeXR - 1) - doorIndex.x;
        }
        else if (doorIndex.y == sizeYR)
        {
            rotatedDoorIndex.x = rotatedSizeX;
            rotatedDoorIndex.y = (sizeXR - 1) - doorIndex.x;
        }
        return rotatedDoorIndex;
    }

    private void rotatePhysicalDoorIndices()
    {
        /*
        List<GameObject> doorSpawns = CF_Utils.findChildsWithTag(this.gameObject, NameDefines.tag_doorSpawn);
        foreach(GameObject doorSpawn in doorSpawns)
        {
            GameObject physDoor = CF_Utils.findChildWithTag(doorSpawn, NameDefines.tag_door);
            DoorIndex doorIndex = physDoor.GetComponent<DoorIndex>();
            int rotations = rotation / 90;
            RoomIndex newIndex = doorIndex.getDoorIndex();

            int sX = startSizeX;
            int sY = startSizeY;
            for (int i = 0; i < rotations; i++)
            {
                newIndex = rotateIndex(newIndex, sX,sY);
                int tempSize = sX;
                sX = sY;
                sY = tempSize;
            }
            doorIndex.DoorIndexX = newIndex.x;
            doorIndex.DoorIndexY = newIndex.y;
        }
        */
    }

    private void assignPhysicalDoorsToDoors()
    {
        /*
        List<GameObject> physDoors = CF_Utils.findChildsWithTag(this.gameObject, NameDefines.tag_door);
        foreach (GameObject physDoor in physDoors)
        {
            foreach(Door door in doors)
            {
                if(physDoor.GetComponent<DoorIndex>().getDoorIndex() == door.getRelativeDoorIndex())
                {
                    door.setPhysicalDoor(physDoor);
                }
            }
        }
        */
    }

    public void openAllDoors()
    {
        foreach(Door door in doors)
        {
            openDoor(door);
        }
    }

    private void openDoor(Door door)
    {/*
        Room connectedRoom = door.getConnectedRoom();
        Door connectedDoor = connectedRoom.doors.Find(d => (int)d.getDirection() == ((int)door.getDirection() + 2 % 4));
        connectedDoor.open();
        door.open();*/
    }

    public List<RoomIndex> getDoors()
    {
        return this.doorIndices;
    }
    public Vector3 getIndex()
    {
        return this.index;
    }

    public float getChance()
    {
        return floatChance;
    }

    public bool containsDoor(RoomIndex index)
    {
        foreach (RoomIndex i in this.doorIndices)
        {
            if (i.Equals(index))
            {
                return true;
            }
        }

        return false;
    }

    static public void setBaseSize(float baseSize)
    {
        Room.baseSize = baseSize;
    }

    static public float getBaseSize()
    {
        return Room.baseSize;
    }

    public Vector3 doorToWorldIndex(Vector3 door)
    {
        return this.index + door;
    }

    public RoomIndex doorToLocalIndex(RoomIndex door, DoorDirection direction)
    {
        /*
        switch (direction)
        {
            case DoorDirection.NORTH:
                door.y += 1;
                break;
            case DoorDirection.EAST:
                door.x += 1;
                break;
            case DoorDirection.SOUTH:
                door.y -= 1;
                break;
            case DoorDirection.WEST:
                door.x -= 1;
                break;
        }
        return door - this.index;

        */
        return new RoomIndex(0,0);
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Room objAsPart = obj as Room;
        if (objAsPart == null) return false;
        else return Equals(objAsPart);
    }

    public override int GetHashCode()
    {
        return name.GetHashCode() + index.GetHashCode();
    }

    public bool Equals(Room other)
    {
        if (other == null) return false;

        return (this.name.Equals(other.name)) && (this.index.Equals(other.index));
    }

    public static bool operator ==(Room m1, Room m2)
    {
        if (object.ReferenceEquals(m1, null))
        {
            return object.ReferenceEquals(m2, null);
        }

        return m1.Equals(m2);
    }

    public static bool operator !=(Room m1, Room m2)
    {
        return !(m1 == m2);
    }
}
