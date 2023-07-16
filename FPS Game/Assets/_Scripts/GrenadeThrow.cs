using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrow : MonoBehaviour
{
    [SerializeField]
    public bool equipped;
    [SerializeField]
    public Rigidbody rb;
    [SerializeField]
    public Collider coll;
    [SerializeField]
    public float dropForwardForce, dropUpwardForce, throwForceForward = 10f, throwForceUp = 5f, pickUpRange, timeBetweenSwitching = 0.5f;
    private bool ShouldThrow => Input.GetKeyDown(playerController.shootKey) && equipped;
    private bool ShouldPickUp => Input.GetKeyDown(playerController.actionKey) && !gunContainer.SlotFull && !equipped;
    public Transform player;
    public Camera fpsCam;
    public WeaponSlot gunContainer;
    public Grenade grenadeScript;
    private PlayerController playerController;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    void Start()
    {
        grenadeScript.enabled = false;
        if (!equipped)
        {
            grenadeScript.enabled = true;
            rb.isKinematic = false;
            coll.isTrigger = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        else
        {
            grenadeScript.enabled = false;
            rb.isKinematic = true;
            coll.isTrigger = true;
            rb.interpolation = RigidbodyInterpolation.Extrapolate;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (ShouldThrow && this.gameObject.activeInHierarchy)
        {
            ThrowGrenade();
            Invoke(nameof(ChangeItem), timeBetweenSwitching);
        }

        if (ShouldThrow && this.gameObject.activeInHierarchy)
        {
            Drop();
            Invoke(nameof(ChangeItem), timeBetweenSwitching);
        }

        if (equipped)
        {
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        Vector3 distanceToPlayer = player.position - transform.position;

        if (ShouldPickUp && distanceToPlayer.magnitude <= pickUpRange)
        {
            Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Weapon" && gameObject.name == hit.collider.gameObject.name)
                {
                    PickUp();
                    ChangeItem();
                }
            }
        }
    }

    private void ThrowGrenade()
    {
        equipped = false;
        transform.SetParent(null);
        
        rb.isKinematic = false;
        coll.isTrigger = false;
        grenadeScript.enabled = true;
        grenadeScript.beenThrown = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.velocity = player.GetComponent<Rigidbody>().velocity;
        rb.AddForce(fpsCam.transform.forward * throwForceForward, ForceMode.VelocityChange);
        rb.AddForce(fpsCam.transform.up * throwForceUp, ForceMode.VelocityChange);
    }

    public void Drop()
    {
        equipped = false;

        transform.SetParent(null);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.isKinematic = false;
        coll.isTrigger = false;
        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        rb.AddForce(fpsCam.transform.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.transform.up * dropUpwardForce, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);


        grenadeScript.enabled = false;
    }

    private void PickUp()
    {
        equipped = true;
        transform.SetParent(gunContainer.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        rb.isKinematic = true;
        coll.isTrigger = true;
        grenadeScript.enabled = false;
    }

    private void ChangeItem()
    {
        for (int i = 0; i < gunContainer.transform.childCount; i++)
        {
            gunContainer.transform.GetChild(i).gameObject.SetActive(i == 0);
        }
    }
}
