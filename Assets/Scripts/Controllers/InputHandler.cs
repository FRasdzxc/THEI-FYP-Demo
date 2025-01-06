using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    private Camera mainCamera;
    private Spaceship selectedSpaceship;
    private Planet selectedPlanet;
    private List<Spaceship> queuedSpaceships = new List<Spaceship>();
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap planetTilemap;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        if (GameManager.Instance.IsProcessingTurn()) // Disable mouse click when processing turn
        {
            return;
        }
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = groundTilemap.WorldToCell(mouseWorldPos);
        cellPosition.z = 0;
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) // Prevent clicking tile under ui
        {
            return;
        }
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        if (hit.collider != null)
        {
            Spaceship ship = hit.collider.GetComponent<Spaceship>();
            if (ship != null)
            {
                HandleSpaceshipSelection(ship);
                return; // Exit early if hit a spaceship
            }
        }
        if (selectedSpaceship != null && MovementIndicator.Instance.IsValidMove(cellPosition))
        {
            HandleSpaceshipMovement(cellPosition);
            return; // Exit early if hit path
        }
        if (planetTilemap.HasTile(cellPosition))
        {
            Planet planet = InformationRecorder.Instance.FindPlanet(cellPosition);
            if (planet != null)
            {
                HandlePlanetSelection(planet);
            }
        }

    }
    #region Spaceship Selection
    public void HandleSpaceshipSelection(Spaceship ship)
    {
        if (selectedSpaceship == ship) // If clicking the same ship that's already selected, deselect it
        {
            DeselectSpaceship();
            PlanetDetailsUI.Instance.spaceshipInfoPanel.SetActive(false);
        }
        else // If clicking a different ship or no ship was selected
        {
            if (selectedSpaceship != null) // Deselect previous ship if there was one
            {
                DeselectSpaceship();
            }
            PlanetDetailsUI.Instance.ShowSpaceshipInfoPanel(ship, true);
            SelectSpaceship(ship);
        }
    }
    private void SelectSpaceship(Spaceship ship)
    {
        selectedSpaceship = ship;
        selectedSpaceship.ShowMovementRange();
        selectedSpaceship.OnSelected();
    }

    private void DeselectSpaceship()
    {
        selectedSpaceship.HideMovementRange();
        selectedSpaceship.OnDeselected();
        selectedSpaceship = null;
    }
    #endregion Spaceship Selection
    #region Planet Selection
    private void HandlePlanetSelection(Planet planet)
    {
        if (selectedPlanet == planet)
        {
            DeselectPlanet();
        }
        else
        {
            if (selectedPlanet != null)
            {
                DeselectPlanet();
            }
            SelectPlanet(planet);
        }
    }

    private void SelectPlanet(Planet planet)
    {
        selectedPlanet = planet;
        selectedPlanet.OnSelectedPlanet();
    }

    private void DeselectPlanet()
    {
        selectedPlanet.OnDeselectedPlanet();
        selectedPlanet = null;
    }

    #endregion Planet Selection
    #region Spaceship Movement
    private void HandleSpaceshipMovement(Vector3Int cellPosition)
    {
        Vector3 targetPosition = groundTilemap.CellToWorld(cellPosition);
        selectedSpaceship.Move(cellPosition, targetPosition);
        if (GetQueuedSpaceships().Count > 0)
        {
            foreach (Spaceship spaceship in GetQueuedSpaceships())
            {
                if (selectedSpaceship == spaceship)
                {
                    RemoveQueuedSpaceship(spaceship);
                    break;
                }
            }
        }
        AddQueuedSpaceship(selectedSpaceship);
        if (planetTilemap.HasTile(cellPosition))
        {
            Planet planet = InformationRecorder.Instance.FindPlanet(cellPosition);
            selectedSpaceship.ReadyForBattle(planet);
        }
    }
    public void AddQueuedSpaceship(Spaceship spaceship)
    {
        queuedSpaceships.Add(spaceship);
        Debug.Log("Added Queued Spaceship" + " " + spaceship.spaceshipName);
    }
    public void RemoveQueuedSpaceship(Spaceship spaceship)
    {
        queuedSpaceships.Remove(spaceship);
        Debug.Log("Removed Queued Spaceship" + " " + spaceship.spaceshipName);
    }
    public List<Spaceship> GetQueuedSpaceships()
    {
        return queuedSpaceships;
    }
    #endregion Spaceship Movement
    public void ResetValue()
    {
        selectedPlanet = null;
        selectedSpaceship = null;
        MovementIndicator.Instance.ClearIndicators();
        queuedSpaceships.Clear();
    }
    public void ResetForUI()
    {
        selectedPlanet = null;
        selectedSpaceship = null;
        MovementIndicator.Instance.ClearIndicators();
    }
}