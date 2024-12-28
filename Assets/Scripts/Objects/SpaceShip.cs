// Spaceship.cs
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
    private int currentFuel;
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
    public int soldiers;
    private void Awake()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        gameObject.tag = "Spaceship";
    }
    private void Start()
    {
        UpdateSprite();
        UpdateCollider();
        ResetSpaceship();
        RefillFuel();
    }
    public void UpdateCollider()
    {
        gameObject.AddComponent<Rigidbody2D>();
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        gameObject.AddComponent<PolygonCollider2D>();
        Vector2[] points = new Vector2[collider2d.points.Length];
        for (int i = 0; i < collider2d.points.Length; i++)
        {
            points[i] = collider2d.points[i];
        }
        gameObject.GetComponent<PolygonCollider2D>().points = points;

        // If there are multiple paths (holes in the collider), copy those too
        if (collider2d.pathCount > 1)
        {
            for (int i = 1; i < collider2d.pathCount; i++)
            {
                Vector2[] path = new Vector2[collider2d.GetPath(i).Length];
                collider2d.GetPath(i).CopyTo(path, 0);
                gameObject.GetComponent<PolygonCollider2D>().SetPath(i, path);
            }
        }
        gameObject.GetComponent<PolygonCollider2D>().layerOverridePriority = collider2d.layerOverridePriority;
        gameObject.GetComponent<PolygonCollider2D>().offset = collider2d.offset;
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
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
    public int GetWeaponTier() => (int) data.weaponTier.levels[weaponTierLevel].value;
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
        if (hasMovedThisTurn) return false;

        int distance = CalculateDistance(GetPosition(), targetPosition);
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

        // Immediate movement logic (existing move code)
        if (!CanMove(targetTile)) return false;
        int distance = CalculateDistance(GetPosition(), targetPosition);
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
            hasMovedThisTurn = true;
            queuedMovement = null;
            t_queuedMovement = null;
        });

        return true;
    }
    public void ExecuteQueuedMovement()
    {
        if (queuedMovement.HasValue)
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
        queuedMovement = null;
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
    // Smooth rotation to face a point
    public void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.DORotate(new Vector3(0, 0, angle - 90), 0.3f);
    }

    private int CalculateDistance(Vector3 from, Vector3 to)
    {
        //Vector3 tileCenter = .GetCellCenterWorld(tilePosition);
        Vector3 gameObjectPosition = transform.position;

        //float distance = Vector3.Distance(gameObjectPosition, tileCenter);
        return 1; // Placeholder
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
        // Add visual feedback when selected
        if(t_queuedMovement != null)
        {
            MovementIndicator.Instance.ShowSelectedPath(t_queuedMovement.Value);
        }
        transform.DOScale(1.1f, 0.2f);
    }
    public void OnDeselected()
    {
        // Remove visual feedback
        transform.DOScale(1f, 0.2f);
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
        if(collision.CompareTag("Planet"))
        {
            Planet planet;
            if (t_queuedMovement != null) 
            {
                planet = InformationRecorder.Instance.FindPlanet(t_queuedMovement);
            }else
            {
                planet = InformationRecorder.Instance.FindPlanet(t_position);
            }
            if(planet != null)
            {
                inPlanet = planet;
                planet.AddSpaceship(gameObject.GetComponent<Spaceship>());
                gameObject.SetActive(false);
            }
        }
    }
}