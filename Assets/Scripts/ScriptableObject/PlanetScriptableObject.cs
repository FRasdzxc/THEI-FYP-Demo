using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewPlanetData", menuName = "Game/Planet Data")]
public class PlanetScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class ResourceRange
    {
        public int minValue;
        public int maxValue;
    }
    public List<string> planetNames = new List<string> { 
        "Alderaan", "Betelgeuse", "Ceres", "DeltaV", "Elysium", "Faelon", "Galatea", "Hyperion", "Icarus", 
        "Juno", "Kepler", "Lyra", "Mimas", "Nova", "Orion", "Phaedra", "Quasar", "Rigel", "Sirius", "Triton", 
        "Umbra", "Vega", "Wraith", "Xenon", "Yavin", "Zephyr", "Altair", "Borealis", "Callisto", "Draconis", 
        "Erebos", "Fortuna", "Ganymede", "Halcyon", "Ionia", "Jove", "Kronos", "Luminis", "Metis", "Nemesis", 
        "Obsidian", "Phoenix", "Quantum", "Rhea", "Sagan", "Thalassa", "Utopia", "Vortex", "Wyvern", "Xerxes", 
        "Ymir", "Zaurak", "Aether", "Balor", "Caelum", "Deimos", "Elysia", "Fornax", "Gaia", "Helios", "Ignis", 
        "Janus", "Krios", "Lira", "Maia", "Nereid", "Oberon", "Pallas", "Quillon", "Riven", "Selene", "Titania", 
        "Ursa", "Vesper", "Warden", "Xandar", "Yara", "Zeta", "Avalon", "Boreal", "Cygnus", "Draco", "Eon", 
        "Fomalhaut", "Gemini", "Hesperia", "Io", "Jorel", "Kestrel", "Lothar", "Mistral", "Nyx", "Orpheus", 
        "Pax", "Qwin", "Rhys", "Solara", "Thorne", "Ulric", "Vela" 
    };
    public ResourceRange fuelRange;
    public ResourceRange bioMassRange;
    public ResourceRange asteroidOreRange;
    public ResourceRange darkMatterRange;
    public ResourceRange solarCrystalRange;
    [Range(0f, 1f)]
    public float alienSpawnChance;
    public int resourceLimit = 1000;
    public int minSoldiers = 1;
    public int maxSoldiers = 5;
    public int weaponTierMin = 1;
    public int weaponTierMax = 3;
    public AnimatedTile[] planetSprites;
}