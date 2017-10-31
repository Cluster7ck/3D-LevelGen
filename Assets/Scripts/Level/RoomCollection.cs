using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

[XmlRoot("RoomCollection")]
public class RoomCollection
{

    private float _baseSize;
    [XmlIgnore]
    public float BaseSize
    {
        get { return _baseSize; }
        set { _baseSize = value; }
    }

    [XmlElement("BaseSize")]
    public string CustomBaseSize
    {
        get { return BaseSize.ToString("#0.00", CultureInfo.InvariantCulture); }
        set { float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _baseSize); }
    }

    [XmlArray("Rooms"), XmlArrayItem("Room")]
    public List<RoomData> rooms = new List<RoomData>();
    private static RoomCollection instance;

    private RoomCollection() { }

    public static RoomCollection Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new RoomCollection();
            }
            return instance;
        }
    }

    public void Save(string path)
    {
        if (File.Exists(path))
        {
            Load(path);
        }

        var serializer = new XmlSerializer(typeof(RoomCollection));

        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public void Load(string path)
    {
        var serializer = new XmlSerializer(typeof(RoomCollection));
        RoomCollection rc = null;
        using (var stream = new FileStream(path, FileMode.Open))
        {
            rc = serializer.Deserialize(stream) as RoomCollection;
        }
        if(rc != null)
        {
            foreach (RoomData rd in rc.rooms)
            {
                if(rooms.Where(x => x.Name == rd.Name).FirstOrDefault() == null)
                {
                    rooms.Add(rd);
                }
            }

        }
        postDeserialization();
    }

    void postDeserialization()
    {
        foreach(RoomData room in rooms)
        {
            float outChance;
            float.TryParse(room.CustomRelativeChance, NumberStyles.Float, CultureInfo.InvariantCulture, out outChance);
            room.RelativeChance = outChance;
            foreach (Door door in room.doors)
            {
                door.Room = room;
            }
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static RoomCollection LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(RoomCollection));

        return serializer.Deserialize(new StringReader(text)) as RoomCollection;
    }
    /*
    public void show()
    {
        Debug.Log("Size: " + this.baseSize);

        foreach (Room room in this.rooms)
        {

            foreach (RoomIndex index in room.doorIndices)
            {
                Debug.Log(index.x + "," + index.y + " ");
            }
        }
    }
    */

    public int getNumberRooms()
    {
        return this.rooms.Count;
    }
}
