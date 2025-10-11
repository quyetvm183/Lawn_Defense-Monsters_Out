
/* 
 * 
 * Display the floating text in the game
 * 
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace RGame
{
	public class FloatingText : MonoBehaviour
	{
		//the text to show the message
		public Text floatingText;
		public int defaultFontSize = 18;
		Vector2 currentPos;


		public void SetText(string text, Color color, Vector2 worldPos, int fontSize = -1)
		{
			//Init the text parameters color and message
			floatingText.color = color;
			floatingText.text = text;
			currentPos = worldPos;

			if (fontSize != -1)
			{
				floatingText.fontSize = fontSize;
			}
			else
				floatingText.fontSize = defaultFontSize;

		}

		void Update()
		{
			//always stay the first position
			var _position = Camera.main.WorldToScreenPoint(currentPos);
			floatingText.transform.position = _position;
		}
	}
}