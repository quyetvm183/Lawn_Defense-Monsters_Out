using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    public class LevelWave : MonoBehaviour
    {
        //Set the level number
        public int level = 1;
        //The default mane for the level
        public int givenMana = 1000;
        [Range(1, 5)]
        public int enemyFortrestLevel = 1;
        //The enemy list
        public EnemyWave[] Waves;
    }
}