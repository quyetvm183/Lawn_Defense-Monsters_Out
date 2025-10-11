using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    [AddComponentMenu("ADDP/Enemy AI/Melee Attack")]
    public class EnemyMeleeAttack : MonoBehaviour
    {
        //set the target layer
        public LayerMask targetLayer;
        //set the checkpoint
        public Transform checkPoint;
        //how many target damaged per hit
        [ReadOnly] public int maxTargetPerHit = 1;
        //the rate between 2 attacks
        public float radiusCheck = 1;
        //give damage to player
        public float dealDamage = 20;

        [Range(1, 100)]
        public int criticalPercent = 10;
        public float meleeRate = 1;
        float lastShoot = -999;
        public bool isAttacking { get; set; }
        [HideInInspector] public GameObject MeleeObj;
        [Range(0, 1)]
        public float soundAttacksVol = 0.5f;
        public AudioClip[] soundAttacks;
        WeaponEffect hasWeaponEffect;

        void Start()
        {
            //meleeDamageObj.SetActive (false);
            if (GetComponent<Enemy>() && GetComponent<Enemy>().upgradedCharacterID)
            {
                hasWeaponEffect = GetComponent<Enemy>().upgradedCharacterID.weaponEffect;
                maxTargetPerHit = GetComponent<Enemy>().upgradedCharacterID.maxTargetPerHit;
            }

        }

        public bool AllowAction()
        {
            //check the time with the rate time
            return Time.time - lastShoot > meleeRate;
        }

        // Update is called once per frame
        bool isFacingRight = false;

        public bool CheckPlayer(bool _isFacingRight)
        {
            isFacingRight = _isFacingRight;
            //check the player by casting the circle
            RaycastHit2D hit = Physics2D.CircleCast(checkPoint.position, radiusCheck, Vector2.zero, 0, targetLayer);
            if (hit)
                return true;
            else
                return false;
        }

        public void Action()
        {
            //save the last attack time
            lastShoot = Time.time;
        }


        void EndAttack()
        {
            isAttacking = false;
        }

        public void Check4Hit()
        {
            //check the player by casting the circle
            RaycastHit2D[] hits = Physics2D.CircleCastAll(checkPoint.position, radiusCheck * 1.2f, Vector2.zero, 0, targetLayer);
            int counterHit = 0;
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (counterHit < maxTargetPerHit)
                    {
                        //if the target can be damaged, deal damage to the target
                        var takeDamage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                        if (takeDamage != null)
                        {
                            float _damage = dealDamage + (int)(Random.Range(-0.1f, 0.1f) * dealDamage);
                            if (Random.Range(0, 100) < criticalPercent)
                            {
                                _damage *= 2;
                                FloatingTextManager.Instance.ShowText("CRIT!", Vector3.up, Color.red, hit.collider.gameObject.transform.position, 30);
                            }

                            if (hasWeaponEffect != null)
                            {
                                takeDamage.TakeDamage(_damage, Vector2.zero, hit.point, gameObject, BODYPART.NONE, hasWeaponEffect);
                            }
                            else
                                takeDamage.TakeDamage(_damage, Vector2.zero, hit.point, gameObject);

                            counterHit++;
                        }

                        if (soundAttacks.Length > 0)
                            SoundManager.PlaySfx(soundAttacks[Random.Range(0, soundAttacks.Length)], soundAttacksVol);
                    }
                }
            }
        }

        public void EndCheck4Hit()
        {
            CancelInvoke();
            Invoke("EndAttack", 1);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkPoint.position, radiusCheck);
        }
    }
}