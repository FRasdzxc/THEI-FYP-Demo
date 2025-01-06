// Planet.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Planet : MonoBehaviour
{
    [Header("Basic Informations")]
    public string planetName;
    public AnimatedTile planetSprite;
    public Vector3Int t_position;
    public bool isOwned = false;
    public bool isHomePlanet = false;

    [Header("Resources")]
    public int bioMass;
    public int asteroidOre;
    public int darkMatter;
    public int solarCrystal;

    // Growth rates (randomly selected between min and max)
    public int bioMassGrowthRate;
    public int asteroidOreGrowthRate;
    public int darkMatterGrowthRate;
    public int solarCrystalGrowthRate;

    public int resourceLimit;

    [Header("Alien Information")]
    public bool hasAlien;
    public int soldiers;
    public int weaponTier;

    private PlanetDetailsUI detailsUI;

    [Header("Spaceships")]
    private List<Spaceship> spaceships = new List<Spaceship>(); // Spaceships entered planet

    private void Awake()
    {
        detailsUI = FindObjectOfType<PlanetDetailsUI>();
    }

    public void InitializeGrowthRates(PlanetScriptableObject planetData)
    {
        // Set random growth rates within the specified ranges
        bioMassGrowthRate = Random.Range(planetData.bioMassRange.minValue, planetData.bioMassRange.maxValue + 1);
        asteroidOreGrowthRate = Random.Range(planetData.asteroidOreRange.minValue, planetData.asteroidOreRange.maxValue + 1);
        darkMatterGrowthRate = Random.Range(planetData.darkMatterRange.minValue, planetData.darkMatterRange.maxValue + 1);
        solarCrystalGrowthRate = Random.Range(planetData.solarCrystalRange.minValue, planetData.solarCrystalRange.maxValue + 1);

        // Initialize resources at zero
        bioMass = 0;
        asteroidOre = 0;
        darkMatter = 0;
        solarCrystal = 0;
    }

    public void UpdateResources()
    {
        bioMass = Mathf.Min(resourceLimit, bioMass + bioMassGrowthRate);
        asteroidOre = Mathf.Min(resourceLimit, asteroidOre + asteroidOreGrowthRate);
        darkMatter = Mathf.Min(resourceLimit, darkMatter + darkMatterGrowthRate);
        solarCrystal = Mathf.Min(resourceLimit, solarCrystal + solarCrystalGrowthRate);
    }

    public void OnSelectedPlanet()
    {
        MovementIndicator.Instance.ShowSelectedPath(t_position);
        detailsUI.ShowPlanetDetails(this);
    }

    public void OnDeselectedPlanet()
    {
        MovementIndicator.Instance.ClearIndicators();
        detailsUI.HidePanel();
    }

    // Getter methods for UI display
    public int GetBioMassGrowthRate() => bioMassGrowthRate;
    public int GetAsteroidOreGrowthRate() => asteroidOreGrowthRate;
    public int GetDarkMatterGrowthRate() => darkMatterGrowthRate;
    public int GetSolarCrystalGrowthRate() => solarCrystalGrowthRate;
    public void AddSpaceship(Spaceship spaceship)
    {
        spaceships.Add(spaceship);
    }
    public void RemoveSpaceship(Spaceship spaceship)
    {
        spaceships.Remove(spaceship);
    }
    public List<Spaceship> GetSpaceships()
    {
        return spaceships;
    }
}