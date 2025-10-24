using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    [AddComponentMenu("ADDP/Enemy AI/Range Attack")]
    public class EnemyRangeAttack : MonoBehaviour
    {
        public LayerMask enemyLayer;
        //the fire point to shoot the bullet
        public Transform checkPoint;
        [Header("AIM TARGET")]
        //allow anim target or not
        public bool aimTarget = false;
        public Vector2 aimTargetOffset = new Vector2(0, 0.5f);
        [Space]
        //the fire point to shoot the bullet
        public Transform firePoint;
        public AudioClip soundShoot;
        [Range(0, 1)]
        public float soundShootVolume = 0.5f;

        public Transform shootingPoint;
        public float damage = 30;
        public float detectDistance = 5;
        public Projectile bullet;
        //the rate of shooting
        public float shootingRate = 1;
        //shoot multiple bullet
        [HideInInspector] public int multiShoot = 1;
        [HideInInspector] public float multiShootRate = 0.2f;
        float lastShoot = -999;
        [HideInInspector] public GameObject GunObj;
        Vector3 dir = Vector3.right;
        public bool isAttacking { get; set; }

        WeaponEffect hasWeaponEffect;
        void Start()
        {
            if (GetComponent<Enemy>() && GetComponent<Enemy>().upgradedCharacterID)
                hasWeaponEffect = GetComponent<Enemy>().upgradedCharacterID.weaponEffect;
        }

        public Vector3 firePosition()
        {
            Vector3 _firePoint = firePoint.position;
            return _firePoint;
        }

        public bool AllowAction()
        {
            //check the time rate for the next shoot
            return Time.time - lastShoot > shootingRate;
        }

        Vector3 _target;
        public bool CheckPlayer(bool isFacingRight)
        {
            dir = isFacingRight ? Vector2.right : Vector2.left;
            bool isHit = false;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(checkPoint.position, detectDistance, Vector2.zero, 0, enemyLayer);
            if (hits.Length > 0)
            {
                float closestDistance = 99999;
                foreach (var obj in hits)
                {
                    var checkEnemy = (ICanTakeDamage)obj.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                    if (checkEnemy != null)
                    {
                        if (Mathf.Abs(obj.transform.position.x - checkPoint.position.x) < closestDistance)
                        {
                            closestDistance = Mathf.Abs(obj.transform.position.x - checkPoint.position.x);

                            //_target = obj.transform;

                            var hit = Physics2D.Raycast(checkPoint.position, (obj.point - (Vector2)checkPoint.position), detectDistance, enemyLayer);
                            Debug.DrawRay(checkPoint.position, (obj.point - (Vector2)checkPoint.position) * 100, Color.red);
                            _target = /*target.position + Vector3.up * Random.Range(0.2f, 0.6f);*/ obj.collider.gameObject.transform.position;
                            //_target.y = Mathf.Max(_target.y, checkPoint.position.y - 0.1f);

                            isHit = true;
                        }
                    }
                }
            }

            return isHit;

        }

        public void Action()
        {
            //Save the last time shooting
            isAttacking = true;
            lastShoot = Time.time;
        }

        public void Shoot(bool isFacingRight)
        {
            //start shoot the bullet
            StartCoroutine(ShootCo(isFacingRight));
        }

        IEnumerator ShootCo(bool isFacingRight)
        {
            for (int i = 0; i < multiShoot; i++)
            {
                SoundManager.PlaySfx(soundShoot, soundShootVolume);
                //fire the bullet with the given amount
                float shootAngle = 0;
                shootAngle = isFacingRight ? 0 : 180;
                var obj = Instantiate(bullet.gameObject, shootingPoint != null ? shootingPoint.position : firePosition(), Quaternion.Euler(0, 0, shootAngle));
                //Spawn the projectile and set the new parameters for it
                var projectile = obj.GetComponent<Projectile>();
                //get the shooting direction
                Vector3 _dir;
                if (aimTarget)
                {
                    _dir = _target - shootingPoint.position;
                    _dir += (Vector3)aimTargetOffset;
                    _dir.Normalize();
                }
                else
                    _dir = Vector2.right * (isFacingRight ? 1 : -1);
                //Check and init the weapon
                if (hasWeaponEffect != null)
                {
                    projectile.Initialize(gameObject, _dir, Vector2.zero, false, damage * 0.9f, damage * 1.1f, 0, Vector2.zero, hasWeaponEffect);
                }
                else
                    projectile.Initialize(gameObject, _dir, Vector2.zero, false, damage * 0.9f, damage * 1.1f, 0);
                projectile.gameObject.SetActive(true);
                yield return new WaitForSeconds(multiShootRate);
            }

            CancelInvoke();
            Invoke("EndAttack", 1);
        }

        void EndAttack()
        {
            //End attacking flag
            isAttacking = false;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawRay(checkPoint.position, dir * detectDistance);
        }
    }
}