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
    public bool allowButtonHold;
    public float spread;
    public float timeBetweenShooting, timeBetweenShots;
    public int  bulletsPerTap;
    public float recoilForce;
    public float fireRate;
    [Header("Bullet")]
    public float shootForce, upwardForce;

    [Header("Reloading")]
    public int currentAmmo;
    public int magSize;
    public float reloadTime;
    
    [HideInInspector]
    public bool reloading;
}
