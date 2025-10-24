
/* 
 * 
 * This is the most used script for enemy
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
	[AddComponentMenu("ADDP/Enemy AI/Smart Enemy Ground Control")]
	[RequireComponent(typeof(Controller2D))]
	public class SmartEnemyGrounded : Enemy, ICanTakeDamage
	{
		public bool isSocking { get; set; }
		//the state of the enemy
		public bool isDead { get; set; }
		[HideInInspector]
		public Vector3 velocity;
		//the moving direction of the enemy
		private Vector2 _direction;
		[HideInInspector]
		public Controller2D controller;

		float velocityXSmoothing = 0;
		Vector2 pushForce;
		private float _directionFace;

		[Header("New")]

		bool allowCheckAttack = true;

		EnemyRangeAttack rangeAttack;
		EnemyMeleeAttack meleeAttack;
		EnemyThrowAttack throwAttack;
		SpawnItemHelper spawnItem;

		public override void Start()
		{
			base.Start();
			//Get the component
			controller = GetComponent<Controller2D>();
			//get the direction base on the facing of enemy
			_direction = isFacingRight() ? Vector2.right : Vector2.left;
			//If the direction is not same with the start behavior. flip the direction
			if ((_direction == Vector2.right && startBehavior == STARTBEHAVIOR.WALK_LEFT) || (_direction == Vector2.left && startBehavior == STARTBEHAVIOR.WALK_RIGHT))
			{
				Flip();
			}
			isPlaying = true;
			isSocking = false;
			//set the facing to 1
			controller.collisions.faceDir = 1;
			//Get the component
			rangeAttack = GetComponent<EnemyRangeAttack>();
			meleeAttack = GetComponent<EnemyMeleeAttack>();
			throwAttack = GetComponent<EnemyThrowAttack>();
			//Active the gun object if the attack is set to Range
			if (rangeAttack && rangeAttack.GunObj)
				rangeAttack.GunObj.SetActive(attackType == ATTACKTYPE.RANGE);
			//Active the melee object if the attack is set to melee
			if (meleeAttack && meleeAttack.MeleeObj)
				meleeAttack.MeleeObj.SetActive(attackType == ATTACKTYPE.MELEE);

			spawnItem = GetComponent<SpawnItemHelper>();


			//Do Get Upgrade
			if (upgradedCharacterID != null)
			{
				if (meleeAttack)
				{
					meleeAttack.dealDamage = upgradedCharacterID.UpgradeMeleeDamage;
					meleeAttack.criticalPercent = upgradedCharacterID.UpgradeCriticalDamage;
				}
				if (rangeAttack)
				{
					rangeAttack.damage = upgradedCharacterID.UpgradeRangeDamage;
				}
			}
		}

		public override void Update()
		{
			base.Update();

			HandleAnimation();
			//If the enemy is not in the walk state, don't allow the enemy move
			if (enemyState != ENEMYSTATE.WALK || GameManager.Instance.State != GameManager.GameState.Playing)
			{
				velocity.x = 0;
				return;
			}
			//If detect the target, call detect function
			if (checkTarget.CheckTarget(isFacingRight() ? 1 : -1))
				DetectPlayer(delayChasePlayerWhenDetect);
		}

		public virtual void LateUpdate()
		{
			//If the GameManager Playing state is not in the Playing mode stop
			if (GameManager.Instance.State != GameManager.GameState.Playing)
				return;
			//If the Playing state is not in the Playing mode stop
			else if (!isPlaying || isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
			{
				velocity = Vector2.zero;
				return;
			}
			//Get the velocity
			float targetVelocityX = _direction.x * moveSpeed;
			//If the enemy is shocking, stop move
			if (isSocking || enemyEffect == ENEMYEFFECT.SHOKING)
			{
				targetVelocityX = 0;
			}
			//If is not in the walk mode or is freezing, stop move
			if (enemyState != ENEMYSTATE.WALK || enemyEffect == ENEMYEFFECT.FREEZE)
				targetVelocityX = 0;

			if (isStopping || isStunning)
				targetVelocityX = 0;
			//Get the velocity X
			velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? 0.1f : 0.2f);
			//Get the velocity Y
			velocity.y += -gravity * Time.deltaTime;

			if ((_direction.x > 0 && controller.collisions.right) || (_direction.x < 0 && controller.collisions.left))
				velocity.x = 0;
			//Move the character controller
			controller.Move(velocity * Time.deltaTime * multipleSpeed, false, isFacingRight());

			if (controller.collisions.above || controller.collisions.below)
				velocity.y = 0;
			//Allow check attack when detected the target
			if (isPlaying && isPlayerDetected && allowCheckAttack && enemyEffect != ENEMYEFFECT.FREEZE)
			{
				CheckAttack();
			}
		}

		void Flip()
		{
			//Change the direction of moving
			_direction = -_direction;
			transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, isFacingRight() ? 0 : 180, transform.rotation.z));
		}

		public override void Stun(float time = 2)
		{
			//Call the stun action
			base.Stun(time);
			StartCoroutine(StunCo(time));
		}

		IEnumerator StunCo(float time)
		{
			//Set the animation to stun state and trigger the is stunning mode
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
			isStunning = false;
		}

		public override void DetectPlayer(float delayChase = 0)
		{
			base.DetectPlayer(delayChase);
		}

		public void CallMinion()
		{
			AnimSetTrigger("callMinion");
			SetEnemyState(ENEMYSTATE.ATTACK);
			allowCheckAttack = false;
		}

		void CheckAttack()
		{
			//CHECK AND CALL MINION IF THIS ENEMY HAS SCRIPT CALLMINION
			switch (attackType)
			{
				//if the attack is range
				case ATTACKTYPE.RANGE:
					if (rangeAttack.AllowAction())
					{
						//set the state is attacking
						SetEnemyState(ENEMYSTATE.ATTACK);
						//Try to shoot the bullet
						if (rangeAttack.CheckPlayer(isFacingRight()))
						{
							//trigger the attack time
							rangeAttack.Action();
							//set the trigger animation
							AnimSetTrigger("shoot");
							//call detect player in case the player is not detected yet
							DetectPlayer();
						}
						else if (!rangeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
						{
							//If can't attack, set enemy keep moving
							SetEnemyState(ENEMYSTATE.WALK);
						}
					}

					break;
				//if the attack is melee
				case ATTACKTYPE.MELEE:
					if (meleeAttack.AllowAction())
					{
						if (meleeAttack.CheckPlayer(isFacingRight()))
						{
							//set the state is attacking
							SetEnemyState(ENEMYSTATE.ATTACK);
							meleeAttack.Action();
							//set the trigger animation
							AnimSetTrigger("melee");
						}
						else if (!meleeAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
						{
							//If can't attack, set enemy keep moving
							SetEnemyState(ENEMYSTATE.WALK);
						}
					}
					break;
				//if the attack is throw
				case ATTACKTYPE.THROW:
					if (throwAttack.AllowAction())
					{
						SetEnemyState(ENEMYSTATE.ATTACK);
						//trigger the attack time
						if (throwAttack.CheckPlayer())
						{
							throwAttack.Action();
							//set the trigger animation
							AnimSetTrigger("throw");
						}
						else if (!throwAttack.isAttacking && enemyState == ENEMYSTATE.ATTACK)
						{
							//If can't attack, set enemy keep moving
							SetEnemyState(ENEMYSTATE.WALK);
						}
					}
					break;
				default:
					break;
			}
		}

		void AllowCheckAttack()
		{
			allowCheckAttack = true;
		}

		void HandleAnimation()
		{
			//update the animation
			AnimSetFloat("speed", Mathf.Abs(velocity.x));
		}

		public void SetForce(float x, float y)
		{
			velocity = new Vector3(x, y, 0);
		}
		// Event called by Animation
		public void AnimMeleeAttackStart()
		{
			meleeAttack.Check4Hit();
		}
		// Event called by Animation
		public void AnimMeleeAttackEnd()
		{
			meleeAttack.EndCheck4Hit();
		}
		// Event called by Animation
		public void AnimThrow()
		{
			throwAttack.Throw(isFacingRight());
		}
		// Event called by Animation
		public void AnimShoot()
		{
			rangeAttack.Shoot(isFacingRight());
		}

		public override void Die()
		{
			//stop if already in the dead mode
			if (isDead)
				return;

			base.Die();
			//trigger the dead state
			isDead = true;
			//cancel all the current invoke
			CancelInvoke();
			//disable all the collider 2D
			var cols = GetComponents<BoxCollider2D>();
			foreach (var col in cols)
				col.enabled = false;

			if (spawnItem && spawnItem.spawnWhenDie)
				spawnItem.Spawn();

			AnimSetBool("isDead", true);
			if (Random.Range(0, 2) == 1)
				AnimSetTrigger("die2");

			if (enemyEffect == ENEMYEFFECT.BURNING)
				return;
			//destroy the game object immediately if choose explosion
			if (enemyEffect == ENEMYEFFECT.EXPLOSION || dieBehavior == DIEBEHAVIOR.DESTROY)
			{
				gameObject.SetActive(false);
				return;
			}
			//Stop all the function if available
			StopAllCoroutines();
			StartCoroutine(DisableEnemy(AnimationHelper.getAnimationLength(anim, "Die") + 2f));
		}

		public override void Hit(Vector2 force, bool pushBack = false, bool knockDownRagdoll = false, bool shock = false)
		{
			//no action if enemy is not in the playing mode
			if (!isPlaying || isStunning)
				return;
			//call the hit event
			base.Hit(force, pushBack, knockDownRagdoll, shock);
			if (isDead)
				return;
			//trigger the hit animation
			AnimSetTrigger("hit");
			//try to spawn the item when get hit
			if (spawnItem && spawnItem.spawnWhenHit)
				spawnItem.Spawn();
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
	}
}