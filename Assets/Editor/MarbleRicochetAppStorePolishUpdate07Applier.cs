#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MarbleRicochetAppStorePolishUpdate07Applier
{
    [MenuItem("Tools/Marble Ricochet/Apply App Store Polish Update 07")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 07.");
            return;
        }

        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (manager == null || canvas == null)
        {
            EditorUtility.DisplayDialog(
                "Update 07",
                "Could not find RPGameManager or Canvas.\n\nFirst run:\nTools > Marble Ricochet > Replace Project With Marble Ricochet",
                "OK"
            );
            return;
        }

        ApplyProductTuning(manager);
        ApplyMobileReadableLayout(manager);
        EnsureProgression(manager);
        EnsureShop(manager, canvas.transform);
        EnsureSafeArea(canvas.transform);

        manager.ApplySelectedSkin();
        manager.RefreshMetaUI();

        EditorUtility.SetDirty(manager);
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog(
            "Update 07 Applied",
            "App Store Polish Update 07 applied.\n\nThis improves mobile readability, ball position, pull strength, shop clarity, and the first 30 levels.",
            "OK"
        );
    }

    private static void ApplyProductTuning(RPGameManager manager)
    {
        manager.maxPullDistance = 2.05f;
        manager.maxLaunchPower = 10.9f;
        manager.minLaunchPower = 3.4f;
    }

    private static void EnsureProgression(RPGameManager manager)
    {
        RPProgressionManager progression = Object.FindFirstObjectByType<RPProgressionManager>();

        if (progression == null)
        {
            GameObject progressionObject = new GameObject("RPProgressionManager");
            progression = progressionObject.AddComponent<RPProgressionManager>();
        }

        manager.progression = progression;
    }

    private static void ApplyMobileReadableLayout(RPGameManager manager)
    {
        SetText(manager.titleText, 50, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.78f), new Vector2(0.5f, 0.78f), Vector2.zero, new Vector2(940f, 115f));
        SetText(manager.subtitleText, 24, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.69f), new Vector2(0.5f, 0.69f), Vector2.zero, new Vector2(900f, 60f));

        SetText(manager.menuProgressText, 20, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.61f), new Vector2(0.5f, 0.61f), Vector2.zero, new Vector2(900f, 55f));

        MoveButton("ContinueButton", new Vector2(0.5f, 0.51f), new Vector2(430f, 72f));
        MoveButton("NewGameButton", new Vector2(0.5f, 0.42f), new Vector2(360f, 64f));
        MoveButton("ShopButton", new Vector2(0.5f, 0.34f), new Vector2(330f, 60f));
        MoveButton("ResetProgressButton", new Vector2(0.5f, 0.18f), new Vector2(220f, 50f));

        SetText(manager.levelText, 20, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(30f, -22f), new Vector2(380f, 88f));
        SetText(manager.targetsText, 20, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -22f), new Vector2(180f, 75f));
        SetText(manager.shotsText, 20, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-30f, -22f), new Vector2(180f, 75f));
        SetText(manager.coinsText, 17, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(30f, -108f), new Vector2(155f, 62f));
        SetText(manager.streakText, 17, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-30f, -108f), new Vector2(155f, 62f));
        SetText(manager.hintTokensText, 17, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -108f), new Vector2(155f, 62f));
        SetText(manager.hintText, 20, TextAnchor.LowerCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 28f), new Vector2(860f, 62f));

        if (manager.progressSlider != null)
        {
            RectTransform rect = manager.progressSlider.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -102f);
            rect.sizeDelta = new Vector2(300f, 10f);
        }

        SetText(manager.winTitleText, 38, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.73f), new Vector2(0.5f, 0.73f), Vector2.zero, new Vector2(860f, 82f));
        SetText(manager.winStatsText, 25, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.58f), new Vector2(0.5f, 0.58f), Vector2.zero, new Vector2(820f, 205f));
        SetText(manager.winRewardText, 22, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.43f), new Vector2(0.5f, 0.43f), Vector2.zero, new Vector2(820f, 125f));

        MoveButton("RetryButton", new Vector2(0.5f, 0.32f), new Vector2(370f, 60f));
        MoveButton("NextButton", new Vector2(0.5f, 0.24f), new Vector2(400f, 68f));
        MoveButton("MenuButton", new Vector2(0.5f, 0.16f), new Vector2(300f, 56f));
        MoveButton("HintButton", new Vector2(0.92f, 0.08f), new Vector2(110f, 60f));
    }

    private static void EnsureShop(RPGameManager manager, Transform canvasTransform)
    {
        RPShopManager shop = Object.FindFirstObjectByType<RPShopManager>();

        if (shop == null)
        {
            GameObject shopObject = new GameObject("RPShopManager");
            shop = shopObject.AddComponent<RPShopManager>();
        }

        shop.progression = manager.progression;
        shop.gameManager = manager;
        manager.shopManager = shop;

        if (manager.menuPanel != null)
        {
            Button shopButton = GetOrCreateButton(manager.menuPanel.transform, "ShopButton", "SHOP", new Vector2(0.5f, 0.34f), new Vector2(330f, 60f));
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

        shop.skinNameText = GetOrCreateText(panel.transform, "ShopSkinNameText", "Classic Marble", 34, new Vector2(0.5f, 0.72f), new Vector2(800f, 82f), new Color(0.98f, 0.92f, 0.76f));
        shop.skinPriceText = GetOrCreateText(panel.transform, "ShopSkinPriceText", "Selected", 25, new Vector2(0.5f, 0.64f), new Vector2(800f, 62f), new Color(1f, 0.76f, 0.36f));
        shop.shopStatusText = GetOrCreateText(panel.transform, "ShopStatusText", "Coins: 0 • Hints: 0", 21, new Vector2(0.5f, 0.55f), new Vector2(850f, 64f), new Color(0.86f, 0.82f, 0.72f));
        shop.adStatusText = GetOrCreateText(panel.transform, "AdStatusText", "Rewarded ads are placeholders until SDK integration.", 18, new Vector2(0.5f, 0.29f), new Vector2(900f, 70f), new Color(0.75f, 0.88f, 1f));

        Button prev = GetOrCreateButton(panel.transform, "PrevSkinButton", "‹", new Vector2(0.25f, 0.68f), new Vector2(85f, 70f));
        Button next = GetOrCreateButton(panel.transform, "NextSkinButton", "›", new Vector2(0.75f, 0.68f), new Vector2(85f, 70f));
        Button buy = GetOrCreateButton(panel.transform, "BuySkinButton", "BUY / SELECT", new Vector2(0.5f, 0.45f), new Vector2(410f, 66f));
        Button adCoins = GetOrCreateButton(panel.transform, "RewardCoinsButton", "WATCH AD +50 COINS", new Vector2(0.5f, 0.37f), new Vector2(460f, 62f));
        Button adHint = GetOrCreateButton(panel.transform, "RewardHintButton", "WATCH AD +1 HINT", new Vector2(0.5f, 0.21f), new Vector2(420f, 58f));
        Button close = GetOrCreateButton(panel.transform, "CloseShopButton", "CLOSE", new Vector2(0.5f, 0.13f), new Vector2(300f, 54f));

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

    private static void EnsureSafeArea(Transform canvasTransform)
    {
        GameObject marker = GameObject.Find("MobileSafeAreaMarker");

        if (marker == null)
        {
            marker = new GameObject("MobileSafeAreaMarker");
            marker.transform.SetParent(canvasTransform, false);
            RectTransform rect = marker.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
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

        Text label = obj.GetComponentInChildren<Text>();
        if (label != null)
        {
            label.fontSize = Mathf.Min(label.fontSize, 24);
        }
    }

    private static Text GetOrCreateText(Transform parent, string name, string content, int fontSize, Vector2 anchor, Vector2 size, Color color)
    {
        Text text = GameObject.Find(name)?.GetComponent<Text>();

        if (text != null)
        {
            SetText(text, fontSize, TextAnchor.MiddleCenter, anchor, anchor, Vector2.zero, size);
            text.color = color;
            return text;
        }

        return CreateText(name, parent, content, fontSize, TextAnchor.MiddleCenter, anchor, anchor, Vector2.zero, size, color);
    }

    private static Button GetOrCreateButton(Transform parent, string name, string label, Vector2 anchor, Vector2 size)
    {
        Button button = GameObject.Find(name)?.GetComponent<Button>();

        if (button != null)
        {
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            return button;
        }

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

        CreateText("Text", obj.transform, label, 23, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(1f, 0.94f, 0.80f));

        return button;
    }
}
#endif