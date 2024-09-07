using UnityEngine;
using UnityEngine.Tilemaps;

public class SpaceShip : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile normalTile;
    public Tile highlightTile; // The tile used for highlighting
    private Vector3Int currentPosition = new Vector3Int();
    private Vector3Int previousTilePosition = new Vector3Int(100, 100, 0);
    private Vector3Int tilePosition = new Vector3Int();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tilemap.WorldToCell(mouseWorldPos);
            cell.z = 0;
            TileBase clickedTile = tilemap.GetTile(cell);
            if(clickedTile == highlightTile)
            {
                transform.position = tilemap.CellToWorld(cell);
            }
        }
        // Get the GameObject's position
        Vector3 worldPosition = transform.position;
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        // Check if the GameObject has moved to a new tile
        if (cellPosition != previousTilePosition)
        {
            // Reset the previous tile
            tilemap.SetTile(previousTilePosition, normalTile);
            string[] direction = new string[]
            {
                new string("Right"),
                new string("Left"),
                new string("UpLeft"),
                new string("UpRight"),
                new string("DownLeft"),
                new string("DownRight")
            };
            Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),  // Right 0
            new Vector3Int(-1, 0, 0), // Left 1
            new Vector3Int(0, 1, 0), // Up-Left-Odd 2
            new Vector3Int(-1, 1, 0),  // Up-Left-Even 3
            new Vector3Int(1, 1, 0), // Up-Right-Odd 4
            new Vector3Int(0, 1, 0), // Up-Right-Even 5
            new Vector3Int(0, -1, 0), // Down-Left-Odd 6
            new Vector3Int(-1, -1, 0),  // Down-Left-Even 7
            new Vector3Int(1, -1, 0), // Down-Right-Odd 8
            new Vector3Int(0, -1, 0) // Down-Right-Even 9
        };
            foreach (string dir in direction)
            {
                currentPosition = cellPosition;
                tilePosition = cellPosition;
                for (int i = 0; i <= 3; i++)
                {
                    if (currentPosition.y < 0)
                    {
                        currentPosition.y *= -1;
                    }
                    switch(dir)
                    {
                        case "Left":
                            tilePosition += directions[0];
                            break;
                        case "Right":
                            tilePosition += directions[1];
                            break;
                        default:
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
                            default:
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
                            default:
                                break;
                        }
                    }
                    //switch (dir)
                    //{
                    //    case "Right":
                    //        tilePosition += directions[0];
                    //        break;
                    //    case "Left":
                    //        tilePosition += directions[1];
                    //        break;
                    //    case "UpLeft":
                    //        if(currentPosition.y % 2 == 1)
                    //        {
                    //            tilePosition += directions[2];
                    //        } else
                    //        {
                    //            tilePosition += directions[3];
                    //        }
                    //        break;
                    //    case "UpRight":
                    //        if (currentPosition.y % 2 == 1)
                    //        {
                    //            tilePosition += directions[4];
                    //        }
                    //        else
                    //        {
                    //            tilePosition += directions[5];
                    //        }
                    //        break;
                    //    case "DownLeft":
                    //        if (currentPosition.y % 2 == 1)
                    //        {
                    //            tilePosition += directions[6];
                    //        }
                    //        else
                    //        {
                    //            tilePosition += directions[7];
                    //        }
                    //        break;
                    //    case "DownRight":
                    //        if (currentPosition.y % 2 == 1)
                    //        {
                    //            tilePosition += directions[8];
                    //        }
                    //        else
                    //        {
                    //            tilePosition += directions[9];
                    //        }
                    //        break;
                    //}
                    currentPosition = tilePosition;
                    tilemap.SetTile(tilePosition, highlightTile);
                }
            }

            // Update the previous tile position
            previousTilePosition = cellPosition;
        }
    }
}