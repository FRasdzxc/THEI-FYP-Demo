using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    public Tilemap tilemap;
    public AnimatedTile normalTile;
    public Tile highlightTile;
    public Tile selectedTile;
    public enum Tiles { normal, highlight, selected }
    public enum SelectedType { player, enemy, tile , highlight}
    private InformationRecorder ir;
    private Vector3Int currentPosition = new Vector3Int();
    private Vector3Int previousTilePosition = new Vector3Int(100, 100, 0);
    private Vector3Int tilePosition = new Vector3Int();
    private bool selected = false;
    private bool colide = false;
    private SelectedType previousSelectedType;
    private int previousSpeed;
    private void Start()
    {
        ir = GameObject.FindWithTag("EventSystem").GetComponent<InformationRecorder>();
    }
    public void ResetRound()
    {
        if(selected)
        {
            switch (previousSelectedType)
            {
                case SelectedType.player:
                    ChangeTiles(previousTilePosition, Tiles.normal, previousSpeed);
                    tilemap.SetTile(previousTilePosition, normalTile);
                    break;
                case SelectedType.tile:
                    tilemap.SetTile(previousTilePosition, normalTile);
                    break;
            }
            selected = false;
        }
    }

    public void Clicked(Vector3Int cell, int speed, SelectedType type)
    {
        if (!selected)
        {
            switch (type)
            {
                case SelectedType.player:
                    ChangeTiles(cell, Tiles.highlight, speed);
                    tilemap.SetTile(cell, selectedTile);
                    previousSelectedType = SelectedType.player;
                    break;
                case SelectedType.tile:
                    tilemap.SetTile(cell, selectedTile);
                    previousSelectedType = SelectedType.tile;
                    break;
            }
            previousTilePosition = cell;
            previousSpeed = speed;
            selected = true;
        }
        else
        {
            if(cell == previousTilePosition)
            {
                switch (type)
                {
                    case SelectedType.player:
                        ChangeTiles(cell, Tiles.normal, speed);
                        tilemap.SetTile(cell, normalTile);
                        break;
                    case SelectedType.tile:
                        tilemap.SetTile(cell, normalTile);
                        break;
                }
                selected = false;
            }
            else
            {
                switch (previousSelectedType)
                {
                    case SelectedType.player:
                        ChangeTiles(previousTilePosition, Tiles.normal, speed);
                        tilemap.SetTile(previousTilePosition, normalTile);
                        break;
                    case SelectedType.tile:
                        tilemap.SetTile(previousTilePosition, normalTile);
                        break;
                }
                switch (type)
                {
                    case SelectedType.player:
                        ChangeTiles(cell, Tiles.highlight, speed);
                        tilemap.SetTile(cell, selectedTile);
                        previousSelectedType = SelectedType.player;
                        break;
                    case SelectedType.tile:
                        tilemap.SetTile(cell, selectedTile);
                        previousSelectedType = SelectedType.tile;
                        break;
                    case SelectedType.highlight:
                        tilemap.SetTile(previousTilePosition, normalTile);
                        selected = false;
                        break;
                }
                previousTilePosition = cell;
            }
        }
    }

    public void ChangeTiles(Vector3Int cellPosition, Tiles t, int speed)
    {
        Tile tile = null;
        AnimatedTile aTile = null;
        switch (t)
        {
            case Tiles.normal:
                aTile = normalTile;
                break;
            case Tiles.highlight:
                tile = highlightTile;
                break;
            case Tiles.selected:
                tile = selectedTile;
                break;
        }
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
            for (int i = 0; i <= speed; i++)
            {
                if(!colide)
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
                    int index = 0;
                    foreach (Vector3Int x in ir.GetTileObjectList())
                    {
                        if(x != tilePosition || t == Tiles.normal)
                        {
                            if(index == ir.GetTileObjectList().Count - 1)
                            {
                                currentPosition = tilePosition;
                                tilemap.SetTile(tilePosition, tile);
                            }
                            index++;
                        }
                        else
                        {
                            foreach(SpaceShip spaceship in ir.GetSpaceShips())
                            {
                                if(spaceship.GetCell() == tilePosition)
                                {
                                    break;
                                }
                            }
                            foreach(Enemy enemy in ir.GetEnemies())
                            {
                                if(enemy.GetCell() == tilePosition)
                                {
                                    currentPosition = tilePosition;
                                    tilemap.SetTile(tilePosition, tile);
                                    break;
                                }
                            }
                            colide = true;
                            break;
                        }
                    }
                }
                else
                {
                    colide = false;
                    break;
                }
            }
        }
    }
    public Tile GetTile(Vector3Int cell)
    {
        return (Tile) tilemap.GetTile(cell);
    }
    public bool GetSelected()
    {
        return selected;
    }
    public Tiles GetTileType(Vector3Int cell)
    {
        if ((Tile) tilemap.GetTile(cell) == normalTile) {
            return Tiles.normal;
        }
        else if((Tile)tilemap.GetTile(cell) == highlightTile)
        {
            return Tiles.highlight;
        }else if ((Tile)tilemap.GetTile(cell) == selectedTile)
        {
            return Tiles.selected;
        }
        Debug.Log("Cannot Find Tile Type");
        return Tiles.normal;
    }
}
