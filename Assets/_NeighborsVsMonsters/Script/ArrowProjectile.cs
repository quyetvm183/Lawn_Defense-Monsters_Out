using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    public class ArrowProjectile : Projectile, IListener, ICanTakeDamage
    {
        //replace the arrow image when hit the target
        public Sprite hitImageBlood;
        public SpriteRenderer arrowImage;
        Vector2 oldPos;
        int Damage = 30;
        //destroy effect when arrow disable
        public GameObject DestroyEffect;
        public int pointToGivePlayer;
        //how long the arrow last
        public float timeToLive = 3;
        //Place the sound and set the sound volume
        public AudioClip soundHitEnemy;
        [Range(0, 1)]
        public float soundHitEnemyVolume = 0.5f;
        public AudioClip soundHitNothing;
        [Range(0, 1)]
        public float soundHitNothingVolume = 0.5f;

        public GameObject ExplosionObj;
        float timeToLiveCounter = 0;
        public bool parentToHitObject = true;

        bool isHit = false;
        Rigidbody2D rig;

        void OnEnable()
        {
            //Init the time to live, the arrow will be destroy after this time
            timeToLiveCounter = timeToLive;
            isHit = false;
            //Get the rig component
            if (rig == null)
                rig = GetComponent<Rigidbody2D>();
            rig.isKinematic = false;
        }

        public void Init(Vector2 velocityForce, float gravityScale, int damage)
        {
            //Setup the new parameters for the arrow, called by the Archer
            Damage = damage;

            rig = GetComponent<Rigidbody2D>();
            rig.gravityScale = gravityScale;
            rig.linearVelocity = velocityForce;
        }

        void Start()
        {
            //Store the original position
            oldPos = transform.position;

            GameManager.Instance.listeners.Add(this);
        }

        public Vector2 checkTargetDistanceOffset = new Vector2(-0.25f, 0);
        public float checkTargetDistance = 1;

        // Update is called once per frame
        void Update()
        {
            //if the arrow hit anything, stop move
            if (isHit)
                return;
            //move the arrow to the right axis
            if ((Vector2)transform.position != oldPos)
            {
                transform.right = ((Vector2)transform.position - oldPos).normalized;
            }

            //check if the arrow hit anything
            RaycastHit2D hit = Physics2D.Linecast(oldPos, transform.position, LayerCollision);
            if (hit)
            {
                Hit(hit);
                isHit = true;
            }

            oldPos = transform.position;
            //if the time over the live time, destroy the arrow
            if ((timeToLiveCounter -= Time.deltaTime) <= 0)
            {
                DestroyProjectile();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine((Vector2)transform.position + checkTargetDistanceOffset, (Vector2)transform.position + checkTargetDistanceOffset + (Vector2)transform.right * checkTargetDistance);
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {

        }

        void Hit(RaycastHit2D other)
        {
            //try to find the head and set the arrow to it
            transform.position = other.point + (Vector2)(transform.position - transform.Find("head").position);

            //try to check if hit object is body part or not to deal the damage
            var takeDamage = (ICanTakeDamage)other.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
            if (takeDamage != null)
            {
                OnCollideTakeDamage(other.collider, takeDamage);
            }
            else
                OnCollideOther(other.collider);
        }

        IEnumerator DestroyProjectile(float delay = 0)
        {
            //Init the destroy progress, the arrow will be destroyed after the delay time
            var rig = GetComponent<Rigidbody2D>();
            rig.linearVelocity = Vector2.zero;
            rig.isKinematic = true;

            yield return new WaitForSeconds(delay);
            if (DestroyEffect != null)
                Instantiate(DestroyEffect, transform.position, Quaternion.identity);

            if (Explosion)
            {
                var bullet = Instantiate(ExplosionObj, transform.position, Quaternion.identity) as GameObject;
            }

            gameObject.SetActive(false);
        }

        public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE)
        {
            //take damage and destroy the arrow after 1 second
            SoundManager.PlaySfx(soundHitNothing, soundHitNothingVolume);
            StartCoroutine(DestroyProjectile(1));
        }

        protected override void OnCollideOther(Collider2D other)
        {
            //Destroy the arrow if hit other object but the target
            SoundManager.PlaySfx(soundHitNothing, soundHitNothingVolume);
            StartCoroutine(DestroyProjectile(3));
            if (parentToHitObject)
                transform.parent = other.gameObject.transform;

        }

        protected override void OnCollideTakeDamage(Collider2D other, ICanTakeDamage takedamage)
        {
            //Debug.LogError(other.name);
            base.OnCollideTakeDamage(other, takedamage);

            takedamage.TakeDamage(Damage, Vector2.zero, transform.position, Owner);
            SoundManager.PlaySfx(soundHitEnemy, soundHitEnemyVolume);
            StartCoroutine(DestroyProjectile(0));

            if (parentToHitObject)
                transform.parent = other.gameObject.transform;

            if (arrowImage && hitImageBlood)
            {
                arrowImage.sprite = hitImageBlood;
            }
        }

        protected override void OnCollideTakeDamageBodyPart(Collider2D other, ICanTakeDamageBodyPart takedamage)
        {
            base.OnCollideTakeDamageBodyPart(other, takedamage);
            WeaponEffect weaponEffect = new WeaponEffect();
            //Deal the damage to the target
            takedamage.TakeDamage(Damage, force, transform.position, Owner);
            StartCoroutine(DestroyProjectile(0));

            //set parent to the arrow if this option was choosen
            if (parentToHitObject)
                transform.parent = other.gameObject.transform;

            if (arrowImage && hitImageBlood)
            {
                arrowImage.sprite = hitImageBlood;
            }
        }

        bool isStop = false;
        #region IListener implementation

        public void IPlay()
        {
        }

        public void ISuccess()
        {
        }

        public void IPause()
        {
        }

        public void IUnPause()
        {
        }

        public void IGameOver()
        {
        }

        public void IOnRespawn()
        {
        }

        public void IOnStopMovingOn()
        {
            isStop = true;
        }

        public void IOnStopMovingOff()
        {
            isStop = false;
        }

        public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null)
        {
            StartCoroutine(DestroyProjectile(0));
        }

        #endregion
    }
}