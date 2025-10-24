using UnityEngine;
using System.Collections;
namespace RGame
{
	public class SimpleProjectile : Projectile, ICanTakeDamage, IListener
	{
		//set damage for the projectile
		public int Damage = 30;
		//set the destroy object
		public GameObject DestroyEffect;
		public int pointToGivePlayer = 100;
		//time to alive
		public float timeToLive = 3;
		public Sprite newBulletImage;
		//Set the sound and volume for the object
		public AudioClip soundHitEnemy;
		[Range(0, 1)]
		public float soundHitEnemyVolume = 0.5f;
		public AudioClip soundHitNothing;
		[Range(0, 1)]
		public float soundHitNothingVolume = 0.5f;

		public GameObject ExplosionObj;
		private SpriteRenderer rend;

		public GameObject NormalFX;
		public GameObject DartFX;
		public GameObject destroyParent;
		float timeToLiveCounter = 0;
		void OnEnable()
		{
			//init the live time of the object
			timeToLiveCounter = timeToLive;
		}
		void Start()
		{
			if (Explosion)
			{
				rend = GetComponent<SpriteRenderer>();
			}
			if (NormalFX)
				NormalFX.SetActive(!Explosion);
			if (DartFX)
				DartFX.SetActive(Explosion);
			//rigister the listener
			GameManager.Instance.listeners.Add(this);
		}

		bool comeBackToPlayer = false;
		void Update()
		{
			if (isStop)
				return;
			//try to destroy the parent if it was set
			if (destroyParent == null)
				destroyParent = gameObject;
			//Destroy the object if the time is run out
			if ((timeToLiveCounter -= Time.deltaTime) <= 0)
			{

				if (Explosion && CanGoBackOwner)
					comeBackToPlayer = true;
				else
					DestroyProjectile();
			}
			//The object can come back to the player if it was set
			if (comeBackToPlayer)
			{
				Vector3 comebackto = Owner.transform.position;
				destroyParent.transform.position = Vector2.MoveTowards(destroyParent.transform.position, comebackto, Speed * Time.deltaTime);
				if (Vector2.Distance(transform.position, comebackto) < 0.26f)
					(destroyParent != null ? destroyParent : gameObject).SetActive(false);
			}
			else
			{
				//Move the object by the speed
				transform.Translate((Direction + new Vector2(InitialVelocity.x, 0)) * Speed * Time.deltaTime, Space.World);
			}
		}

		void DestroyProjectile()
		{
			//Spawn the destroy effect then destroy the object
			if (DestroyEffect != null)
				Instantiate(DestroyEffect, transform.position, Quaternion.identity);

			 (destroyParent != null ? destroyParent : gameObject).SetActive(false);
		}


		public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null)
		{
			//Call sound and destroy the object
			SoundManager.PlaySfx(soundHitNothing, soundHitNothingVolume);
			DestroyProjectile();
		}

		protected override void OnCollideOther(Collider2D other)
		{
			SoundManager.PlaySfx(soundHitNothing, soundHitNothingVolume);
			DestroyProjectile();
		}

		protected override void OnCollideTakeDamage(Collider2D other, ICanTakeDamage takedamage)
		{
			//Deal the damage to the target with the Damage or New Damage
			takedamage.TakeDamage((NewDamage == 0 ? Damage : NewDamage), Vector2.zero, transform.position, Owner, BODYPART.NONE, weaponEffect);
			SoundManager.PlaySfx(soundHitEnemy, soundHitEnemyVolume);
			DestroyProjectile();
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
		#endregion
	}

}