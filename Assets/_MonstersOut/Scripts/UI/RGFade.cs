using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
namespace RGame
{
	public static class RGFade
	{
		public static IEnumerator FadeImage(Image target, float duration, Color color)
		{
			if (target == null)
				yield break;
			//Get the alpha color
			float alpha = target.color.a;
			//Init the time counter and make the fade effect with the duration value
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
			{
				if (target == null)
					yield break;
				//caculating the color then add it to the target
				Color newColor = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
				target.color = newColor;
				yield return null;
			}
			target.color = color;

		}
		public static IEnumerator FadeText(Text target, float duration, Color color)
		{
			if (target == null)
				yield break;
			//Get the alpha color
			float alpha = target.color.a;
			//Init the time counter and make the fade effect with the duration value
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
			{
				if (target == null)
					yield break;
				//caculating the color then add it to the target
				Color newColor = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
				target.color = newColor;
				yield return null;
			}
			target.color = color;
		}
		public static IEnumerator FadeSprite(SpriteRenderer target, float duration, Color color)
		{
			if (target == null)
				yield break;
			//Get the alpha color
			float alpha = target.material.color.a;
			//Init the time counter and make the fade effect with the duration value
			float t = 0f;
			while (t < 1.0f)
			{
				if (target == null)
					yield break;
				//caculating the color then add it to the target
				Color newColor = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
				target.material.color = newColor;
				//counting the timer
				t += Time.deltaTime / duration;

				yield return null;

			}
			//when it's done, set the final color
			Color finalColor = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
			target.material.color = finalColor;
		}

		public static IEnumerator FadeSpriteRenderer(SpriteRenderer target, float duration, Color color)
		{
			if (target == null)
				yield break;
			//Get the alpha value
			float alpha = target.color.a;
			//Init the time counter and make the fade effect with the duration value
			float t = 0f;
			while (t < 1.0f)
			{
				if (target == null)
					yield break;

				Color newColor = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
				target.color = newColor;

				t += Time.deltaTime / duration;

				yield return null;

			}
			Color finalColor = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
			target.color = finalColor;
		}

		public static IEnumerator FadeTexture(Material target, float duration, Color color)
		{
			if (target == null)
				yield break;
			//Get the alpha color
			float alpha = target.color.a;
			float r = target.color.r;
			float g = target.color.g;
			float b = target.color.b;

			float t = 0f;
			//Init the time counter and make the fade effect with the duration value
			while (t < 1.0f)
			{
				if (target == null)
					yield break;

				Color newColor = new Color(Mathf.SmoothStep(r, color.r, t), Mathf.SmoothStep(g, color.g, t), Mathf.SmoothStep(b, color.b, t), Mathf.SmoothStep(alpha, color.a, t));
				target.color = newColor;

				t += Time.deltaTime / duration;

				yield return null;

			}
			//when it's done, set the final color
			Color finalColor = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
			target.color = finalColor;
		}

		public static IEnumerator FadeCanvasGroup(CanvasGroup target, float duration, float targetAlpha)
		{
			if (target == null)
				yield break;
			//Get the alpha value
			float currentAlpha = target.alpha;
			//Init the time counter and make the fade effect with the duration value
			float t = 0f;
			while (t < 1.0f)
			{
				if (target == null)
					yield break;
				//caculating the alpha then add it to the target
				float newAlpha = Mathf.SmoothStep(currentAlpha, targetAlpha, t);
				target.alpha = newAlpha;

				t += Time.deltaTime / duration;

				yield return null;

			}
			//when it's done, set the final alpha
			target.alpha = targetAlpha;
		}
	}
}