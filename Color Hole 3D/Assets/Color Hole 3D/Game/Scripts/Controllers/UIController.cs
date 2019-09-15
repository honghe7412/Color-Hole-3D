using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [Header("References")]
    public Text currentLevelText;
    public Text nextLevelText;
    public Image firstStageFillImage;
    public Image secondStageFillImage;

    [Space(5)]
    public GameObject menuPanel;
    public GameObject gameOverPanel;
    public Text gameOverStageText;
    public Button continueButton;
    public GameObject levelCompletedPanel;
    public GameObject levelCompletedParticle;
    public Animator menuPanelAnimator;

    [Space()]
    public Image countDownFillImage;
    public Text countDownText;

    private int hideParameter;

    private void Awake()
    {
        instance = this;

        hideParameter = Animator.StringToHash("Hide");
    }

    public void InitLevel(int levelNumber)
    {
        currentLevelText.text = levelNumber.ToString();
        nextLevelText.text = (levelNumber + 1).ToString();

        firstStageFillImage.fillAmount = 0f;
        secondStageFillImage.fillAmount = 0f;

        menuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        levelCompletedPanel.SetActive(false);
    }

    public void HideMenu()
    {
        menuPanelAnimator.SetTrigger(hideParameter);
    }

    public void UpdateLevelProgress(GameController.LevelState stage, float progress)
    {
        if (stage == GameController.LevelState.First)
        {
            firstStageFillImage.fillAmount = progress;
        }
        else
        {
            secondStageFillImage.fillAmount = progress;
        }
    }

    public void ShowLevelCompletedPanel()
    {
        levelCompletedPanel.SetActive(true);
        levelCompletedParticle.SetActive(true);
    }

    public void ShowGameOverPanel(int levelNumber)
    {
        gameOverStageText.text = "STAGE " + levelNumber;
        gameOverPanel.SetActive(true);

        StartCoroutine(GameOverPanelCoroutine());
    }

    private IEnumerator GameOverPanelCoroutine()
    {
        float countDownTime = GameController.ReviveCountDownTime;
        float timer = countDownTime;
        float lastCheckTime = Time.timeSinceLevelLoad;
        bool needUpdateAddLoadingState = !AdsManager.IsRewardBasedVideoLoaded(GameController.RevardedVideoType);

        continueButton.interactable = !needUpdateAddLoadingState;

        while (gameOverPanel.activeSelf && timer > 0)
        {
            countDownFillImage.fillAmount = timer / countDownTime;
            countDownText.text = ((int)timer + 1).ToString();

            timer -= Time.deltaTime;

            if(lastCheckTime +1 < Time.timeSinceLevelLoad)
            {
                continueButton.interactable = AdsManager.IsRewardBasedVideoLoaded(GameController.RevardedVideoType);
                lastCheckTime = Time.timeSinceLevelLoad;
            }

            yield return null;
        }

        if (gameOverPanel.activeSelf)
        {
            NoThanksButton();
        }
    }

    #region Buttons & Callbacks

    public void ContinueButton()
    {
        gameOverPanel.SetActive(false);
        GameController.OnReviveUsed();
    }


    public void NoThanksButton()
    {
        gameOverPanel.SetActive(false);
        GameController.OnGameOverPanelClosed();
    }

    public void OnMenuHiden()
    {
        menuPanel.SetActive(false);
    }

    public void TapToPlayButton()
    {
        GameController.OnTapPerformed();
    }

    public void SettingsButton()
    {

    }

    public void NoAdsButton()
    {

    }


    // developement

    public void FirstLevelButton()
    {
        GameController.LoadFirstLevel();
    }

    public void PrevLevelButton()
    {
        GameController.LoadPrevLevel();
    }

    public void NextLevelButton()
    {
        GameController.LoadNextLevel();
    }

    public void ChangeColorButton()
    {
        GameController.ChangeColorPreset();
    }

    public void NextStageButton()
    {
        GameController.SkipFirstStage();
    }

    #endregion
}