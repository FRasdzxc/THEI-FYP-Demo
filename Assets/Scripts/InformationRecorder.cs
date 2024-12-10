using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InformationRecorder : MonoBehaviour
{
    public Tilemap tilemap;
    private GameObject[] enemies;
    private List<Enemy> enemiesData = new List<Enemy>();
    private GameObject[] spaceships;
    private List<SpaceShip> spaceshipsData = new List<SpaceShip>();
    private List<Vector3Int> objectList = new List<Vector3Int>();
    public void UpdateEnemies()
    {
        if(enemiesData.Count > 0)
        {
            enemiesData.Clear();
        }
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(enemies.Length > 0)
        {
            foreach (GameObject enemy in enemies)
            {
                Enemy data = (Enemy) enemy.GetComponent("Enemy");
                Vector3 worldPosition = enemy.transform.position;
                Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
                cellPosition.z = 0;
                data.SetCell(cellPosition);
                SetTileObjectList(cellPosition);
                enemiesData.Add(data);
                //Debug.Log(enemy.name + ": (" + cellPosition.x + ", " + cellPosition.y + ")");
                //Debug.Log("Health: " + data.GetHealth() + "\nDamage: " + data.GetDamage());
            }
        }
    }
    public void UpdateSpaceShips()
    {
        if(spaceshipsData.Count > 0)
        {
            spaceshipsData.Clear();
        }
        spaceships = GameObject.FindGameObjectsWithTag("Player");
        if(spaceships.Length > 0)
        {
            foreach(GameObject spaceship in spaceships)
            {
                SpaceShip data = (SpaceShip)spaceship.GetComponent("SpaceShip");
                Vector3 worldPosition = spaceship.transform.position;
                Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
                cellPosition.z = 0;
                data.SetCell(cellPosition);
                SetTileObjectList(cellPosition);
                spaceshipsData.Add(data);
                //Debug.Log(spaceship.name + ": (" + cellPosition.x + ", " + cellPosition.y + ")");
            }
        }
    }
    public void SetTileObjectList(Vector3Int cell)
    {
        objectList.Add(cell);
    }
    public void UpdateTileObjectList()
    {
        if (objectList.Count > 0)
        {
            objectList.Clear();
        }
    }
    public void DeleteTileObjectList(Vector3Int cell)
    {
        objectList.Remove(cell);
    }
    public List<Vector3Int> GetTileObjectList()
    {
        return objectList;
    }
    public List<Enemy> GetEnemies()
    {
        return enemiesData;
    }
    public List<SpaceShip> GetSpaceShips()
    {
        return spaceshipsData;
    }
}
