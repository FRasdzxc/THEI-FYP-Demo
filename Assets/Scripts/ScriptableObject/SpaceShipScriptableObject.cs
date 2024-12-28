using UnityEngine;

[CreateAssetMenu(fileName = "SpaceShipData", menuName = "Game/SpaceShip Data")]
public class SpaceShipScriptableObject : ScriptableObject
{
    public enum SpaceshipType
    {
        Delivery,
        Scout,
        Combat
    }

    [System.Serializable]
    public class UpgradeLevel
    {
        public double value;
        [Header("Upgrade Costs")]
        public int bioMassCost;
        public int asteroidOreCost;
        public int darkMatterCost;
        public int solarCrystalCost;
    }

    [System.Serializable]
    public class UpgradableAttribute
    {
        public UpgradeLevel[] levels = new UpgradeLevel[3];
    }

    [Header("Basic Info")]
    public SpaceshipType type;
    public string[] tierNames; // Names for each tier (3 tiers)
    public Sprite[] tierSprites; // Sprites for each tier

    [Header("Base Attributes")]
    public UpgradableAttribute fuelTank;
    public UpgradableAttribute speed;
    public UpgradableAttribute viewDistance;

    [Header("Combat Attributes")]
    public UpgradableAttribute maxSoldiers;
    public UpgradableAttribute weaponTier;

    [Header("Cargo Attributes")]
    public UpgradableAttribute bioMassCapacity;
    public UpgradableAttribute asteroidOreCapacity;
    public UpgradableAttribute darkMatterCapacity;
    public UpgradableAttribute solarCrystalCapacity;

    [Header("Tier Upgrade Costs")]
    public int[] tierUpgradeBioMassCost;
    public int[] tierUpgradeAsteroidOreCost;
    public int[] tierUpgradeDarkMatterCost;
    public int[] tierUpgradeSolarCrystalCost;
}