using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    private InformationRecorder ir;
    // Start is called before the first frame update
    void Start()
    {
        ir = GameObject.FindWithTag("EventSystem").GetComponent<InformationRecorder>();
    }
    public void Battle(SpaceShip spaceship, Enemy enemy)
    {
        do
        {
            
        } while (spaceship.GetHealth() > 0 || enemy.GetHealth() > 0);
    }
    public void CheckBattle(SpaceShip spaceship)
    {
        foreach (Enemy enemy in ir.GetEnemies())
        {
            Debug.Log(enemy.GetCell() + " + " + spaceship.GetCell());
            if (enemy.GetCell() == spaceship.GetCell())
            {
                enemy.DestroyObject();
            }
        }
    }
}
