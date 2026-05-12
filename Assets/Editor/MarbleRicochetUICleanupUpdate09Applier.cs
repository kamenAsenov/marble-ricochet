#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MarbleRicochetUICleanupUpdate09Applier
{
    [MenuItem("Tools/Marble Ricochet/Apply UI Cleanup Update 09")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 09.");
            return;
        }

        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (manager == null || canvas == null)
        {
            EditorUtility.DisplayDialog(
                "Update 09",
                "Could not find RPGameManager or Canvas.\n\nFirst run:\nTools > Marble Ricochet > Replace Project With Marble Ricochet",
                "OK"
            );
            return;
        }

        ApplyMenuCleanup(manager);
        ApplyHudCleanup(manager);
        ApplyGameplayToastCleanup(manager, canvas.transform);
        ApplyWinScreenCleanup(manager);
        ApplyShopCleanup();
        ApplyButtonTextCleanup();

        manager.maxPullDistance = 2.05f;
        manager.maxLaunchPower = 10.9f;
        manager.minLaunchPower = 3.4f;

        EditorUtility.SetDirty(manager);
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog(
            "Update 09 Applied",
            "UI Cleanup Update 09 applied.\n\nMenus, win screen, onboarding and HUD spacing should now be cleaner on mobile portrait.",
            "OK"
        );
    }

    private static void ApplyMenuCleanup(RPGameManager manager)
    {
        SetText(manager.titleText, 46, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.80f), new Vector2(0.5f, 0.80f), Vector2.zero, new Vector2(940f, 105f));
        SetText(manager.subtitleText, 21, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.71f), new Vector2(0.5f, 0.71f), Vector2.zero, new Vector2(880f, 55f));
        SetText(manager.menuProgressText, 18, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.63f), new Vector2(0.5f, 0.63f), Vector2.zero, new Vector2(880f, 50f));

        SetTextObject("HowToPlayText", 17, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.265f), new Vector2(0.5f, 0.265f), Vector2.zero, new Vector2(820f, 75f));

        MoveButton("ContinueButton", new Vector2(0.5f, 0.535f), new Vector2(410f, 64f), 20);
        MoveButton("NewGameButton", new Vector2(0.5f, 0.445f), new Vector2(350f, 58f), 19);
        MoveButton("ShopButton", new Vector2(0.5f, 0.36f), new Vector2(320f, 56f), 19);
        MoveButton("ResetProgressButton", new Vector2(0.5f, 0.145f), new Vector2(220f, 46f), 17);
        MoveButton("StartButton", new Vector2(0.5f, 0.535f), new Vector2(410f, 64f), 20);
    }

    private static void ApplyHudCleanup(RPGameManager manager)
    {
        SetText(manager.levelText, 18, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(28f, -22f), new Vector2(360f, 82f));
        SetText(manager.targetsText, 18, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -22f), new Vector2(160f, 70f));
        SetText(manager.shotsText, 18, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-28f, -22f), new Vector2(160f, 70f));
        SetText(manager.coinsText, 15, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(28f, -100f), new Vector2(145f, 56f));
        SetText(manager.streakText, 15, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-28f, -100f), new Vector2(145f, 56f));
        SetText(manager.hintTokensText, 15, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -100f), new Vector2(145f, 56f));
        SetText(manager.hintText, 18, TextAnchor.LowerCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 24f), new Vector2(820f, 52f));

        if (manager.progressSlider != null)
        {
            RectTransform rect = manager.progressSlider.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -92f);
            rect.sizeDelta = new Vector2(260f, 8f);
        }

        MoveButton("HintButton", new Vector2(0.91f, 0.075f), new Vector2(104f, 54f), 17);
    }

    private static void ApplyGameplayToastCleanup(RPGameManager manager, Transform canvasTransform)
    {
        GameObject panel = GameObject.Find("OnboardingPanel");

        if (panel == null)
        {
            panel = new GameObject("OnboardingPanel");
            panel.transform.SetParent(canvasTransform, false);

            Image image = panel.AddComponent<Image>();
            image.color = new Color(0.035f, 0.028f, 0.022f, 0.82f);
            image.raycastTarget = false;
        }

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            panelRect = panel.AddComponent<RectTransform>();
        }

        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = new Vector2(0f, 118f);
        panelRect.sizeDelta = new Vector2(760f, 82f);

        Text text = GameObject.Find("OnboardingText")?.GetComponent<Text>();

        if (text == null)
        {
            text = CreateText("OnboardingText", panel.transform, "Pull back.\nRelease to shoot.", 20, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0.98f, 0.92f, 0.76f));
        }
        else
        {
            text.fontSize = 20;
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        RPOnboardingManager onboarding = Object.FindFirstObjectByType<RPOnboardingManager>();

        if (onboarding == null)
        {
            GameObject obj = new GameObject("RPOnboardingManager");
            onboarding = obj.AddComponent<RPOnboardingManager>();
        }

        onboarding.gameManager = manager;
        onboarding.onboardingPanel = panel;
        onboarding.onboardingText = text;
        panel.SetActive(false);
    }

    private static void ApplyWinScreenCleanup(RPGameManager manager)
    {
        SetText(manager.winTitleText, 34, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.78f), new Vector2(0.5f, 0.78f), Vector2.zero, new Vector2(820f, 78f));
        SetText(manager.winStatsText, 21, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.625f), new Vector2(0.5f, 0.625f), Vector2.zero, new Vector2(760f, 155f));
        SetText(manager.winRewardText, 18, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.49f), new Vector2(0.5f, 0.49f), Vector2.zero, new Vector2(760f, 82f));

        Text nextPreview = GameObject.Find("NextLevelPreviewText")?.GetComponent<Text>();

        if (nextPreview == null && manager.winPanel != null)
        {
            nextPreview = CreateText(
                "NextLevelPreviewText",
                manager.winPanel.transform,
                "Next level",
                16,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.385f),
                new Vector2(0.5f, 0.385f),
                Vector2.zero,
                new Vector2(760f, 46f),
                new Color(0.78f, 0.90f, 1f)
            );
        }
        else
        {
            SetText(nextPreview, 16, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.385f), new Vector2(0.5f, 0.385f), Vector2.zero, new Vector2(760f, 46f));
        }

        RPWinFlowEnhancer enhancer = Object.FindFirstObjectByType<RPWinFlowEnhancer>();

        if (enhancer == null)
        {
            GameObject obj = new GameObject("RPWinFlowEnhancer");
            enhancer = obj.AddComponent<RPWinFlowEnhancer>();
        }

        enhancer.gameManager = manager;
        enhancer.winPanel = manager.winPanel;
        enhancer.nextPreviewText = nextPreview;

        MoveButton("RetryButton", new Vector2(0.5f, 0.315f), new Vector2(320f, 52f), 17);
        MoveButton("NextButton", new Vector2(0.5f, 0.235f), new Vector2(360f, 62f), 19);
        MoveButton("MenuButton", new Vector2(0.5f, 0.155f), new Vector2(280f, 50f), 17);
    }

    private static void ApplyShopCleanup()
    {
        SetTextObject("ShopSkinNameText", 28, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), Vector2.zero, new Vector2(760f, 70f));
        SetTextObject("ShopSkinPriceText", 21, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.64f), new Vector2(0.5f, 0.64f), Vector2.zero, new Vector2(760f, 52f));
        SetTextObject("ShopStatusText", 18, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.555f), new Vector2(0.5f, 0.555f), Vector2.zero, new Vector2(800f, 52f));
        SetTextObject("AdStatusText", 15, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.285f), new Vector2(0.5f, 0.285f), Vector2.zero, new Vector2(820f, 58f));

        MoveButton("PrevSkinButton", new Vector2(0.24f, 0.68f), new Vector2(75f, 62f), 22);
        MoveButton("NextSkinButton", new Vector2(0.76f, 0.68f), new Vector2(75f, 62f), 22);
        MoveButton("BuySkinButton", new Vector2(0.5f, 0.45f), new Vector2(360f, 56f), 18);
        MoveButton("RewardCoinsButton", new Vector2(0.5f, 0.37f), new Vector2(395f, 52f), 17);
        MoveButton("RewardHintButton", new Vector2(0.5f, 0.215f), new Vector2(370f, 50f), 17);
        MoveButton("CloseShopButton", new Vector2(0.5f, 0.135f), new Vector2(260f, 46f), 16);
    }

    private static void ApplyButtonTextCleanup()
    {
        string[] buttonNames =
        {
            "ContinueButton",
            "NewGameButton",
            "ShopButton",
            "ResetProgressButton",
            "RetryButton",
            "NextButton",
            "MenuButton",
            "HintButton",
            "BuySkinButton",
            "RewardCoinsButton",
            "RewardHintButton",
            "CloseShopButton"
        };

        foreach (string name in buttonNames)
        {
            GameObject obj = GameObject.Find(name);

            if (obj == null)
            {
                continue;
            }

            Text label = obj.GetComponentInChildren<Text>();

            if (label != null)
            {
                label.horizontalOverflow = HorizontalWrapMode.Wrap;
                label.verticalOverflow = VerticalWrapMode.Truncate;
                label.resizeTextForBestFit = false;
            }
        }
    }

    private static void SetTextObject(string name, int fontSize, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        Text text = GameObject.Find(name)?.GetComponent<Text>();
        SetText(text, fontSize, alignment, anchorMin, anchorMax, anchoredPosition, sizeDelta);
    }

    private static void SetText(Text text, int fontSize, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        if (text == null) return;

        text.fontSize = fontSize;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;

        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
    }

    private static void MoveButton(string name, Vector2 anchor, Vector2 size, int fontSize)
    {
        GameObject obj = GameObject.Find(name);
        if (obj == null) return;

        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
        }

        Text label = obj.GetComponentInChildren<Text>();

        if (label != null)
        {
            label.fontSize = fontSize;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
        }
    }

    private static Text CreateText(
        string name,
        Transform parent,
        string content,
        int fontSize,
        TextAnchor alignment,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 sizeDelta,
        Color color
    )
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
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;

        Shadow shadow = obj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.86f);
        shadow.effectDistance = new Vector2(2f, -2f);

        return text;
    }
}
#endif