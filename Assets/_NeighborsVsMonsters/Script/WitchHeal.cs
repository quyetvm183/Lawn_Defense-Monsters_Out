using System.Collections;
using UnityEngine;
namespace RGame
{
    [RequireComponent(typeof(CheckTargetHelper))]
    [RequireComponent(typeof(Controller2D))]
    public class WitchHeal : Enemy, ICanTakeDamage, IListener
    {
        [Header("===HEAL===")]
        public int healValue = 20;
        public float healRate = 2;
        public GameObject healFXOnTarget;
        public LayerMask healTargetLayer;
        public float keepDistanceWithTarget = 4;
        public AudioClip soundFx;
        public GameObject staffFx, healFx;
        public Transform staffPoint;
        public CheckTargetHelper checkTargetHelper;

        [ReadOnly] public bool isAvailable = true;

        public bool isDead { get; set; }

        [HideInInspector]
        public Vector3 velocity;
        private Vector2 _direction;
        [HideInInspector]
        public Controller2D controller;

        float velocityXSmoothing = 0;
        Vector2 pushForce;
        private float _directionFace;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();

            AnimSetBool("isSummon", true);
            controller = GetComponent<Controller2D>();
            _direction = isFacingRight() ? Vector2.right : Vector2.left;

            if ((_direction == Vector2.right && startBehavior == STARTBEHAVIOR.WALK_LEFT) || (_direction == Vector2.left && startBehavior == STARTBEHAVIOR.WALK_RIGHT))
            {
                Flip();
            }
            isPlaying = true;

            controller.collisions.faceDir = 1;


            StartCoroutine(AutoCheckAndShoot());
        }

        public override void Update()
        {
            base.Update();
            HandleAnimation();

            if (enemyState != ENEMYSTATE.WALK)
            {
                velocity.x = 0;
                return;
            }

            if (checkTarget.CheckTarget(isFacingRight() ? 1 : -1))
                DetectPlayer(delayChasePlayerWhenDetect);
        }

        public virtual void LateUpdate()
        {
            if (GameManager.Instance.State != GameManager.GameState.Playing)
                return;
            else if (!isPlaying || enemyEffect == ENEMYEFFECT.SHOKING || target == null)
            {
                velocity.y += -gravity * Time.deltaTime;
                velocity.x = 0;
                controller.Move(velocity * Time.deltaTime * multipleSpeed, false, isFacingRight());
                return;
            }

            if (transform.position.x < target.position.x)       //follow the target
                _direction.x = 1;
            else
                _direction.x = -1;

            float targetVelocityX = _direction.x * moveSpeed;

            if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
                targetVelocityX = 0;

            if (isStopping || isStunning)
                targetVelocityX = 0;

            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? 0.1f : 0.2f);

            velocity.y += -gravity * Time.deltaTime;

