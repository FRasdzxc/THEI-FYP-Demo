using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using DG.Tweening;

public class Spaceship : MonoBehaviour
{
    [Header("Basic Info")]
    public SpaceShipScriptableObject data;
    public int currentTier = 0; // 0-2 for three tiers
    public string spaceshipName;
    public Vector3Int t_position; // Position on Tile
    public int ownerPlayerId;
    private Planet inPlanet;
    bool battle = false;

    [Header("Movement")]
    public bool hasMovedThisTurn;
    public float moveSpeed = 5f; // Speed of movement animation
    public float rotateSpeed = 360f; // Degrees per second for rotation
    private Vector3Int? t_queuedMovement = null;
    private Vector3? queuedMovement = null;
    public bool hasQueuedMove => queuedMovement.HasValue;

    [Header("Components")]
    public PolygonCollider2D collider2d;
    private SpriteRenderer spriteRenderer;

    [Header("Attributes")]
    public double soldiers;
    public double weaponTier;
    public int currentFuel;
    public int fuelTankLevel;
    public int speedLevel;
    public int viewDistanceLevel;
    public int soldiersLevel;
    public int weaponTierLevel;
    public int bioMassCapacityLevel;
    public int asteroidOreCapacityLevel;
    public int darkMatterCapacityLevel;
    public int solarCrystalCapacityLevel;

