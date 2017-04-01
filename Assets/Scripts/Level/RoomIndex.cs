using System.Xml.Serialization;


public class RoomIndex : System.IEquatable<RoomIndex>
{
    [XmlElement("IndexX")]
    public int x;

    [XmlElement("IndexY")]
    public int y;

    public RoomIndex()
    {

    }

    public RoomIndex(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public RoomIndex(RoomIndex other)
    {
        this.x = other.x;
        this.y = other.y;
    }

    public override bool Equals(object other)
    {
        RoomIndex index = other as RoomIndex;

        if (index == null)
        {
            return false;
        }

        return this.Equals(index);
    }

    public bool Equals(RoomIndex other)
    {
        if (other == null) return false;

        return this.x == other.x && this.y == other.y;
    }

    public override int GetHashCode()
    {
        int max = System.Math.Max(this.x, this.y);

        return max * max + System.Math.Max(this.y, 2 * this.y - this.x);
    }

    public RoomIndex getReverseIndex()
    {
        RoomIndex index = new RoomIndex(this);

        if (this.x < 0)
        {
            index.x += 2;
        }

        else if (this.x > 0)
        {
            index.x -= 2;
        }

        if (this.y < 0)
        {
            index.y += 2;
        }

        else if (this.y > 0)
        {
            index.y -= 2;
        }

        return index;
    }

    public static RoomIndex operator +(RoomIndex m1, RoomIndex m2)
    {
        return new RoomIndex(m1.x + m2.x, m1.y + m2.y);
    }

    public static RoomIndex operator -(RoomIndex m1, RoomIndex m2)
    {
        return new RoomIndex(m1.x - m2.x, m1.y - m2.y);
    }

    public static bool operator ==(RoomIndex m1, RoomIndex m2)
    {
        if (object.ReferenceEquals(m1, null))
        {
            return object.ReferenceEquals(m2, null);
        }

        return m1.Equals(m2);
    }

    public static bool operator !=(RoomIndex m1, RoomIndex m2)
    {
        return !(m1 == m2);
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}
