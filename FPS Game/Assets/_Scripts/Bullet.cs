using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _bulletHolePrefab;
    [SerializeField] private float bulletHoleLifeSpan;
    private Rigidbody rb;
    void Start()
    {
        Destroy(gameObject, 5f);
        rb = GetComponent<Rigidbody>();

    }
    private void Update()
    {
        if (rb.velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);
            transform.rotation = targetRotation;
        }
    }
    private void OnCollisionEnter(Collision collElement)
    {
        if (!collElement.gameObject.CompareTag("Target") && !collElement.gameObject.CompareTag("Weapon"))
        {
            ContactPoint contact = collElement.GetContact(0);
            GameObject bulletHole = Instantiate(_bulletHolePrefab, contact.point + contact.normal * 0.001f, Quaternion.LookRotation(contact.normal)) as GameObject;
            bulletHole.transform.parent = collElement.transform;
            Destroy(bulletHole, bulletHoleLifeSpan);
        }
        if (collElement.gameObject.CompareTag("Weapon") && collElement.gameObject.GetComponent<PickUpController>().equipped)
        {
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
