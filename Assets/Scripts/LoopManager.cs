using System.Collections;
using UnityEngine;

public enum PlayerDecision
{
    ContinueForward,
    TurnBack
}

public class LoopManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Spawn Point")]
    public Vector3 spawnPosition = new Vector3(0f, 0f, -11f);

    [Header("Loop State")]
    public int currentLoop = 1;
    public int correctDecisions = 0;
    public int mistakes = 0;
    public bool anomalyIsActiveThisLoop = false;
    public bool gameOver = false;

    [Header("Difficulty Curve")]
    public int easyLoopsEndAt = 4;
    public int mediumLoopsEndAt = 9;

    [Header("Timing")]
    public float decisionTransitionDelay = 0.18f;
    public float fadeDuration = 0.35f;

    [Header("Anomalies")]
    public GameObject[] anomalies;

    [Header("Systems")]
    public GameStateUI gameStateUI;
    public HorrorAudioManager audioManager;
    public ScreenMoodController screenMood;
    public FadeController fadeController;

    private GameObject activeAnomaly;
    private string activeAnomalyName = "Normal";
    private bool transitionInProgress = false;

    private void Start()
    {
        if (fadeController != null)
        {
            fadeController.ClearInstant();
        }

        StartNewLoop();

        if (gameStateUI != null)
        {
            gameStateUI.ShowCenterPrompt("LOOP ROOM\n\nMemorize the corridor.", 2.4f);
        }
    }

    public void RegisterDecision(PlayerDecision decision)
    {
        if (gameOver || transitionInProgress)
        {
            return;
        }

        bool correctDecision =
            (anomalyIsActiveThisLoop && decision == PlayerDecision.TurnBack) ||
            (!anomalyIsActiveThisLoop && decision == PlayerDecision.ContinueForward);

        if (correctDecision)
        {
            StartCoroutine(HandleCorrectDecision(decision));
        }
        else
        {
            HandleWrongDecision(decision);
        }
    }

    private IEnumerator HandleCorrectDecision(PlayerDecision decision)
    {
        transitionInProgress = true;
        SetPlayerMovement(false);

        correctDecisions++;

        if (audioManager != null)
        {
            audioManager.PlayCorrectDecisionSting();
        }

        if (gameStateUI != null)
        {
            string text = decision == PlayerDecision.TurnBack ? "You turned back." : "You continued.";
            gameStateUI.ShowShortFeedback(text);
        }

        yield return new WaitForSeconds(decisionTransitionDelay);

        if (fadeController != null)
        {
            yield return fadeController.FadeToBlack(fadeDuration);
        }

        currentLoop++;
        ResetPlayerPosition();
        StartNewLoop();

        if (fadeController != null)
        {
            yield return fadeController.FadeFromBlack(fadeDuration);
        }

        SetPlayerMovement(true);
        transitionInProgress = false;
    }

    private void HandleWrongDecision(PlayerDecision decision)
    {
        mistakes++;
        gameOver = true;
        transitionInProgress = false;

        SetPlayerMovement(false);

        if (audioManager != null)
        {
            audioManager.PlayWrongDecisionSting();
        }

        if (screenMood != null)
        {
            screenMood.SetDangerMood(true);
        }

        DisableAllAnomalies();

        string expected = anomalyIsActiveThisLoop ? "TURN BACK" : "CONTINUE";
        string chosen = decision == PlayerDecision.TurnBack ? "TURN BACK" : "CONTINUE";

        if (gameStateUI != null)
        {
            gameStateUI.ShowGameOver(currentLoop, correctDecisions, expected, chosen, activeAnomalyName);
        }

        Debug.Log("Wrong decision. Expected: " + expected + ", chosen: " + chosen + ", reality: " + activeAnomalyName);
    }

    public void RestartGame()
    {
        StopAllCoroutines();

        gameOver = false;
        transitionInProgress = false;
        currentLoop = 1;
        correctDecisions = 0;
        mistakes = 0;

        if (screenMood != null)
        {
            screenMood.SetDangerMood(false);
        }

        ResetPlayerPosition();
        SetPlayerMovement(true);
        StartNewLoop();

        if (fadeController != null)
        {
            fadeController.ClearInstant();
        }

        if (gameStateUI != null)
        {
            gameStateUI.ShowCenterPrompt("Again.", 1.2f);
        }
    }

    private void StartNewLoop()
    {
        DisableAllAnomalies();

        anomalyIsActiveThisLoop = ShouldSpawnAnomalyThisLoop();

        if (anomalyIsActiveThisLoop)
        {
            activeAnomaly = ActivateAnomalyForCurrentDifficulty();
            activeAnomalyName = GetAnomalyName(activeAnomaly);
        }
        else
        {
            activeAnomaly = null;
            activeAnomalyName = "Normal loop";
        }

        if (audioManager != null)
        {
            audioManager.SetTensionByLoop(currentLoop, anomalyIsActiveThisLoop);
        }

        if (screenMood != null)
        {
            screenMood.SetLoopMood(currentLoop, anomalyIsActiveThisLoop);
        }

        if (gameStateUI != null)
        {
            gameStateUI.UpdateLoopInfo(currentLoop, correctDecisions, anomalyIsActiveThisLoop, GetCurrentDifficultyLabel());
        }

        Debug.Log("Loop " + currentLoop + " | " + GetCurrentDifficultyLabel() + " | Anomaly: " + anomalyIsActiveThisLoop + " | " + activeAnomalyName);
    }

    private bool ShouldSpawnAnomalyThisLoop()
    {
        if (currentLoop == 1)
        {
            return false;
        }

        if (currentLoop <= easyLoopsEndAt)
        {
            return Random.value <= 0.52f;
        }

        if (currentLoop <= mediumLoopsEndAt)
        {
            return Random.value <= 0.68f;
        }

        return Random.value <= 0.80f;
    }

    private GameObject ActivateAnomalyForCurrentDifficulty()
    {
        AnomalyDifficulty maxDifficulty = GetMaxAllowedDifficulty();
        GameObject[] candidates = GetCandidates(maxDifficulty);

        if (candidates.Length == 0)
        {
            return null;
        }

        GameObject selected = candidates[Random.Range(0, candidates.Length)];
        selected.SetActive(true);
        return selected;
    }

    private GameObject[] GetCandidates(AnomalyDifficulty maxDifficulty)
    {
        if (anomalies == null || anomalies.Length == 0)
        {
            return new GameObject[0];
        }

        int count = 0;

        foreach (GameObject anomaly in anomalies)
        {
            if (IsAllowedAnomaly(anomaly, maxDifficulty))
            {
                count++;
            }
        }

        GameObject[] result = new GameObject[count];
        int index = 0;

        foreach (GameObject anomaly in anomalies)
        {
            if (IsAllowedAnomaly(anomaly, maxDifficulty))
            {
                result[index] = anomaly;
                index++;
            }
        }

        return result;
    }

    private bool IsAllowedAnomaly(GameObject anomaly, AnomalyDifficulty maxDifficulty)
    {
        if (anomaly == null)
        {
            return false;
        }

        AnomalyDefinition definition = anomaly.GetComponent<AnomalyDefinition>();
        return definition == null || definition.difficulty <= maxDifficulty;
    }

    private AnomalyDifficulty GetMaxAllowedDifficulty()
    {
        if (currentLoop <= easyLoopsEndAt)
        {
            return AnomalyDifficulty.Easy;
        }

        if (currentLoop <= mediumLoopsEndAt)
        {
            return AnomalyDifficulty.Medium;
        }

        return AnomalyDifficulty.Hard;
    }

    private string GetCurrentDifficultyLabel()
    {
        if (currentLoop <= easyLoopsEndAt)
        {
            return "EASY";
        }

        if (currentLoop <= mediumLoopsEndAt)
        {
            return "MEDIUM";
        }

        return "HARD";
    }

    private string GetAnomalyName(GameObject anomaly)
    {
        if (anomaly == null)
        {
            return "Unknown";
        }

        AnomalyDefinition definition = anomaly.GetComponent<AnomalyDefinition>();

        if (definition != null && !string.IsNullOrWhiteSpace(definition.displayName))
        {
            return definition.displayName;
        }

        return anomaly.name;
    }

    private void ResetPlayerPosition()
    {
        if (player == null)
        {
            return;
        }

        CharacterController controller = player.GetComponent<CharacterController>();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();

        if (controller != null)
        {
            controller.enabled = false;
            player.position = spawnPosition;
            player.rotation = Quaternion.identity;
            controller.enabled = true;
        }
        else
        {
            player.position = spawnPosition;
            player.rotation = Quaternion.identity;
        }

        if (movement != null)
        {
            movement.ResetLook();
        }
    }

    private void SetPlayerMovement(bool enabled)
    {
        if (player == null)
        {
            return;
        }

        PlayerMovement movement = player.GetComponent<PlayerMovement>();

        if (movement != null)
        {
            movement.SetMovementEnabled(enabled);
        }
    }

    private void DisableAllAnomalies()
    {
        if (anomalies == null)
        {
            return;
        }

        foreach (GameObject anomaly in anomalies)
        {
            if (anomaly != null)
            {
                anomaly.SetActive(false);
            }
        }
    }
}