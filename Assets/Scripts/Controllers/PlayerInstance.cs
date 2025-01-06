using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInstance : MonoBehaviour
{
    public static PlayerInstance Instance { get; private set; }
    public int playerId;
    public int totalBioMass = 0;
    public int totalAsteroidOre = 0;
    public int totalDarkMatter = 0;
    public int totalSolarCrystal = 0;

    private List<Spaceship> ownedSpaceships = new List<Spaceship>();
    private List<Planet> ownedPlanets = new List<Planet>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddResources(int bioMass, int asteroidOre, int darkMatter, int solarCrystal)
    {
        totalBioMass += bioMass;
        totalAsteroidOre += asteroidOre;
        totalDarkMatter += darkMatter;
        totalSolarCrystal += solarCrystal;
    }

    public void AddSpaceship(Spaceship spaceship)
    {
        if (!ownedSpaceships.Contains(spaceship))
        {
            ownedSpaceships.Add(spaceship);
        }
    }

    public void RemoveSpaceship(Spaceship spaceship)
    {
        if (ownedSpaceships.Contains(spaceship))
        {
            ownedSpaceships.Remove(spaceship);
        }
    }

    public void AddPlanet(Planet planet)
    {
        if (!ownedPlanets.Contains(planet))
        {
            ownedPlanets.Add(planet);
            planet.isOwned = true;
        }
    }

    public void RemovePlanet(Planet planet)
    {
        if (ownedPlanets.Contains(planet))
        {
            ownedPlanets.Remove(planet);
            planet.isOwned = false;
        }
    }

    public List<Spaceship> GetOwnedSpaceships()
    {
        return ownedSpaceships;
    }

    public List<Planet> GetOwnedPlanets()
    {
        return ownedPlanets;
    }
    public int GetTotalBio()
    {
        return totalBioMass;
    }
    public int GetTotalAsteriod()
    {
        return totalAsteroidOre;
    }
    public int GetTotalDark()
    {
        return totalDarkMatter;
    }
    public int GetTotalSolar()
    {
        return totalSolarCrystal;
    }
}
