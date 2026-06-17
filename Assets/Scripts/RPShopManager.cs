using UnityEngine;
using UnityEngine.UI;

public class RPShopManager : MonoBehaviour
{
    [System.Serializable]
    public class SkinData
    {
        public int id;
        public string name;
        public int price;
        public Color color;
    }

    public RPProgressionManager progression;
    public RPGameManager gameManager;
    public RPBallStatsManager ballStats;
    public GameObject shopPanel;
    public Text shopStatusText;
    public Text skinNameText;
    public Text skinPriceText;
    public Text adStatusText;
    public Text ballStatsText;

    public SkinData[] skins =
    {
        new SkinData { id = 0, name = "Classic Marble", price = 0, color = new Color(0.94f, 0.92f, 0.84f) },
        new SkinData { id = 1, name = "Gold Marble", price = 220, color = new Color(1.00f, 0.72f, 0.25f) },
        new SkinData { id = 2, name = "Heavy Marble", price = 340, color = new Color(0.08f, 0.07f, 0.065f) },
        new SkinData { id = 3, name = "Rubber Marble", price = 520, color = new Color(0.20f, 0.85f, 1.00f) },
        new SkinData { id = 4, name = "Glass Marble", price = 760, color = new Color(0.95f, 0.10f, 0.18f) }
    };

    private int previewIndex = 0;

    private void Start()
    {
        ResolveReferences();
        Refresh();
    }

    public void OpenShop()
    {
        ResolveReferences();

        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        Refresh();
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    public void NextSkin()
    {
        previewIndex++;

        if (previewIndex >= skins.Length)
        {
            previewIndex = 0;
        }

        Refresh();
    }

    public void PreviousSkin()
    {
        previewIndex--;

        if (previewIndex < 0)
        {
            previewIndex = skins.Length - 1;
        }

        Refresh();
    }

    public void BuyOrSelectCurrentSkin()
    {
        ResolveReferences();

        if (progression == null || skins.Length == 0)
        {
            return;
        }

        SkinData skin = skins[previewIndex];
        bool wasOwned = progression.IsSkinOwned(skin.id);
        bool success = progression.TryBuySkin(skin.id, skin.price);

        if (shopStatusText != null)
        {
            if (success && wasOwned)
            {
                shopStatusText.text = "Selected: " + skin.name;
            }
            else if (success)
            {
                shopStatusText.text = "Unlocked: " + skin.name;
            }
            else
            {
                int missing = Mathf.Max(0, skin.price - progression.totalCoins);
                shopStatusText.text = "Need " + missing + " more coins.";
            }
        }

        if (ballStats != null)
        {
            ballStats.ApplySelectedBallStats();
        }

        if (gameManager != null)
        {
            gameManager.ApplySelectedSkin();
            gameManager.RefreshMetaUI();
        }

        Refresh();
    }

    public void WatchAdForCoinsPlaceholder()
    {
        ResolveReferences();

        if (progression != null)
        {
            progression.AddCoins(50);
        }

        if (adStatusText != null)
        {
            adStatusText.text = "Reward preview: +50 coins.\nReal rewarded ads connect later.";
        }

        if (gameManager != null)
        {
            gameManager.RefreshMetaUI();
        }

        Refresh();
    }

    public void WatchAdForHintPlaceholder()
    {
        ResolveReferences();

        if (progression != null)
        {
            progression.AddHintTokens(1);
        }

        if (adStatusText != null)
        {
            adStatusText.text = "Reward preview: +1 hint.\nReal rewarded ads connect later.";
        }

        if (gameManager != null)
        {
            gameManager.RefreshMetaUI();
        }

        Refresh();
    }

    public Color GetSelectedSkinColor()
    {
        ResolveReferences();

        if (progression == null || skins == null || skins.Length == 0)
        {
            return new Color(0.94f, 0.92f, 0.84f);
        }

        foreach (SkinData skin in skins)
        {
            if (skin.id == progression.selectedSkin)
            {
                return skin.color;
            }
        }

        return skins[0].color;
    }

    private void Refresh()
    {
        ResolveReferences();

        if (progression == null || skins == null || skins.Length == 0)
        {
            return;
        }

        previewIndex = Mathf.Clamp(previewIndex, 0, skins.Length - 1);
        SkinData skin = skins[previewIndex];

        bool owned = progression.IsSkinOwned(skin.id);

        if (skinNameText != null)
        {
            skinNameText.text = skin.name;
        }

        if (skinPriceText != null)
        {
            if (owned)
            {
                skinPriceText.text = skin.id == progression.selectedSkin ? "Selected" : "Owned — tap to select";
            }
            else
            {
                skinPriceText.text = skin.price + " coins";
            }
        }

        if (shopStatusText != null)
        {
            shopStatusText.text = "Coins: " + progression.totalCoins + "  •  Hints: " + progression.hintTokens;
        }

        if (ballStatsText != null)
        {
            if (ballStats != null)
            {
                ballStatsText.text = ballStats.GetShopStatLine(skin.id);
            }
            else
            {
                ballStatsText.text = "Ability: Balanced";
            }
        }
    }

    private void ResolveReferences()
    {
        if (progression == null)
        {
            progression = FindFirstObjectByType<RPProgressionManager>();
        }

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<RPGameManager>();
        }

        if (ballStats == null)
        {
            ballStats = FindFirstObjectByType<RPBallStatsManager>();
        }
    }
}