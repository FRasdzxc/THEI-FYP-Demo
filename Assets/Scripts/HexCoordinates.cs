using UnityEngine;

public class HexCoordinates
{
    public int x;
    public int y;
    public int z;

    public HexCoordinates(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(Vector3Int offset)
    {
        int x = offset.x - (offset.y - (offset.y & 1)) / 2;
        int z = offset.y;
        int y = -x - z;
        return new HexCoordinates(x, y, z);
    }
    public Vector3Int ToOffsetCoordinates()
    {
        int col = x + (z - (z & 1)) / 2;
        int row = z;
        return new Vector3Int(col, row, 0);
    }

    public int DistanceTo(HexCoordinates other)
    {
        return (Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y) + Mathf.Abs(z - other.z)) / 2;
    }
}