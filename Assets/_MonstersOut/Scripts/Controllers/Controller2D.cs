using UnityEngine;
using System.Collections;
namespace RGame
{
	public class Controller2D : RaycastController
	{
		//max angle that the character can move
		float maxClimbAngle = 80;
		//max angle that the character can move down
		float maxDescendAngle = 80;
		//Distance to check the ground
		float checkGroundAheadLength = 0.35f;

		public CollisionInfo collisions;
		[HideInInspector]
		public Vector2 playerInput;
		[HideInInspector]
		public bool HandlePhysic = true;
		[HideInInspector]
		public bool inverseGravity = false;

		private bool isFacingRight;
		bool ignoreCheckGroundAhead = true;

		public override void Start()
		{
			base.Start();
			//Set the default facing to the right
			collisions.faceDir = 1;
		}

		public void Move(Vector3 velocity, bool standingOnPlatform, bool _isFacingRight = false, bool _ignoreCheckGroundAhead = true)
		{
			//Move action, called by the character with the parameters
			isFacingRight = _isFacingRight;
			ignoreCheckGroundAhead = _ignoreCheckGroundAhead;
			Move(velocity, Vector2.zero, standingOnPlatform);
		}

		public void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform = false)
		{
			//Caculating the ray to detect the things
			CalculateRaySpacing();
			UpdateRaycastOrigins();
			collisions.Reset();
			collisions.velocityOld = velocity;
			playerInput = input;
			//set the facing direction
			if (velocity.x != 0)
			{
				collisions.faceDir = (int)Mathf.Sign(velocity.x);
			}
			//if the velocity Y lower than 0, meaning the character is moving down the slope
			if (velocity.y < 0)
			{
				DescendSlope(ref velocity);
			}
			//if allow handle the physic then caculating the moving speed
			if (HandlePhysic)
			{
				HorizontalCollisions(ref velocity);
				if (velocity.y != 0)
				{
					VerticalCollisions(ref velocity);
				}
			}

			CheckGroundedAhead(velocity);
			//move the character
			transform.Translate(velocity, Space.World);
			//trigger the standing state
			if (standingOnPlatform)
			{
				collisions.below = true;
			}
		}

		[ReadOnly] public bool isWall;

		void HorizontalCollisions(ref Vector3 velocity)
		{
			//init the direction and the length of the ray
			float directionX = collisions.faceDir;
			float rayLength = Mathf.Abs(velocity.x) + skinWidth;
			//limit the length
			if (Mathf.Abs(velocity.x) < skinWidth)
			{
				rayLength = 5 * skinWidth;
			}

			isWall = false;
			//test all the ray of the horizontal
			for (int i = 0; i < horizontalRayCount; i++)
			{
				Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
				rayOrigin += (Vector2)transform.up * (horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

				Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
				//check the hit object
				if (hit)
				{

					if (hit.distance == 0)
					{
						continue;
					}
					//calculating the slope angle
					float slopeAngle = Vector2.Angle(hit.normal, transform.up);

					if (slopeAngle > 85 && slopeAngle < 95)
						isWall = true;
					//check if the character move up or down the slope
					if (i == 0 && slopeAngle <= maxClimbAngle)
					{
						if (collisions.descendingSlope)
						{
							collisions.descendingSlope = false;
							velocity = collisions.velocityOld;
						}
						float distanceToSlopeStart = 0;
						if (slopeAngle != collisions.slopeAngleOld)
						{
							distanceToSlopeStart = hit.distance - skinWidth;
							velocity.x -= distanceToSlopeStart * directionX;
						}
						//set the new velocity X for the character
						ClimbSlope(ref velocity, slopeAngle);
						velocity.x += distanceToSlopeStart * directionX;
					}
					//if can't move over the slope then tell that is the wall
					if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
					{
						velocity.x = (hit.distance - skinWidth) * directionX;
						rayLength = hit.distance;

						if (collisions.climbingSlope)
						{
							velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
						}

						collisions.left = directionX == -1;
						collisions.right = directionX == 1;

						collisions.ClosestHit = hit;
					}
				}
			}
		}

		void VerticalCollisions(ref Vector3 velocity)
		{
			//init the direction and the length of the ray
			float directionY = Mathf.Sign(velocity.y);
			float rayLength = Mathf.Abs(velocity.y) + skinWidth;

			for (int i = 0; i < verticalRayCount; i++)
			{

				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += (Vector2)Vector2.right * (verticalRaySpacing * i + velocity.x);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

				Debug.DrawRay(rayOrigin, transform.up * directionY * rayLength, Color.red);
				//check the hit object
				if (hit)
				{
					//if contact with the through tag and holding the down button, ignore it
					if (hit.collider.tag == "Through")
					{
						if (directionY == (inverseGravity ? -1 : 1) || hit.distance == 0)
						{
							continue;
						}

						//if character is falling down, ignore it
						if (collisions.fallingThroughPlatform)
						{
							continue;
						}
						//if contact with the through tag and holding the down button, ignore it
						if (playerInput.y == -1)
						{
							collisions.fallingThroughPlatform = true;
							Invoke("ResetFallingThroughPlatform", .2f);
							continue;
						}
					}
					//caculating the new velocity Y
					velocity.y = (hit.distance - skinWidth) * directionY;
					rayLength = hit.distance;

					if (collisions.climbingSlope)
					{
						velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
					}
					//set contact to the obstacle
					if (!inverseGravity)
					{
						collisions.below = directionY == -1;
						collisions.above = directionY == 1;
					}
					else
					{
						collisions.below = directionY == 1;
						collisions.above = directionY == -1;
					}

					collisions.ClosestHit = hit;
					collisions.hitBelowObj = null;
					collisions.hitAboveObj = null;

					if (directionY == -1)
						collisions.hitBelowObj = hit.collider.gameObject;

					if (directionY == 1)
						collisions.hitAboveObj = hit.collider.gameObject;
				}
			}
			//if climbing the slope
			if (collisions.climbingSlope)
			{
				//init the value
				float directionX = Mathf.Sign(velocity.x);
				rayLength = Mathf.Abs(velocity.x) + skinWidth;
				Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + (Vector2)transform.up * velocity.y;
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, transform.right * directionX, rayLength, collisionMask);
				//check the hit object
				if (hit)
				{
					//check the slope angle then allow the character move over the slope or not
					float slopeAngle = Vector2.Angle(hit.normal, transform.up);
					if (slopeAngle != collisions.slopeAngle)
					{
						velocity.x = (hit.distance - skinWidth) * directionX;
						collisions.slopeAngle = slopeAngle;
					}
				}
			}
		}

