using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;

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

        // References
        public Camera fpsCam;
        public Transform attackPoint;
        public GameObject bulletPrefab;
        public PlayerController playerController;
        public GameObject hitPrefab;
        private Animator animator;
        [SerializeField] private GunData gunData;
        [SerializeField] private string enemyTag;

        // Graphics
        public ParticleSystem muzzleFlash;
        public TextMeshProUGUI ammunitionDisplay;

        private bool allowInvoke = true;
        private float time = 0;
        private bool spawnBullet = false;
        private Vector3 walkSpread = Vector3.zero;
        private Vector3 targetPoint;
        private void Awake()
        {
            bulletsLeftInMagazine = gunData.magSize;
            isReadyToShoot = true;
            animator = GetComponent<Animator>();
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
            HandleInput();

            if (ammunitionDisplay != null)
            {
                ammunitionDisplay.enabled = true;
                ammunitionDisplay.SetText(bulletsLeftInMagazine + " / " + gunData.magSize);
            }
            if (playerController.IsMovingOrJumping)
            {
                walkSpread = new Vector3(Random.Range(-gunData.spread * 10f, gunData.spread * 10f), Random.Range(-gunData.spread * 10f, gunData.spread * 10f), 0);
            }
            else
            {
                walkSpread = Vector3.zero;
            }
        }

        private void HandleInput()
        {
            if (gunData.isAutomatic)
                isShooting = ShouldShoot;
            else
            {
                isShooting = ShouldShootOnClick;
            }

            if (ShouldReload || bulletsLeftInMagazine == 0) Reload();

            if (isReadyToShoot && isShooting && !isReloading && bulletsLeftInMagazine > 0 && gameObject.activeSelf)
            {
                Shoot();
                if (animator != null)
                {
                    animator.SetTrigger("Shoot");
                }
            }
            if (!isShooting && time >= gunData.recoilResetTime)
            {
                bulletsShot = 0;
            }
            if (!isShooting || bulletsLeftInMagazine == 0)
            {
                playerController.SetGunRotation(
                    Vector3.Lerp(
                        playerController.gunRotation,
                        Vector3.zero,
                        (gunData.timeBetweenShots * 60) * Time.deltaTime
                    )
                );
            }
            time += Time.smoothDeltaTime;
        }

        private void Shoot()
        {
            isReadyToShoot = false;
            Ray ray;
            if (playerController.IsMovingOrJumping)
            {
                ray = fpsCam.ViewportPointToRay(new Vector3(Random.Range(0.5f - gunData.spread, 0.5f + gunData.spread), Random.Range(0.5f - gunData.spread, 0.5f + gunData.spread), 0));
            }
            else
            {
                ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                HandleGunRecoil();
            }

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
                Target damageable = hit.transform.GetComponentInParent<Target>();
                if (hit.collider.gameObject.CompareTag("Head") && damageable != null)
                {
                    damageable.TakeDamage(gunData.damage * gunData.headDamageMultiplier);
                }
                else if (damageable != null)
                {
                    damageable.TakeDamage(gunData.damage);
                }
                GameObject hitPoint = Instantiate(hitPrefab, targetPoint, Quaternion.identity);
                Destroy(hitPoint, 5);
            }
            else
            {
                targetPoint = ray.GetPoint(gunData.maxDistance);
            }

            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

            spawnBullet = true;

            if (muzzleFlash != null)
            {
                ParticleSystem currentMuzzleFlash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
                currentMuzzleFlash.transform.forward = directionWithoutSpread.normalized;
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
            isReloading = false;
        }

        private void Reload()
        {
            isReloading = true;
            Invoke("ReloadFinished", gunData.reloadTime);
        }

        private void ReloadFinished()
        {
            if (gameObject.activeInHierarchy && isReloading)
            {
                bulletsLeftInMagazine = gunData.magSize;
                bulletsShot = 0;
                isReloading = false;
            }
            else
            {
                isReloading = false;
            }
        }
        private void SpawnBullet()
        {
            GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
            if (currentBullet != null)
            {
                currentBullet.transform.LookAt(targetPoint);
                currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * gunData.shootForce, ForceMode.Impulse);
            }
            spawnBullet = false;
        }
    }
}