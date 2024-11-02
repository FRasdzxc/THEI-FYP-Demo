using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyScriptableObject enemySO;
    private float health;
    private float damage;
    private Vector3Int cell;
    void Start()
    {
        health = enemySO.health;
        damage = enemySO.damage;
    }
    public void SetHealth(float health)
    {
        this.health = health;
    }
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    public void SetCell(Vector3Int cell)
    {
        this.cell = cell;
    }
    public float GetHealth()
    {
        return this.health;
    }
    public float GetDamage()
    {
        return this.damage;
    }
    public Vector3Int GetCell()
    {
        return this.cell;
    }
    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
