using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject _bulletHolePrefab;
    [SerializeField] private float bulletHoleLifeSpan;
    [SerializeField] private GameObject bloodEffect;
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
        GameObject collGameObject = collElement.gameObject;
        if (!collGameObject.CompareTag("Target") && !collGameObject.CompareTag("Weapon"))
        {
            ContactPoint contact = collElement.GetContact(0);
            GameObject bulletHole = Instantiate(_bulletHolePrefab, contact.point + contact.normal * 0.001f, Quaternion.LookRotation(contact.normal));
            bulletHole.transform.parent = collElement.transform;
            Destroy(bulletHole, bulletHoleLifeSpan);
        }
        if (collGameObject.CompareTag("Player") || collGameObject.CompareTag("Target") || collGameObject.CompareTag("Head"))
        {
            ContactPoint contact = collElement.GetContact(0);
            GameObject bloodEffectTMP = Instantiate(bloodEffect, contact.point + contact.normal * 0.001f, Quaternion.LookRotation(contact.normal));
            bloodEffectTMP.transform.parent = collElement.transform;
            Destroy(gameObject);
        }

        if (collGameObject.CompareTag("Weapon") || collGameObject.CompareTag("Player"))
        {
            if (collGameObject.GetComponentInParent<EnemyAi>() != null || collGameObject.GetComponentInParent<PlayerController>() != null)
            {
                return;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
