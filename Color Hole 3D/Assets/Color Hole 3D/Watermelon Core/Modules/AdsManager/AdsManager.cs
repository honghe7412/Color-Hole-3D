#pragma warning disable 0649
#pragma warning disable 0162

#if MODULE_ADMOB
using GoogleMobileAds.Api;
#endif
#if MODULE_UNITYADS
using UnityEngine.Monetization;
using UnityEngine.Advertisements;
#endif
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Watermelon
{
    [Define("MODULE_ADMOB")]
    [Define("MODULE_UNITYADS")]
    public class AdsManager : MonoBehaviour
    {
        private static AdsManager instance;

        [SerializeField]
        private AdsSettings settings;
        public static AdsSettings Settings
        {
            get { return instance.settings; }
        }

#if MODULE_ADMOB
        private BannerView bannerView;
        private InterstitialAd interstitial;
        private RewardBasedVideoAd rewardBasedVideo;
#endif

#if MODULE_UNITYADS
        private ShowAdPlacementContent interstitialUnityAds;
        private ShowAdPlacementContent rewardedBasedVideoUnityAds;
#endif

        private RewardedVideoCallback rewardedVideoCallback;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (settings == null)
            {
                Debug.LogError("[AdMob]: Settings don't exist!");

                Destroy(this);
            }

#if MODULE_ADMOB
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(settings.GetAppID());

            // Get singleton reward based video ad reference.
            rewardBasedVideo = RewardBasedVideoAd.Instance;

            // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
            rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
            rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
            rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
            rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
            rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
            rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
#endif

#if MODULE_UNITYADS
            string unityAdsAppID = settings.GetUnityAdsAppID();

            Monetization.Initialize(unityAdsAppID, settings.testMode);
            Advertisement.Initialize(unityAdsAppID, settings.testMode);
#endif
        }

        public static void RequestInterstitial(InterstitialType interstitialType)
        {
            if (IsInterstitialLoaded(interstitialType))
                return;

            switch (interstitialType)
            {
                case InterstitialType.Interstitial_ADMOB:
#if MODULE_ADMOB
                    string adUnitId = instance.settings.GetInterstitialID();

                    // Clean up interstitial ad before creating a new one.
                    if (instance.interstitial != null)
                    {
                        instance.interstitial.Destroy();
                    }

                    // Create an interstitial.
                    instance.interstitial = new InterstitialAd(adUnitId);

                    // Register for ad events.
                    instance.interstitial.OnAdLoaded += instance.HandleInterstitialLoaded;
                    instance.interstitial.OnAdFailedToLoad += instance.HandleInterstitialFailedToLoad;
                    instance.interstitial.OnAdOpening += instance.HandleInterstitialOpened;
                    instance.interstitial.OnAdClosed += instance.HandleInterstitialClosed;
                    instance.interstitial.OnAdLeavingApplication += instance.HandleInterstitialLeftApplication;

                    // Load an interstitial ad.
                    instance.interstitial.LoadAd(instance.settings.CreateAdRequest());
#endif
                    break;
                case InterstitialType.Interstitial_UNITYADS:
#if MODULE_UNITYADS
                    instance.interstitialUnityAds = Monetization.GetPlacementContent(instance.settings.GetUnityAdsInterstitialID()) as ShowAdPlacementContent;
#endif
                    break;
            }
        }

        public static void RequestRewardBasedVideo(RewardedVideoType rewardedVideoType)
        {
            if (IsRewardBasedVideoLoaded(rewardedVideoType))
                return;

            switch (rewardedVideoType)
            {
                case RewardedVideoType.RewardedVideo_ADMOB:
#if MODULE_ADMOB
                    string adUnitId = instance.settings.GetRewardedVideoID();
        
                    instance.rewardBasedVideo.LoadAd(instance.settings.CreateAdRequest(), adUnitId);
#endif
                    break;
                case RewardedVideoType.RewardedVideo_UNITYADS:
#if MODULE_UNITYADS
                    instance.rewardedBasedVideoUnityAds = Monetization.GetPlacementContent(instance.settings.GetUnityAdsRewardedVideoID()) as ShowAdPlacementContent;
#endif
                    break;
            }
        }

        public static bool IsBannerLoaded(BannerType bannerType)
        {
            switch (bannerType)
            {
                case BannerType.Banner_ADMOB:
#if MODULE_ADMOB
                    return true;
#endif
                    break;
                case BannerType.Banner_UNITYADS:
#if MODULE_UNITYADS
                    return Advertisement.IsReady(instance.settings.GetUnityAdsBannerID());
#endif
                    break;
            }

            return false;
        }

        public static bool IsInterstitialLoaded(InterstitialType interstitialType)
        {
            switch (interstitialType)
            {
                case InterstitialType.Interstitial_ADMOB:
#if MODULE_ADMOB
                    return instance.interstitial != null && instance.interstitial.IsLoaded();
#endif
                    break;
                case InterstitialType.Interstitial_UNITYADS:
#if MODULE_UNITYADS
                    return instance.interstitialUnityAds != null && Monetization.IsReady(instance.settings.GetUnityAdsInterstitialID());
#endif
                    break;
            }

            return false;
        }

        public static bool IsRewardBasedVideoLoaded(RewardedVideoType rewardedVideoType)
        {
            switch (rewardedVideoType)
            {
                case RewardedVideoType.RewardedVideo_ADMOB:
#if MODULE_ADMOB
                    return instance.rewardBasedVideo != null && instance.rewardBasedVideo.IsLoaded();
#endif
                    break;
                case RewardedVideoType.RewardedVideo_UNITYADS:
#if MODULE_UNITYADS
                    return instance.rewardedBasedVideoUnityAds != null && Monetization.IsReady(instance.settings.GetUnityAdsRewardedVideoID());
#endif
                    break;
            }

            return false;
        }

        public static void ShowBanner(BannerType bannerType)
        {
            switch (bannerType)
            {
                case BannerType.Banner_ADMOB:
#if MODULE_ADMOB
                    string adUnitId = instance.settings.GetBannerID();

                    // Clean up banner ad before creating a new one.
                    if (instance.bannerView != null)
                    {
                        instance.bannerView.Destroy();
                    }

                    // Create a 320x50 banner at the top of the screen.
                    instance.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

                    // Register for ad events.
                    instance.bannerView.OnAdLoaded += instance.HandleAdLoaded;
                    instance.bannerView.OnAdFailedToLoad += instance.HandleAdFailedToLoad;
                    instance.bannerView.OnAdOpening += instance.HandleAdOpened;
                    instance.bannerView.OnAdClosed += instance.HandleAdClosed;
                    instance.bannerView.OnAdLeavingApplication += instance.HandleAdLeftApplication;

                    // Load a banner ad.
                    instance.bannerView.LoadAd(instance.settings.CreateAdRequest());
#endif
                    break;
                case BannerType.Banner_UNITYADS:
#if MODULE_UNITYADS
                    Advertisement.Banner.Show(instance.settings.GetUnityAdsBannerID());
#endif
                    break;
            }
        }

        public static void ShowInterstitial(InterstitialType interstitialType)
        {
            if (!IsInterstitialLoaded(interstitialType))
                return;

            switch (interstitialType)
            {
                case InterstitialType.Interstitial_ADMOB:
#if MODULE_ADMOB
                    instance.interstitial.Show();
#endif
                    break;
                case InterstitialType.Interstitial_UNITYADS:
#if MODULE_UNITYADS
                    instance.interstitialUnityAds.Show();
#endif
                    break;
            }
        }

        public static void ShowRewardBasedVideo(RewardedVideoType rewardedVideoType, RewardedVideoCallback callback)
        {
            if (!IsRewardBasedVideoLoaded(rewardedVideoType))
            {
                callback.Invoke(false);
                return;
            }

            if (instance.rewardedVideoCallback != null)
                instance.rewardedVideoCallback = null;

#if UNITY_EDITOR
            callback.Invoke(true);

            return;
#endif

            instance.rewardedVideoCallback = callback;

            switch (rewardedVideoType)
            {
                case RewardedVideoType.RewardedVideo_ADMOB:
#if MODULE_ADMOB
#if UNITY_EDITOR
                    Tween.NextFrame(delegate
                    {
                        callback.Invoke(true);
                    });
#endif

                    instance.rewardBasedVideo.Show();
#endif
                    break;
                case RewardedVideoType.RewardedVideo_UNITYADS:
#if MODULE_UNITYADS
                    ShowAdCallbacks options = new ShowAdCallbacks();
                    options.finishCallback = (UnityEngine.Monetization.ShowResult result) =>
                    {
                        if (result == UnityEngine.Monetization.ShowResult.Finished)
                        {
                            // Reward the player
                            if (instance.rewardedVideoCallback != null)
                            {
                                RewardedVideoCallback videoCallbackTemp = instance.rewardedVideoCallback;
                                Tween.NextFrame(delegate
                                {
                                    videoCallbackTemp.Invoke(true);

                                    PauseManager.SetPause(false);
                                });

                                instance.rewardedVideoCallback = null;
                            }
                        }
                        else
                        {
                            if (instance.rewardedVideoCallback != null)
                            {
                                RewardedVideoCallback videoCallbackTemp = instance.rewardedVideoCallback;

                                Tween.NextFrame(delegate
                                {
                                    videoCallbackTemp.Invoke(false);

                                    PauseManager.SetPause(false);
                                });

                                instance.rewardedVideoCallback = null;
                            }
                        }
                    };

                    instance.rewardedBasedVideoUnityAds.Show(options);
#endif
                    break;
            }
        }

        public static void DestroyBanner(BannerType bannerType)
        {
            switch (bannerType)
            {
                case BannerType.Banner_ADMOB:
#if MODULE_ADMOB
                    if (instance.bannerView != null)
                        instance.bannerView.Destroy();
#endif
                    break;
                case BannerType.Banner_UNITYADS:
#if MODULE_UNITYADS
                    Advertisement.Banner.Hide(true);
#endif
                    break;
            }
        }

        public static BannerType GetBannerType()
        {
            BannerType bannerType = instance.settings.bannerType;
            if (bannerType == BannerType.Banner_Random)
                bannerType = Random.Range(0, 1.0f) > 0.5f ? BannerType.Banner_ADMOB : BannerType.Banner_UNITYADS;

            return bannerType;
        }

        public static InterstitialType GetInterstitialType()
        {
            InterstitialType interstitialType = instance.settings.interstitialType;
            if (interstitialType == InterstitialType.Interstitial_Random)
                interstitialType = Random.Range(0, 1.0f) > 0.5f ? InterstitialType.Interstitial_ADMOB : InterstitialType.Interstitial_UNITYADS;

            return interstitialType;
        }

        public static RewardedVideoType GetRewardedVideoType()
        {
            RewardedVideoType rewardedVideoType = instance.settings.rewardedVideoType;
            if (rewardedVideoType == RewardedVideoType.RewardedVideo_Random)
                rewardedVideoType = Random.Range(0, 1.0f) > 0.5f ? RewardedVideoType.RewardedVideo_ADMOB : RewardedVideoType.RewardedVideo_UNITYADS;

            return rewardedVideoType;
        }

        #region Banner callback handlers
#if MODULE_ADMOB
    public void HandleAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleAdLoaded event received");
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {
        Debug.Log("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        Debug.Log("HandleAdLeftApplication event received");
    }
#endif
        #endregion

        #region Interstitial callback handlers
#if MODULE_ADMOB
    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialLoaded event received");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("HandleInterstitialFailedToLoad event received with message: " + args.Message);
    }

    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialOpened event received");
    }

    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialClosed event received");
    }

    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialLeftApplication event received");
    }
