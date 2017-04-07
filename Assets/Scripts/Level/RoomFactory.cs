using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFactory {

    private static string PrefabPath = "";
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();
    
    void LoadRoomPrefabs()
    {
        foreach(RoomData rd in RoomCollection.Instance.rooms)
        {
            prefabMap.Add(rd.Name, Resources.Load("Rooms/" + rd.Name) as GameObject);
        }
    }

    RoomMono CreateRoomObject(RoomData roomData)
    {
        GameObject go = GameObject.Instantiate(prefabMap[roomData.Name]);
        RoomMono roomMono = go.AddComponent<RoomMono>();
        roomMono.init(RoomCollection.Instance.BaseSize, roomData.Name, roomData.RelativeChance, roomData.Dimensions, roomData.LowerBounds, roomData.UpperBounds);
        
        return roomMono;
    }

    RoomData CreateRoomDataObject(RoomMono roomMono)
    {
        return new RoomData(roomMono.name, roomMono.type, roomMono.relativeChance, roomMono.dimensions, roomMono.lowerBound, roomMono.upperBound, roomMono.doors);
    }
}
