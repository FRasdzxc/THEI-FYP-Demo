using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InformationRecorder : MonoBehaviour
{
    public Tilemap tilemap;
    private GameObject[] enemies;
    private List<Enemy> enemiesData = new List<Enemy>();
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
                Enemy data = (Enemy)enemy.GetComponent("Enemy");
                Vector3 worldPosition = enemy.transform.position;
                Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
                data.SetCell(cellPosition);
                enemiesData.Add(data);
                Debug.Log(enemy.name + ": (" + cellPosition.x + ", " + cellPosition.y + ")");
                Debug.Log("Health: " + data.GetHealth() + "\nDamage: " + data.GetDamage());
            }
        }
    }
    public List<Enemy> GetEnemies()
    {
        return enemiesData;
    }
}
