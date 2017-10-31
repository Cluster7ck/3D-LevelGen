using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomIndex : IEquatable<RoomIndex> {

    /// <summary>
    /// x component
    /// </summary>
    public int x;

    /// <summary>
    /// y component
    /// </summary>
    public int y;

    /// <summary>
    /// z component
    /// </summary>
    public int z;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public RoomIndex(int rX, int rY, int rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    /// <summary>
    /// Default Constructor. Returns (0, 0, 0)
    /// </summary>
    public RoomIndex(RoomIndex other)
    {
        x = other.x;
        y = other.y;
        z = other.z;
    }

    /// <summary>
    /// Default Constructor. Returns (0, 0, 0)
    /// </summary>
    public RoomIndex()
    {
        x = 0;
        y = 0;
        z = 0;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("({0}, {1}, {2})", x, y, z);
    }

    /// <summary>
    /// Automatic conversion from RoomIndex to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector3(RoomIndex rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to RoomIndex
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator RoomIndex(Vector3 rValue)
    {
        return new RoomIndex(Mathf.RoundToInt(rValue.x), Mathf.RoundToInt(rValue.y), Mathf.RoundToInt(rValue.z));
    }

    public bool Equals(RoomIndex other)
    {
        if (other == null)
            return false;

        if (this.x == other.x && this.y == other.y && this.z == other.z)
            return true;
        else
            return false;
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null)
            return false;

        RoomIndex roomObj = obj as RoomIndex;
        if (roomObj == null)
            return false;
        else
            return Equals(roomObj);
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode();
    }

    public static bool operator ==(RoomIndex index1, RoomIndex index2)
    {
        if (((object)index1) == null || ((object)index2) == null)
            return System.Object.Equals(index1, index2);

        return index1.Equals(index2);
    }

    public static bool operator !=(RoomIndex index1, RoomIndex index2)
    {
        if (((object)index1) == null || ((object)index2) == null)
            return !System.Object.Equals(index1, index2);

        return !(index1.Equals(index2));
    }

    public static RoomIndex operator +(RoomIndex index1, RoomIndex index2)
    {
        return new RoomIndex(index1.x + index2.x, index1.y + index2.y, index1.z + index2.z);
    }

    public static RoomIndex operator -(RoomIndex index1, RoomIndex index2)
    {
        return new RoomIndex(index1.x - index2.x, index1.y - index2.y, index1.z - index2.z);
    }

    public static RoomIndex operator +(RoomIndex index1, Vector3 index2)
    {
        return new RoomIndex(index1.x + Mathf.RoundToInt(index2.x), index1.y + Mathf.RoundToInt(index2.y), index1.z + Mathf.RoundToInt(index2.z));
    }

    public static RoomIndex operator -(RoomIndex index1, Vector3 index2)
    {
        return new RoomIndex(index1.x - Mathf.RoundToInt(index2.x), index1.y - Mathf.RoundToInt(index2.y), index1.z - Mathf.RoundToInt(index2.z));
    }
}
