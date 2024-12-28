using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; } //Use this Instance to control the spaceship in planet UI
    private Camera mainCamera;
    private Spaceship selectedSpaceship;
    private Planet selectedPlanet;
    private List<Spaceship> queuedSpaceships = new List<Spaceship>();
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap planetTilemap;
    [SerializeField] private HexagonalTilemapGenerator tilemapGenerator;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameManager.Instance.IsProcessingTurn()) // Disable mouse click when processing turn
            {
                return;
            }
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = groundTilemap.WorldToCell(mouseWorldPos);
            cellPosition.z = 0;
            bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if(isOverUI) // Prevent clicking tile under ui
            {
                return;
            }
            if (hit.collider != null)
            {
                Spaceship ship = hit.collider.GetComponent<Spaceship>();
                if (ship != null)
                {
                    HandleSpaceshipSelection(ship);
                    return; // Exit early if we hit a spaceship
                }
            }
            if (selectedSpaceship != null && MovementIndicator.Instance.IsValidMove(cellPosition))
            {
                Vector3 targetPosition = groundTilemap.CellToWorld(cellPosition);
                selectedSpaceship.Move(cellPosition, targetPosition);
                AddQueuedSpaceship(selectedSpaceship);
                return;
            }
            // Then check for planet interaction if we didn't hit a spaceship
            if (planetTilemap.HasTile(cellPosition))
            {
                Planet planet = tilemapGenerator.GetPlanetAtPosition(cellPosition);
                if (planet != null)
                {
                    HandlePlanetSelection(planet);
                    return; // Exit early if we clicked a planet
                }
            }
        }
    }

    public void HandleSpaceshipSelection(Spaceship ship)
    {
        if (selectedSpaceship == ship) // If clicking the same ship that's already selected, deselect it
        {
            selectedSpaceship.HideMovementRange();
            selectedSpaceship.OnDeselected();
            selectedSpaceship = null;
        }
        else // If clicking a different ship or no ship was selected
        {
            if (selectedSpaceship != null) // Deselect previous ship if there was one
            {
                selectedSpaceship.HideMovementRange();
                selectedSpaceship.OnDeselected();
            }
            selectedSpaceship = ship; // Select new ship
            selectedSpaceship.ShowMovementRange();
            selectedSpaceship.OnSelected();
        }
    }

    private void HandlePlanetSelection(Planet planet)
    {
        if(selectedPlanet == planet)
        {
            selectedPlanet.OnDeselectedPlanet();
            selectedPlanet = null;
        }
        else
        {
            // Deselect previous planet if there was one
            if(selectedPlanet != null)
            {
                selectedPlanet.OnDeselectedPlanet();
            }
            // Select new planet
            selectedPlanet = planet;
            selectedPlanet.OnSelectedPlanet();
        }
    }
    public void AddQueuedSpaceship(Spaceship spaceship)
    {
        queuedSpaceships.Add(selectedSpaceship);
        Debug.Log("Added Queued Spaceship" + " " + spaceship.spaceshipName);
    }
    public List<Spaceship> GetQueuedSpaceships()
    {
        return queuedSpaceships;
    }
    public void ResetValue()
    {
        selectedPlanet = null;
        selectedSpaceship = null;
        queuedSpaceships.Clear();
    }
}