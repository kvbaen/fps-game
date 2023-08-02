using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Gun", menuName="Weapon/Gun")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    public new string name;

    [Header("Shooting")]
    public float damage;
    public float maxDistance;
    public bool isAutomatic;
    public float timeBetweenShots, timeBetweenMultipleShots;
    public Vector3[] recoilPattern;
    public float recoilResetTime;
    public float spread;
    [Header("Bullet")]
    public float shootForce;

    [Header("Reloading")]
    public int currentAmmo;
    public int magSize;
    public float reloadTime;
}
