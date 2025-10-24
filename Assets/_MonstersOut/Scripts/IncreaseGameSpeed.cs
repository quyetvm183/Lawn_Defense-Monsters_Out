using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RGame
{
    public class IncreaseGameSpeed : MonoBehaviour
    {
        //Set the speed of the game
        public float timeSpeedUp = 2;
        public GameObject blinkingObj;
        public Text speedTxt;
        //Place the helper object to show to the user
        public GameObject helperObj;
        private void Start()
        {
            //Init the value
            speedTxt.text = "Speed x1";
            helperObj.SetActive(false);
            //Check every 10 seconds
            Invoke("ShowHelper", 10);
        }

        void ShowHelper()
        {
            //Check if allow show the helper or not
            if (PlayerPrefs.GetInt("IncreaseGameSpeedDontShowAgain", 0) == 0)
                helperObj.SetActive(true);
        }

        public void ChangeSpeed()
        {
            //If time == 1 then set it to the new speed
            if (Time.timeScale == 1)
            {
                Time.timeScale = timeSpeedUp;
                //Action the blinking
                StartCoroutine(BlinkingCo());
                speedTxt.text = "Speed x" + timeSpeedUp;
                SoundManager.PlaySfx(SoundManager.Instance.soundTimeUp);
            }
            //If time == new speed then set it to the normal speed
            else
            {
                blinkingObj.SetActive(true);
                Time.timeScale = 1;
                StopAllCoroutines();
                speedTxt.text = "Speed x1";
                SoundManager.PlaySfx(SoundManager.Instance.soundTimeDown);
            }
            //No allow the tutorial again
            PlayerPrefs.SetInt("IncreaseGameSpeedDontShowAgain", 1);
            helperObj.SetActive(false);
        }

        IEnumerator BlinkingCo()
        {
            while (true)
            {
                blinkingObj.SetActive(true);
                yield return new WaitForSeconds(0.8f);
                blinkingObj.SetActive(false);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}