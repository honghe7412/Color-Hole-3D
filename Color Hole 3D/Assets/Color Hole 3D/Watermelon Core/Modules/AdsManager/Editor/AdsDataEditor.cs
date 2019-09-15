using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    [CustomEditor(typeof(AdsSettings))]
    public class AdsDataEditor : Editor
    {
        private SerializedProperty bannerTypeProperty;
        private SerializedProperty interstitialTypeProperty;
        private SerializedProperty rewardedVideoTypeProperty;

        private SerializedProperty testModeProperty;

#if MODULE_ADMOB
        private SerializedProperty targetGenderProperty;
        private SerializedProperty testDevicesProperty;
#endif

        // AdMob
        private SerializedProperty androidAppIDProperty;
        private SerializedProperty IOSAppIDProperty;

        private SerializedProperty androidBannerIDProperty;
        private SerializedProperty IOSBannerIDProperty;

        private SerializedProperty androidInterstitialIDProperty;
        private SerializedProperty IOSInterstitialIDProperty;

        private SerializedProperty androidRewardedVideoIDProperty;
        private SerializedProperty IOSRewardedVideoIDProperty;

        // Unity Ads
        private SerializedProperty androidUnityAdsAppIDProperty;
        private SerializedProperty IOSUnityAdsAppIDProperty;

        private SerializedProperty androidUnityAdsBannerIDProperty;
        private SerializedProperty IOSUnityAdsBannerIDProperty;

        private SerializedProperty androidUnityAdsInterstitialIDProperty;
        private SerializedProperty IOSUnityAdsInterstitialIDProperty;

        private SerializedProperty androidUnityAdsRewardedVideoIDProperty;
        private SerializedProperty IOSUnityAdsRewardedVideoIDProperty;

        private bool showAdMob = false;
        private bool showUnityAds = false;

        private void OnEnable()
        {
            bannerTypeProperty = serializedObject.FindProperty("bannerType");
            interstitialTypeProperty = serializedObject.FindProperty("interstitialType");
            rewardedVideoTypeProperty = serializedObject.FindProperty("rewardedVideoType");

            testModeProperty = serializedObject.FindProperty("testMode");

#if MODULE_ADMOB
            targetGenderProperty = serializedObject.FindProperty("targetGender");
            testDevicesProperty = serializedObject.FindProperty("testDevices");
#endif

            // AdMob
            androidAppIDProperty = serializedObject.FindProperty("androidAppID");
            IOSAppIDProperty = serializedObject.FindProperty("IOSAppID");

            androidBannerIDProperty = serializedObject.FindProperty("androidBannerID");
            IOSBannerIDProperty = serializedObject.FindProperty("IOSBannerID");

            androidInterstitialIDProperty = serializedObject.FindProperty("androidInterstitialID");
            IOSInterstitialIDProperty = serializedObject.FindProperty("IOSInterstitialID");

            androidRewardedVideoIDProperty = serializedObject.FindProperty("androidRewardedVideoID");
            IOSRewardedVideoIDProperty = serializedObject.FindProperty("IOSRewardedVideoID");

            // Unity Ads
            androidUnityAdsAppIDProperty = serializedObject.FindProperty("androidUnityAdsAppID");
            IOSUnityAdsAppIDProperty = serializedObject.FindProperty("IOSUnityAdsAppID");

            androidUnityAdsBannerIDProperty = serializedObject.FindProperty("androidUnityAdsBannerID");
            IOSUnityAdsBannerIDProperty = serializedObject.FindProperty("IOSUnityAdsBannerID");

            androidUnityAdsInterstitialIDProperty = serializedObject.FindProperty("androidUnityAdsInterstitialID");
            IOSUnityAdsInterstitialIDProperty = serializedObject.FindProperty("IOSUnityAdsInterstitialID");

            androidUnityAdsRewardedVideoIDProperty = serializedObject.FindProperty("androidUnityAdsRewardedVideoID");
            IOSUnityAdsRewardedVideoIDProperty = serializedObject.FindProperty("IOSUnityAdsRewardedVideoID");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(bannerTypeProperty);
            EditorGUILayout.PropertyField(interstitialTypeProperty);
            EditorGUILayout.PropertyField(rewardedVideoTypeProperty);

            EditorGUILayout.LabelField(GUIContent.none, GUI.skin.horizontalSlider);

            EditorGUILayout.PropertyField(testModeProperty);

#if MODULE_ADMOB
            EditorGUILayout.PropertyField(targetGenderProperty);
            EditorGUILayout.PropertyField(testDevicesProperty);
#endif

            GUILayout.Space(12);

            if (GUILayout.Button("AdMob ↓", GUILayout.Height(20), GUILayout.ExpandWidth(true)))
            {
                showAdMob = !showAdMob;
            }

            if (showAdMob)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.PropertyField(androidAppIDProperty);
                EditorGUILayout.PropertyField(IOSAppIDProperty);
                EditorGUILayout.PropertyField(androidBannerIDProperty);
                EditorGUILayout.PropertyField(IOSBannerIDProperty);
                EditorGUILayout.PropertyField(androidInterstitialIDProperty);
                EditorGUILayout.PropertyField(IOSInterstitialIDProperty);
                EditorGUILayout.PropertyField(androidRewardedVideoIDProperty);
                EditorGUILayout.PropertyField(IOSRewardedVideoIDProperty);
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Unity Ads ↓", GUILayout.Height(20), GUILayout.ExpandWidth(true)))
            {
                showUnityAds = !showUnityAds;
            }

            if (showUnityAds)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.PropertyField(androidUnityAdsAppIDProperty);
                EditorGUILayout.PropertyField(IOSUnityAdsAppIDProperty);
                EditorGUILayout.PropertyField(androidUnityAdsBannerIDProperty);
                EditorGUILayout.PropertyField(IOSUnityAdsBannerIDProperty);
                EditorGUILayout.PropertyField(androidUnityAdsInterstitialIDProperty);
                EditorGUILayout.PropertyField(IOSUnityAdsInterstitialIDProperty);
                EditorGUILayout.PropertyField(androidUnityAdsRewardedVideoIDProperty);
                EditorGUILayout.PropertyField(IOSUnityAdsRewardedVideoIDProperty);
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}