// SpaceshipManager.cs
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class SpaceshipManager : MonoBehaviour
{
    public static SpaceshipManager Instance { get; private set; }

    private Spaceship selectedSpaceship;
    private List<Spaceship> spaceships = new List<Spaceship>();
    [Header("References")]
    public Tilemap groundTilemap;
    public GameObject tilePrefab; // Assign this in the inspector
    private PolygonCollider2D tileCollider;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            tileCollider = tilePrefab.GetComponent<PolygonCollider2D>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Spaceship CreateSpaceship(SpaceShipScriptableObject data, Vector3 position, int playerId)
    {
        GameObject spaceshipObj = new GameObject($"{data.type} Ship");
        Spaceship ship = spaceshipObj.AddComponent<Spaceship>();
        ship.collider2d = tileCollider;
        ship.data = data;
        ship.t_position = groundTilemap.WorldToCell(position);
        ship.t_position.z = 0;
        ship.ownerPlayerId = playerId;
        ship.spaceshipName = $"{data.type} Ship {playerId}-{Random.Range(1000, 9999)}";
        
        // Set initial position
        ship.transform.position = new Vector3(position.x, position.y, position.z);

        spaceships.Add(ship);
        return ship;
    }

    public List<Spaceship> GetAllSpaceships()
    {
        return spaceships;
    }

    public List<Spaceship> GetPlayerSpaceships(int playerId)
    {
        return spaceships.FindAll(ship => ship.ownerPlayerId == playerId);
    }
}