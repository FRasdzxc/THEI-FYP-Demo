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

    private void Awake()
    {
        Time.timeScale = 1;
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private bool AreAllSpaceshipsDestroyed()
    {
        if(InformationRecorder.Instance.GetAllSpaceships().Count <= 0)
        {
            return true;
        }
        return false;
    }
    private void EndGame()
    {
        Time.timeScale = 0;
        PlanetDetailsUI.Instance.HidePanel();
        PlanetDetailsUI.Instance.ShowGameOverPanel();
        Debug.Log("All spaceships are destroyed. Game Over!");
    }
    public void EndTurn()
    {
        if (isProcessingTurn) return;
        PlanetDetailsUI.Instance.HidePanel();
        PlanetDetailsUI.Instance.ShowSpaceshipInfoPanel(null, false);
        StartCoroutine(ProcessEndTurn());
    }

    private IEnumerator ProcessEndTurn()
    {
        isProcessingTurn = true;

        yield return StartCoroutine(ExecuteQueuedMovements());

        PlanetsUpdateResources();

        if (AreAllSpaceshipsDestroyed())
        {
            EndGame();
            yield break;
        }
        // Move to next player
        currentPlayerIndex = (currentPlayerIndex + 1) % numberOfPlayers;

        if (currentPlayerIndex == 0)
        {
            currentTurn++;
        }

        OnTurnChanged();

        isProcessingTurn = false;
    }
    private void OnTurnChanged()
    {
        Debug.Log($"Turn {currentTurn}, Player {currentPlayerIndex + 1}'s turn");
    }

    public bool IsProcessingTurn() => isProcessingTurn;
    #region Spaceship
    private IEnumerator ExecuteQueuedMovements()
    {
        List<Spaceship> spaceships = new List<Spaceship>(InputHandler.Instance.GetQueuedSpaceships());

        foreach (Spaceship spaceship in spaceships)
        {
            if (spaceship.CheckBattle())
            {
                EventManager.Instance.ProcessAllEvents();
                yield return new WaitForSeconds(1f);

            }
            else if (spaceship.hasQueuedMove)
            {
                spaceship.ExecuteQueuedMovement();
                yield return new WaitForSeconds(1f); // Wait between each ship's movement
            }
            if(spaceship != null)
            {
                spaceship.ResetSpaceship();
            }
        }
        InputHandler.Instance.ResetValue();
    }
    #endregion Spaceship
    #region Planet
    private void PlanetsUpdateResources()
    {
        List<Planet> planets = InformationRecorder.Instance.GetAllOwnedPlanets(); // Update all planets' resources
        foreach (Planet planet in planets)
        {
            planet.UpdateResources();
        }
    }
    #endregion Planet
}