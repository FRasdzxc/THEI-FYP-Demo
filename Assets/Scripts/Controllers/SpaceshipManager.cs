// SpaceshipManager.cs
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class SpaceshipManager : MonoBehaviour
{
    public static SpaceshipManager Instance { get; private set; }

    private List<Spaceship> spaceships = new List<Spaceship>();

    [Header("Ship Data")]
    public SpaceShipScriptableObject deliveryShipData;
    public SpaceShipScriptableObject scoutShipData;
    public SpaceShipScriptableObject combatShipData;

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
    public Spaceship CreateSpaceship(SpaceShipScriptableObject data, Vector3 position, int playerId, bool checkResources = true)
    {
        if(checkResources)
        {
            if (PlayerInstance.Instance.GetTotalAsteriod() < data.asteroidOreCost || PlayerInstance.Instance.GetTotalBio() < data.bioMassCost || PlayerInstance.Instance.GetTotalDark() < data.darkMatterCost || PlayerInstance.Instance.GetTotalSolar() < data.solarCrystalCost)
            {
                PlanetDetailsUI.Instance.ShowWarningMessage("Not enough resources to create" + data.type + " spaceship");
                return null;
            }
            PlayerInstance.Instance.AddResources(-data.bioMassCost, -data.asteroidOreCost, -data.darkMatterCost, -data.solarCrystalCost);
        }
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
        if(checkResources)
        {
            PlanetDetailsUI.Instance.ToggleSpaceshipCreationPanel(false);
            PlanetDetailsUI.Instance.ShowWarningMessage("Spaceship created successfully");
            PlanetDetailsUI.Instance.UpdatePlanetDetails();
        }
        return ship;
    }
    #region Getters
    public List<Spaceship> GetAllSpaceships()
    {
        return spaceships;
    }
    public void RemoveSpaceship(Spaceship spaceship)
    {
        spaceships.Remove(spaceship);
    }

    public List<Spaceship> GetPlayerSpaceships(int playerId)
    {
        return spaceships.FindAll(ship => ship.ownerPlayerId == playerId);
    }
    public SpaceShipScriptableObject GetCombat()
    {
        return combatShipData;
    }
    public SpaceShipScriptableObject GetScout()
    {
        return scoutShipData;
    }
    public SpaceShipScriptableObject GetDelivery()
    {
        return deliveryShipData;
    }
    #endregion Getters
}