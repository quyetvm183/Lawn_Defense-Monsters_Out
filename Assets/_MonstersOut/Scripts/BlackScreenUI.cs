using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RGame
{
	public class BlackScreenUI : MonoBehaviour
	{
		public static BlackScreenUI instance;
		CanvasGroup canvas;
		Image image;

		void Start()
		{
			//get the component
			instance = this;
			canvas = GetComponent<CanvasGroup>();
			image = GetComponent<Image>();
		}

		// Update is called once per frame
		public void Show(float timer, Color _color)
		{
			//show mean first show the alpha 0, then increase the alpha to 1 by call the RGFade
			image.color = _color;
			canvas.alpha = 0;
			StartCoroutine(RGFade.FadeCanvasGroup(GetComponent<CanvasGroup>(), timer, 1));
		}

		public void Show(float timer)
		{
			//show mean first show the alpha 0, then increase the alpha to 1 by call the RGFade
			image.color = Color.black;
			canvas.alpha = 0;
			StartCoroutine(RGFade.FadeCanvasGroup(GetComponent<CanvasGroup>(), timer, 1));
		}

		public void Hide(float timer)
		{
			//show mean first show the alpha 1, then increase the alpha to 0 by call the RGFade
			canvas.alpha = 1;
			StartCoroutine(RGFade.FadeCanvasGroup(GetComponent<CanvasGroup>(), timer, 0));
		}
	}
}