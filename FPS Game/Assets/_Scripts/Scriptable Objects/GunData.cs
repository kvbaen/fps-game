using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    public new string name;
    public bool isTwoHanded;

    [Header("Shooting")]
    public float takeTime;
    public float damage;
    public float maxDistance;
    public bool isAutomatic;
    public float timeBetweenShots, timeBetweenMultipleShots;
    public Vector3[] recoilPattern;
    public float recoilResetTime;
    public float spread, headDamageMultiplier;

    [Header("Bullet")]
    public float shootForce;
    public GameObject bulletPrefab;

    [Header("Reloading")]
    public int currentAmmo;
    public int magSize;
    public float reloadTime;
    public int magNumber;
}
