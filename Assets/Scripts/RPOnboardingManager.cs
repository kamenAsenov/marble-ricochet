using UnityEngine;
using UnityEngine.UI;

public class RPOnboardingManager : MonoBehaviour
{
    public RPGameManager gameManager;
    public GameObject onboardingPanel;
    public Text onboardingText;

    private int lastLevel = -1;
    private bool dismissedForThisShot = false;

    private void Update()
    {
        if (gameManager == null || onboardingPanel == null || onboardingText == null)
        {
            return;
        }

        bool shouldShow = ShouldShowOnboarding(out string message);

        onboardingPanel.SetActive(shouldShow);

        if (gameManager.hintText != null)
        {
            gameManager.hintText.gameObject.SetActive(!shouldShow);
        }

        if (shouldShow)
        {
            onboardingText.text = message;
        }
    }

    private bool ShouldShowOnboarding(out string message)
    {
        message = "";

        if (gameManager.state != RPGameState.Aiming)
        {
            dismissedForThisShot = false;
            return false;
        }

        if (gameManager.currentLevelIndex != lastLevel)
        {
            lastLevel = gameManager.currentLevelIndex;
            dismissedForThisShot = false;
        }

        if (gameManager.shotsUsed > 0)
        {
            dismissedForThisShot = true;
        }

        if (dismissedForThisShot)
        {
            return false;
        }

        message = GetMessage(gameManager.currentLevelIndex);
        return !string.IsNullOrEmpty(message);
    }

    private string GetMessage(int levelIndex)
    {
        switch (levelIndex)
        {
            case 0:
                return "Pull back from the marble.\nRelease to shoot.";
            case 1:
                return "Use a softer pull.\nThe line previews the shot.";
            case 2:
                return "Aim for the angle.\nLet the bounce do the work.";
            default:
                return "";
        }
    }
}