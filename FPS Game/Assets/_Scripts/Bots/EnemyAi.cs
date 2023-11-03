using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    Animator animator;
    AudioManager audioManager;
    [Header("Enemy Parameters")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsPlayer;
    public BotData botData;

    [Header("Patroling Parameters")]
    public Transform[] waypoints;
    private Vector3 _walkPoint;
    private int _waypointIndex;
    private bool _walkPointSet;

    [Header("Attacking Parameters")]
    private bool _alreadyAttacked;
    private PlayerController _playerController;
    private readonly string isAimingHash = "isAiming";
    private readonly string speedHash = "Speed";
    public GameObject gunHolder;
    public Transform[] guns;
    public Transform activeGun;
    private BotIK botIK;
    private Vector3 hitTarget;
    private Transform attackPoint;
    private int currentAmmoInMagazine;
    private int totalAmmo;
    private bool ShouldReload => currentAmmoInMagazine == 0 && totalAmmo >= botData.gunData.magSize && !IsReloading;
    private bool CanShoot => currentAmmoInMagazine > 0 && !_alreadyAttacked && !IsReloading;
    private bool IsReloading = false;

    [Header("States Parameters")]
    public bool playerInAttackRange;
    public bool playerInSightRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        _playerController = FindObjectOfType<PlayerController>();
        audioManager = FindObjectOfType<AudioManager>();
        botIK = GetComponent<BotIK>();
        currentAmmoInMagazine = botData.gunData.magSize;
        totalAmmo = botData.gunData.magSize * botData.gunData.magNumber;
        if (gunHolder != null)
        {
            guns = new Transform[gunHolder.transform.childCount];
            for (int i = 0; i < gunHolder.transform.childCount; i++)
            {
                guns[i] = gunHolder.transform.GetChild(i);
                guns[i].gameObject.SetActive(botData.gunData.name == guns[i].name);
                if (botData.gunData.name == guns[i].name)
                {
                    activeGun = guns[i];
                    attackPoint = activeGun.Find("AttackPoint");
                }
            }
            if (botData.gunData.isTwoHanded)
            {
                animator.SetBool("isTwoHanded", true);
            }
            else
            {
                animator.SetBool("isTwoHanded", false);
            }
        }
        if (botIK != null)
        {
            botIK.AttachArms(activeGun, botData.gunData.isTwoHanded);
        }
    }

    private void Update()
    {
        animator.SetFloat(speedHash, agent.velocity.magnitude);
        //Check for sight and attack range
        if (botData != null)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, botData.sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, botData.attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
    }

    private void Patroling()
    {
        agent.speed = botData.patrollingSpeed;
        botIK.ActivateAim(false);
        if (animator.GetBool(isAimingHash)) animator.SetBool(isAimingHash, false);
        if (waypoints.Length > 0)
        {
            if (!_walkPointSet) SearchWalkPoint();

            if (_walkPointSet)
                agent.SetDestination(_walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - _walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            _walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        _walkPoint = _walkPoint = waypoints[_waypointIndex].position;
        _waypointIndex++;
        if (_waypointIndex == waypoints.Length) _waypointIndex = 0;
        _walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (animator.GetBool(isAimingHash)) animator.SetBool(isAimingHash, false);
        agent.SetDestination(player.position);
        agent.speed = botData.chasingSpeed;
        botIK.ActivateAim(true);
        //RotateTowardPlayer();
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);
        botIK.ActivateAim(true);
        RotateTowardPlayer();
        if (CanShoot)
        {
            ///Attack code here
            if (!animator.GetBool(isAimingHash)) animator.SetBool(isAimingHash, true);
            //animator.SetTrigger("Shoot");
            Vector3 randomDirection = Random.insideUnitCircle * botData.gunSpread;
            ///Zawsze celuje w stronę bohatera
            Vector3 aimDirection = (player.position - attackPoint.position) + randomDirection;
            ///Celuje w stronę którą trzyma broń
            //Vector3 aimDirection = attackPoint.transform.TransformDirection(-Vector3.right) + randomDirection;
            Physics.Raycast(attackPoint.position, aimDirection, out RaycastHit hit, botData.attackRange);
            Debug.DrawRay(attackPoint.position, aimDirection);
            hitTarget = hit.point;
            if (hit.collider != null && hit.collider.name == _playerController.name)
            {
                _playerController.TakeDamage(botData.gunData.damage);
            }
            _alreadyAttacked = true;
            currentAmmoInMagazine--;
            Invoke(nameof(ResetAttack), botData.timeBetweenAttacks);
        }
        else if (ShouldReload)
        {
            ReloadGun();
        }
    }
    private void ResetAttack()
    {
        _alreadyAttacked = false;
        animator.SetBool(isAimingHash, false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, botData.attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, botData.sightRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(attackPoint.position, new Vector3(botData.gunSpread, botData.gunSpread, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(attackPoint.position, attackPoint.transform.TransformDirection(-Vector3.right) * 10);
    }

    private void SpawnBullet()
    {
        GameObject currentBullet = Instantiate(botData.gunData.bulletPrefab, attackPoint.position, Quaternion.identity);
        audioManager.Play(botData.gunData.bulletPrefab.name + " Shoot", activeGun.gameObject);
        if (currentBullet != null)
        {
            currentBullet.transform.LookAt(hitTarget);
            currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * botData.gunData.shootForce, ForceMode.Impulse);
        }
    }

    private void RotateTowardPlayer()
    {
        Vector3 directionToTarget = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.velocity.magnitude > 0 ? agent.velocity.magnitude : 5) /*Quaternion.RotateTowards(transform.rotation, lookRotation, 10)*/;
    }

    private void ReloadGun()
    {
        IsReloading = true;
        animator.SetTrigger("Reload");
        //botIK.ChangeIKAmountElbow(0.25f);
        //botIK.ChangeIKAmountHand(0.25f);
        currentAmmoInMagazine = botData.gunData.magSize;
        totalAmmo -= botData.gunData.magSize;
    }

    private void FinishRealod()
    {
        //botIK.ChangeIKAmountElbow(1f);
        //botIK.ChangeIKAmountHand(1f);
        IsReloading = false;
        _alreadyAttacked = false;
        animator.SetBool(isAimingHash, false);
    }
}
