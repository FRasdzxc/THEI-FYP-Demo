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
        if(InputHandler.Instance.GetQueuedSpaceships().Count <= 0)
        {
            PlanetDetailsUI.Instance.ShowWarningMessage("You must move at least one spaceship before ending your turn.");
            return;
        }
        PlanetDetailsUI.Instance.HidePanel();
        StartCoroutine(ProcessEndTurn());
    }

    private IEnumerator ProcessEndTurn()
    {
        isProcessingTurn = true;

        yield return StartCoroutine(ExecuteQueuedMovements());

        if (AreAllSpaceshipsDestroyed())
        {
            EndGame();
            yield break;
        }

        PlanetsUpdateResources();

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
        foreach (Spaceship spaceship in InputHandler.Instance.GetQueuedSpaceships())
        {
            if (spaceship.IsInBattle())
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