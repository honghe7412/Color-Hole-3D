using UnityEngine;

namespace Watermelon
{
    [SetupTab("Settings", priority = 0)]
    [CreateAssetMenu(fileName = "Game Settings", menuName = "Content/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [LineSpacer("Ads"), Tooltip("Delay in seconds between interstitial appearings.")]
        public float interstitialShowingDelay = 30f;
        [Tooltip("Length of game over count down before skipping revive in seconds.")]
        public float reviveCountDownTime = 10f;
    }
}
