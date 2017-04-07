using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Xml.Serialization;
using System.Collections.Generic;

public enum RoomType
{
    STANDARD,
    HOUSE,
    STREET
}

[System.Serializable]
[XmlInclude(typeof(Door))]
public class RoomData : System.IEquatable<RoomData>
{
    static public float BaseSize;

    public string Name { get; set; }
    public RoomType Type { get; set; }
    public int Rotation { get; set; }

    //float serialization is weird
    private float _relativeChance;
    [XmlIgnore]
    public float RelativeChance
    {
        get { return _relativeChance; }
        set { _relativeChance = value; }
    }

    [XmlElement("RelativeChance")]
    public string CustomRelativeChance
    {
        get { return RelativeChance.ToString("#0.00", CultureInfo.InvariantCulture); }
        set { float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _relativeChance); }
    }

    //Need bounds for y levels. Dimensions enough for xz indices
    public SerializableVector3 Dimensions { get; set; }
    public SerializableVector3 UpperBounds { get; set; }
    public SerializableVector3 LowerBounds { get; set; }

    [XmlArray("Doors"), XmlArrayItem("Door")]
    public List<Door> doors;

    [XmlIgnoreAttribute]
    public Vector3 Index { get; set; }
    [XmlIgnoreAttribute]
    public Vector3 WorldPosition { get; set; }

    [XmlIgnoreAttribute]
	public Dictionary<DoorDirection,RoomData> connectedRooms = new Dictionary<DoorDirection,RoomData>();

    public RoomData()
    {
        doors = new List<Door>();
    }

    public RoomData(string name, RoomType type, float RelativeChance, Vector3 dimensions, Vector3 lowerBound, Vector3 upperBound, List<Door> doors)
    {
        this.Name = name;
        this.Type = type;
        this.RelativeChance = RelativeChance;
        this.Dimensions = dimensions;
        this.LowerBounds = lowerBound;
        this.UpperBounds = upperBound;

        this.doors = new List<Door>(doors.Count);
        foreach (Door door in doors)
        {
            this.doors.Add(new Door(door));
        }
    }
    public RoomData(RoomData other)
    {
        this.Name = other.Name;
        this.Type = other.Type;
        this.Rotation = other.Rotation;
        this.Dimensions = other.Dimensions;
        this.WorldPosition = other.WorldPosition;

        this.doors = new List<Door>(other.doors.Count);
        foreach (Door door in other.doors)
        {
            this.doors.Add(new Door(door));
        }
    }

    /*
    public void PostDeserialization()
    {
        RelativeChanceFloat = float.Parse(RelativeChance, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
    }
    */

    /// <summary>
    /// Rotates the dimensions and door indices of the Room by 'int rotations'
    /// </summary>
    /// <param name="rotations"></param>
    public void rotateData_90Deg(int rotations)
    {

        for (int i = 0; i < rotations; i++)
        {
            Rotation = (Rotation + 90) % 360;
            List<Vector3> rotatedDoors = new List<Vector3>();
            foreach (Door door in doors)
            {
                //Set door index dor.setRellativeIndex()
                door.rotateDoorIndex(UpperBounds);
            }

            UpperBounds = new Vector3(UpperBounds.z, UpperBounds.y, UpperBounds.x);
            Dimensions = new Vector3(Dimensions.z, Dimensions.y, Dimensions.x);
        }

    }

    public bool containsDoor(Vector3 index)
    {
        foreach (Door door in this.doors)
        {
            if (door.RelativeIndex == index)
            {
                return true;
            }
        }

        return false;
    }

    public Vector3 doorToWorldIndex(Vector3 door)
    {
        return this.Index + door;
    } 

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        RoomData objAsPart = obj as RoomData;
        if (objAsPart == null) return false;
        else return Equals(objAsPart);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode() + Index.GetHashCode();
    }

    public bool Equals(RoomData other)
    {
        if (other == null) return false;

        return (this.Name.Equals(other.Name)) && (this.Index.Equals(other.Index));
    }

    public static bool operator ==(RoomData m1, RoomData m2)
    {
        if (object.ReferenceEquals(m1, null))
        {
            return object.ReferenceEquals(m2, null);
        }

        return m1.Equals(m2);
    }

    public static bool operator !=(RoomData m1, RoomData m2)
    {
        return !(m1 == m2);
    }
}