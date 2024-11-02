using UnityEngine;
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "EnemyData", order = 52)]
public class EnemyScriptableObject : ScriptableObject
{
    public string enemyName;
    public float speed;
    public float health;
    public float damage;
    public Sprite sprite;
}
