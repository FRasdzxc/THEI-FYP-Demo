using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class HexagonalTilemapGenerator : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap backgroundTilemap;
    public Tilemap planetTilemap;
    public GameObject planetPrefab;
    public GameObject homePlanetPrefab; // New prefab for home planet

    [Header("Background Tiles")]
    public AnimatedTile innerSpaceTile;
    public AnimatedTile mediumTile;
    public AnimatedTile outerSpaceTile;

    [Header("Planet Data")]
    public PlanetScriptableObject innerSpacePlanets;
    public PlanetScriptableObject mediumSpacePlanets;
    public PlanetScriptableObject outerSpacePlanets;
    public PlanetScriptableObject homePlanetData; // Data for home planet

    [Header("Generation Settings")]
    public int radius = 5;
    [Range(0f, 1f)]
    public float planetSpawnChance = 0.3f;
    public int minimumPlanetDistance = 5; // Minimum distance between planets

    private List<Planet> generatedPlanets = new List<Planet>();

    private void Start()
    {
        GenerateHexagonalPattern();
    }

    private void GenerateHexagonalPattern()
    {
        // Generate home planet first at (0,0,0)
        GenerateHomePlanet();

        // Generate background and planets for each layer
        GenerateLayer(innerSpaceTile, innerSpacePlanets, 0);
        GenerateLayer(mediumTile, mediumSpacePlanets, 1);
        GenerateLayer(outerSpaceTile, outerSpacePlanets, 2);
    }

    private void GenerateHomePlanet()
    {
        Vector3Int homePosition = new Vector3Int(0, 0, 0);
        GameObject homePlanetObj = Instantiate(homePlanetPrefab, transform);
        Planet homePlanet = homePlanetObj.GetComponent<Planet>();

        homePlanet.planetName = "Home Planet";
        homePlanet.planetSprite = homePlanetData.planetSprites[0];
        homePlanet.t_position = homePosition;
        homePlanet.resourceLimit = homePlanetData.resourceLimit;

        SetupResources(homePlanet, homePlanetData);
        homePlanet.hasAlien = false;
        homePlanet.soldiers = 0;
        homePlanet.weaponTier = 0;

        generatedPlanets.Add(homePlanet);
        planetTilemap.SetTile(homePosition, homePlanet.planetSprite);
        backgroundTilemap.SetTile(homePosition, innerSpaceTile);
    }

    private void GenerateLayer(AnimatedTile backgroundTile, PlanetScriptableObject planetData, int layerIndex)
    {
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                int s = -q - r;

                if (Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(s) <= 2 * radius)
                {
                    Vector3Int position = AxialToOffset(q, r);
                    float distance = Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s));

                    bool isInLayer = false;
                    if (layerIndex == 0 && distance <= radius / 3f)
                        isInLayer = true;
                    else if (layerIndex == 1 && distance > radius / 3f && distance <= 2f * radius / 3f)
                        isInLayer = true;
                    else if (layerIndex == 2 && distance > 2f * radius / 3f && distance <= radius)
                        isInLayer = true;

                    if (isInLayer)
                    {
                        // Set background tile
                        backgroundTilemap.SetTile(position, backgroundTile);

                        // Try to generate a planet if it's far enough from other planets
                        if (Random.value < planetSpawnChance && IsFarEnoughFromOtherPlanets(position))
                        {
                            GeneratePlanet(position, planetData);
                        }
                    }
                }
            }
        }
    }

    private bool IsFarEnoughFromOtherPlanets(Vector3Int position)
    {
        foreach (Planet planet in generatedPlanets)
        {
            float distance = CalculateHexDistance(position, planet.t_position);
            if (distance < minimumPlanetDistance)
            {
                return false;
            }
        }
        return true;
    }

    private float CalculateHexDistance(Vector3Int pos1, Vector3Int pos2)
    {
        // Convert to cube coordinates
        Vector3Int cube1 = OffsetToCube(pos1);
        Vector3Int cube2 = OffsetToCube(pos2);

        // Calculate Manhattan distance in cube coordinates
        return (Mathf.Abs(cube1.x - cube2.x) +
                Mathf.Abs(cube1.y - cube2.y) +
                Mathf.Abs(cube1.z - cube2.z)) / 2f;
    }

    private Vector3Int OffsetToCube(Vector3Int offset)
    {
        int x = offset.x - (offset.y - (offset.y & 1)) / 2;
        int z = offset.y;
        int y = -x - z;
        return new Vector3Int(x, y, z);
    }

    private Vector3Int AxialToOffset(int q, int r)
    {
        int col = q + (r - (r & 1)) / 2;
        int row = r;
        return new Vector3Int(col, row, 0);
    }

    // Existing methods remain the same
    private void GeneratePlanet(Vector3Int t_position, PlanetScriptableObject planetData)
    {
        if (planetTilemap.HasTile(t_position))
            return;

        GameObject planetObj = Instantiate(planetPrefab, transform);
        Planet planet = planetObj.GetComponent<Planet>();

        planet.planetName = planetData.planetNames[Random.Range(0, planetData.planetNames.Count)];
        planet.planetSprite = planetData.planetSprites[Random.Range(0, planetData.planetSprites.Length)];
        planet.t_position = t_position;
        planet.resourceLimit = planetData.resourceLimit;

        SetupResources(planet, planetData);
        SetupAliens(planet, planetData);

        generatedPlanets.Add(planet);
        planetTilemap.SetTile(t_position, planet.planetSprite);
    }
    private void SetupResources(Planet planet, PlanetScriptableObject planetData)
    {
        planet.InitializeGrowthRates(planetData);
    }
    private void SetupAliens(Planet planet, PlanetScriptableObject planetData)
    {
        if (Random.value < planetData.alienSpawnChance)
        {
            planet.hasAlien = true;
            planet.soldiers = Random.Range(planetData.minSoldiers, planetData.maxSoldiers + 1);
            planet.weaponTier = Random.Range(planetData.weaponTierMin, planetData.weaponTierMax + 1);
        }
        else
        {
            planet.hasAlien = false;
            planet.soldiers = 0;
            planet.weaponTier = 0;
        }
    }

    // Method to get planet data at a specific position
    public Planet GetPlanetAtPosition(Vector3Int position)
    {
        return generatedPlanets.Find(p => p.t_position == position);
    }
    public List<Planet> GetAllPlanets()
    {
        return generatedPlanets;
    }
}