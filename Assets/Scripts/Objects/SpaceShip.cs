using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpaceShip : MonoBehaviour
{
    public SpaceShipScriptableObject spaceShipSO;
    public Tilemap tilemap;
    private bool hasMoved;
    private bool selected;
    private float health;
    private float damage;
    private List<Enemy> enemies = new List<Enemy>();
    private Vector3Int currentPosition = new Vector3Int();

    void Start()
    {
        hasMoved = false;
    }

    void CheckBattle(Vector3Int cell)
    {
        foreach(Enemy enemy in enemies)
        {
            if(enemy.GetCell() == cell)
            {
                enemy.DestroyObject();
            }
        }
    }
    public void SetSelected(bool x)
    {
        selected = x;
    }
    public void SetCell(Vector3Int cell)
    {
        currentPosition = cell;
    }
    public bool GetSelected()
    {
        return selected;
    }
    public Vector3Int GetCell()
    {
        return currentPosition;
    }
}