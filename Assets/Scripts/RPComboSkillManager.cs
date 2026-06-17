using UnityEngine;
using UnityEngine.UI;

public class RPComboSkillManager : MonoBehaviour
{
    [Header("References")]
    public RPGameManager gameManager;
    public RPProgressionManager progression;
    public RPBallStatsManager ballStats;

    [Header("UI")]
    public Text comboText;
    public Text scoreBreakdownText;
    public Text objectiveText;
    public GameObject objectivePanel;

    [Header("Tuning")]
    public float comboTextVisibleTime = 1.15f;
    public float objectiveVisibleTime = 2.2f;

    private int lastLevelIndex = -1;
    private int lastShotsUsed = -1;
    private RPGameState lastState;

    private int currentShotBounces = 0;
    private int currentShotBreaks = 0;
    private int bestComboThisLevel = 0;
    private int bankShotsThisLevel = 0;
    private int trickShotsThisLevel = 0;
    private int comboScoreThisLevel = 0;
    private int lastBonusCoins = 0;
    private bool levelBonusPaid = false;

    private float comboTimer = 0f;
    private float objectiveTimer = 0f;

    private void Start()
    {
        ResolveReferences();
        HideCombo();
        HideObjective();
    }

    private void Update()
    {
        ResolveReferences();

        if (gameManager == null)
        {
            return;
        }

        if (gameManager.currentLevelIndex != lastLevelIndex)
        {
            OnLevelChanged();
        }

        if (gameManager.shotsUsed != lastShotsUsed)
        {
            OnNewShotStarted();
        }

        if (lastState != gameManager.state)
        {
            if (gameManager.state == RPGameState.LevelComplete)
            {
                OnLevelComplete();
            }

            lastState = gameManager.state;
        }

        TickComboText();
        TickObjectivePanel();
    }

    public void RegisterRailBounce()
    {
        if (gameManager == null || gameManager.state != RPGameState.BallMoving)
        {
            return;
        }

        currentShotBounces++;

        if (currentShotBounces == 1)
        {
            ShowCombo("Bank Shot!", new Color(0.75f, 0.92f, 1f));
            bankShotsThisLevel++;
            comboScoreThisLevel += 50;
        }
        else if (currentShotBounces == 2)
        {
            ShowCombo("Double Bank!", new Color(0.75f, 0.92f, 1f));
            trickShotsThisLevel++;
            comboScoreThisLevel += 90;
        }
        else if (currentShotBounces >= 3)
        {
            ShowCombo("Trick Route!", new Color(1f, 0.78f, 0.32f));
            trickShotsThisLevel++;
            comboScoreThisLevel += 130;
        }
    }

    public void RegisterGlassBreak()
    {
        if (gameManager == null)
        {
            return;
        }

        currentShotBreaks++;
        bestComboThisLevel = Mathf.Max(bestComboThisLevel, currentShotBreaks);

        int bonus = 80;

        if (currentShotBounces > 0)
        {
            bonus += 70;
        }

        if (currentShotBreaks >= 2)
        {
            bonus += currentShotBreaks * 100;
            ShowCombo("Combo x" + currentShotBreaks + "!", new Color(1f, 0.78f, 0.32f));
        }
        else if (currentShotBounces > 0)
        {
            ShowCombo("Bank Break!", new Color(0.75f, 0.92f, 1f));
        }
        else
        {
            ShowCombo("Glass Break!", new Color(0.80f, 0.96f, 1f));
        }

        comboScoreThisLevel += bonus;
    }

    public void RegisterBumperHit()
    {
        if (gameManager == null || gameManager.state != RPGameState.BallMoving)
        {
            return;
        }

        ShowCombo("Bumper Boost!", new Color(1f, 0.72f, 0.28f));
        comboScoreThisLevel += 60;
    }

    private void OnLevelChanged()
    {
        lastLevelIndex = gameManager.currentLevelIndex;
        lastShotsUsed = gameManager.shotsUsed;
        lastState = gameManager.state;

        currentShotBounces = 0;
        currentShotBreaks = 0;
        bestComboThisLevel = 0;
        bankShotsThisLevel = 0;
        trickShotsThisLevel = 0;
        comboScoreThisLevel = 0;
        lastBonusCoins = 0;
        levelBonusPaid = false;

        ShowObjectiveForCurrentLevel();
        ClearScoreBreakdown();
    }

    private void OnNewShotStarted()
    {
        lastShotsUsed = gameManager.shotsUsed;
        currentShotBounces = 0;
        currentShotBreaks = 0;
    }

    private void OnLevelComplete()
    {
        if (levelBonusPaid)
        {
            return;
        }

        levelBonusPaid = true;

        int bonusCoins = CalculateSkillBonusCoins();

        if (ballStats != null)
        {
            bonusCoins = ballStats.ApplyCoinBonus(bonusCoins);
        }

        lastBonusCoins = bonusCoins;

        if (progression != null && bonusCoins > 0)
        {
            progression.AddCoins(bonusCoins);
        }

        ApplyWinBreakdown();
    }

