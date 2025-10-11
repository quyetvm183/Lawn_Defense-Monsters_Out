using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance;
        //The up and down of the Y axis to spawn the character betwwen this value
        public float spawnHeightZone = 0.35f;

        void Start()
        {
            Instance = this;
        }

        public GameObject SpawnCharacter(GameObject character)
        {
            //Spawn the character with the random Y position
            return Instantiate(character, transform.position + Vector3.up * Random.Range(-spawnHeightZone, spawnHeightZone), character.transform.rotation) as GameObject;
        }
    }
}