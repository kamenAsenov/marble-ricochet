using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GameStateUI : MonoBehaviour
{
    public LoopManager loopManager;

    public Text loopText;
    public Text streakText;
    public Text difficultyText;
    public Text hintText;
    public Text feedbackText;
    public Text centerPromptText;
    public GameObject gameOverPanel;
    public Text gameOverText;

    private Coroutine feedbackRoutine;
    private Coroutine centerPromptRoutine;

    public void UpdateLoopInfo(int loop, int correctDecisions, bool anomalyActive, string difficultyLabel)
    {
        if (loopText != null) loopText.text = "LOOP " + loop;
        if (streakText != null) streakText.text = "STREAK " + correctDecisions;
        if (difficultyText != null) difficultyText.text = difficultyLabel;

        if (hintText != null)
        {
            if (loop <= 2)
            {
                hintText.text = "Normal corridor = continue forward. Something changed = turn back.";
            }
            else if (loop <= 4)
            {
                hintText.text = "Early loops are readable. Trust your eyes.";
            }
            else
            {
                hintText.text = "Changed? Turn back. Normal? Continue.";
            }
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void ShowShortFeedback(string message)
    {
        if (feedbackText == null)
        {
            return;
        }

        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
        }

        feedbackRoutine = StartCoroutine(FeedbackRoutine(message));
    }

    public void ShowCenterPrompt(string message, float duration)
    {
        if (centerPromptText == null)
        {
            return;
        }

        if (centerPromptRoutine != null)
        {
            StopCoroutine(centerPromptRoutine);
        }

        centerPromptRoutine = StartCoroutine(CenterPromptRoutine(message, duration));
    }

    private IEnumerator FeedbackRoutine(string message)
    {
        feedbackText.text = message;
        feedbackText.enabled = true;
        yield return new WaitForSeconds(0.65f);
        feedbackText.enabled = false;
    }

    private IEnumerator CenterPromptRoutine(string message, float duration)
    {
        centerPromptText.text = message;
        centerPromptText.enabled = true;

        yield return new WaitForSeconds(duration);

        centerPromptText.enabled = false;
    }

    public void ShowGameOver(int loop, int correctDecisions, string expected, string chosen, string anomalyName)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (gameOverText != null)
        {
            gameOverText.text =
                "WRONG CHOICE\n\n" +
                "Loop reached: " + loop + "\n" +
                "Correct decisions: " + correctDecisions + "\n\n" +
                "Expected: " + expected + "\n" +
                "You chose: " + chosen + "\n\n" +
                "Reality state: " + anomalyName + "\n\n" +
                "Press R to restart";
        }
    }

    private void Update()
    {
        if (RestartPressed() && loopManager != null && loopManager.gameOver)
        {
            loopManager.RestartGame();
        }
    }

    private bool RestartPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.R);
#endif
    }
}