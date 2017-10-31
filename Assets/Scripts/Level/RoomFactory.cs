using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFactory {
    
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();
    
    public RoomFactory()
    {
        LoadRoomPrefabs();
    }

    void LoadRoomPrefabs()
    {
        foreach(RoomData rd in RoomCollection.Instance.rooms)
        {
            prefabMap.Add(rd.Name, Resources.Load("Rooms/" + rd.Name) as GameObject);
        }
    }

    public RoomMono CreateRoomObject(RoomData roomData, Transform parent, int roomNumber)
    {
        GameObject go = GameObject.Instantiate(prefabMap[roomData.Name],parent);
        go.name = roomNumber + " " + roomData.Index.ToString();
        RoomMono roomMono = go.AddComponent<RoomMono>();
        roomMono.init(RoomCollection.Instance.BaseSize, roomData.Name, roomData.RelativeChance, roomData.Rotation, roomData.Dimensions, roomData.LowerBounds, roomData.UpperBounds,roomData.Index);
        
        return roomMono;
    }

    public RoomData CreateRoomDataObject(RoomMono roomMono)
    {
        return new RoomData(roomMono.name, roomMono.type, roomMono.relativeChance, roomMono.dimensions, roomMono.lowerBound, roomMono.upperBound, roomMono.doors);
    }
}
