/// <summary>
/// Game manager. 
/// Handle all the actions, parameter of the game
/// You can easy get the state of the game with the IListener script.
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace RGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [HideInInspector]
        public bool isWatchingAd;
        //the status of the game
        public enum GameState { Menu, Playing, GameOver, Success, Pause };
        public GameState State { get; set; }
        [ReadOnly] public int levelStarGot;
        //SEt the layer for player, enemy and ground
        public LayerMask layerGround, layerEnemy, layerPlayer;
        [HideInInspector]
        public List<IListener> listeners;

        //add listener called by late actived object
        public void AddListener(IListener _listener)
        {
            //check if this added or not
            if (!listeners.Contains(_listener))
                listeners.Add(_listener);
        }
        //remove listener when Die or Disable
        public void RemoveListener(IListener _listener)
        {
            //check if this added or not
            if (listeners.Contains(_listener))
                listeners.Remove(_listener);
        }

        [Header("LEVELS")]
        public GameObject[] gameLevels;

        void Awake()
        {
            //set the game target frame rate
            Application.targetFrameRate = 60;
            Instance = this;
            //set the game state to Menu
            State = GameState.Menu;
            listeners = new List<IListener>();
            //Get and Spawn the level object
            if (GameMode.Instance == null)
                Instantiate(gameLevels[1], Vector2.zero, Quaternion.identity);
            else
                Instantiate(gameLevels[GlobalValue.levelPlaying - 1], Vector2.zero, Quaternion.identity);
        }

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            //Show the ad banner
            if (AdsManager.Instance)
                AdsManager.Instance.ShowAdmobBanner(false);
        }

        //called by MenuManager
        public void StartGame()
        {
            //change the state to Playing
            State = GameState.Playing;
            //Get all objects that have IListener
            var listener_ = FindObjectsOfType<MonoBehaviour>().OfType<IListener>();
            foreach (var _listener in listener_)
            {
                listeners.Add(_listener);
            }

            foreach (var item in listeners)
            {
                item.IPlay();
            }
        }

        public void Gamepause()
        {
            //Call the pause event
            State = GameState.Pause;
            foreach (var item in listeners)
                item.IPause();
        }

        public void UnPause()
        {
            //Call the UnPause event
            State = GameState.Playing;
            foreach (var item in listeners)
                item.IUnPause();
        }

        public void Victory()
        {
            if (State == GameState.Success)
                return;
            Time.timeScale = 1;
            SoundManager.Instance.PauseMusic(true);
            SoundManager.PlaySfx(SoundManager.Instance.soundVictory, 0.6f);
            //change the state to Finish
            State = GameState.Success;
            //Show the ads
            if (AdsManager.Instance)
            {
                AdsManager.Instance.ShowAdmobBanner(true);
                AdsManager.Instance.ShowNormalAd(State);
            }
            //Call the finish event
            foreach (var item in listeners)
            {
                if (item != null)
                    item.ISuccess();
            }

            //save level and save star
            if (GlobalValue.levelPlaying > GlobalValue.LevelPass)
                GlobalValue.LevelPass = GlobalValue.levelPlaying;
        }

        private void OnDisable()
        {
            if (State == GameState.Success)
                GlobalValue.LevelStar(GlobalValue.levelPlaying, levelStarGot);
        }

        public void GameOver()
        {
            Time.timeScale = 1;
            //Stop music
            SoundManager.Instance.PauseMusic(true);
            if (State == GameState.GameOver)
                return;
            //Set the gameover state
            State = GameState.GameOver;
            //Show ads
            if (AdsManager.Instance)
            {
                AdsManager.Instance.ShowAdmobBanner(true);
                AdsManager.Instance.ShowNormalAd(State);
            }
            //Call the GameOver event
            foreach (var item in listeners)
                item.IGameOver();
        }

        [HideInInspector]
        public List<GameObject> enemyAlives;
        [HideInInspector]
        public List<GameObject> listEnemyChasingPlayer;

        public void RigisterEnemy(GameObject obj)
        {
            //Add the enemy to the list
            enemyAlives.Add(obj);
        }

        public void RemoveEnemy(GameObject obj)
        {
            //Remove the enemy to the list
            enemyAlives.Remove(obj);
        }

        public int EnemyAlive()
        {
            //Check the enemy alive or not
            return enemyAlives.Count;
        }
    }
}