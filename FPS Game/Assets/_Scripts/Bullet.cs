using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _bulletHolePrefab;
    [SerializeField] private float bulletHoleLifeSpan;
    void Start()
    {
        Destroy(gameObject, 20f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "Target" && other.gameObject.tag != "Weapon")
        {
            ContactPoint contact = other.GetContact(0);
            GameObject bulletHole = Instantiate(_bulletHolePrefab, contact.point + contact.normal * 0.001f, Quaternion.LookRotation(contact.normal)) as GameObject;
            Destroy(bulletHole, bulletHoleLifeSpan);
        }
        if (other.gameObject.tag != "Weapon")
            Destroy(gameObject);
    }
}
