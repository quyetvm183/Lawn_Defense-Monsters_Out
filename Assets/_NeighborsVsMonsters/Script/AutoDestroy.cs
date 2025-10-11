using UnityEngine;
using System.Collections;
namespace RGame
{
	public class AutoDestroy : MonoBehaviour
	{
		public float time = 1;
		//only disble the object instead destroy it
		public bool onlyDisactivity = true;

		// Use this for initialization
		IEnumerator Start()
		{
			//Begin the auto destroy progress
			yield return new WaitForSeconds(time);
			if (onlyDisactivity)
				gameObject.SetActive(false);
			else
				Destroy(gameObject, time);
		}
	}
}