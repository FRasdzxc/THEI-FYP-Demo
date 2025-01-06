using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private Queue<GameEvent> eventQueue = new Queue<GameEvent>();

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
    }

    public void QueueBattleEvent(Planet planet, Spaceship ship)
    {
        BattleEvent battleEvent = new BattleEvent(planet, ship);
        eventQueue.Enqueue(battleEvent);
    }

    public bool HasEvents()
    {
        return eventQueue.Count > 0;
    }

    public void ProcessAllEvents()
    {
        while (eventQueue.Count > 0)
        {
            GameEvent gameEvent = eventQueue.Dequeue();
            gameEvent.Execute();
        }
    }
}

// Base class for all game events
public abstract class GameEvent
{
    public abstract void Execute();
}

public class BattleEvent : GameEvent
{
    private Planet planet;
    private Spaceship ship;

    public BattleEvent(Planet planet, Spaceship ship)
    {
        this.planet = planet;
        this.ship = ship;
    }

    public override void Execute()
    {
        if (!planet.hasAlien)
        {
            // If aliens somehow disappeared before battle execution
            ship.OnBattleVictory(planet);
            return;
        }

        double planetPower = planet.soldiers * planet.weaponTier;
        double shipPower = ship.soldiers * ship.GetWeaponTier();

        if (shipPower < planetPower)
        {
            // Ship loses
            Debug.Log($"Ship {ship.spaceshipName} was destroyed by aliens on {planet.planetName}!");
            ship.DestroyShip();
        }
        else
        {
            // Ship wins
            Debug.Log($"Ship {ship.spaceshipName} defeated aliens on {planet.planetName}!");
            planet.hasAlien = false;
            planet.soldiers = 0;
            planet.weaponTier = 0;

            // After victory, enter the planet
            ship.OnBattleVictory(planet);
        }
    }
}