using UnityEngine;

[CreateAssetMenu(fileName = "BotData", menuName = "Bot")]
public class BotData : ScriptableObject
{
    [Header("Info")]
    public new string name;

    [Header("Attack")]
    public float shootForce;
    public float attackRange;
    public float sightRange;
    public float gunSpread;
    public GunData gunData;
    public float timeBetweenAttacks;

    [Header("Movement")]
    public float patrollingSpeed = 1.5f;
    public float chasingSpeed = 4f;
}
