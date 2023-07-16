using System;
using System.Collections;
using System.Collections.Generic;
using FpsGame.Manager;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    public float radius = 5f;
    public float explosionForce = 700f;
    public float damage;

    public GameObject explosionEffect;

    private float countDown;
    private bool hasExploded = false;
    [HideInInspector]
    public bool beenThrown = false;

    // Start is called before the first frame update
    void Start()
    {
        countDown = delay;
    }

    // Update is called once per frame
    void Update()
    {

        if (beenThrown)
        {
            countDown -= Time.deltaTime;
        }

        if (countDown <= 0f && !hasExploded && beenThrown)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, radius);
            }
            Target target = nearbyObject.GetComponent<Target>();
            if (target != null)
            {
                float distance = Vector3.Distance(nearbyObject.transform.position, transform.position);
                target.TakeDamage(damage - (float)Math.Floor(distance * 5f));
            }
        }

        Destroy(gameObject);
    }

}
