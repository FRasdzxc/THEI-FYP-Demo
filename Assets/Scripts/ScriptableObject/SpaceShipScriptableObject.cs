using UnityEngine;

[CreateAssetMenu(fileName ="NewSpaceShipData", menuName ="SpaceshipData", order = 51)]
public class SpaceShipScriptableObject : ScriptableObject
{
    public string spaceshipName;
    public float speed;
    public float health;
    public float damage;
    public Sprite sprite;
}
