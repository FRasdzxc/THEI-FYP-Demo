using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InformationRecorder : MonoBehaviour
{
    public static InformationRecorder Instance { get; private set; }
    [SerializeField] private Tilemap indicatorTilemap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap planetTilemap;
    [SerializeField] private HexagonalTilemapGenerator tilemapGenerator;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public Planet FindPlanet(Vector3Int? position)
    {
        List<Planet> planets = tilemapGenerator.GetAllPlanets();
        foreach(Planet planet in planets)
        {
            if(planet.t_position == (Vector3Int) position)
            {
                return planet;
            }
        }
        return null;
    }
    public List<Planet> GetAllOwnedPlanets()
    {
        List<Planet> planets = tilemapGenerator.GetAllPlanets();
        List<Planet> ownedPlanets = new List<Planet>();
        foreach (Planet planet in planets)
        {
            if (planet.isOwned)
            {
                ownedPlanets.Add(planet);
            }
        }
        return ownedPlanets;
    }
    public List<Vector3Int> GetAllSpaceships()
    {
        List<Spaceship> spaceships = SpaceshipManager.Instance.GetAllSpaceships();
        List<Vector3Int> t_spaceships = new List<Vector3Int>();
        foreach(Spaceship spaceship in spaceships)
        {
            t_spaceships.Add(spaceship.t_position);
        }
        return t_spaceships;
    }
    public List<Vector3Int> GetAllPlanets()
    {
        List<Planet> planets = tilemapGenerator.GetAllPlanets();
        List<Vector3Int> t_planets = new List<Vector3Int>();
        foreach(Planet planet in planets)
        {
            t_planets.Add(planet.t_position);
        }
        return t_planets;
    }
    public List<Vector3Int> GetAllObjects()
    {
        List<Vector3Int> objects = new List<Vector3Int>();
        objects.AddRange(GetAllPlanets());
        objects.AddRange(GetAllSpaceships());
        return objects;
    }
}
