using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class AdsManagerExampleScript : MonoBehaviour
    {
        private Vector2 scrollView;

        public Text text;

        private void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        private void Log(string condition, string stackTrace, LogType type)
        {
            text.text += condition + "\n";
        }

        private void OnGUI()
        {
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height / 2));
            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(40), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Show Banner", GUILayout.ExpandHeight(true)))
            {
                AdsManager.ShowBanner(BannerType.Banner_UNITYADS);
            }
            if (GUILayout.Button("Destroy Banner", GUILayout.ExpandHeight(true)))
            {
                AdsManager.DestroyBanner(BannerType.Banner_UNITYADS);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(40), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Request Interstitial", GUILayout.ExpandHeight(true)))
            {
                AdsManager.RequestInterstitial(InterstitialType.Interstitial_UNITYADS);
            }
            if (GUILayout.Button("Show Interstitial", GUILayout.ExpandHeight(true)))
            {
                AdsManager.ShowInterstitial(InterstitialType.Interstitial_UNITYADS);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(40), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Request Video", GUILayout.ExpandHeight(true)))
            {
                AdsManager.RequestRewardBasedVideo(RewardedVideoType.RewardedVideo_UNITYADS);
            }
            if (GUILayout.Button("Show Video", GUILayout.ExpandHeight(true)))
            {
                AdsManager.ShowRewardBasedVideo(RewardedVideoType.RewardedVideo_UNITYADS, (state) =>
                {
                    Debug.Log(state);
                });
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("ADMOB");
            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(40), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Show Banner", GUILayout.ExpandHeight(true)))
            {
                AdsManager.ShowBanner(BannerType.Banner_ADMOB);
            }
            if (GUILayout.Button("Destroy Banner", GUILayout.ExpandHeight(true)))
            {
                AdsManager.DestroyBanner(BannerType.Banner_ADMOB);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(40), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Request Interstitial", GUILayout.ExpandHeight(true)))
            {
                AdsManager.RequestInterstitial(InterstitialType.Interstitial_ADMOB);
            }
            if (GUILayout.Button("Show Interstitial", GUILayout.ExpandHeight(true)))
            {
                AdsManager.ShowInterstitial(InterstitialType.Interstitial_ADMOB);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(40), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Request Video", GUILayout.ExpandHeight(true)))
            {
                AdsManager.RequestRewardBasedVideo(RewardedVideoType.RewardedVideo_ADMOB);
            }
            if (GUILayout.Button("Show Video", GUILayout.ExpandHeight(true)))
            {
                AdsManager.ShowRewardBasedVideo(RewardedVideoType.RewardedVideo_ADMOB, (state) =>
                {
                    Debug.Log(state);
                });
            }
            GUILayout.EndHorizontal();
            GUI.EndGroup();
        }
    }
}