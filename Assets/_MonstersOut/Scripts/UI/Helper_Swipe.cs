using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    public class Helper_Swipe : MonoBehaviour
    {
        float cameraLastPos;
        //Show the helper panel if after this time value the screen is not touch
        public float showHelperIfCameraIdle = 5;
        Transform cameraMain;
        float lastMoveTime = 0;
        public GameObject helperObj;
        bool isShown = false;

        private void Awake()
        {
            //Check if can show it or not
            if (PlayerPrefs.GetInt("DontShowAgain", 0) == 1)
            {
                helperObj.SetActive(false);
                Destroy(this);
            }
        }
        private void Start()
        {
            //Get the camera position and init the value
            cameraMain = Camera.main.transform;
            cameraLastPos = cameraMain.position.x;
            //Check it after 5 seconds
            InvokeRepeating("CheckingIdle", 5, 0.1f);
            lastMoveTime = Time.time;
            helperObj.SetActive(false);
        }
        void CheckingIdle()
        {
            //If the screen is not move then show the tutorial panel
            if (cameraLastPos != cameraMain.position.x)
            {
                cameraLastPos = cameraMain.position.x;
                lastMoveTime = Time.time;
                helperObj.SetActive(false);
            }
            else if (Time.time - lastMoveTime > showHelperIfCameraIdle)
            {
                //Show the helper UI
                if (!isShown)
                    helperObj.SetActive(true);
                isShown = true;
            }
        }

        public void DontShowAgain()
        {
            //User click dont show the helper again
            PlayerPrefs.SetInt("DontShowAgain", 1);
            helperObj.SetActive(false);
            Destroy(this);
        }
    }
}