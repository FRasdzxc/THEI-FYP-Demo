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
    public void AdjustHealth(float x)
    {
        health += x;
    }
    public void AdjustDamage(float x)
    {
        damage += x;
    }
    public void SetCell(Vector3Int cell)
    {
        this.cell = cell;
    }
    public float GetHealth()
    {
        return health;
    }
    public float GetDamage()
    {
        return damage;
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
