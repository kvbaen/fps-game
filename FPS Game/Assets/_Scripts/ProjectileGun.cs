using UnityEngine;
using TMPro;


namespace FpsGame.ProjectileGun
{
    public class ProjectileGun : MonoBehaviour
    {
        public GameObject Bullet;
        private PlayerController playerController;

        //Gun stats
        int _bulletsLeft, _bulletsShot;

        //bools
        bool _shooting, _readyToShoot, _reloading;
        private bool ShouldShoot => Input.GetKey(playerController.shootKey);
        private bool ShouldReload => Input.GetKeyDown(playerController.reloadKey) && _bulletsLeft < _gunData.magSize && !_reloading && this.gameObject.activeSelf;

        //Reference
        public Camera FpsCam;
        public Transform AttackPoint;
        [SerializeField] private GunData _gunData;
        [SerializeField] private string EnemyTag;

        //Graphics
        public ParticleSystem MuzzleFlash;
        public TextMeshProUGUI AmmunitionDisplay;

        //bug fixing :D
        public bool AllowInvoke = true;
        float time = 0;
        private void Awake()
        {
            //make sure magazine is full
            _bulletsLeft = _gunData.magSize;
            _readyToShoot = true;
            playerController = GetComponentInParent<PlayerController>();
        }

        private void FixedUpdate()
        {
            MyInput();

            //Set ammo display, if it exists :D
            if (AmmunitionDisplay != null)
            {
                AmmunitionDisplay.enabled = true;
                AmmunitionDisplay.SetText(_bulletsLeft / _gunData.bulletsPerTap + " / " + _gunData.magSize / _gunData.bulletsPerTap);
            }
        }
        private void MyInput()
        {
            //Check if allowed to hold down button and take corresponding input
            if (time > _gunData.timeBetweenShooting)
            {
                if (_gunData.allowButtonHold) _shooting = ShouldShoot;
                else
                {
                    if (_bulletsShot == 0)
                    {
                        _shooting = ShouldShoot;
                        _readyToShoot = true;
                    }
                }
            }
            //Reloading
            if (!ShouldShoot) { _bulletsShot = 0; }

            if (ShouldReload) Reload();
            //Reload automatically when trying to shoot without ammo
            if (_readyToShoot && _shooting && !_reloading && _bulletsLeft <= 0 && this.gameObject.activeSelf) Reload();

            //Shooting
            if (_readyToShoot && _shooting && !_reloading && _bulletsLeft > 0 && this.gameObject.activeSelf)
            {
                Shoot();
                time = 0;
            }
            time += Time.smoothDeltaTime;
        }

        private void Shoot()
        {
            _readyToShoot = false;
            //Find the exact hit position using a raycast
            Ray ray = FpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
            RaycastHit hit;

            //check if ray hits something
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
                if (hit.collider.gameObject.tag == EnemyTag)
                {
                    IDamageable damageable = hit.transform.GetComponent<IDamageable>();
                    damageable?.TakeDamage(_gunData.damage);
                }
            }
            else
                targetPoint = ray.GetPoint(_gunData.maxDistance); //Just a point far away from the player

            //Calculate direction from attackPoint to targetPoint
            Vector3 directionWithoutSpread = targetPoint - AttackPoint.position;

            //Calculate spread
            float x = Random.Range(-_gunData.spread, _gunData.spread);
            float y = Random.Range(-_gunData.spread, _gunData.spread);

            //Calculate new direction with spread
            Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

            //Instantiate bullet/projectile
            GameObject currentBullet = Instantiate(Bullet, AttackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
            currentBullet.transform.forward = directionWithSpread.normalized;
            /*if (Bullet.name == "cal_7_v2")
            {
                currentBullet.transform.Rotate(Vector3.right * -90);
            }
            else
            {
                currentBullet.transform.rotation = Quaternion.LookRotation(directionWithSpread);
            }*/

            //Add forces to bullet
            if (_bulletsShot == 0)
            {
                currentBullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * _gunData.shootForce, ForceMode.Impulse);
            }
            else
            {
                currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * _gunData.shootForce, ForceMode.Impulse);
            }
            currentBullet.GetComponent<Rigidbody>().AddForce(FpsCam.transform.up * _gunData.upwardForce, ForceMode.Impulse);

            //Instantiate muzzle flash, if you have one
            if (MuzzleFlash != null)
            {
                ParticleSystem currentMuzzleFlash = Instantiate(MuzzleFlash, AttackPoint.position, Quaternion.identity);
                currentMuzzleFlash.transform.forward = directionWithSpread.normalized;
            }
            _bulletsLeft--;
            _bulletsShot++;


            //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
            if (AllowInvoke && this.gameObject.activeInHierarchy)
            {
                if (_gunData.allowButtonHold)
                {
                    Invoke("ResetShot", _gunData.timeBetweenShooting);
                    AllowInvoke = false;
                }
                //Add recoil to player (should only be called once)
                //PlayerRb.AddForce(-directionWithSpread.normalized * _gunData.recoilForce, ForceMode.Impulse);
            }

            //if more than one bulletsPerTap make sure to repeat shoot function
            if (_bulletsShot < _gunData.bulletsPerTap && _bulletsLeft > 0 && this.gameObject.activeInHierarchy)
            {
                Invoke("Shoot", _gunData.timeBetweenShots);
            }

        }
        private void ResetShot()
        {
            //Allow shooting and invoking again
            _readyToShoot = true;
            AllowInvoke = true;
        }

        private void OnDisable() => _reloading = false; 
        private void Reload()
        {
           _reloading = true;
           Invoke("ReloadFinished", _gunData.reloadTime); //Invoke ReloadFinished function with your reloadTime as delay
            
        }
        private void ReloadFinished()
        {
            if (this.gameObject.activeInHierarchy && _reloading)
            {
                //Fill magazine
                _bulletsLeft = _gunData.magSize;
                _reloading = false;
            }
            else
            {
                _reloading = false;
            }
        }
    }
}
