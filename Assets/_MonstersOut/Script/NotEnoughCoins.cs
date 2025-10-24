using UnityEngine;
using System.Collections;
namespace RGame
{
	public class NotEnoughCoins : MonoBehaviour
	{
		public static NotEnoughCoins Instance;
		public GameObject Panel;

		void Awake()
		{
			Instance = this;
		}

		// Use this for initialization
		void Start()
		{
			//Hide the panel on start
			Panel.SetActive(false);
		}

		public void ShowUp()
		{
			//Show the panel
			Panel.SetActive(true);
		}

		public void Close()
		{
			//Hide the panel
			Panel.SetActive(false);
		}
	}
}