using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    [System.Serializable]
    public class EnemySpawn
    {
        //delay for first enemy
        public float wait = 3;
        //enemy spawned
        public GameObject enemy;
        //the number of enemy need spawned
        public int numberEnemy = 5;
        //time delay spawn next enemy
        public float rate = 1;
    }
}
