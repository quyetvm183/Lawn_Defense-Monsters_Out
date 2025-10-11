
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGame;

#if UNITY_ANDROID || UNITY_IOS
using GoogleMobileAds.Api;
#endif
using System;
namespace RGame
{
    public class AdmobController : MonoBehaviour
    {
        public static AdmobController Instance;
        //delegate   ()
        public delegate void RewardedAdResult(bool isWatched);

        //event  
        public static event RewardedAdResult AdResult;

        public bool useBanner = false;
#if UNITY_ANDROID || UNITY_IOS
        public AdPosition bannerPosition = AdPosition.Bottom;
#endif
        //public bool useInterstitial = true;

        [Header("ANDROID")]
        public string androidID;
        public string androidBanner;
        public string androidInters;
        public string androidVideo;

        [Header("IOS")]
        public string iosID;
        public string iosBanner;
        public string iosInters;
        public string iosVideo;
#if UNITY_ANDROID || UNITY_IOS
        private BannerView bannerView;
        private InterstitialAd interstitial;
        private RewardedAd rewardedAd;
#endif

        private void Awake()
        {
            if (AdmobController.Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
#if UNITY_ANDROID
            string appId = androidID;
#elif UNITY_IPHONE
                    string appId = iosID;
#else
                    string appId = "unexpected_platform";
#endif
#if UNITY_ANDROID || UNITY_IOS
            MobileAds.Initialize(initStatus => { });
            if (useBanner)
                RequestBanner();
            RequestInterstitial();
            RequestRewardedVideo();
#endif
        }

        #region BANNER

        private void RequestBanner()
        {
#if UNITY_ANDROID
            string appId = androidBanner;
#elif UNITY_IPHONE
        string appId = iosBanner;
#else
        string appId = "unexpected_platform";
#endif

#if UNITY_ANDROID || UNITY_IOS
            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(appId, AdSize.Banner, bannerPosition);
            // Create an empty ad request.
            //AdRequest request = new AdRequest.Builder().Build();
            var adRequest = new AdRequest();

            // Load the banner with the request.
            bannerView.LoadAd(adRequest);
            // Called when an ad request has successfully loaded.
            bannerView.OnBannerAdLoaded += BannerView_OnBannerAdLoaded; ;
#endif
        }

        private void BannerView_OnBannerAdLoaded()
        {
            ShowBanner(true);
        }

        public void ShowBanner(bool show)
        {
            if (useBanner)
            {
#if UNITY_ANDROID || UNITY_IOS
                if (show)
                    bannerView.Show();
                else
                    bannerView.Hide();
#endif
            }
        }

        #endregion

        #region INTERSTITIAL
        private void RequestInterstitial()
        {
#if UNITY_ANDROID
            string appId = androidInters;
#elif UNITY_IPHONE
            string appId = iosInters;
#else
            string appId = "unexpected_platform";
#endif
#if UNITY_ANDROID || UNITY_IOS
            //interstitial = new InterstitialAd(appId);

            //interstitial.OnAdOpening += HandleOnAdOpening;
            //interstitial.OnAdClosed += HandleOnAdClosed;

            //LoadInterstitial();
            // Clean up the old ad before loading a new one.
            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }

            LoadInterstitial();
            Debug.Log("Loading the interstitial ad.");


#endif
        }

        public void LoadInterstitial()
        {
#if UNITY_ANDROID
            string appId = androidInters;
#elif UNITY_IPHONE
            string appId = iosInters;
#else
            string appId = "unexpected_platform";
#endif

#if UNITY_ANDROID || UNITY_IOS
            //// Create an empty ad request.
            //AdRequest request = new AdRequest.Builder().Build();
            //// Load the interstitial with the request.
            //interstitial.LoadAd(request);

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            InterstitialAd.Load(appId, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("interstitial ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Interstitial ad loaded with response : "
                              + ad.GetResponseInfo());

                    interstitial = ad;
                });

            interstitial.OnAdFullScreenContentOpened += Interstitial_OnAdFullScreenContentOpened;
            interstitial.OnAdFullScreenContentClosed += Interstitial_OnAdFullScreenContentClosed;
#endif
        }

        private void Interstitial_OnAdFullScreenContentClosed()
        {
            LoadInterstitial();
            GameManager.Instance.isWatchingAd = false;
        }

        private void Interstitial_OnAdFullScreenContentOpened()
        {
            GameManager.Instance.isWatchingAd = true;
        }

        //public void HandleOnAdOpening(object sender, EventArgs args)
        //{
        //    GameManager.Instance.isWatchingAd = true;
        //}

        //public void HandleOnAdClosed(object sender, EventArgs args)
        //{
        //    LoadInterstitial();
        //    GameManager.Instance.isWatchingAd = false;
        //}


        public bool isInterstitialAdReady()
        {
#if UNITY_ANDROID || UNITY_IOS
            return interstitial.CanShowAd();
#else
        return false;
#endif
        }

        public bool ForceShowInterstitialAd()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (interstitial.CanShowAd())
            {
                interstitial.Show();
                return true;
            }
            else
                return false;

#else
        return false;
#endif
        }

        #endregion

        #region REWARDED VIDEO AD

        public bool isRewardedVideoAdReady()
        {
#if UNITY_ANDROID || UNITY_IOS
            return this.rewardedAd.CanShowAd();
#else
        return false;
#endif
        }

        public void WatchRewardedVideoAd()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (this.rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    // TODO: Reward the user.
                    Debug.Log(String.Format("Show rewarded", reward.Type, reward.Amount));
                });
            }
#endif
        }

        private void RequestRewardedVideo()
        {
#if UNITY_ANDROID
            string appId = androidVideo;
#elif UNITY_IPHONE
            string appId = iosVideo;
#else
            string appId = "unexpected_platform";
#endif
#if UNITY_ANDROID || UNITY_IOS
            //// Initialize an InterstitialAd.
            //this.rewardedAd = new RewardedAd(appId);
            //// Create an empty ad request.
            //AdRequest request = new AdRequest.Builder().Build();
            //// Load the rewarded ad with the request.
            //this.rewardedAd.LoadAd(request);

            //rewardedAd.OnAdOpening += HandleVideoOnAdOpening;
            //rewardedAd.OnAdClosed += HandleVideoOnAdClosed;
            //this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

            // Clean up the old ad before loading a new one.
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }

            Debug.Log("Loading the rewarded ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            RewardedAd.Load(appId, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("Rewarded ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("Rewarded ad loaded with response : "
                              + ad.GetResponseInfo());

                    rewardedAd = ad;

                    rewardedAd.OnAdFullScreenContentClosed += RewardedAd_OnAdFullScreenContentClosed1; ;
                    rewardedAd.OnAdPaid += RewardedAd_OnAdPaid;
                });



            LoadInterstitial();
#endif
        }
#if UNITY_ANDROID || UNITY_IOS
        private void RewardedAd_OnAdFullScreenContentClosed1()
        {
            //Debug.LogError("RewardedAd_OnAdFullScreenContentClosed1");
            RequestRewardedVideo();
            AdResult(true);
        }

        private void RewardedAd_OnAdPaid(AdValue obj)
        {
            Debug.LogError("RewardedAd_OnAdPaid");
            //AdResult(true);
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        //private void HandleUserEarnedReward(object sender, Reward e)
        //{
        //    AdResult(true);
        //}

        //private void RewardedAd_OnAdFullScreenContentClosed()
        //{
        //    RequestRewardedVideo();
        //}

        //private void HandleVideoOnAdOpening(object sender, EventArgs e)
        //{
        //}
#endif
        #endregion
    }
}