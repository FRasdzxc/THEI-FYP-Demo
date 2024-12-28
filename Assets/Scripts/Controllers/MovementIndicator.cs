// MovementIndicator.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementIndicator : MonoBehaviour
{
    public static MovementIndicator Instance { get; private set; }
    [SerializeField] private Tilemap indicatorTilemap;
    [SerializeField] private TileBase availablePathTile;
    [SerializeField] private TileBase selectedPathTile; // Tile to show the queued movement

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ClearIndicators()
    {
        indicatorTilemap.ClearAllTiles();
    }

    public void ShowAvailablePaths(Vector3Int cellPosition, int speed)
    {
        ClearIndicators();

        string[] direction = new string[]
        {
        "Right",
        "Left",
        "UpLeft",
        "UpRight",
        "DownLeft",
        "DownRight"
        };

        Vector3Int[] directions = new Vector3Int[]
        {
        new Vector3Int(1, 0, 0),   // Right 0
        new Vector3Int(-1, 0, 0),  // Left 1
        new Vector3Int(0, 1, 0),   // Up-Left-Odd 2
        new Vector3Int(-1, 1, 0),  // Up-Left-Even 3
        new Vector3Int(1, 1, 0),   // Up-Right-Odd 4
        new Vector3Int(0, 1, 0),   // Up-Right-Even 5
        new Vector3Int(0, -1, 0),  // Down-Left-Odd 6
        new Vector3Int(-1, -1, 0), // Down-Left-Even 7
        new Vector3Int(1, -1, 0),  // Down-Right-Odd 8
        new Vector3Int(0, -1, 0)   // Down-Right-Even 9
        };

        Vector3Int currentPosition = new Vector3Int(0, 0, 0);
        Vector3Int tilePosition = new Vector3Int(0, 0, 0);
        bool colide = false;

        foreach (string dir in direction)
        {
            currentPosition = cellPosition;
            tilePosition = cellPosition;

            for (int i = 0; i <= speed; i++)
            {
                if (!colide)
                {
                    if (currentPosition.y < 0)
                    {
                        currentPosition.y *= -1;
                    }

                    switch (dir)
                    {
                        case "Left":
                            tilePosition += directions[0];
                            break;
                        case "Right":
                            tilePosition += directions[1];
                            break;
                    }

                    if (currentPosition.y % 2 == 1)
                    {
                        switch (dir)
                        {
                            case "UpLeft":
                                tilePosition += directions[2];
                                break;
                            case "UpRight":
                                tilePosition += directions[4];
                                break;
                            case "DownLeft":
                                tilePosition += directions[6];
                                break;
                            case "DownRight":
                                tilePosition += directions[8];
                                break;
                        }
                    }
                    else
                    {
                        switch (dir)
                        {
                            case "UpLeft":
                                tilePosition += directions[3];
                                break;
                            case "UpRight":
                                tilePosition += directions[5];
                                break;
                            case "DownLeft":
                                tilePosition += directions[7];
                                break;
                            case "DownRight":
                                tilePosition += directions[9];
                                break;
                        }
                    }

                    // Check if the tile is valid and not occupied
                    foreach (Vector3Int x in InformationRecorder.Instance.GetAllObjects())
                    {
                        if(x == tilePosition)
                        {
                            colide = true;
                            break;
                        }
                    }
                    currentPosition = tilePosition;
                    indicatorTilemap.SetTile(tilePosition, availablePathTile);
                }
                else
                {
                    colide = false;
                    break;
                }
            }
        }
        indicatorTilemap.SetTile(cellPosition, selectedPathTile);
    }

    public void ShowSelectedPath(Vector3Int position)
    {
        // Replace available path tile with selected path tile
        indicatorTilemap.SetTile(position, selectedPathTile);
    }

    public void DeselectSelectedPath(Vector3Int position)
    {
        indicatorTilemap.SetTile(position, availablePathTile);
    }

    public bool IsValidMove(Vector3Int position)
    {
        return indicatorTilemap.HasTile(position);
    }
}