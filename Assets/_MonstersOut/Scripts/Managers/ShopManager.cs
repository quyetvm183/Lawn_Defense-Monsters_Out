using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RGame
{
    public class ShopManager : MonoBehaviour
    {
        //shop panels Upgrade, Coin and Items
        public GameObject[] shopPanels;
        public Sprite buttonActiveImage, buttonInActiveImage;
        public Image upgradeBut, buyCoinBut;
        void Start()
        {
            //Disable all the panel
            DisableObj();
            //active the first panel on start
            ActivePanel(shopPanels[0]);
            //active the first button
            SetActiveBut(0);
        }

        void DisableObj()
        {
            //Disable all the object in the shop panels
            foreach (var obj in shopPanels)
            {
                obj.SetActive(false);
            }
        }

        void ActivePanel(GameObject obj)
        {
            //active the obj
            obj.SetActive(true);
        }

        public void SwichPanel(GameObject obj)
        {
            for (int i = 0; i < shopPanels.Length; i++)
            {
                //Only active the obj in the shop panel list
                if (obj == shopPanels[i])
                {
                    DisableObj();
                    ActivePanel(shopPanels[i]);
                    SetActiveBut(i);

                    break;
                }
            }
            SoundManager.Click();
        }

        void SetActiveBut(int i)
        {
            //reset the button image
            upgradeBut.sprite = buttonInActiveImage;
            buyCoinBut.sprite = buttonInActiveImage;
            //set the active button with the active image
            switch (i)
            {
                case 0:
                    upgradeBut.sprite = buttonActiveImage;
                    break;
                case 1:
                    buyCoinBut.sprite = buttonActiveImage;
                    break;
                default:

                    break;
            }
        }
    }
}