		bool CheckGroundedAhead(Vector3 velocity)
		{
			//get the direction
			float directionX = collisions.faceDir;
			//get the ray cast
			if (velocity.x == 0)
				directionX = isFacingRight ? 1 : -1;

			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			//try to hit the target
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, checkGroundAheadLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.down * checkGroundAheadLength, Color.green);
			//if hit the target and the hit object is not the owner then set the ground is true
			if (hit)
			{
				collisions.isGrounedAhead = true;
				return true;
			}
			else
				return false;
		}

		void ClimbSlope(ref Vector3 velocity, float slopeAngle)
		{
			//Caculating the slope speed then return the new velocity
			float moveDistance = Mathf.Abs(velocity.x);
			float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

			if (velocity.y <= climbVelocityY)
			{
				velocity.y = climbVelocityY;
				velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
				collisions.below = true;
				collisions.climbingSlope = true;
				collisions.slopeAngle = slopeAngle;
			}
		}

		void DescendSlope(ref Vector3 velocity)
		{
			//Caculating the slope speed then return the new velocity
			float directionX = Mathf.Sign(velocity.x);
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
			//if hit any target
			if (hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
				{
					if (Mathf.Sign(hit.normal.x) == directionX)
					{
						if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
						{
							float moveDistance = Mathf.Abs(velocity.x);
							float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
							velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
							velocity.y -= descendVelocityY;

							collisions.slopeAngle = slopeAngle;
							collisions.descendingSlope = true;
							collisions.below = true;
						}
					}
				}
			}
		}

		void ResetFallingThroughPlatform()
		{
			collisions.fallingThroughPlatform = false;
		}

		public struct CollisionInfo
		{
			public bool above, below;
			public bool left, right;

			public RaycastHit2D ClosestHit;
			public GameObject hitBelowObj, hitAboveObj;

			public bool isGrounedAhead;

			public bool climbingSlope;
			public bool descendingSlope;
			public float slopeAngle, slopeAngleOld;
			public Vector3 velocityOld;
			public int faceDir;
			public bool fallingThroughPlatform;

			public void Reset()
			{
				above = below = false;
				left = right = false;
				isGrounedAhead = false;
				climbingSlope = false;
				descendingSlope = false;

				slopeAngleOld = slopeAngle;
				slopeAngle = 0;
			}
		}
	}
}