#endif
        #endregion

        #region RewardBasedVideo callback handlers
#if MODULE_ADMOB
    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        if (rewardedVideoCallback != null)
        {
            RewardedVideoCallback videoCallbackTemp = rewardedVideoCallback;

            Tween.NextFrame(delegate
            {
                videoCallbackTemp.Invoke(false);

                PauseManager.SetPause(false);
            });

            rewardedVideoCallback = null;
        }

        Debug.Log("HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        if (rewardedVideoCallback != null)
        {
            RewardedVideoCallback videoCallbackTemp = rewardedVideoCallback;
            Tween.NextFrame(delegate
            {
                videoCallbackTemp.Invoke(false);

                PauseManager.SetPause(false);
            });

            rewardedVideoCallback = null;
        }

        Debug.Log("HandleRewardBasedVideoClosed event received");
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        if (rewardedVideoCallback != null)
        {
            RewardedVideoCallback videoCallbackTemp = rewardedVideoCallback;
            Tween.NextFrame(delegate
            {
                videoCallbackTemp.Invoke(true);

                PauseManager.SetPause(false);
            });

            rewardedVideoCallback = null;
        }

        string type = args.Type;
        double amount = args.Amount;

        Debug.Log("HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardBasedVideoLeftApplication event received");
    }
#endif
        #endregion

        public delegate void RewardedVideoCallback(bool hasReward);
    }
}