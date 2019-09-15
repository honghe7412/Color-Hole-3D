using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameSettings gameSettings;

    private LevelState levelState;
    public static LevelState CurrentState
    {
        get { return instance.levelState; }
    }

    private int firstStageItemsAmount;
    public static int FirstStageItemsAmount
    {
        get { return instance.firstStageItemsAmount; }
        set { instance.firstStageItemsAmount = value; }
    }

    private int secondStageItemsAmount;
    public static int SecondStageItemsAmount
    {
        get { return instance.secondStageItemsAmount; }
        set { instance.secondStageItemsAmount = value; }
    }

    private bool waitingForTap;
    public static bool WaitingForTap
    {
        get { return instance.waitingForTap; }
    }

    public static float ReviveCountDownTime
    {
        get { return instance.gameSettings.reviveCountDownTime; }
    }

    private BannerType bannerType;
    private InterstitialType interstitialType;
    public static RewardedVideoType RevardedVideoType
    {
        get { return instance.rewardedVideoType; }
    }
    private RewardedVideoType rewardedVideoType;

    private static float lastAddsTime = float.MinValue;

    private bool firstStageCompleted;
    private bool reviveUsed;

    private int currentLevel;
    private int currentItemsAbsorbed = 0;

    public enum LevelState
    {
        First,
        Second,
        Changing,
        GameNotActive,
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        // showing interstitial
        if (lastAddsTime + gameSettings.interstitialShowingDelay < Time.realtimeSinceStartup)
        {
            AdsManager.ShowInterstitial(interstitialType);

            lastAddsTime = Time.realtimeSinceStartup;
        }

        reviveUsed = false;
        firstStageCompleted = false;
        currentItemsAbsorbed = 0;
        currentLevel = GameSettingsPrefs.Get<int>("current level");

        levelState = LevelState.First;
        LevelController.instance.LoadLevel(currentLevel);
        UIController.instance.InitLevel(currentLevel);
        CameraController.instance.InitCameraOnFirstStage();
        ColorsController.SetRandomPreset();

        GroundGenerator.InitPlayground();

        waitingForTap = true;

        // ads
        bannerType = AdsManager.GetBannerType();
        interstitialType = AdsManager.GetInterstitialType();
        rewardedVideoType = AdsManager.GetRewardedVideoType();

        AdsManager.RequestInterstitial(interstitialType);
        AdsManager.RequestRewardBasedVideo(rewardedVideoType);

        AdsManager.ShowBanner(bannerType);
    }

    public static void OnTapPerformed()
    {
        if (instance.waitingForTap)
        {
            instance.waitingForTap = false;
            TouchController.instance.Activate();
            UIController.instance.HideMenu();
        }
    }

    private void OnFirstStageCompleted()
    {
        firstStageCompleted = true;
        levelState = LevelState.Changing;
        LevelController.instance.OnFirstStageCompleted();
    }

    public static void OnSecondStageReached()
    {
        instance.levelState = LevelState.Second;
        TouchController.instance.Activate();
    }

    public static void OnObstacleAbsorbed()
    {
        if (instance.levelState == LevelState.First || instance.levelState == LevelState.Second)
        {
            instance.levelState = LevelState.GameNotActive;
            TouchController.instance.Deactivate();
            CameraController.instance.ShakeCamera();

            if (!instance.reviveUsed)
            {
                Tween.DelayedCall(1f, instance.GameOver);
            }
            else
            {
                Tween.DelayedCall(1f, instance.StartGame);
            }
        }
    }

    public static void OnItemAbsorbed()
    {
        if (instance.levelState == LevelState.Changing)
            return;


        // allowing to absorb items even when game is over - to prevent bugs when player uses revive
        instance.currentItemsAbsorbed++;

        if (instance.levelState == LevelState.GameNotActive)
            return;


        UIController.instance.UpdateLevelProgress(instance.levelState, (float)instance.currentItemsAbsorbed / (instance.levelState == LevelState.First ? instance.firstStageItemsAmount : instance.secondStageItemsAmount));

        if (CurrentState == LevelState.First && instance.currentItemsAbsorbed >= instance.firstStageItemsAmount)
        {
            instance.OnFirstStageCompleted();
            instance.currentItemsAbsorbed = 0;
        }
        else if (CurrentState == LevelState.Second && instance.currentItemsAbsorbed >= instance.secondStageItemsAmount)
        {
            instance.LevelComplete();
        }
    }

    private void LevelComplete()
    {
        levelState = LevelState.GameNotActive;
        TouchController.instance.Deactivate();
        UIController.instance.ShowLevelCompletedPanel();

        currentLevel++;

        if (currentLevel > LevelController.instance.LevelsAmount)
        {
            currentLevel = 1;
        }

        GameSettingsPrefs.Set("current level", currentLevel);

        Tween.DelayedCall(2f, StartGame);
    }

    private void GameOver()
    {
        TouchController.instance.Deactivate();
        UIController.instance.ShowGameOverPanel(currentLevel);
    }

    public static void OnReviveUsed()
    {
        AdsManager.ShowRewardBasedVideo(instance.rewardedVideoType, (hasReward) =>
        {
            if (hasReward)
            {
                instance.reviveUsed = true;
                instance.levelState = instance.firstStageCompleted ? LevelState.Second : LevelState.First;
                TouchController.instance.Activate();
            }
            else
            {
                OnGameOverPanelClosed();
            }
        });
    }

    public static void OnGameOverPanelClosed()
    {
        instance.StartGame();
    }

    private void OnDestroy()
    {
        AdsManager.DestroyBanner(bannerType);
    }

    #region Developement

    public static void LoadFirstLevel()
    {
        GameSettingsPrefs.Set("current level", 1);
        instance.StartGame();
    }

    public static void LoadPrevLevel()
    {
        int prevLevel = instance.currentLevel - 1;
        if (prevLevel <= 0)
        {
            prevLevel = LevelController.instance.LevelsAmount;
        }

        TouchController.instance.Deactivate();
        GameSettingsPrefs.Set("current level", prevLevel);
        instance.StartGame();
    }

    public static void LoadNextLevel()
    {
        int nextLevel = instance.currentLevel + 1;
        if (nextLevel >= LevelController.instance.LevelsAmount + 1)
        {
            nextLevel = 1;
        }

        TouchController.instance.Deactivate();
        GameSettingsPrefs.Set("current level", nextLevel);
        instance.StartGame();
    }

    public static void ChangeColorPreset()
    {
        ColorsController.SetRandomPreset();
    }

    public static void SkipFirstStage()
    {
        instance.OnFirstStageCompleted();
    }

    #endregion
}