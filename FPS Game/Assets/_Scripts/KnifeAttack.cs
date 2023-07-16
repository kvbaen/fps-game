using FpsGame.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KnifeAttack:MonoBehaviour
{
    public float attackRange = 2f;
    public int attackDamage = 40;
    public float timeBetweenAttack;
    public LayerMask attackLayer;
    private float time = 0;

    private bool isAttacking = false;
    private InputManager _inputManager;
    public TextMeshProUGUI AmmunitionDisplay;
    public Transform AttackPoint;
    [SerializeField] private Camera _fpsCam;
    [SerializeField] private string EnemyTag;
    [SerializeField] private GameObject _bulletHolePrefab;
    [SerializeField] private float knifeHoleLifeSpan = 5;
    private void Start()
    {
        _inputManager = GetComponentInParent<InputManager>();
    }

    private void Update()
    {
        if (_inputManager.Shoot && !isAttacking && this.gameObject.activeInHierarchy && time>=timeBetweenAttack)
        {
            isAttacking = true;
            PerformAttack();
            time = 0;
        }
        if (this.gameObject.activeInHierarchy)
        {
            time += Time.smoothDeltaTime;
        }
        if (AmmunitionDisplay != null && this.gameObject.activeInHierarchy)
        {
            AmmunitionDisplay.enabled = false;
        }
    }

    private void PerformAttack()
    {
        Ray ray = _fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange))
        {
            if (hit.collider.gameObject.tag == EnemyTag)
            {
                IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable?.TakeDamage(attackDamage);
                }
            }
            else if(hit.collider.gameObject.tag != "Weapon") 
            {
                GameObject knifeHole = Instantiate(_bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal)) as GameObject;
                Destroy(knifeHole, knifeHoleLifeSpan);
            }
            if(hit.collider.gameObject.tag == "Weapon")
            {
                Rigidbody gunRB = hit.collider.gameObject.GetComponent<Rigidbody>();
                if (gunRB != null)
                {
                    gunRB.AddForce(_fpsCam.transform.forward * 5, ForceMode.Impulse);
                }
                
            }
        }
        FinishAttack();
    }

    public void FinishAttack()
    {
        isAttacking = false;
    }
}
