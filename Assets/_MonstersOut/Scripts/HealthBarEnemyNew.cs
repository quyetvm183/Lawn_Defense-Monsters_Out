using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
	public class HealthBarEnemyNew : MonoBehaviour
	{
		//place the healthbar object
		public Transform healthBar;
		//when hit, show the bar and hide it by this value
		public float showTime = 1f;
		public float hideSpeed = 0.5f;
		//place the sprite object to scale it
		public SpriteRenderer backgroundImage;
		public SpriteRenderer barImage;

		Color oriBGImage, oriBarImage;

		Transform target;
		Vector3 offset;

		// Use this for initialization
		void Start()
		{
			//Setup the healthbar
			healthBar.localScale = new Vector2(1, healthBar.localScale.y);
			oriBGImage = backgroundImage.color;
			oriBarImage = barImage.color;

			//hide all
			backgroundImage.color = new Color(oriBGImage.r, oriBGImage.g, oriBGImage.b, 0);
			barImage.color = new Color(oriBarImage.r, oriBarImage.g, oriBarImage.b, 0);
		}

		public void Init(Transform _target, Vector3 _offset)
		{
			//set the new parameters for the healthbar
			target = _target;
			offset = _offset;
		}

		private void Update()
		{
			//if there is the target owner, follow him
			if (target)
			{
				transform.position = target.position + offset;
			}
		}

		public void UpdateValue(float value)
		{
			//Stop all the action
			StopAllCoroutines();
			CancelInvoke();
			//set the new update value
			backgroundImage.color = oriBGImage;
			barImage.color = oriBarImage;

			value = Mathf.Max(0, value);
			healthBar.localScale = new Vector2(value, healthBar.localScale.y);
			if (value > 0)
				Invoke("HideBar", showTime);
			else
				gameObject.SetActive(false);
		}

		private void HideBar()
		{
			//call the hide and show effect
			if (gameObject.activeInHierarchy)
			{
				StartCoroutine(RGFade.FadeSpriteRenderer(backgroundImage, hideSpeed, new Color(oriBGImage.r, oriBGImage.g, oriBGImage.b, 0)));
				StartCoroutine(RGFade.FadeSpriteRenderer(barImage, hideSpeed, new Color(oriBarImage.r, oriBarImage.g, oriBarImage.b, 0)));
			}
		}
	}
}