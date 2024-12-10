using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public AnimatedTile interTile;
    public AnimatedTile mediumTile;
    public AnimatedTile outerTile;
    public int width = 18;
    public int height = 10;

    void Start()
    {
        GenerateHexagonalTilemap();
    }

    void GenerateHexagonalTilemap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                AnimatedTile tile = GetTileForPosition(x);
                tilemap.SetTile(tilePosition, tile);
            }
        }
    }

    AnimatedTile GetTileForPosition(int x)
    {
        if (x >= 0 && x <= 5)
        {
            return interTile;
        }
        else if (x >= 6 && x <= 11)
        {
            return mediumTile;
        }
        else if (x >= 12 && x <= 17)
        {
            return outerTile;
        }
        return null;
    }
}
