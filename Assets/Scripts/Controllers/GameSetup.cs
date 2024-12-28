// GameSetup.cs
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameSetup : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap groundTileMap;

    [Header("Ship Data")]
    [SerializeField] private SpaceShipScriptableObject deliveryShipData;
    [SerializeField] private SpaceShipScriptableObject scoutShipData;
    [SerializeField] private SpaceShipScriptableObject combatShipData;

    private void Start()
    {
        SetupInitialShips();
    }

    private void SetupInitialShips()
    {
        // Create one of each ship type as an example
        SpaceshipManager.Instance.CreateSpaceship(
            deliveryShipData,
            GetHexagonalTileCenterPosition(new Vector3Int(0, 0, 0)),
            playerId: 1
        );

        SpaceshipManager.Instance.CreateSpaceship(
            scoutShipData,
            GetHexagonalTileCenterPosition(new Vector3Int(0, 0, 0)),
            playerId: 1
        );

        SpaceshipManager.Instance.CreateSpaceship(
            combatShipData,
            GetHexagonalTileCenterPosition(new Vector3Int(0, 0, 0)),
            playerId: 1
        );
    }
    public Vector3 GetHexagonalTileCenterPosition(Vector3Int tilePosition)
    {
        Vector3 tileCenter = groundTileMap.GetCellCenterWorld(tilePosition);
        tileCenter.z = 0;
        return tileCenter;
    }
}