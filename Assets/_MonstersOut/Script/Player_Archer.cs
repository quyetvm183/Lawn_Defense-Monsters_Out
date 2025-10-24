using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    [RequireComponent(typeof(CheckTargetHelper))]
    [RequireComponent(typeof(Controller2D))]
    public class Player_Archer : Enemy, ICanTakeDamage, IListener
    {
        public CheckTargetHelper checkTargetHelper;
        [Header("ARROW SHOOT")]
        public float shootRate = 1;
        //the default force of the arrow
        public float force = 20;
        [Range(0.01f, 0.1f)]
        //Set the rate of checking
        [ReadOnly] public float stepCheck = 0.1f;
        //set the angle checking
        [ReadOnly] public float stepAngle = 1;
        //Set the gravity for the arrow
        [ReadOnly] public float gravityScale = 3.5f;
        [ReadOnly] public bool onlyShootTargetInFront = true;

        [Header("ARROW DAMAGE")]
        public ArrowProjectile arrow;
        public WeaponEffect weaponEffect;
        //Set the arrow damage default
        public int arrowDamage = 30;
        public Transform firePostion;

        [Header("Sound")]
        public float soundShootVolume = 0.5f;
        public AudioClip[] soundShoot;
        private float x1, y1;
        bool isTargetRight = false;
        float lastShoot;

        [ReadOnly] public bool isAvailable = true;
        public bool isSocking { get; set; }
        public bool isDead { get; set; }

        [HideInInspector]
        public Vector3 velocity;
        private Vector2 _direction;
        [HideInInspector]
        public Controller2D controller;

        float velocityXSmoothing = 0;

        [Header("New")]
        bool isLoading = false;
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            //Setup and get the component
            controller = GetComponent<Controller2D>();
            _direction = isFacingRight() ? Vector2.right : Vector2.left;
            //Check the direction to Flip the face
            if ((_direction == Vector2.right && startBehavior == STARTBEHAVIOR.WALK_LEFT) || (_direction == Vector2.left && startBehavior == STARTBEHAVIOR.WALK_RIGHT))
            {
                Flip();
            }
            isPlaying = true;
            isSocking = false;
            //Facing to the right by default
            controller.collisions.faceDir = 1;
            //Auto check and shoot
            StartCoroutine(AutoCheckAndShoot());
            //Get the arrow damage from the ID
            arrowDamage = upgradedCharacterID.UpgradeRangeDamage;
        }

        public override void Update()
        {
            base.Update();
            HandleAnimation();
            //If no in the Walk state, stop moving
            if (enemyState != ENEMYSTATE.WALK)
            {
                velocity.x = 0;
                return;
            }
            //Check the target
            if (checkTarget.CheckTarget(isFacingRight() ? 1 : -1))
                DetectPlayer(delayChasePlayerWhenDetect);
        }

        public virtual void LateUpdate()
        {
            //stop moving if the game is not in the playing state
            if (GameManager.Instance.State != GameManager.GameState.Playing)
            {
                velocity.x = 0;
                return;
            }
            //Check the state to stop moving
            else if (!isPlaying || isSocking || enemyEffect == ENEMYEFFECT.SHOKING || isLoading || checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1)))
            {
                velocity = Vector2.zero;
                return;
            }
            //Get the target velocity
            float targetVelocityX = _direction.x * moveSpeed;
            if (isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
            {
                targetVelocityX = 0;
            }
            //Check the state
            if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
                targetVelocityX = 0;

            if (isStopping || isStunning)
                targetVelocityX = 0;
            //Get the velocity
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? 0.1f : 0.2f);
            //Get the gravity
            velocity.y += -gravity * Time.deltaTime;
            //If contact the wall, stop move
            if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left))
                velocity.x = 0;
            //Move the character
            controller.Move(velocity * Time.deltaTime * multipleSpeed, false, isFacingRight());

            if (controller.collisions.above || controller.collisions.below)
                velocity.y = 0;
        }

        void Flip()
        {
            //Flip the direction and facing
            _direction = -_direction;
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, isFacingRight() ? 0 : 180, transform.rotation.z));
        }

        public override void Stun(float time = 2)
        {
            //Do the stun action
            base.Stun(time);
            StartCoroutine(StunCo(time));
        }

        IEnumerator StunCo(float time)
        {
            //Set the animation state and wait for the end stun
            AnimSetTrigger("stun");
            isStunning = true;
            yield return new WaitForSeconds(time);
            isStunning = false;
        }

        public override void StunManuallyOn()
        {
            AnimSetTrigger("stun");
            isStunning = true;
        }

        public override void StunManuallyOff()
        {
            //Stop stunning
            isStunning = false;
        }

        public override void DetectPlayer(float delayChase = 0)
        {
            base.DetectPlayer(delayChase);
        }

        void HandleAnimation()
        {
            //Update the animation state
            AnimSetFloat("speed", Mathf.Abs(velocity.x));
            AnimSetBool("isRunning", Mathf.Abs(velocity.x) > walkSpeed);
            AnimSetBool("isStunning", isStunning);
        }

        public void SetForce(float x, float y)
        {
            //Set the force for the character
            velocity = new Vector3(x, y, 0);
        }

        public override void Die()
        {
            //Stop if it's dead
            if (isDead)
                return;

            base.Die();
            //Set the dead state
            isDead = true;

            CancelInvoke();
            //Disable all the controller
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
            //Stop all the actions
            StopAllCoroutines();
            StartCoroutine(DisableEnemy(AnimationHelper.getAnimationLength(anim, "Die") + 2f));
        }

        public override void Hit(Vector2 force, bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
        {
            //If the character unavailable or is stunning, stop action
            if (!isPlaying || isStunning)
                return;
            //call the hit event
            base.Hit(force, pushBack, knockDownRagdoll, shock);
            if (isDead)
                return;
            //trigger the hit animation
            AnimSetTrigger("hit");
            //try to push the enemy back
            if (pushBack)
                StartCoroutine(PushBack(force));
            else if (shock)
                StartCoroutine(Shock());
        }

        public override void KnockBack(Vector2 force, float stunningTime = 0)
        {
            base.KnockBack(force);
            //try to set the velocity to the enemy
            SetForce(force.x, force.y);
        }

        public IEnumerator PushBack(Vector2 force)
        {
            //try to push the enemy back with the force value
            SetForce(force.x, force.y);
            //if the enemy alread dead, call Die function
            if (isDead)
            {
                Die();
                yield break;
            }
        }

        public IEnumerator Shock()
        {
            //if the enemy alread dead, call Die function
            if (isDead)
            {
                Die();
                yield break;
            }
        }

        IEnumerator DisableEnemy(float delay)
        {
            //delay the time before disable the enemy
            yield return new WaitForSeconds(delay);
            if (disableFX)
                Instantiate(disableFX, spawnDisableFX != null ? spawnDisableFX.position : transform.position, Quaternion.identity);
            //disable the game object
            gameObject.SetActive(false);
        }

        Vector2 autoShootPoint;
        Transform target;

        IEnumerator AutoCheckAndShoot()
        {
            while (true)
            {
                //Reset the target
                target = null;
                yield return null;
                //If no detect the target, wait
                while (!checkTargetHelper.CheckTarget((isFacingRight() ? 1 : -1))) { yield return null; };
                //Cast to check the target
                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 100, Vector2.zero, 0, GameManager.Instance.layerEnemy);
                //If hit the target
                if (hits.Length > 0)
                {
                    //Check the closest target
                    float closestDistance = 99999;
                    foreach (var obj in hits)
                    {
                        var checkEnemy = (ICanTakeDamage)obj.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                        if (checkEnemy != null)
                        {
                            if (Mathf.Abs(obj.transform.position.x - transform.position.x) < closestDistance)
                            {
                                closestDistance = Mathf.Abs(obj.transform.position.x - transform.position.x);

                                target = obj.transform;
                                //Check if any object between the character and target or not
                                var hit = Physics2D.Raycast(transform.position, (obj.point - (Vector2)transform.position), 100, GameManager.Instance.layerEnemy);
                                Debug.DrawRay(transform.position, (obj.point - (Vector2)transform.position) * 100, Color.red);
                                //Get the hit point
                                autoShootPoint = hit.point;
                                autoShootPoint.y = Mathf.Max(autoShootPoint.y, firePostion.position.y - 0.1f);
                            }
                        }
                    }

                    if (target)
                    {
                        //Shoot the arrow
                        Shoot();
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
        }

        public void Shoot()
        {
            if (!isAvailable || target == null || GameManager.Instance.State != GameManager.GameState.Playing)
                return;
            //Check if the target is the right side
            isTargetRight = autoShootPoint.x > transform.position.x;
            //If only shoot the target front
            if (onlyShootTargetInFront && ((isTargetRight && !isFacingRight()) || (isFacingRight() && !isTargetRight)))
                return;

            StartCoroutine(CheckTarget());
        }


        IEnumerator CheckTarget()
        {
            //Get the mouse position
            Vector3 mouseTempLook = autoShootPoint;
            mouseTempLook -= transform.position;
            mouseTempLook.x *= (isFacingRight() ? -1 : 1);
            yield return null;
            //Get the begin arrow point
            Vector2 fromPosition = firePostion.position;
            Vector2 target = autoShootPoint;
            //Get the begin angle
            float beginAngle = Vector2ToAngle(target - fromPosition);
            Vector2 ballPos = fromPosition;
            //Get the final angle
            float cloestAngleDistance = int.MaxValue;

            bool checkingPerAngle = true;
            while (checkingPerAngle)
            {
                //Init the value
                int k = 0;
                Vector2 lastPos = fromPosition;
                bool isCheckingAngle = true;
                float clostestDistance = int.MaxValue;

                while (isCheckingAngle)
                {
                    Vector2 shotForce = force * AngleToVector2(beginAngle);
                    //X position for each point is found
                    x1 = ballPos.x + shotForce.x * Time.fixedDeltaTime * (stepCheck * k);   
                    y1 = ballPos.y + shotForce.y * Time.fixedDeltaTime * (stepCheck * k) - (-(Physics2D.gravity.y * gravityScale) / 2f * Time.fixedDeltaTime * Time.fixedDeltaTime * (stepCheck * k) * (stepCheck * k));    //Y position for each point is found
                    //Get the distance from the target to the shooting point
                    float _distance = Vector2.Distance(target, new Vector2(x1, y1));
                    if (_distance < clostestDistance)
                        clostestDistance = _distance;

                    if ((y1 < lastPos.y) && (y1 < target.y))
                        isCheckingAngle = false;
                    else
                        k++;
                    //Get the last position
                    lastPos = new Vector2(x1, y1);
                }

                if (clostestDistance >= cloestAngleDistance)
                    checkingPerAngle = false;
                else
                {
                    cloestAngleDistance = clostestDistance;

                    if (isTargetRight)
                        beginAngle += stepAngle;
                    else
                        beginAngle -= stepAngle;
                }
            }
            //Look at the target
            var lookAt = AngleToVector2(beginAngle) * 10;
            lookAt.x *= (isFacingRight() ? -1 : 1);

            yield return null;
            anim.SetTrigger("shoot");
            //Fire number arrow
            ArrowProjectile _tempArrow;
            //Spawn the arrow and init the value
            _tempArrow = Instantiate(arrow, fromPosition, Quaternion.identity);
            _tempArrow.Init(force * AngleToVector2(beginAngle), gravityScale, arrowDamage);

            SoundManager.PlaySfx(soundShoot[Random.Range(0, soundShoot.Length)], soundShootVolume);
            //Reloading the arrow
            StartCoroutine(ReloadingCo());
        }


        IEnumerator ReloadingCo()
        {
            //Make the arrow cant be shoot
            isAvailable = false;
            //Init the value
            lastShoot = Time.time;
            isLoading = true;
            yield return new WaitForSeconds(0.1f);
            anim.SetBool("isLoading", true);
            //Wait for the shooting rate
            while (Time.time < (lastShoot + shootRate)) { yield return null; }

            anim.SetBool("isLoading", false);
            //Delay a little time
            yield return new WaitForSeconds(0.2f);
            //Allow shoot again
            isAvailable = true;
            isLoading = false;
        }


        public static Vector2 AngleToVector2(float degree)
        {
            //Get the direction to shoot
            Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, degree) * Vector2.right);
            return dir;
        }

        public float Vector2ToAngle(Vector2 vec2)
        {
            //Get the angle to shoot
            var angle = Mathf.Atan2(vec2.y, vec2.x) * Mathf.Rad2Deg;
            return angle;
        }
    }
}