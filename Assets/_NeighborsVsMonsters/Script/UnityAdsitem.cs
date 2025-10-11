using UnityEngine;
using UnityEngine.UI;
namespace RGame
{
    public class UnityAdsitem : MonoBehaviour
    {
        //Place the button
        public GameObject but;
        public Text rewardedTxt;

        private void Update()
        {
            //Only show the button if there is the ads available
            but.SetActive(AdsManager.Instance && AdsManager.Instance.isRewardedAdReady());
            if (AdsManager.Instance)
            {
                //Check the Ads
                if (!AdsManager.Instance.isRewardedAdReady())
                    rewardedTxt.text = "NO AD AVAILABLE NOW!";
                else
                    rewardedTxt.text = "+" + AdsManager.Instance.getRewarded;
            }
            else
                rewardedTxt.text = "NO AD AVAILABLE NOW!";
        }

        public void WatchVideoAd()
        {
            //Watch ads and get the event
            if (AdsManager.Instance)
            {
                SoundManager.Click();
                AdsManager.AdResult += AdsManager_AdResult;
                AdsManager.Instance.ShowRewardedAds();
            }
        }

        private void AdsManager_AdResult(bool isSuccess, int rewarded)
        {
            //Check and reward to the player
            AdsManager.AdResult -= AdsManager_AdResult;
            if (isSuccess)
            {
                GlobalValue.SavedCoins += rewarded;
                SoundManager.PlaySfx(SoundManager.Instance.soundPurchased);
            }
        }
    }
}