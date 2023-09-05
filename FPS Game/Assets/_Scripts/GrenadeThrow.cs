using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrow : MonoBehaviour
{
    [SerializeField]
    public bool equipped;
    private Rigidbody _rb;
    private Collider _coll;
    [SerializeField]
    public float dropForwardForce, dropUpwardForce, throwForceForward = 10f, throwForceUp = 5f, pickUpRange, timeBetweenSwitching = 0.5f;
    private bool ShouldThrow => Input.GetKey(playerController.shootKey) && equipped;
    private bool ShouldDrop => Input.GetKey(playerController.dropKey) && equipped;
    private bool ShouldPickUp => Input.GetKey(playerController.actionKey) && !gunContainer.SlotFull && !equipped;
    public Camera fpsCam;
    public WeaponSlot gunContainer;
    private Grenade _grenadeScript;
    private MenuController menuController;
    public PlayerController playerController;
    private void Awake()
    {
        _grenadeScript = GetComponent<Grenade>();
        _rb = GetComponentInParent<Rigidbody>();
        _coll = GetComponentInParent<Collider>();
        menuController = FindObjectOfType<MenuController>();
    }
    void Start()
    {
        _grenadeScript.enabled = false;
        if (!equipped)
        {
            _grenadeScript.enabled = true;
            _rb.isKinematic = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        else
        {
            _grenadeScript.enabled = false;
            _rb.isKinematic = true;
            _rb.interpolation = RigidbodyInterpolation.Extrapolate;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (ShouldThrow && this.gameObject.activeInHierarchy && !menuController._isGamePaused)
        {
            ThrowGrenade();
            Invoke(nameof(ChangeItem), timeBetweenSwitching);
        }

        if (ShouldDrop && this.gameObject.activeInHierarchy && !menuController._isGamePaused)
        {
            Drop();
            Invoke(nameof(ChangeItem), timeBetweenSwitching);
        }

        if (equipped)
        {
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        Vector3 distanceToPlayer = playerController.transform.position - transform.position;

        if (ShouldPickUp && distanceToPlayer.magnitude <= pickUpRange && !menuController._isGamePaused)
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

        _rb.isKinematic = false;
        _coll.isTrigger = false;
        _grenadeScript.enabled = true;
        _grenadeScript.beenThrown = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        /*rb.velocity = 2f;*/
        _rb.AddForce(fpsCam.transform.forward * (throwForceForward + playerController.characterVelocity), ForceMode.VelocityChange);
        _rb.AddForce(fpsCam.transform.up * (throwForceUp + playerController.characterVelocity), ForceMode.VelocityChange);
    }

    public void Drop()
    {
        equipped = false;

        transform.SetParent(null);
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.isKinematic = false;
        /*rb.velocity = 2f;*/

        _rb.AddForce(fpsCam.transform.forward * dropForwardForce, ForceMode.Impulse);
        _rb.AddForce(fpsCam.transform.up * dropUpwardForce, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        _rb.AddTorque(new Vector3(random, random, random) * 10);


        _grenadeScript.enabled = false;
    }

    private void PickUp()
    {
        equipped = true;
        transform.SetParent(gunContainer.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        _rb.isKinematic = true;
        _grenadeScript.enabled = false;
    }

    private void ChangeItem()
    {
        for (int i = 0; i < gunContainer.transform.childCount; i++)
        {
            gunContainer.transform.GetChild(i).gameObject.SetActive(i == 0);
        }
    }
}
