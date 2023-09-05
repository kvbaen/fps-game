using System.Collections;
using System.Collections.Generic;
using FpsGame.ProjectileGun;
using UnityEngine;

public class PickUpController : MonoBehaviour
{

    private ProjectileGun gunScript;
    private Rigidbody rb;
    public WeaponSlot gunContainer;
    public Camera fpsCam;
    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;
    public bool equipped;
    public PlayerController playerController;
    private MenuController menuController;
    private Animator animator;
    private bool ShouldPickUp => Input.GetKey(playerController.actionKey) && !equipped;
    private bool ShouldDrop => Input.GetKey(playerController.dropKey) && equipped;
    private void Awake()
    {
        gunScript = GetComponent<ProjectileGun>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        menuController = FindObjectOfType<MenuController>();
    }
    void Start()
    {
        if (!equipped)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            gunScript.enabled = false;
            rb.isKinematic = false;
            if (animator != null)
                animator.enabled = false;
        }
        else
        {
            rb.interpolation = RigidbodyInterpolation.None;
            gunScript.enabled = true;
            rb.isKinematic = true;
            if (animator != null)
                animator.enabled = true;
        }
    }


    void Update()
    {
        Vector3 distanceToPlayer = playerController.transform.position - transform.position;
        if (ShouldPickUp && distanceToPlayer.magnitude <= pickUpRange && !menuController._isGamePaused)
        {
            Ray ray = fpsCam.ViewportPointToRay(new Vector2(0.5f, 0.5f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickUpRange))
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

        if (ShouldDrop && !menuController._isGamePaused) Drop();
    }

    private void PickUp()
    {
        equipped = true;
        transform.SetParent(gunContainer.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        gunContainer.SlotFull = gunContainer.transform.childCount >= gunContainer.maxCountItems;
        /*transform.localScale = Vector3.one;*/
        rb.interpolation = RigidbodyInterpolation.None;
        rb.isKinematic = true;
        gunScript.enabled = true;
        if (animator != null)
            animator.enabled = true;
    }

    private void Drop()
    {
        equipped = false;

        transform.SetParent(null);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.isKinematic = false;
        gunContainer.SlotFull = gunContainer.transform.childCount >= gunContainer.maxCountItems;
        rb.AddForce(fpsCam.transform.forward * (dropForwardForce + playerController.characterVelocity), ForceMode.Impulse);
        rb.AddForce(fpsCam.transform.up * (dropUpwardForce + playerController.characterVelocity), ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);
        if (animator != null)
            animator.enabled = false;
        gunScript.enabled = false;
    }
}
