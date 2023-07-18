using System.Collections;
using System.Collections.Generic;
using FpsGame.ProjectileGun;
using UnityEngine;

public class PickUpController : MonoBehaviour
{

    public ProjectileGun gunScript;
    public Rigidbody rb;
    public Collider coll;
    public Transform player;
    public WeaponSlot gunContainer;
    public Camera fpsCam;
    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;
    public bool equipped;
    private PlayerController playerController;
    private bool ShouldPickUp => Input.GetKeyDown(playerController.actionKey) && !equipped;
    private bool ShouldDrop => Input.GetKeyDown(playerController.dropKey) && equipped;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    void Start()
    { 
        if (!equipped)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            gunScript.enabled = false;
            rb.isKinematic = false;
            rb.detectCollisions = true;
            coll.isTrigger = false;
        }
        else
        {
            rb.interpolation = RigidbodyInterpolation.None;
            gunScript.enabled = true;
            rb.detectCollisions = false;
            rb.isKinematic = true;
            coll.isTrigger = true;
        }
    }

    
    void FixedUpdate()
    {
        /*if (equipped)
        {
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }*/
        Vector3 distanceToPlayer = player.position - transform.position;
        if (ShouldPickUp && distanceToPlayer.magnitude <= pickUpRange)
        {
            Ray ray = fpsCam.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickUpRange))
            {
                Debug.Log(hit.collider.gameObject.name + " " + hit.collider.gameObject.name);
                if (hit.collider.gameObject.tag == "Weapon" && gameObject.name == hit.collider.gameObject.name)
                {
                    
                    if (!gunContainer.SlotFull)
                    {
                        Debug.Log("Picking up");
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

        if(ShouldDrop) Drop();
    }

    private void PickUp()
    {
        equipped = true;
        transform.SetParent(gunContainer.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        /*transform.localScale = Vector3.one;*/
        rb.interpolation = RigidbodyInterpolation.None;
        rb.isKinematic = true;
        coll.isTrigger = true;
        gunScript.enabled = true;
    }

    private void Drop()
    {
        equipped = false;

        transform.SetParent(null);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.isKinematic = false;
        coll.isTrigger = false;
        gunContainer.SlotFull = gunContainer.transform.childCount >= gunContainer.maxCountItems;
        rb.AddForce(fpsCam.transform.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.transform.up * dropUpwardForce, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);
         
        gunScript.enabled = false;
    }
}
