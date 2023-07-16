using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMuzzleFlash : MonoBehaviour
{
    [SerializeField] private float _DestroyTime;
    void Start()
    {
        Destroy(gameObject, _DestroyTime);
    }

}