            if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left))
                velocity.x = 0;

            if (Mathf.Abs(transform.position.x - target.position.x) < keepDistanceWithTarget)
                velocity.x = 0;

            controller.Move(velocity * Time.deltaTime * multipleSpeed, false, isFacingRight());

            if (controller.collisions.above || controller.collisions.below)
                velocity.y = 0;
        }

        Transform target;

        IEnumerator AutoCheckAndShoot()
        {
            while (true)
            {
                target = null;
                yield return null;

                //while (!checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1))) { yield return null; };

                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 100, Vector2.zero, 0, healTargetLayer);
                if (hits.Length > 0)
                {
                    float fartestTargetDistance = -9999;
                    foreach (var obj in hits)
                    {
                        var checkEnemy = (ICanTakeDamage)obj.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                        if (checkEnemy != null)
                        {
                            var isEnemy = obj.collider.GetComponent<Enemy>();
                            //make sure the enemy still alive
                            if (isEnemy && (isEnemy.currentHealth < isEnemy.health) && (isEnemy.currentHealth > 0))
                            {
                                if (obj.transform.position.x > fartestTargetDistance)
                                {
                                    fartestTargetDistance = obj.transform.position.x;
                                    target = obj.transform;
                                }
                                //var hit = Physics2D.Raycast(transform.position, (obj.point - (Vector2)transform.position), 100, healTargetLayer);
                            }
                            //else if (obj.collider.GetComponent<Player_Archer>() && obj.collider.GetComponent<Player_Archer>().currentHealth < obj.collider.GetComponent<Player_Archer>().health)
                            //{
                            //    target = obj.transform;
                            //}
                        }
                    }

                    while (target != null)
                    {

                        if ((target.gameObject != gameObject) && (Mathf.Abs(transform.position.x - target.position.x) < keepDistanceWithTarget))
                        {
                            var isEnemy = target.GetComponent<Enemy>();
                            //Check the current target, only heal if the health lower and he is alive
                            if (target.gameObject.activeInHierarchy && (isEnemy.currentHealth < isEnemy.health) && (isEnemy.currentHealth > 0))
                            {
                                while (GameManager.Instance.State != GameManager.GameState.Playing) { yield return null; }

                                anim.SetTrigger("heal");
                                yield return new WaitForSeconds(healRate);
                            }
                            else
                                target = null;
                        }
                        yield return null;
                    }
                }
                else
                    Debug.LogError("No target");
            }
        }

        void Flip()
        {
            _direction = -_direction;
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, isFacingRight() ? 0 : 180, transform.rotation.z));
        }

        public override void DetectPlayer(float delayChase = 0)
        {
            base.DetectPlayer(delayChase);
        }

        void HandleAnimation()
        {
            AnimSetFloat("speed", Mathf.Abs(velocity.x));
            AnimSetBool("isRunning", Mathf.Abs(velocity.x) > walkSpeed);
            AnimSetBool("isStunning", isStunning);
        }

        public void SetForce(float x, float y)
        {
            velocity = new Vector3(x, y, 0);
        }

        public override void Die()
        {
            if (isDead)
                return;

            base.Die();

            isDead = true;

            CancelInvoke();

            var cols = GetComponents<BoxCollider2D>();
            foreach (var col in cols)
                col.enabled = false;

            AnimSetBool("isDead", true);
            if (Random.Range(0, 2) == 1)
                AnimSetTrigger("die2");

            if (enemyEffect == ENEMYEFFECT.BURNING)
                return;

            if (enemyEffect == ENEMYEFFECT.EXPLOSION || dieBehavior == DIEBEHAVIOR.DESTROY)
            {
                gameObject.SetActive(false);
                return;
            }

            StopAllCoroutines();
            StartCoroutine(DisableEnemy(AnimationHelper.getAnimationLength(anim, "Die") + 2f));
        }

        public override void Hit(Vector2 force, bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
        {
            if (!isPlaying || isStunning)
                return;

            base.Hit(force, pushBack, knockDownRagdoll, shock);
            if (isDead)
                return;

            AnimSetTrigger("hit");

            if (knockDownRagdoll)
                ;
            else if (pushBack)
                StartCoroutine(PushBack(force));
            else
                ;
        }

        public override void KnockBack(Vector2 force, float stunningTime = 0)
        {
            base.KnockBack(force);

            SetForce(force.x, force.y);
        }

        public IEnumerator PushBack(Vector2 force)
        {
            SetForce(force.x, force.y);

            if (isDead)
            {
                Die();
                yield break;
            }
        }

        IEnumerator DisableEnemy(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (disableFX)
                Instantiate(disableFX, spawnDisableFX != null ? spawnDisableFX.position : transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }



        public void AnimHeal()
        {
            if (target)
            {
                if (healFXOnTarget)
                    Instantiate(healFXOnTarget, target.position, Quaternion.identity);

                target.GetComponent<Enemy>().Heal(healValue);

                if (staffFx)
                    Instantiate(staffFx, staffPoint.position, Quaternion.identity);

                if (healFx)
                    Instantiate(healFx, target.position + Vector3.up, Quaternion.identity);

                SoundManager.PlaySfx(soundFx);
            }
        }

        public void Victory()
        {
            anim.SetBool("victory", true);
        }
    }
}