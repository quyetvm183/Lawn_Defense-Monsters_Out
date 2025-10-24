using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RGame
{
    public class ShopItemUpgrade : MonoBehaviour
    {
        public string itemName = "ITEM NAME";
        public string infor = "information for item";
        //the number of upgrade
        public int maxUpgrade = 5;
        public Image[] upgradeDots;
        public Sprite dotImageOn, dotImageOff;
        public Text nameTxt, inforTxt;
        [ReadOnly] public int coinPrice = 1;
        public Text coinTxt;
        public Button upgradeButton;
        [Header("Strong Wall")]
        public float StrongPerUpgrade = 0.2f;

        void Start()
        {
            if (GameMode.Instance)
            {
                coinPrice = GameMode.Instance.upgradeFortressPrice;
            }
            //Show the item information
            nameTxt.text = itemName;
            inforTxt.text = infor;
            coinTxt.text = coinPrice + "";
            //Update the item state
            UpdateStatus();
        }

        void UpdateStatus()
        {
            int currentUpgrade = GlobalValue.UpgradeStrongWall;
            //If the upgraded times reached to the limit, no allow it press again
            if (currentUpgrade >= maxUpgrade)
            {
                coinTxt.text = "MAX";
                upgradeButton.interactable = false;
                upgradeButton.GetComponent<Image>().enabled = false;
                SetDots(maxUpgrade);
            }
            else
            {
                //update the dots
                SetDots(currentUpgrade);
            }
        }

        void SetDots(int number)
        {
            //Set the number of dots that present for the upgrade times
            for (int i = 0; i < upgradeDots.Length; i++)
            {
                if (i < number)
                    upgradeDots[i].sprite = dotImageOn;
                else
                    upgradeDots[i].sprite = dotImageOff;

                if (i >= maxUpgrade)
                    upgradeDots[i].gameObject.SetActive(false);
            }
        }

        public void Upgrade()
        {
            //If the price is lower than the saved coins, do the upgrade
            if (GlobalValue.SavedCoins >= coinPrice)
            {
                //play sound and descrease the coins
                SoundManager.PlaySfx(SoundManager.Instance.soundUpgrade);
                GlobalValue.SavedCoins -= coinPrice;
                //upgrade the item
                GlobalValue.UpgradeStrongWall++;
                //upgrade the extra value of the item
                GlobalValue.StrongWallExtra += StrongPerUpgrade;

                UpdateStatus();
            }
            else
            {
                SoundManager.PlaySfx(SoundManager.Instance.soundNotEnoughCoin);
                if (AdsManager.Instance && AdsManager.Instance.isRewardedAdReady())
                    NotEnoughCoins.Instance.ShowUp();
            }
        }
    }
}