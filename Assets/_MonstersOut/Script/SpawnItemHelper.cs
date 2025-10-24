using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
	public class SpawnItemHelper : MonoBehaviour
	{
		//allow spawn  the item when get hit
		public bool spawnWhenHit = false;
		//allow spawn  the item when get die
		public bool spawnWhenDie = true;
		//set the chance to spawn the item
		[Range(0, 1)]
		public float chanceSpawn = 0.5f;
		public GameObject[] Items;
		public Transform spawnPoint;

		void Start()
		{
			//init the spawnpoint if there are no spawnpoint was placed
			if (spawnPoint == null)
				spawnPoint = transform;
		}

		public void Spawn()
		{
			//Spawn the item randomly from the list
			if (Items.Length > 0 && Random.Range(0f, chanceSpawn) < chanceSpawn)
			{
				Instantiate(Items[Random.Range(0, Items.Length)], spawnPoint.position, Quaternion.identity);
			}
		}
	}
}