    [Header("Resources")]
    public int bioMass;
    public int asteroidOre;
    public int darkMatter;
    public int solarCrystal;
    private void Awake()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        gameObject.tag = "Spaceship";
    }
    private void Start()
    {
        soldiers = 1000;
        weaponTier = 1;
        UpdateSprite();
        UpdateCollider();
        ResetSpaceship();
        RefillFuel();
    }
    public void UpdateCollider()
    {
        var rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        var polyCollider = gameObject.AddComponent<PolygonCollider2D>();
        polyCollider.points = collider2d.points;
        polyCollider.isTrigger = true;
        polyCollider.layerOverridePriority = collider2d.layerOverridePriority;
        polyCollider.offset = collider2d.offset;

        // If there are multiple paths (holes in the collider), copy those too
        for (int i = 1; i < collider2d.pathCount; i++)
        {
            polyCollider.SetPath(i, collider2d.GetPath(i));
        }
    }

    public void RefillFuel()
    {
        currentFuel = GetMaxFuel();
    }

    #region Attribute Getters
    public int GetMaxFuel() => (int)data.fuelTank.levels[fuelTankLevel].value;
    public int GetSpeed() => (int) data.speed.levels[speedLevel].value;
    public int GetViewDistance() => (int) data.viewDistance.levels[viewDistanceLevel].value;
    public int GetMaxSoldiers() => (int) data.maxSoldiers.levels[soldiersLevel].value;
    public double GetWeaponTier() => (double) data.weaponTier.levels[weaponTierLevel].value;
    public int GetBioMassCapacity() => (int) data.bioMassCapacity.levels[bioMassCapacityLevel].value;
    public int GetAsteroidOreCapacity() => (int) data.asteroidOreCapacity.levels[asteroidOreCapacityLevel].value;
    public int GetDarkMatterCapacity() => (int) data.darkMatterCapacity.levels[darkMatterCapacityLevel].value;
    public int GetSolarCrystalCapacity() => (int) data.solarCrystalCapacity.levels[solarCrystalCapacityLevel].value;
    #endregion

    #region Upgrade Methods
    public bool CanUpgradeAttribute(ref int currentLevel, SpaceShipScriptableObject.UpgradableAttribute attribute)
    {
        if (currentLevel >= 2) return false; // Max level reached

        SpaceShipScriptableObject.UpgradeLevel nextLevel = attribute.levels[currentLevel + 1];
        return HasEnoughResources(
            nextLevel.bioMassCost,
            nextLevel.asteroidOreCost,
            nextLevel.darkMatterCost,
            nextLevel.solarCrystalCost
        );
    }

    public bool UpgradeAttribute(ref int currentLevel, SpaceShipScriptableObject.UpgradableAttribute attribute)
    {
        if (!CanUpgradeAttribute(ref currentLevel, attribute)) return false;

        SpaceShipScriptableObject.UpgradeLevel nextLevel = attribute.levels[currentLevel + 1];
        if (SpendResources(
            nextLevel.bioMassCost,
            nextLevel.asteroidOreCost,
            nextLevel.darkMatterCost,
            nextLevel.solarCrystalCost
        ))
        {
            currentLevel++;
            return true;
        }
        return false;
    }

    public bool CanUpgradeTier()
    {
        if (currentTier >= 2) return false; // Max tier reached

        return HasEnoughResources(
            data.tierUpgradeBioMassCost[currentTier],
            data.tierUpgradeAsteroidOreCost[currentTier],
            data.tierUpgradeDarkMatterCost[currentTier],
            data.tierUpgradeSolarCrystalCost[currentTier]
        );
    }

    public bool UpgradeTier()
    {
        if (!CanUpgradeTier()) return false;

        if (SpendResources(
            data.tierUpgradeBioMassCost[currentTier],
            data.tierUpgradeAsteroidOreCost[currentTier],
            data.tierUpgradeDarkMatterCost[currentTier],
            data.tierUpgradeSolarCrystalCost[currentTier]
        ))
        {
            currentTier++;
            // Update sprite based on new tier
            UpdateSprite();
            return true;
        }
        return false;
    }
    #endregion

    #region Resource Management
    private bool HasEnoughResources(int bioMassCost, int asteroidOreCost, int darkMatterCost, int solarCrystalCost)
    {
        return bioMass >= bioMassCost &&
               asteroidOre >= asteroidOreCost &&
               darkMatter >= darkMatterCost &&
               solarCrystal >= solarCrystalCost;
    }

    private bool SpendResources(int bioMassCost, int asteroidOreCost, int darkMatterCost, int solarCrystalCost)
    {
        if (!HasEnoughResources(bioMassCost, asteroidOreCost, darkMatterCost, solarCrystalCost))
            return false;

        bioMass -= bioMassCost;
        asteroidOre -= asteroidOreCost;
        darkMatter -= darkMatterCost;
        solarCrystal -= solarCrystalCost;
        return true;
    }

    public bool CanAddResource(string resourceType, int amount)
    {
        switch (resourceType.ToLower())
        {
            case "biomass": return bioMass + amount <= GetBioMassCapacity();
            case "asteroidore": return asteroidOre + amount <= GetAsteroidOreCapacity();
            case "darkmatter": return darkMatter + amount <= GetDarkMatterCapacity();
            case "solarcrystal": return solarCrystal + amount <= GetSolarCrystalCapacity();
            default: return false;
        }
    }

    public bool AddResource(string resourceType, int amount)
    {
        if (!CanAddResource(resourceType, amount)) return false;

        switch (resourceType.ToLower())
        {
            case "biomass": bioMass += amount; break;
            case "asteroidore": asteroidOre += amount; break;
            case "darkmatter": darkMatter += amount; break;
            case "solarcrystal": solarCrystal += amount; break;
            default: return false;
        }
        return true;
    }
    #endregion

    #region Movement
    public bool CanMove(Vector3Int targetPosition)
    {
        int distance = CalculateDistance(t_position, targetPosition);
        Debug.Log("Spaceship " + spaceshipName + " is moving to " + targetPosition);
        Debug.Log("hasMovedThisTurn: " + hasMovedThisTurn);
        Debug.Log($"Distance: {distance}, Fuel: {currentFuel}, Speed: {GetSpeed()}");
        Debug.Log($"Can move: {currentFuel >= distance && distance <= GetSpeed()}");
        return currentFuel >= distance && distance <= GetSpeed();
    }

    public bool Move(Vector3Int targetTile, Vector3 targetPosition, bool immediate = false)
    {
        if (!immediate)
        {
            // Queue the movement for later
            if (MovementIndicator.Instance.IsValidMove(targetTile))
            {
                if(t_queuedMovement != null)
                {
                    MovementIndicator.Instance.DeselectSelectedPath(t_queuedMovement.Value);
                }
                t_queuedMovement = targetTile;
                queuedMovement = targetPosition;
                MovementIndicator.Instance.ShowSelectedPath(targetTile);
                return true;
            }
            return false;
        }
        if (!CanMove(targetTile)) return false;
        t_queuedMovement = targetTile;
        queuedMovement = targetPosition;
        // Immediate movement logic (existing move code)
        int distance = CalculateDistance(t_position, targetTile);
        currentFuel -= distance;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(targetPosition.x, targetPosition.y, 0);

        Vector3 direction = (targetPos - currentPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Sequence moveSequence = DOTween.Sequence();
        moveSequence.Append(transform.DORotate(new Vector3(0, 0, angle - 90), 0.3f));
        moveSequence.Append(transform.DOMove(targetPos, distance / moveSpeed));

        moveSequence.OnComplete(() => {
            t_position = targetTile;
            queuedMovement = null;
            t_queuedMovement = null;
            battle = false;
        });

        return true;
    }
    public void ExecuteQueuedMovement()
    {
        if (queuedMovement.HasValue && !battle) //i think that when spaceship is having a battle, the collider trigger with the current planet and then it 
        {
            if(!gameObject.activeSelf) // in Planet
            {
                inPlanet.RemoveSpaceship(this);
                gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
                gameObject.SetActive(true);
            }
            Move(t_queuedMovement.Value, queuedMovement.Value, true);
        }
    }

    public void CancelQueuedMovement()
    {
        MovementIndicator.Instance.ClearIndicators();
    }

    public void ShowMovementRange()
    {
        MovementIndicator.Instance.ShowAvailablePaths(t_position, GetSpeed());
    }

    public void HideMovementRange()
    {
        MovementIndicator.Instance.ClearIndicators();
    }

    private int CalculateDistance(Vector3Int from, Vector3Int to)
    {
        HexCoordinates fromHex = HexCoordinates.FromOffsetCoordinates(from);
        HexCoordinates toHex = HexCoordinates.FromOffsetCoordinates(to);
        return fromHex.DistanceTo(toHex);
    }
    #endregion

    private void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = data.tierSprites[currentTier];
            spriteRenderer.sortingOrder = 1; // Ensure spaceship is visible above tiles
        }
    }
    public void OnSelected()
    {
        if(t_queuedMovement != null)
        {
            MovementIndicator.Instance.ShowSelectedPath(t_queuedMovement.Value);
        }
    }
    public void OnDeselected()
    {

    }
    private void OnDestroy()
    {
        // Kill all tweens associated with this object when destroyed
        transform.DOKill();
    }
    public Vector3 GetPosition()
    {
        return this.transform.position;
    }
    public void ResetSpaceship()
    {
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
        this.hasMovedThisTurn = false;
        this.t_queuedMovement = null;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            Planet planet;
            if (t_queuedMovement != null)
            {
                planet = InformationRecorder.Instance.FindPlanet(t_queuedMovement);
            }
            else
            {
                planet = InformationRecorder.Instance.FindPlanet(t_position);
            }

            if (planet != null)
            {
                inPlanet = planet;
                planet.isOwned = true;
                planet.AddSpaceship(this);
                gameObject.SetActive(false);
            }
        }
    }
    public void ReadyForBattle(Planet planet)
    {
        if (planet.hasAlien)
        {
            EventManager.Instance.QueueBattleEvent(planet, this);
            CancelQueuedMovement();
            battle = true;
            Debug.Log($"Spaceship {spaceshipName} engaging aliens on {planet.planetName}!");
        }
    }
    public bool IsInBattle()
    {
        return battle;
    }
    public void DestroyShip()
    {
        if (inPlanet != null)
        {
            inPlanet.RemoveSpaceship(this);
        }

        CancelQueuedMovement();
        AudioManager.Instance.PlayExplodeSFX();
        InputHandler.Instance.RemoveQueuedSpaceship(this);
        SpaceshipManager.Instance.RemoveSpaceship(this);
        Destroy(gameObject);
    }

    public void OnBattleVictory(Planet planet)
    {
        Debug.Log("Victory");
        Move(t_queuedMovement.Value, queuedMovement.Value, true);
    }
}