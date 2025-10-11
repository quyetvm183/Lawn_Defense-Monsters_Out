using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
	public class FloatingTextManager : MonoBehaviour
	{
		public static FloatingTextManager Instance;
		[Header("Floating Text")]
		public GameObject FloatingText;

		// Use this for initialization
		void Awake()
		{
			Instance = this;
		}
		//Call this function with the own pare and position
		public void ShowText(FloatingTextParameter para, Vector2 ownerPosition)
		{
			if (FloatingText == null)
			{
				Debug.LogError("Need place FloatingText to GameManage object");
				return;
			}
			//Spawn the text object then place it to the world
			GameObject floatingText = Instantiate(FloatingText, Camera.main.WorldToScreenPoint(para.localTextOffset + ownerPosition), Quaternion.identity);
			var _position = Camera.main.WorldToScreenPoint(para.localTextOffset + ownerPosition);
			//Set the text object to the Menu Canvas
			floatingText.transform.SetParent(MenuManager.Instance.transform, false);
			floatingText.transform.position = _position;
			//Set the message for the text object
			var _FloatingText = floatingText.GetComponent<FloatingText>();
			_FloatingText.SetText(para.message, para.textColor, para.localTextOffset + ownerPosition);
			floatingText.SetActive(true);
		}

		public void ShowText(string message, Vector2 localOffset, Color textColor, Vector2 ownerPosition, int fontSize = -1)
		{
			if (FloatingText == null)
			{
				Debug.LogError("Need place FloatingText to GameManage object");
				return;
			}
			//Spawn the text object then place it to the world
			GameObject floatingText = Instantiate(FloatingText, Camera.main.WorldToScreenPoint(localOffset + ownerPosition), Quaternion.identity);
			var _position = Camera.main.WorldToScreenPoint(localOffset + ownerPosition);
			//Set the text object to the Menu Canvas
			floatingText.transform.SetParent(MenuManager.Instance.transform, false);
			floatingText.transform.position = _position;
			//Set the message for the text object
			var _FloatingText = floatingText.GetComponent<FloatingText>();
			_FloatingText.SetText(message, textColor, localOffset + ownerPosition, fontSize);
			floatingText.SetActive(true);
		}
	}

	[System.Serializable]
	public class FloatingTextParameter
	{
		[Header("Display Text Message")]
		public string message = "MESSAGE";
		public Vector2 localTextOffset = new Vector2(0, 1);
		public Color textColor = Color.yellow;
		[Tooltip("-1 mean no apply fontSize")]
		public int fontSize = -1;
	}
}