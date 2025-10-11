/// <summary>
/// The UI Level, check the current level
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace RGame
{
    public class Level : MonoBehaviour
    {
        //set the world number
        public int world = 1;
        //set the level number
        public int level = 1;
        public bool isUnlock = false;
        public Text numberTxt;
        public GameObject imgLock, imgOpen, imgPass;
        //place the start group and star item
        public GameObject starGroup;
        public GameObject star1;
        public GameObject star2;
        public GameObject star3;

        public bool loadSceneManual = false;
        public string loadSceneName = "story1";

        // Use this for initialization
        void Start()
        {
            //Set the highest level
            if (level > GlobalValue.finishGameAtLevel)
            {
                GlobalValue.finishGameAtLevel = level;
            }
            //Show the level information
            numberTxt.text = level + "";
            //Level available if this level lower than the level pass + 1
            var openLevel = isUnlock ? true : GlobalValue.LevelPass + 1 >= level;
            //get the stars of the current level
            var stars = GlobalValue.LevelStar(level);
            //Check and show the collected star
            star1.SetActive(openLevel && stars >= 1);
            star2.SetActive(openLevel && stars >= 2);
            star3.SetActive(openLevel && stars >= 3);
            //Init the level state icon
            imgLock.SetActive(false);
            imgOpen.SetActive(false);
            imgPass.SetActive(false);
            starGroup.SetActive(false);

            if (openLevel)
            {
                //if this is the highest level, show the open icon
                if (GlobalValue.LevelPass + 1 == level)
                {
                    imgOpen.SetActive(true);
                    FindObjectOfType<MapControllerUI>().SetCurrentWorld(world);
                }
                else
                {
                    //if this level was passed, just how the pass icon
                    imgPass.SetActive(true);
                    starGroup.SetActive(true);

                }

            }
            else
            {
                //show the locked icon
                imgLock.SetActive(true);
                numberTxt.gameObject.SetActive(false);
            }
            //only allow press the button if the level can play
            GetComponent<Button>().interactable = openLevel;
        }

        public void Play()
        {
            //set the level and load the scene
            GlobalValue.levelPlaying = level;
            SoundManager.Click();

            MainMenuHomeScene.Instance.LoadScene();

        }

        public void Play(string _levelSceneName = null)
        {
            //set the level and load the scene
            SoundManager.Click();
            GlobalValue.levelPlaying = level;
            MainMenuHomeScene.Instance.LoadScene(_levelSceneName);
        }
    }
}