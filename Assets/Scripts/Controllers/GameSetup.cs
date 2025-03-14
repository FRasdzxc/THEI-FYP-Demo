// GameSetup.cs
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameSetup : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap groundTileMap;

    private void Start()
    {
        SetupInitialShips();
    }

    private void SetupInitialShips()
    {
        // Create one of each ship type as an example
        SpaceshipManager.Instance.CreateSpaceship(
            SpaceshipManager.Instance.GetDelivery(),
            GetHexagonalTileCenterPosition(new Vector3Int(0, 0, 0)),
            playerId: 1,
            false
        );

        SpaceshipManager.Instance.CreateSpaceship(
            SpaceshipManager.Instance.GetScout(),
            GetHexagonalTileCenterPosition(new Vector3Int(0, 0, 0)),
            playerId: 1,
            false
        );

        SpaceshipManager.Instance.CreateSpaceship(
            SpaceshipManager.Instance.GetCombat(),
            GetHexagonalTileCenterPosition(new Vector3Int(0, 0, 0)),
            playerId: 1,
            false
        );
    }
    public Vector3 GetHexagonalTileCenterPosition(Vector3Int tilePosition)
    {
        Vector3 tileCenter = groundTileMap.GetCellCenterWorld(tilePosition);
        tileCenter.z = 0;
        return tileCenter;
    }
}