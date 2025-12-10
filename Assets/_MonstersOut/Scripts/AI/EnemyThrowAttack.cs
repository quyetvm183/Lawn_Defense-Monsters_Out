using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
	[AddComponentMenu("ADDP/Enemy AI/Throw Attack")]
	public class EnemyThrowAttack : MonoBehaviour
	{
		[Header("Grenade")]
		//the angle to throw the bomb
		public float angleThrow = 60;
		//how strong?
		public float throwForceMin = 290;
		public float throwForceMax = 320;
		//allow the object rotate
		public float addTorque = 100;
		public float throwRate = 0.5f;
		//throw the bomb at this position
		public Transform throwPosition;
		//the bomb prefab object
		public GameObject _Grenade;
		public AudioClip soundAttack;
		float lastShoot = -999;

		public LayerMask targetPlayer;
		public bool onlyAttackTheFortrest = true;
		public Transform checkPoint;
		public float radiusDetectPlayer = 5;
		public bool isAttacking { get; set; }

		public bool AllowAction()
		{
			//check the time rate for the next shoot
			return Time.time - lastShoot > throwRate;
		}

		public void Throw(bool isFacingRight)
		{
			//Get the throw position
			Vector3 throwPos = throwPosition.position;
			//Spawn the object
			GameObject obj = Instantiate(_Grenade, throwPos, Quaternion.identity) as GameObject;
			//Caculating the angle for shooting the object
			float angle;
			angle = isFacingRight ? angleThrow : 135;
			//set the new angle for the object
			obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
			//Get and set the force to the Rigidbody of the object
			obj.GetComponent<Rigidbody2D>().AddRelativeForce(obj.transform.right * Random.Range(throwForceMin, throwForceMax));
			obj.GetComponent<Rigidbody2D>().AddTorque(obj.transform.right.x * addTorque);

		}

		public bool CheckPlayer()
		{
			//Check if detect player to throw the object
			RaycastHit2D[] hits = Physics2D.CircleCastAll(checkPoint.position, radiusDetectPlayer, Vector2.zero, 0, targetPlayer);
			if (hits.Length > 0)
			{
				foreach (var hit in hits)
				{
					if (onlyAttackTheFortrest)
					{
						if (hit.collider.gameObject.GetComponent<TheFortrest>())
							return true;
					}
					else
						return true;

				}
			}
			return false;
		}

		public void Action()
		{
			if (_Grenade == null)
				return;
			//Save the last shoot time for the next attack
			lastShoot = Time.time;
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(checkPoint.position, radiusDetectPlayer);
		}
	}
}