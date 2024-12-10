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
    private Vector3Int currentPosition = new Vector3Int();

    void Start()
    {
        health = spaceShipSO.health;
        damage = spaceShipSO.damage;
        hasMoved = false;
    }
    public void ResetRound()
    {
        selected = false;
        hasMoved = false;
    }
    public void AdjustHealth(float x)
    {
        health += x;
    }
    public void SetSelected(bool x)
    {
        selected = x;
    }
    public void SetCell(Vector3Int cell)
    {
        currentPosition = cell;
    }
    public void SetHasMoved(bool moved)
    {
        hasMoved = moved;
    }
    public float GetHealth()
    {
        return health;
    }
    public float GetDamage()
    {
        return damage;
    }
    public bool GetSelected()
    {
        return selected;
    }
    public Vector3Int GetCell()
    {
        return currentPosition;
    }
    public bool GetHasMoved()
    {
        return hasMoved;
    }
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}