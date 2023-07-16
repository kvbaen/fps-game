using System.Collections;
using System.Collections.Generic;
using FpsGame.Manager;
using FpsGame.ProjectileGun;
using UnityEngine;

public class PickUpController : MonoBehaviour
{

    public ProjectileGun gunScript;
    public Rigidbody rb;
    public Collider coll;
    public Transform player, fpsCam; 
    public WeaponSlot gunContainer;
    public Camera FpsCam;
    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;
    public bool equipped;

    private InputManager _inputManager;

    void Start()
    { 
        _inputManager = GetComponent<InputManager>();
        if (!equipped)
        {
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        else
        {
            gunScript.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
        }
    }

    
    void Update()
    {
        if (equipped)
        {
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && _inputManager.Action)
        {
            Ray ray = FpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Weapon" && gameObject.name == hit.collider.gameObject.name)
                {
                    if (!gunContainer.SlotFull)
                    {
                        PickUp();
                    }
                    else
                    {
                        PickUpController gunController = gunContainer.transform.GetChild(0).transform.GetComponent<PickUpController>();
                        gunController.Drop();
                    }
                }
            }
        }

        if(equipped && _inputManager.Drop) Drop();
    }

    private void PickUp()
    {
        equipped = true;
        transform.SetParent(gunContainer.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        /*transform.localScale = Vector3.one;*/
      
        rb.isKinematic = true;
        coll.isTrigger = true;
        gunScript.enabled = true;
    }

    private void Drop()
    {
        equipped = false;

        transform.SetParent(null);
       
        rb.isKinematic = false;
        coll.isTrigger = false;
        rb.velocity = player.GetComponent<Rigidbody>().velocity;
        gunContainer.SlotFull = gunContainer.transform.childCount >= gunContainer.maxCountItems;
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);
         
        gunScript.enabled = false;
    }
}
