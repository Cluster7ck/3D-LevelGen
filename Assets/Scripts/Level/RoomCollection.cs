using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("Modules")]
public class RoomCollection
{
    [XmlElement("BaseSize")]
    public string baseSizeString;

    public float baseSize;

    [XmlArray("Rooms"), XmlArrayItem("Room")]
    public Room[] rooms;

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(RoomCollection));

        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static RoomCollection Load(string path)
    {
        var serializer = new XmlSerializer(typeof(RoomCollection));

        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as RoomCollection;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static RoomCollection LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(RoomCollection));

        return serializer.Deserialize(new StringReader(text)) as RoomCollection;
    }

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

    public void PostDeserialization()
    {
        baseSize = float.Parse(baseSizeString, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        Room.baseSize = baseSize;
        if (rooms != null)
        {
            foreach (Room room in rooms)
            {
                room.PostDeserialization();
            }
        }
    }

    public int getNumberRooms()
    {
        return this.rooms.Length;
    }

    public float getBaseSize()
    {
        return this.baseSize;
    }
}