    private int CalculateSkillBonusCoins()
    {
        int bonus = 0;

        if (bankShotsThisLevel > 0)
        {
            bonus += 5;
        }

        if (bestComboThisLevel >= 2)
        {
            bonus += 10 + bestComboThisLevel * 3;
        }

        if (trickShotsThisLevel > 0)
        {
            bonus += 8;
        }

        if (gameManager != null && gameManager.shotsUsed <= GetCurrentPar())
        {
            bonus += 12;
        }

        if ((gameManager.currentLevelIndex + 1) % 5 == 0)
        {
            bonus += 25;
        }

        return bonus;
    }

    private int GetCurrentPar()
    {
        int level = gameManager != null ? gameManager.currentLevelIndex + 1 : 1;

        if (level <= 4) return 1;
        if (level <= 15) return 2;
        return 3;
    }

    private void ApplyWinBreakdown()
    {
        if (scoreBreakdownText != null)
        {
            scoreBreakdownText.gameObject.SetActive(true);
            string ballBonus = ballStats != null && ballStats.coinBonusPercent > 0 ? "  •  Gold bonus active" : "";
            scoreBreakdownText.text =
                "Skill score: " + comboScoreThisLevel +
                "\nBest combo: x" + Mathf.Max(1, bestComboThisLevel) +
                "  •  Banks: " + bankShotsThisLevel +
                "\nSkill bonus: +" + lastBonusCoins + " coins" + ballBonus;
        }

        if (gameManager != null && gameManager.winRewardText != null && lastBonusCoins > 0)
        {
            gameManager.winRewardText.text += "\nSkill bonus +" + lastBonusCoins;
        }

        if (progression != null && gameManager != null)
        {
            gameManager.RefreshMetaUI();
        }
    }

    private void ShowObjectiveForCurrentLevel()
    {
        if (objectivePanel == null || objectiveText == null)
        {
            return;
        }

        string chapter = GetChapterName(gameManager.currentLevelIndex);
        string objective = GetObjectiveText(gameManager.currentLevelIndex);

        objectiveText.text = chapter + "\n" + objective;
        objectivePanel.SetActive(true);
        objectiveTimer = objectiveVisibleTime;
    }

    private string GetChapterName(int index)
    {
        int level = index + 1;

        if (level <= 5) return "Chapter 1 — Learn the Shot";
        if (level <= 10) return "Chapter 2 — Bank Shots";
        if (level <= 15) return "Chapter 3 — Hazards";
        if (level <= 20) return "Chapter 4 — Timing";
        if (level <= 25) return "Chapter 5 — Breakable Glass";
        if (level <= 30) return "Chapter 6 — Mastery";
        return "Challenge Level";
    }

    private string GetObjectiveText(int index)
    {
        int level = index + 1;

        if (level == 11) return "New hazard: avoid the pocket.";
        if (level == 12) return "Pockets punish risky routes.";
        if (level == 16) return "New hazard: rotating blockers.";
        if (level >= 17 && level <= 20) return "Time your shot and avoid pockets.";

        if (level == 1) return "Break the glass. Rails bounce.";
        if (level == 2) return "Use a softer pull.";
        if (level == 3) return "Try your first angled shot.";
        if (level == 4) return "Break both glasses with one clean route.";
        if (level == 5) return "Clear under par for a perfect route.";

        if (level <= 10) return "Use rails to earn Bank Shot bonuses.";
        if (level <= 15) return "Find the cleanest safe route.";
        if (level <= 20) return "Use timing and rails.";
        if (level <= 25) return "Break glass walls and clear the path.";
        if (level <= 30) return "Combine banks, bumpers and perfect shots.";
        return "Clear the table. Chase better combos.";
    }

    private void ShowCombo(string message, Color color)
    {
        if (comboText == null)
        {
            return;
        }

        comboText.gameObject.SetActive(true);
        comboText.text = message;
        comboText.color = color;
        comboText.transform.localScale = Vector3.one * 1.16f;
        comboTimer = comboTextVisibleTime;
    }

    private void TickComboText()
    {
        if (comboText == null || !comboText.gameObject.activeSelf)
        {
            return;
        }

        comboTimer -= Time.deltaTime;
        comboText.transform.localScale = Vector3.Lerp(comboText.transform.localScale, Vector3.one, Time.deltaTime * 7f);

        if (comboTimer <= 0f)
        {
            HideCombo();
        }
    }

    private void TickObjectivePanel()
    {
        if (objectivePanel == null || !objectivePanel.activeSelf)
        {
            return;
        }

        objectiveTimer -= Time.deltaTime;

        if (objectiveTimer <= 0f)
        {
            HideObjective();
        }
    }

    private void HideCombo()
    {
        if (comboText != null)
        {
            comboText.gameObject.SetActive(false);
        }
    }

    private void HideObjective()
    {
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(false);
        }
    }

    private void ClearScoreBreakdown()
    {
        if (scoreBreakdownText != null)
        {
            scoreBreakdownText.text = "";
            scoreBreakdownText.gameObject.SetActive(false);
        }
    }

    private void ResolveReferences()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<RPGameManager>();
        }

        if (progression == null)
        {
            progression = FindFirstObjectByType<RPProgressionManager>();
        }

        if (ballStats == null)
        {
            ballStats = FindFirstObjectByType<RPBallStatsManager>();
        }
    }
}