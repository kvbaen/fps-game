using UnityEngine;
using TMPro;

namespace FpsGame.ProjectileGun
{
    public class ProjectileGun : MonoBehaviour
    {
        // Gun stats
        private int bulletsLeftInMagazine, bulletsShot;

        // Flags
        private bool isShooting = false, isReadyToShoot = false, isReloading;
        private bool ShouldShoot => Input.GetKey(playerController.shootKey);
        private bool ShouldShootOnClick => Input.GetKeyDown(playerController.shootKey);
        private bool ShouldReload => Input.GetKey(playerController.reloadKey) && bulletsLeftInMagazine < gunData.magSize && !isReloading && gameObject.activeSelf;
        private bool CanReload => ammoCount > 0 && !isReloading;

        // References
        public Camera fpsCam;
        public Transform attackPoint;
        public PlayerController playerController;
        public GameObject hitPrefab;
        private Animator animator;
        private AudioSource audioSource;
        private AudioManager audioManager;
        [SerializeField] public GunData gunData;

        // Graphics
        public ParticleSystem muzzleFlash;
        public TextMeshProUGUI ammunitionDisplay;

        private bool allowInvoke = true;
        private float time = 0;
        private bool spawnBullet = false;
        private Vector3 walkSpread = Vector3.zero;
        private Vector3 targetPoint;
        private float waitTime;
        private int ammoCount;
        private void Awake()
        {
            bulletsLeftInMagazine = gunData.magSize;
            isReadyToShoot = true;
            waitTime = gunData.takeTime;
            animator = GetComponent<Animator>();
            ammoCount = gunData.magNumber * gunData.magSize;
            audioSource = GetComponent<AudioSource>();
            audioManager = FindObjectOfType<AudioManager>();

        }
        private void FixedUpdate()
        {
            if (spawnBullet)
            {
                SpawnBullet();
            }
        }
        private void Update()
        {
            if (waitTime > 0) waitTime -= Time.deltaTime;
            HandleInput();

            if (ammunitionDisplay != null)
            {
                ammunitionDisplay.enabled = true;
                ammunitionDisplay.SetText(bulletsLeftInMagazine + " / " + ammoCount);
            }
            if (playerController.IsMovingOrJumping)
            {
                walkSpread = new Vector3(Random.Range(-gunData.spread * 10f, gunData.spread * 10f), Random.Range(-gunData.spread * 10f, gunData.spread * 10f), 0);
            }
            else
            {
                walkSpread = Vector3.zero;
            }
            time += Time.smoothDeltaTime;
        }

        private void HandleInput()
        {
            if (gunData.isAutomatic)
                isShooting = ShouldShoot;
            else
            {
                isShooting = ShouldShootOnClick;
            }

            if (waitTime > 0) return;

            if ((ShouldReload || bulletsLeftInMagazine == 0) && CanReload) Reload();

            if (isReadyToShoot && isShooting && !isReloading && bulletsLeftInMagazine > 0 && gameObject.activeSelf)
            {
                Shoot();
                if (!animator.GetBool("isShooting"))
                {
                    animator.SetBool("isShooting", true);
                }
            }
            if (!isShooting && time >= gunData.recoilResetTime)
            {
                bulletsShot = 0;
            }
            if ((!isShooting || bulletsLeftInMagazine == 0) && playerController.gunRotation != Vector3.zero)
            {
                playerController.SetGunRotation(
                    Vector3.Lerp(
                        playerController.gunRotation,
                        Vector3.zero,
                        (gunData.timeBetweenShots * 60) * Time.deltaTime
                    )
                );

                if (Vector3.Distance(playerController.gunRotation, Vector3.zero) < 0.1)
                {
                    playerController.SetGunRotation(Vector3.zero);
                }
            }
        }

