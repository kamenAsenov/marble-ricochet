using UnityEngine;

public class RPProgressionManager : MonoBehaviour
{
    private const string CoinsKey = "MR_TotalCoins";
    private const string StarsKey = "MR_TotalStars";
    private const string CompletedKey = "MR_LevelsCompleted";
    private const string HighestUnlockedKey = "MR_HighestUnlockedLevel";
    private const string PerfectStreakKey = "MR_PerfectStreak";
    private const string BestPerfectStreakKey = "MR_BestPerfectStreak";
    private const string HintTokensKey = "MR_HintTokens";
    private const string SelectedSkinKey = "MR_SelectedSkin";

    public int totalCoins = 0;
    public int totalStars = 0;
    public int perfectStreak = 0;
    public int bestPerfectStreak = 0;
    public int levelsCompleted = 0;
    public int highestUnlockedLevel = 0;
    public int hintTokens = 0;
    public int selectedSkin = 0;

    private void Awake()
    {
        Load();
    }

    public void Load()
    {
        totalCoins = PlayerPrefs.GetInt(CoinsKey, 0);
        totalStars = PlayerPrefs.GetInt(StarsKey, 0);
        levelsCompleted = PlayerPrefs.GetInt(CompletedKey, 0);
        highestUnlockedLevel = PlayerPrefs.GetInt(HighestUnlockedKey, 0);
        perfectStreak = PlayerPrefs.GetInt(PerfectStreakKey, 0);
        bestPerfectStreak = PlayerPrefs.GetInt(BestPerfectStreakKey, 0);
        hintTokens = PlayerPrefs.GetInt(HintTokensKey, 0);
        selectedSkin = PlayerPrefs.GetInt(SelectedSkinKey, 0);
    }

    public void Save()
    {
        PlayerPrefs.SetInt(CoinsKey, totalCoins);
        PlayerPrefs.SetInt(StarsKey, totalStars);
        PlayerPrefs.SetInt(CompletedKey, levelsCompleted);
        PlayerPrefs.SetInt(HighestUnlockedKey, highestUnlockedLevel);
        PlayerPrefs.SetInt(PerfectStreakKey, perfectStreak);
        PlayerPrefs.SetInt(BestPerfectStreakKey, bestPerfectStreak);
        PlayerPrefs.SetInt(HintTokensKey, hintTokens);
        PlayerPrefs.SetInt(SelectedSkinKey, selectedSkin);
        PlayerPrefs.Save();
    }

    public int GetBestStarsForLevel(int levelIndex)
    {
        return PlayerPrefs.GetInt(GetLevelStarsKey(levelIndex), 0);
    }

    public int RegisterLevelResult(int levelIndex, int stars, int shotsUsed, int parShots)
    {
        int previousBestStars = GetBestStarsForLevel(levelIndex);
        bool improved = stars > previousBestStars;
        bool firstClear = previousBestStars <= 0;
        bool perfect = shotsUsed <= parShots;

        if (firstClear)
        {
            levelsCompleted++;
        }

        if (improved)
        {
            totalStars += stars - previousBestStars;
            PlayerPrefs.SetInt(GetLevelStarsKey(levelIndex), stars);
        }

        highestUnlockedLevel = Mathf.Max(highestUnlockedLevel, levelIndex + 1);

        int baseCoins = 8 + stars * 4;
        int firstClearBonus = firstClear ? 15 : 0;
        int improvementBonus = improved && !firstClear ? 10 : 0;
        int perfectBonus = perfect ? 15 : 0;

        if (perfect)
        {
            perfectStreak++;
            bestPerfectStreak = Mathf.Max(bestPerfectStreak, perfectStreak);
        }
        else
        {
            perfectStreak = 0;
        }

        int streakBonus = perfectStreak >= 2 ? Mathf.Min(30, perfectStreak * 4) : 0;
        int coinsEarned = baseCoins + firstClearBonus + improvementBonus + perfectBonus + streakBonus;

        totalCoins += coinsEarned;

        Save();
        return coinsEarned;
    }

    public bool IsSkinOwned(int skinId)
    {
        if (skinId == 0)
        {
            return true;
        }

        return PlayerPrefs.GetInt(GetSkinOwnedKey(skinId), 0) == 1;
    }

    public bool TryBuySkin(int skinId, int price)
    {
        if (IsSkinOwned(skinId))
        {
            selectedSkin = skinId;
            Save();
            return true;
        }

        if (totalCoins < price)
        {
            return false;
        }

        totalCoins -= price;
        PlayerPrefs.SetInt(GetSkinOwnedKey(skinId), 1);
        selectedSkin = skinId;
        Save();
        return true;
    }

    public void SelectSkin(int skinId)
    {
        if (IsSkinOwned(skinId))
        {
            selectedSkin = skinId;
            Save();
        }
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        Save();
    }

    public void AddHintTokens(int amount)
    {
        hintTokens += amount;
        Save();
    }

    public bool SpendHintToken()
    {
        if (hintTokens <= 0)
        {
            return false;
        }

        hintTokens--;
        Save();
        return true;
    }

    public string GetMotivationLine(int stars, int shotsUsed, int parShots)
    {
        if (shotsUsed <= parShots && perfectStreak >= 3) return "Perfect streak!";
        if (shotsUsed <= parShots) return "Perfect shot!";
        if (stars >= 3) return "Beautiful clear!";
        if (stars == 2) return "Nice clear!";
        return "Cleared!";
    }

    public string GetMenuProgressLine()
    {
        return "Level " + (highestUnlockedLevel + 1) +
               "  •  Stars " + totalStars +
               "  •  Coins " + totalCoins;
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey(CoinsKey);
        PlayerPrefs.DeleteKey(StarsKey);
        PlayerPrefs.DeleteKey(CompletedKey);
        PlayerPrefs.DeleteKey(HighestUnlockedKey);
        PlayerPrefs.DeleteKey(PerfectStreakKey);
        PlayerPrefs.DeleteKey(BestPerfectStreakKey);
        PlayerPrefs.DeleteKey(HintTokensKey);
        PlayerPrefs.DeleteKey(SelectedSkinKey);

        for (int i = 0; i < 300; i++)
        {
            PlayerPrefs.DeleteKey(GetLevelStarsKey(i));
            PlayerPrefs.DeleteKey(GetSkinOwnedKey(i));
        }

        PlayerPrefs.Save();
        Load();
    }

    private string GetLevelStarsKey(int levelIndex)
    {
        return "MR_Level_" + levelIndex + "_BestStars";
    }

    private string GetSkinOwnedKey(int skinId)
    {
        return "MR_Skin_" + skinId + "_Owned";
    }
}