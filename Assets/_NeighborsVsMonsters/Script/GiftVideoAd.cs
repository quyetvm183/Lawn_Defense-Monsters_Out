using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RGame
{
    public class GiftVideoAd : MonoBehaviour
    {
        //Place the text information
        public Text rewardedTxt;
        public GameObject button;
        bool allowShow = true;
      
        void Start()
        {
            //Show the rewarded text
            if (GameMode.Instance)
            {
                rewardedTxt.text = AdsManager.Instance.getRewarded + "";
            }
        }

        void Update()
        {
            //Hide and Show the button when check Ads
            button.SetActive(allowShow && AdsManager.Instance && AdsManager.Instance.isRewardedAdReady());
        }

        public void WatchVideoAd()
        {
            //Play sound and show the ads
            SoundManager.Click();
            allowShow = false;
            AdsManager.AdResult += AdsManager_AdResult;
            AdsManager.Instance.ShowRewardedAds();
            Invoke("AllowShow", 2);
        }

        private void AdsManager_AdResult(bool isSuccess, int rewarded)
        {
            AdsManager.AdResult -= AdsManager_AdResult;
            //if ok then reward the user
            if (isSuccess)
            {
                GlobalValue.SavedCoins += rewarded;
                SoundManager.PlaySfx(SoundManager.Instance.soundPurchased);
            }
        }

        void AllowShow()
        {
            allowShow = true;
        }
    }
}