        private void Shoot()
        {
            isReadyToShoot = false;
            Ray ray;
            if (playerController.IsMovingOrJumping)
            {
                ray = fpsCam.ViewportPointToRay(
                    new Vector3(
                    Random.Range(0.5f - gunData.spread, 0.5f + gunData.spread),
                    Random.Range(0.5f - gunData.spread, 0.5f + gunData.spread),
                    0
                    ));
            }
            else
            {
                HandleGunRecoil();
                ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            }

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPoint = hit.point;
                Target damageable = hit.transform.GetComponentInParent<Target>();
                if (hit.collider.gameObject.CompareTag("Head") && damageable != null)
                {
                    damageable.TakeDamage(gunData.damage * gunData.headDamageMultiplier, true);
                }
                else if (damageable != null)
                {
                    damageable.TakeDamage(gunData.damage, false);
                }
                /*GameObject hitPoint = Instantiate(hitPrefab, targetPoint, Quaternion.identity);
                Destroy(hitPoint, 5);*/
            }
            else
            {
                targetPoint = ray.GetPoint(gunData.maxDistance);
            }

            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

            spawnBullet = true;

            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            bulletsLeftInMagazine--;

            if (allowInvoke && gameObject.activeInHierarchy)
            {
                Invoke("ResetShot", gunData.timeBetweenShots);
                allowInvoke = false;
            }
        }

        private void ResetShot()
        {
            isReadyToShoot = true;
            if (animator.GetBool("isShooting"))
            {
                animator.SetBool("isShooting", false);
            }
            allowInvoke = true;
            time = 0;
        }

        private void HandleGunRecoil()
        {
            if (time >= gunData.recoilResetTime)
            {
                playerController.SetGunRotation(playerController.gunRotation + gunData.recoilPattern[0]);
                bulletsShot = 1;
            }
            else
            {
                Vector3 newRotation = playerController.gunRotation + new Vector3(
                        gunData.recoilPattern[bulletsShot].x != 0 ? gunData.recoilPattern[bulletsShot].x : Random.Range(-gunData.spread * 3, gunData.spread * 3),
                        gunData.recoilPattern[bulletsShot].y != 0 ? gunData.recoilPattern[bulletsShot].y : Random.Range(-gunData.spread * 3, gunData.spread * 3),
                        gunData.recoilPattern[bulletsShot].z)
                    + walkSpread;
                playerController.SetGunRotation(newRotation);


                if (bulletsShot + 1 <= gunData.recoilPattern.Length - 1)
                {
                    bulletsShot++;
                }
                else
                {
                    bulletsShot = 0;
                }
            }
        }

        private void OnDisable()
        {
            waitTime = gunData.takeTime;
            isReloading = false;
        }

        private void Reload()
        {
            isReloading = true;
            animator.SetBool("isReloading", true);
            Invoke("ReloadFinished", gunData.reloadTime);
        }

        private void ReloadFinished()
        {
            if (gameObject.activeInHierarchy && isReloading)
            {
                int bulletsToRealod = gunData.magSize - bulletsLeftInMagazine;
                if (ammoCount >= bulletsToRealod)
                {
                    bulletsLeftInMagazine = gunData.magSize;
                    ammoCount -= bulletsToRealod;
                }
                else
                {
                    bulletsLeftInMagazine += ammoCount;
                    ammoCount = 0;
                }
                bulletsShot = 0;
                isReloading = false;
                animator.SetBool("isReloading", false);
            }
            else
            {
                isReloading = false;
                animator.SetBool("isReloading", false);
            }
            waitTime = 0.2f;
        }
        private void SpawnBullet()
        {
            GameObject currentBullet = Instantiate(gunData.bulletPrefab, attackPoint.position, Quaternion.identity);
            /*audioSource.Play();*/
            audioManager.Play(gunData.bulletPrefab.name + " Shoot", gameObject);
            if (currentBullet != null)
            {
                currentBullet.transform.LookAt(targetPoint);
                currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * gunData.shootForce, ForceMode.Impulse);
            }
            spawnBullet = false;
        }
    }
}