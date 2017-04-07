using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

public class RoomBuilder : MonoBehaviour {

    public string RoomName;
    public RoomType type;
    public float relativeChance;
    public BlockData[] blocks;
    List<Door> doorObjects = new List<Door>();
    List<DoorMono> doorGameObjects = new List<DoorMono>();

    private GameObject parentObject;
    private List<GameObject> blockObjects = new List<GameObject>();
    private RoomData room;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void buildRoom()
    {
        deleteLast();
        parentObject = new GameObject(RoomName);

        //Check the bounds of the Room. Then move all tiles over, so that the bottom left is at 0,0
        Vector3 lowerBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 upperBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (BlockData block in blocks)
        {
            checkBounds(ref lowerBounds, ref upperBounds, block.index);
        }

        for (int i = 0; i < blocks.Length; i++)
        {
            if(lowerBounds != Vector3.zero)
            {
                blocks[i].index = blocks[i].index - lowerBounds;
            }
        }
        //Bounds have to be recalculated, so the Doors can be placed
        lowerBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        upperBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        foreach (BlockData block in blocks)
        {
            checkBounds(ref lowerBounds, ref upperBounds, block.index);
        }


        int count = 1;
        foreach (BlockData block in blocks)
        {
            if(block.type != null)
            {
                GameObject instance = Instantiate(block.type, block.index, new Quaternion(), parentObject.transform) as GameObject;
                Block blockScript = instance.GetComponent<Block>();
                blockScript.rotate(block.rotation);
                instance.transform.name = count.ToString()+". "+ instance.name.Split('(')[0] + " (" + (int)block.index.x + ", " + (int)block.index.y + ", " + (int)block.index.z + ")";
                blockObjects.Add(instance);

                checkForDoors(block, blockScript, lowerBounds, upperBounds);

                count++;
            }
        }
        Vector3 dimensions = new Vector3(Mathf.Abs(lowerBounds.x - upperBounds.x)+1, Mathf.Abs(lowerBounds.y - upperBounds.y)+1, Mathf.Abs(lowerBounds.z - upperBounds.z)+1);

        room = new RoomData(RoomName, type, relativeChance, dimensions, lowerBounds, upperBounds, doorObjects);
        

    }

    public void SaveRoom()
    {
        SaveToXML(room);
        CreateSimplePrefab(parentObject.transform);
    }

    void rotateRoom()
    {
        RoomMono room = parentObject.GetComponent<RoomMono>();
        if(room != null)
        {
            room.rotate90Deg(1);
        }
    }

    void checkForDoors(BlockData block, Block blockScript, Vector3 lowerBounds, Vector3 upperBounds)
    {
        List<DoorDirection> possibleDir = new List<DoorDirection>();
        if (block.index.x == lowerBounds.x){
            possibleDir.Add(DoorDirection.WEST);
        }
        if (block.index.x == upperBounds.x)
        {
            possibleDir.Add(DoorDirection.EAST);
        }
        if (block.index.z == lowerBounds.z)
        {
            possibleDir.Add(DoorDirection.SOUTH);
        }
        if (block.index.z == upperBounds.z)
        {
            possibleDir.Add(DoorDirection.NORTH);
        }

        foreach(Block.Openings ope in blockScript.openings)
        {
            if (possibleDir.Contains(ope.direction))
            {
                Vector3 relativeIndex = ope.direction.DirectionOffset() + block.index;
                relativeIndex += new Vector3(0, ope.yChange, 0);
                Door newDoor = new Door(relativeIndex, ope.direction);
                doorObjects.Add(newDoor);
            }
        }
    }

    void checkBounds(ref Vector3 lowerBounds, ref Vector3 upperBounds, Vector3 checkVec) {

        if(checkVec.x < lowerBounds.x)
        {
            lowerBounds.x = checkVec.x;
        }
        if(checkVec.x > upperBounds.x)
        {
            upperBounds.x = checkVec.x;
        }
        if (checkVec.y < lowerBounds.y)
        {
            lowerBounds.y = checkVec.y;
        }
        if (checkVec.y > upperBounds.y)
        {
            upperBounds.y = checkVec.y;
        }
        if (checkVec.z < lowerBounds.z)
        {
            lowerBounds.z = checkVec.z;
        }
        if (checkVec.z > upperBounds.z)
        {
            upperBounds.z = checkVec.z;
        }
    }

    public void deleteLast()
    {
        blockObjects.Clear();
        doorObjects.Clear();
        DestroyImmediate(parentObject);
    }

    static void CreateSimplePrefab(Transform transform)
    {
        Object prefab = null;

        try
        {
            prefab = Resources.Load("Rooms/"+transform.name + ".prefab");
        }
        catch (UnityException e)
        {
            Debug.Log(e);
        }
        if (prefab == null)
        {
            prefab = PrefabUtility.CreateEmptyPrefab("Assets/Resources/Rooms/" + transform.gameObject.name + ".prefab");
            PrefabUtility.ReplacePrefab(transform.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
        }
    }

    void SaveToXML(RoomData room)
    {
        RoomCollection.Instance.BaseSize = 1;
        RoomCollection.Instance.rooms.Add(room);
        RoomCollection.Instance.Save(Application.dataPath + "/StreamingAssets/XML/room_collection.xml");
    }


    void OnDrawGizmos()
    {
        if(parentObject != null)
        {
            Gizmos.color = Color.green;
            foreach (Door door in doorObjects)
            {
                Gizmos.DrawCube(door.RelativeIndex, new Vector3(1, 0.1f, 1));
            }
        }
    }

    public GameObject getLastParent()
    {
        return parentObject;
    }

    [System.Serializable]
    public struct BlockData
    {
        public Vector3 index;
        public GameObject type;
        public int rotation;
    }
}
