using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100f;
    public float despawnTime = 5f;
    private Animator animator;
    private EnemyAi enemyAi;
    private BotIK botIK;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyAi = GetComponent<EnemyAi>();
        botIK = GetComponent<BotIK>();
    }
    public void TakeDamage(float damage, bool headShoot)
    {
        health -= damage;
        if (health <= 0 && headShoot)
        {
            animator.SetBool("HeadShoot", true);
            animator.SetTrigger("Death");
            animator.SetBool("isDead", true);
            enemyAi.activeGun.gameObject.SetActive(false);
            enemyAi.enabled = false;
            botIK.enabled = false;
        }
        else if (health <= 0)
        {
            enemyAi.enabled = false;
            botIK.enabled = false;
            enemyAi.activeGun.gameObject.SetActive(false);
            animator.SetBool("isDead", true);
            animator.SetTrigger("Death");
        }
    }
    public void DeSpawn()
    {
        Destroy(gameObject);
    }
}
