using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    public class LevelEnemyManager : MonoBehaviour, IListener
    {
        public static LevelEnemyManager Instance;
        public Vector2 spawnPosition;
        //set the enemy wave
        public EnemyWave[] EnemyWaves;
        int currentWave = 0;
        //the height Y to spawn the enemy randomly from -Y ; Y
        public float spawnHeightZone = 0.35f;
        //store the enemy list
        List<GameObject> listEnemySpawned = new List<GameObject>();

        private void Awake()
        {
            Instance = this;
        }

        int totalEnemy, currentSpawn;
        // Start is called before the first frame update
        void Start()
        {
            //Get the current level parameter
            if (GameLevelSetup.Instance)
                EnemyWaves = GameLevelSetup.Instance.GetLevelWave();

            //calculate number of enemies
            totalEnemy = 0;
            for (int i = 0; i < EnemyWaves.Length; i++)
            {
                //Counting all the enemy in the level
                for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
                {
                    var enemySpawn = EnemyWaves[i].enemySpawns[j];
                    for (int k = 0; k < enemySpawn.numberEnemy; k++)
                    {
                        totalEnemy++;
                    }
                }
            }

            currentSpawn = 0;
        }

        IEnumerator SpawnEnemyCo()
        {
            //Begin spawn all the enemy
            for (int i = 0; i < EnemyWaves.Length; i++)
            {
                //delay on the first enemy of the wave
                yield return new WaitForSeconds(EnemyWaves[i].wait);
                //Spawn all the enemy
                for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
                {
                    //Get the enemy spawn list
                    var enemySpawn = EnemyWaves[i].enemySpawns[j];
                    yield return new WaitForSeconds(enemySpawn.wait);
                    //begin spawn each enemy in the list
                    for (int k = 0; k < enemySpawn.numberEnemy; k++)
                    {
                        spawnPosition = transform.position + Vector3.up * Random.Range(-spawnHeightZone, spawnHeightZone);
                        //spawn the enemy then save it in the temp object then set the parent for it
                        GameObject _temp = Instantiate(enemySpawn.enemy, spawnPosition + Vector2.up * 0.1f, Quaternion.identity) as GameObject;
                        _temp.SetActive(false);
                        _temp.transform.parent = transform;
                        //delay 0.1s before active the enemy
                        yield return new WaitForSeconds(0.1f);
                        _temp.SetActive(true);
                        //_temp.transform.localPosition = Vector2.zero;
                        listEnemySpawned.Add(_temp);
                        //add the current spawn
                        currentSpawn++;
                        //update the enemy percent for the bar
                        MenuManager.Instance.UpdateEnemyWavePercent(currentSpawn, totalEnemy);

                        yield return new WaitForSeconds(enemySpawn.rate);
                    }
                }
            }

            //check all enemy killed
            while (isEnemyAlive()) { yield return new WaitForSeconds(0.1f); }
        }


        bool isEnemyAlive()
        {
            //check all the enemy in the list, if disactive or null
            for (int i = 0; i < listEnemySpawned.Count; i++)
            {
                if (listEnemySpawned[i].activeInHierarchy)
                    return true;
            }
            //if no any enemy available on the scene then go to victory
            return false;
        }

        public void IGameOver()
        {
        }

        public void IOnRespawn()
        {
        }

        public void IOnStopMovingOff()
        {
        }

        public void IOnStopMovingOn()
        {
        }

        public void IPause()
        {
        }

        public void IPlay()
        {
            StartCoroutine(SpawnEnemyCo());
        }

        public void ISuccess()
        {
            StopAllCoroutines();
        }

        public void IUnPause()
        {
        }
    }

    [System.Serializable]
    public class EnemyWave
    {
        //wait the delay time before spawn the next enemy
        public float wait = 3;
        //the list of the enemy need to spawn
        public EnemySpawn[] enemySpawns;
    }
}