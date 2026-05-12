#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MarbleRicochetUpdate06Applier
{
    [MenuItem("Tools/Marble Ricochet/Apply Mobile Layout Shop Ads Update 06")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 06.");
            return;
        }

        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (manager == null || canvas == null)
        {
            EditorUtility.DisplayDialog(
                "Update 06",
                "Could not find RPGameManager or Canvas.\n\nFirst run:\nTools > Marble Ricochet > Replace Project With Marble Ricochet",
                "OK"
            );
            return;
        }

        manager.maxPullDistance = 2.15f;
        manager.maxLaunchPower = 10.8f;

        RPProgressionManager progression = Object.FindFirstObjectByType<RPProgressionManager>();
        if (progression == null)
        {
            GameObject progressionObject = new GameObject("RPProgressionManager");
            progression = progressionObject.AddComponent<RPProgressionManager>();
        }

        manager.progression = progression;

        FixHudLayout(manager);
        FixMenuLayout(manager);
        FixWinLayout(manager);

        RPShopManager shop = Object.FindFirstObjectByType<RPShopManager>();
        if (shop == null)
        {
            GameObject shopObject = new GameObject("RPShopManager");
            shop = shopObject.AddComponent<RPShopManager>();
        }

        shop.progression = progression;
        shop.gameManager = manager;
        manager.shopManager = shop;

        CreateOrFixShop(canvas.transform, manager, shop);
        CreateAdButtons(canvas.transform, manager, shop);
        CreateHintButton(canvas.transform, manager);

        manager.ApplySelectedSkin();

        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(progression);
        EditorUtility.SetDirty(shop);
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog(
            "Update 06 Applied",
            "Mobile layout fixed.\n\nAdded Shop, skins, rewarded ad placeholders, hints and improved slingshot spacing.",
            "OK"
        );
    }

    private static void FixHudLayout(RPGameManager manager)
    {
        SetText(manager.levelText, 22, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(34f, -24f), new Vector2(390f, 92f));
        SetText(manager.targetsText, 22, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(210f, 80f));
        SetText(manager.shotsText, 22, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-34f, -24f), new Vector2(210f, 80f));
        SetText(manager.coinsText, 18, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(34f, -112f), new Vector2(160f, 65f));
        SetText(manager.streakText, 18, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-34f, -112f), new Vector2(160f, 65f));
        SetText(manager.hintText, 21, TextAnchor.LowerCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 30f), new Vector2(880f, 65f));

        if (manager.progressSlider != null)
        {
            RectTransform rect = manager.progressSlider.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -105f);
            rect.sizeDelta = new Vector2(320f, 12f);
        }
    }

    private static void FixMenuLayout(RPGameManager manager)
    {
        if (manager.menuPanel == null) return;

        SetText(manager.titleText, 54, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.76f), new Vector2(0.5f, 0.76f), Vector2.zero, new Vector2(940f, 130f));
        SetText(manager.subtitleText, 26, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.67f), new Vector2(0.5f, 0.67f), Vector2.zero, new Vector2(900f, 70f));

        if (manager.menuProgressText == null)
        {
            manager.menuProgressText = CreateText("MenuProgressText", manager.menuPanel.transform, "Level 1 • Stars 0 • Coins 0", 22, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.59f), new Vector2(0.5f, 0.59f), Vector2.zero, new Vector2(900f, 58f), new Color(1f, 0.76f, 0.36f));
        }
        else
        {
            SetText(manager.menuProgressText, 22, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.59f), new Vector2(0.5f, 0.59f), Vector2.zero, new Vector2(900f, 58f));
        }

        MoveButton("ContinueButton", new Vector2(0.5f, 0.49f), new Vector2(430f, 76f));
        MoveButton("NewGameButton", new Vector2(0.5f, 0.40f), new Vector2(360f, 68f));
        MoveButton("ResetProgressButton", new Vector2(0.5f, 0.20f), new Vector2(240f, 54f));
    }

    private static void FixWinLayout(RPGameManager manager)
    {
        SetText(manager.winTitleText, 43, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), Vector2.zero, new Vector2(880f, 90f));
        SetText(manager.winStatsText, 28, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.57f), new Vector2(0.5f, 0.57f), Vector2.zero, new Vector2(850f, 220f));
        SetText(manager.winRewardText, 24, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.42f), new Vector2(0.5f, 0.42f), Vector2.zero, new Vector2(850f, 135f));

        MoveButton("RetryButton", new Vector2(0.5f, 0.31f), new Vector2(390f, 64f));
        MoveButton("NextButton", new Vector2(0.5f, 0.23f), new Vector2(420f, 74f));
        MoveButton("MenuButton", new Vector2(0.5f, 0.15f), new Vector2(320f, 60f));
    }

    private static void CreateOrFixShop(Transform canvasTransform, RPGameManager manager, RPShopManager shop)
    {
        GameObject shopButtonObj = GameObject.Find("ShopButton");
        Button shopButton = shopButtonObj != null ? shopButtonObj.GetComponent<Button>() : null;

        if (shopButton == null && manager.menuPanel != null)
        {
            shopButton = CreateButton("ShopButton", manager.menuPanel.transform, "SHOP", new Vector2(0.5f, 0.31f), new Vector2(330f, 64f));
        }

        if (shopButton != null)
        {
            shopButton.onClick.RemoveAllListeners();
            UnityEventTools.AddPersistentListener(shopButton.onClick, shop.OpenShop);
        }

        GameObject panel = GameObject.Find("ShopPanel");
        if (panel == null)
        {
            panel = CreatePanel("ShopPanel", canvasTransform, new Color(0.030f, 0.024f, 0.020f, 0.94f));
        }

        shop.shopPanel = panel;
        panel.SetActive(false);

        shop.skinNameText = GetOrCreateText(panel.transform, "ShopSkinNameText", "Classic Marble", 38, new Vector2(0.5f, 0.70f), new Vector2(800f, 90f), new Color(0.98f, 0.92f, 0.76f));
        shop.skinPriceText = GetOrCreateText(panel.transform, "ShopSkinPriceText", "Selected", 28, new Vector2(0.5f, 0.62f), new Vector2(800f, 70f), new Color(1f, 0.76f, 0.36f));
        shop.shopStatusText = GetOrCreateText(panel.transform, "ShopStatusText", "Coins: 0 • Hints: 0", 23, new Vector2(0.5f, 0.52f), new Vector2(850f, 70f), new Color(0.86f, 0.82f, 0.72f));
        shop.adStatusText = GetOrCreateText(panel.transform, "AdStatusText", "Ads will be real SDK later. Now they simulate rewards.", 20, new Vector2(0.5f, 0.28f), new Vector2(920f, 75f), new Color(0.75f, 0.88f, 1f));

        Button prev = GetOrCreateButton(panel.transform, "PrevSkinButton", "‹", new Vector2(0.25f, 0.66f), new Vector2(90f, 75f));
        Button next = GetOrCreateButton(panel.transform, "NextSkinButton", "›", new Vector2(0.75f, 0.66f), new Vector2(90f, 75f));
        Button buy = GetOrCreateButton(panel.transform, "BuySkinButton", "BUY / SELECT", new Vector2(0.5f, 0.43f), new Vector2(430f, 72f));
        Button adCoins = GetOrCreateButton(panel.transform, "RewardCoinsButton", "WATCH AD +50 COINS", new Vector2(0.5f, 0.35f), new Vector2(480f, 68f));
        Button adHint = GetOrCreateButton(panel.transform, "RewardHintButton", "WATCH AD +1 HINT", new Vector2(0.5f, 0.20f), new Vector2(440f, 64f));
        Button close = GetOrCreateButton(panel.transform, "CloseShopButton", "CLOSE", new Vector2(0.5f, 0.12f), new Vector2(320f, 58f));

        prev.onClick.RemoveAllListeners();
        next.onClick.RemoveAllListeners();
        buy.onClick.RemoveAllListeners();
        adCoins.onClick.RemoveAllListeners();
        adHint.onClick.RemoveAllListeners();
        close.onClick.RemoveAllListeners();

        UnityEventTools.AddPersistentListener(prev.onClick, shop.PreviousSkin);
        UnityEventTools.AddPersistentListener(next.onClick, shop.NextSkin);
        UnityEventTools.AddPersistentListener(buy.onClick, shop.BuyOrSelectCurrentSkin);
        UnityEventTools.AddPersistentListener(adCoins.onClick, shop.WatchAdForCoinsPlaceholder);
        UnityEventTools.AddPersistentListener(adHint.onClick, shop.WatchAdForHintPlaceholder);
        UnityEventTools.AddPersistentListener(close.onClick, shop.CloseShop);
    }

    private static void CreateAdButtons(Transform canvasTransform, RPGameManager manager, RPShopManager shop)
    {
        // Shop has ad placeholders. This method is intentionally kept for future SDK integration.
    }

    private static void CreateHintButton(Transform canvasTransform, RPGameManager manager)
    {
        if (manager.hudPanel == null) return;

        Button hintButton = GetOrCreateButton(manager.hudPanel.transform, "HintButton", "HINT", new Vector2(0.92f, 0.08f), new Vector2(120f, 64f));
        hintButton.onClick.RemoveAllListeners();
        UnityEventTools.AddPersistentListener(hintButton.onClick, manager.UseHint);

        if (manager.hintTokensText == null)
        {
            manager.hintTokensText = CreateText("HintTokensText", manager.hudPanel.transform, "HINTS\n0", 18, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -112f), new Vector2(160f, 65f), new Color(1f, 0.76f, 0.36f));
        }
        else
        {
            SetText(manager.hintTokensText, 18, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -112f), new Vector2(160f, 65f));
        }
    }

    private static void SetText(Text text, int fontSize, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        if (text == null) return;

        text.fontSize = fontSize;
        text.alignment = alignment;

        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
    }

    private static void MoveButton(string name, Vector2 anchor, Vector2 size)
    {
        GameObject obj = GameObject.Find(name);
        if (obj == null) return;

        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect == null) return;

        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
    }

    private static Text GetOrCreateText(Transform parent, string name, string content, int fontSize, Vector2 anchor, Vector2 size, Color color)
    {
        Text text = GameObject.Find(name)?.GetComponent<Text>();
        if (text != null) return text;

        return CreateText(name, parent, content, fontSize, TextAnchor.MiddleCenter, anchor, anchor, Vector2.zero, size, color);
    }

    private static Button GetOrCreateButton(Transform parent, string name, string label, Vector2 anchor, Vector2 size)
    {
        Button button = GameObject.Find(name)?.GetComponent<Button>();
        if (button != null) return button;

        return CreateButton(name, parent, label, anchor, size);
    }

    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = panel.AddComponent<Image>();
        image.color = color;

        return panel;
    }

    private static Text CreateText(string name, Transform parent, string content, int fontSize, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        Text text = obj.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;

        Shadow shadow = obj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.86f);
        shadow.effectDistance = new Vector2(2f, -2f);

        return text;
    }

    private static Button CreateButton(string name, Transform parent, string label, Vector2 anchor, Vector2 size)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;

        Image image = obj.AddComponent<Image>();
        image.color = new Color(0.70f, 0.43f, 0.15f, 0.98f);

        Button button = obj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.70f, 0.43f, 0.15f, 0.98f);
        colors.highlightedColor = new Color(0.95f, 0.62f, 0.25f, 1f);
        colors.pressedColor = new Color(0.40f, 0.24f, 0.08f, 1f);
        button.colors = colors;

        CreateText("Text", obj.transform, label, 24, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(1f, 0.94f, 0.80f));

        return button;
    }
}
#endif