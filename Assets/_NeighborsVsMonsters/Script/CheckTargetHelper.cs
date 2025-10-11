/*
 * 
 * Help detect the target ahead
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
	public class CheckTargetHelper : MonoBehaviour
	{
		//The distance to detect the target
		public float detectDistance = 5;
		//the width of the check lines
		public float width = 5;
		//the layer as target
		public LayerMask targetLayer;
		//the number of the line check
		public int numberLineCheck = 5;
		public Transform checkPoint;
		//the direction of checker
		int dir = 1;
		Vector3 limitUp;

		public bool CheckTarget(int direction = 1)
		{
			//get the check direction 1 is right, -1 is left
			dir = direction;
			//get the center point
			Vector3 center = checkPoint.position + (dir == 1 ? Vector3.right : Vector3.left) * detectDistance;
			limitUp = center + checkPoint.up * width * 0.5f;
			//get the distance of the checker
			float distance = 1f / (float)numberLineCheck;
			for (int i = 0; i <= numberLineCheck; i++)
			{
				//cast the line, if hit the target, return the true value
				RaycastHit2D hit = Physics2D.Linecast(checkPoint.position, limitUp - checkPoint.up * width * distance * i, targetLayer);
				if (hit)
					return true;
			}

			return false;
		}

		//call with new distance
		public bool CheckTargetManual(int direction, float customDistance)
		{
			//get the check direction 1 is right, -1 is left
			dir = direction;
			//get the center point
			Vector3 center = checkPoint.position + (dir == 1 ? Vector3.right : Vector3.left) * customDistance;
			limitUp = center + checkPoint.up * width * 0.5f;
			//get the distance of the checker
			float distance = 1f / (float)numberLineCheck;
			for (int i = 0; i <= numberLineCheck; i++)
			{
				//cast the line, if hit the target, return the true value
				RaycastHit2D hit = Physics2D.Linecast(checkPoint.position, limitUp - checkPoint.up * width * distance * i, targetLayer);
				if (hit)
					return true;
			}

			return false;
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.white;

			Vector3 center = checkPoint.position + (dir == 1 ? Vector3.right : Vector3.left) * detectDistance;
			limitUp = center + checkPoint.up * width * 0.5f;


			float distance = 1f / (float)numberLineCheck;
			for (int i = 0; i <= numberLineCheck; i++)
			{
				Gizmos.DrawLine(checkPoint.position, limitUp - checkPoint.up * width * distance * i);
			}
		}
	}
}