using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GameState { PlayerTurn, EnemyTurn }
    private GameState currentState;
    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.PlayerTurn;
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (true)
        {
            switch (currentState)
            {
                case GameState.PlayerTurn:
                    Debug.Log("Player's Turn");
                    yield return new WaitUntil(() => PlayerActionDone());
                    currentState = GameState.EnemyTurn;
                    break;
                case GameState.EnemyTurn:
                    Debug.Log("Enemy's Turn");
                    EnemyAction();
                    currentState = GameState.PlayerTurn;
                    break;
            }
            yield return null;
        }
    }
    bool PlayerActionDone()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
    void EnemyAction()
    {

    }
}
