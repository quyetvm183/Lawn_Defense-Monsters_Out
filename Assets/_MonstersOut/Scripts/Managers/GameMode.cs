using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    public class GameMode : MonoBehaviour
    {
        public static GameMode Instance;
        public AudioClip soundReward;
        [Header("SHOP SETUP")]
        public int upgradeFortressPrice = 1000;
        [Space]
        public int watchVideoAdRewarded = 300;

        [Header("FPS DISPLAY")]
        //Set the resolution
        public Vector2 resolution = new Vector2(1280, 720);
        //Set FPS for the game
        public int setFPS = 60;
        public Purchaser purchase;

        public void BuyItem(int id)
        {
            //Buy the IAP items
            switch (id)
            {
                case 1:
                    purchase.BuyItem1();
                    break;
                case 2:
                    purchase.BuyItem2();
                    break;
                case 3:
                    purchase.BuyItem3();
                    break;
                case 4:
                    purchase.BuyRemoveAds();
                    break;
                default:
                    break;
            }
        }

        private void Awake()
        {
            Instance = this;
            //keep the gameobject alive
            DontDestroyOnLoad(gameObject);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        void Start()
        {
            //set the target frame rate for the game
            Application.targetFrameRate = setFPS;
        }

        public void ResetDATA()
        {
            //Delete and reset all the data
            PlayerPrefs.DeleteAll();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}