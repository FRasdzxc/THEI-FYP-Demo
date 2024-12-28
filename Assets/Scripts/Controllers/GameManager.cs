// GameManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Turn Settings")]
    public int currentTurn = 1;
    public int currentPlayerIndex = 0;
    public int numberOfPlayers = 2;
    private bool isProcessingTurn = false;

    private InputHandler input;
    private HexagonalTilemapGenerator tilemapGenerator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        input = FindObjectOfType<InputHandler>();
        tilemapGenerator = FindObjectOfType<HexagonalTilemapGenerator>();
    }

    public void EndTurn()
    {
        if (isProcessingTurn) return;
        PlanetDetailsUI.Instance.HidePanel();
        StartCoroutine(ProcessEndTurn());
    }

    private IEnumerator ProcessEndTurn()
    {
        isProcessingTurn = true;

        // Execute all queued movements first
        yield return StartCoroutine(ExecuteQueuedMovements());

        // Update all planets' resources
        List<Planet> planets = tilemapGenerator.GetAllPlanets();
        foreach (Planet planet in planets)
        {
            planet.UpdateResources();
        }

        // Move to next player
        currentPlayerIndex = (currentPlayerIndex + 1) % numberOfPlayers;

        // If we've gone through all players, advance to next turn
        if (currentPlayerIndex == 0)
        {
            currentTurn++;
        }

        // Reset movement indicators and trigger turn change events
        MovementIndicator.Instance.ClearIndicators();
        OnTurnChanged();

        isProcessingTurn = false;
    }

    private IEnumerator ExecuteQueuedMovements()
    {
        // Get all spaceships with queued movements
        List<Spaceship> spaceships = InputHandler.Instance.GetQueuedSpaceships();

        foreach (Spaceship spaceship in spaceships)
        {
            if (spaceship.hasQueuedMove)
            {
                spaceship.ExecuteQueuedMovement();
                yield return new WaitForSeconds(1f); // Wait between each ship's movement
            }
            spaceship.ResetSpaceship();
        }
        input.ResetValue();
    }

    private void OnTurnChanged()
    {
        // Implement any logic that should happen when turns change
        Debug.Log($"Turn {currentTurn}, Player {currentPlayerIndex + 1}'s turn");
    }

    public bool IsProcessingTurn() => isProcessingTurn;
}