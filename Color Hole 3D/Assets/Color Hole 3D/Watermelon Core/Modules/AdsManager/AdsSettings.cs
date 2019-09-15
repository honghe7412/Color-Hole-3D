#if MODULE_ADMOB
using GoogleMobileAds.Api;
#endif
#if MODULE_UNITYADS
using UnityEngine.Monetization;
#endif
using UnityEngine;

namespace Watermelon
{
    [SetupTab("Advertising")]
    [CreateAssetMenu(fileName = "Ads Settings", menuName = "Content/Ads Settings")]
    public class AdsSettings : ScriptableObject
    {
        public BannerType bannerType;
        public InterstitialType interstitialType;
        public RewardedVideoType rewardedVideoType;

        //Unity Ads
        [LineSpacer("Application ID")]
        public string androidUnityAdsAppID = "1234567";
        public string IOSUnityAdsAppID = "1234567";

        //Banned ID
        [LineSpacer("Banner ID")]
        public string androidUnityAdsBannerID = "banner";
        public string IOSUnityAdsBannerID = "banner";

        //Interstitial ID
        [LineSpacer("Interstitial ID")]
        public string androidUnityAdsInterstitialID = "video";
        public string IOSUnityAdsInterstitialID = "video";

        //Rewarder Video ID
        [LineSpacer("Rewarded Video ID")]
        public string androidUnityAdsRewardedVideoID = "rewardedVideo";
        public string IOSUnityAdsRewardedVideoID = "rewardedVideo";

        //Application ID
        [LineSpacer("Application ID")]
        [ValidateInput("AppIDAndroidValidate")]
        public string androidAppID = "ca-app-pub-3940256099942544~3347511713";
        [ValidateInput("AppIDIOSValidate")]
        public string IOSAppID = "ca-app-pub-3940256099942544~1458002511";

        //Banned ID
        [LineSpacer("Banner ID")]
        public string androidBannerID = "ca-app-pub-3940256099942544/6300978111";
        public string IOSBannerID = "ca-app-pub-3940256099942544/2934735716";

        //Interstitial ID
        [LineSpacer("Interstitial ID")]
        public string androidInterstitialID = "ca-app-pub-3940256099942544/1033173712";
        public string IOSInterstitialID = "ca-app-pub-3940256099942544/4411468910";

        //Rewarder Video ID
        [LineSpacer("Rewarded Video ID")]
        public string androidRewardedVideoID = "ca-app-pub-3940256099942544/5224354917";
        public string IOSRewardedVideoID = "ca-app-pub-3940256099942544/1712485313";

        public bool testMode = false;


#if MODULE_ADMOB
        public string[] testDevices;
        public Gender targetGender = Gender.Unknown;
#endif

        public string GetUnityAdsAppID()
        {
#if UNITY_ANDROID
            return androidUnityAdsAppID;
#elif UNITY_IOS
            return IOSUnityAdsAppID;
#else
            return "unexpected_platform";
#endif
        }

        public string GetUnityAdsBannerID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return androidUnityAdsBannerID;
#elif UNITY_IOS
            return IOSUnityAdsBannerID;
#else
            return "unexpected_platform";
#endif
        }

        public string GetUnityAdsInterstitialID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return androidUnityAdsInterstitialID;
#elif UNITY_IOS
            return IOSUnityAdsInterstitialID;
#else
            return "unexpected_platform";
#endif
        }

        public string GetUnityAdsRewardedVideoID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return androidUnityAdsRewardedVideoID;
#elif UNITY_IOS
            return IOSUnityAdsRewardedVideoID;
#else
            return "unexpected_platform";
#endif
        }

#if MODULE_ADMOB
        public AdRequest CreateAdRequest()
        {
            AdRequest.Builder builder = new AdRequest.Builder();

            builder = builder.AddTestDevice(AdRequest.TestDeviceSimulator);
            for (int i = 0; i < testDevices.Length; i++)
            {
                builder = builder.AddTestDevice(testDevices[i]);
            }
            builder = builder.SetGender(targetGender);

            return builder.Build();
        }

        public string GetAppID()
        {
#if UNITY_ANDROID
            return androidAppID;
#elif UNITY_IOS
            return IOSAppID;
#else
            return "unexpected_platform";
#endif
        }

        public string GetBannerID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return androidBannerID;
#elif UNITY_IOS
            return IOSBannerID;
#else
            return "unexpected_platform";
#endif
        }

        public string GetInterstitialID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return androidInterstitialID;
#elif UNITY_IOS
            return IOSInterstitialID;
#else
            return "unexpected_platform";
#endif
        }

        public string GetRewardedVideoID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return androidRewardedVideoID;
#elif UNITY_IOS
            return IOSRewardedVideoID;
#else
            return "unexpected_platform";
#endif
        }
#endif
        
        #region Validate
        public ValidatorAttribute.ValidateResult AppIDAndroidValidate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new ValidatorAttribute.ValidateResult(ValidatorAttribute.ValidateType.Error, "App ID (Android) can't be empty!");
            }

            if (value == "ca-app-pub-3940256099942544~3347511713")
            {
                return new ValidatorAttribute.ValidateResult(ValidatorAttribute.ValidateType.Warning, "Change default App ID (Android).");
            }

            return new ValidatorAttribute.ValidateResult(ValidatorAttribute.ValidateType.Success, "App ID (Android) configured!");
        }

        public ValidatorAttribute.ValidateResult AppIDIOSValidate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new ValidatorAttribute.ValidateResult(ValidatorAttribute.ValidateType.Error, "App ID (iOS) can't be empty!");
            }

            if (value == "ca-app-pub-3940256099942544~1458002511")
            {
                return new ValidatorAttribute.ValidateResult(ValidatorAttribute.ValidateType.Warning, "Change default App ID (iOS).");
            }

            return new ValidatorAttribute.ValidateResult(ValidatorAttribute.ValidateType.Success, "App ID (iOS) configured!");
        }
        #endregion
    }

    public enum BannerPosition
    {
        Bottom = 0,
        Top = 1,
    }

    public enum BannerType
    {
        Banner_ADMOB = 0,
        Banner_UNITYADS = 1,
        Banner_Random = 2
    }

    public enum InterstitialType
    {
        Interstitial_ADMOB = 0,
        Interstitial_UNITYADS = 1,
        Interstitial_Random = 2,
    }

    public enum RewardedVideoType
    {
        RewardedVideo_ADMOB = 0,
        RewardedVideo_UNITYADS = 1,
        RewardedVideo_Random = 2,
    }
}