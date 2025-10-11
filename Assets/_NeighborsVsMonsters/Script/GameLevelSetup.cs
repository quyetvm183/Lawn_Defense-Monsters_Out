using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    public class GameLevelSetup : MonoBehaviour
    {
        public static GameLevelSetup Instance;

        [Header("===AUTO FILL MANA===")]
        //Auto add the mana after the rate
        public int amountMana = 2;
        public float rate = 2;
        //Save the level waves
        [ReadOnly] public List<LevelWave> levelWaves = new List<LevelWave>();

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //Set the finish level
            GlobalValue.finishGameAtLevel = levelWaves.Count;
        }

        public EnemyWave[] GetLevelWave()
        {
            //Find and return the level base on the level playing
            foreach (var obj in levelWaves)
            {
                if (obj.level == GlobalValue.levelPlaying)
                    return obj.Waves;
            }

            return null;
        }

        public int GetEnemyFortrestLevel()
        {
            //Return the fortress level health
            foreach (var obj in levelWaves)
            {
                if (obj.level == GlobalValue.levelPlaying)
                    return obj.enemyFortrestLevel;
            }

            return 1;
        }

        public int GetGivenMana()
        {
            //Get the given mana of the level
            foreach (var obj in levelWaves)
            {
                if (obj.level == GlobalValue.levelPlaying)
                    return obj.givenMana;
            }

            return -1;
        }

        public bool isFinalLevel()
        {
            //Check if the final level or not
            return GlobalValue.levelPlaying == levelWaves.Count;
        }

        private void OnDrawGizmos()
        {
            if (levelWaves.Count != transform.childCount)
            {
                var waves = transform.GetComponentsInChildren<LevelWave>();
                levelWaves = new List<LevelWave>(waves);

                for (int i = 0; i < levelWaves.Count; i++)
                {
                    levelWaves[i].level = i + 1;
                    levelWaves[i].gameObject.name = "Level " + levelWaves[i].level;
                }
            }
        }
    }
}