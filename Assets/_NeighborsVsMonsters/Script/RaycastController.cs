using UnityEngine;
using System.Collections;
namespace RGame
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class RaycastController : MonoBehaviour
	{
		//the layer can detect
		public LayerMask collisionMask;
		//set the skin width for the character
		public const float skinWidth = .015f;
		//the number of ray to throw
		public int horizontalRayCount = 4;
		public int verticalRayCount = 4;

		[HideInInspector]
		public float horizontalRaySpacing;
		[HideInInspector]
		public float verticalRaySpacing;

		//[HideInInspector]
		public BoxCollider2D boxcollider;
		public bool disableBoxColliderOnStart = false;
		public RaycastOrigins raycastOrigins;

		public virtual void Awake()
		{
			//try to get the box controller to able init the progress
			if (boxcollider == null)
				boxcollider = GetComponent<BoxCollider2D>();

			if (disableBoxColliderOnStart)
				boxcollider.enabled = false;
		}

		public virtual void OnEnable()
		{

		}

		public virtual void Start()
		{
			//begin caculating the ray
			CalculateRaySpacing();
		}

		public void UpdateRaycastOrigins()
		{
			//get the bonds of the collider then expand the size to -2
			Bounds bounds = boxcollider.bounds;
			bounds.Expand(skinWidth * -2);
			//caculating the raycast positions
			raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
			raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
			raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
			raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
		}

		public void CalculateRaySpacing()
		{
			//get the bonds of the collider then expand the size to -2
			Bounds bounds = boxcollider.bounds;
			bounds.Expand(skinWidth * -2);
			//get the number of the ray cast
			horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
			verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
			//get the space of 2 ray casts
			horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
			verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
		}

		public struct RaycastOrigins
		{
			public Vector2 topLeft, topRight;
			public Vector2 bottomLeft, bottomRight;
		}
	}
}