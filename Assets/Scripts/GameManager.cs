using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private InformationRecorder ir;
    private TilemapController tmCtr;
    private enum GameState { PlayerTurn, EnemyTurn }
    private GameState currentState;
    // Start is called before the first frame update
    void Start()
    {
        ir = GameObject.FindWithTag("EventSystem").GetComponent<InformationRecorder>();
        tmCtr = GameObject.FindWithTag("EventSystem").GetComponent<TilemapController>();
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
                    foreach(SpaceShip spaceship in ir.GetSpaceShips())
                    {
                        spaceship.ResetRound();
                    }
                    tmCtr.ResetRound();
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
