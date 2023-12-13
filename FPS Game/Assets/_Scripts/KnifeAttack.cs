using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class KnifeAttack : MonoBehaviour
{
    public float attackRange = 2f;
    public int attackDamage = 40;
    public float timeBetweenAttack;
    private float time = 0;

    private bool isAttacking = false;
    public TextMeshProUGUI AmmunitionDisplay;
    public Transform AttackPoint;
    [SerializeField] private Camera _fpsCam;
    [SerializeField] private string EnemyTag;
    [SerializeField] private GameObject _bulletHolePrefab;
    [SerializeField] private float knifeHoleLifeSpan = 5;
    private Animator animator;
    private PlayerController playerController;
    private MenuController menuController;
    private bool isPlayerAlive => playerController.health > 0;
    private bool ShouldAttack => Input.GetKey(playerController.shootKey) && !isAttacking && this.gameObject.activeInHierarchy && time >= timeBetweenAttack;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        animator = GetComponent<Animator>();
        menuController = FindObjectOfType<MenuController>();
    }

    private void Update()
    {
        if (ShouldAttack && !menuController._isGamePaused && isPlayerAlive)
        {
            isAttacking = true;
            animator.SetBool("isAttacking", true);
            PerformAttack();
            time = 0;
        }

        if (AmmunitionDisplay != null && this.gameObject.activeInHierarchy)
        {
            AmmunitionDisplay.enabled = false;
        }
        time += Time.smoothDeltaTime;
    }

    private void PerformAttack()
    {
        animator.SetTrigger("Attack");
        Ray ray = _fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange))
        {
            Target damageable = hit.transform.GetComponentInParent<Target>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage, false);
            }
            if (!hit.collider.gameObject.CompareTag("Weapon") && !hit.collider.gameObject.CompareTag("Target"))
            {
                GameObject knifeHole = Instantiate(_bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal)) as GameObject;
                knifeHole.transform.parent = hit.transform;
                Destroy(knifeHole, knifeHoleLifeSpan);
            }
            if (hit.collider.gameObject.CompareTag("Weapon"))
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
        animator.SetBool("isAttacking", false);
    }
}
