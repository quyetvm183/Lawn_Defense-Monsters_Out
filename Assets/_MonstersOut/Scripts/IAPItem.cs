using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RGame
{
    public class IAPItem : MonoBehaviour
    {
        //the ID must unique
        public int ID = 1;
        public Text priceTxt;
        public Text rewardedTxt;

        private void Update()
        {
            if (GameMode.Instance)
            {
                //Check the ID 1,2,3,... then get the price of the item in the Gamemode
                switch (ID)
                {
                    case 1:
                        priceTxt.text = "$" + GameMode.Instance.purchase.price1;
                        rewardedTxt.text = "+" + GameMode.Instance.purchase.reward1 + "";
                        break;
                    case 2:
                        priceTxt.text = "$" + GameMode.Instance.purchase.price2;
                        rewardedTxt.text = "+" + GameMode.Instance.purchase.reward2;
                        break;
                    case 3:
                        priceTxt.text = "$" + GameMode.Instance.purchase.price3;
                        rewardedTxt.text = "+" + GameMode.Instance.purchase.reward3 + "";
                        break;
                    case 4:
                        priceTxt.text = "$" + GameMode.Instance.purchase.removeAdsPrice;

                        if (GlobalValue.RemoveAds)
                        {
                            gameObject.SetActive(false);
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        public void Buy()
        {
            //Try to buy the IAP item
            SoundManager.Click();
            GameMode.Instance.BuyItem(ID);
        }
    }
}