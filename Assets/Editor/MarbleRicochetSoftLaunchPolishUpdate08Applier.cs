#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MarbleRicochetSoftLaunchPolishUpdate08Applier
{
    [MenuItem("Tools/Marble Ricochet/Apply Soft Launch Polish Update 08")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 08.");
            return;
        }

        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (manager == null || canvas == null)
        {
            EditorUtility.DisplayDialog(
                "Update 08",
                "Could not find RPGameManager or Canvas.\n\nFirst run:\nTools > Marble Ricochet > Replace Project With Marble Ricochet",
                "OK"
            );
            return;
        }

        ConfigureMobileBuildSettings();
        ApplyButtonJuice();
        ApplyPanelEntrances(manager);
        EnsureOnboarding(canvas.transform, manager);
        EnsureWinFlow(canvas.transform, manager);
        PolishShop(manager);
        CleanOldObjects();

        manager.minLaunchPower = 3.4f;
        manager.maxLaunchPower = 10.9f;
        manager.maxPullDistance = 2.05f;

        EditorUtility.SetDirty(manager);
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog(
            "Update 08 Applied",
            "Soft Launch Polish Update 08 applied.\n\nTest onboarding, UI animations, shop flow, win screen and portrait settings.",
            "OK"
        );
    }

    private static void ConfigureMobileBuildSettings()
    {
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
        PlayerSettings.productName = "Marble Ricochet";
        PlayerSettings.companyName = "Indie Marble Studio";
    }

    private static void ApplyButtonJuice()
    {
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            if (button.GetComponent<RPUIMicroButton>() == null)
            {
                button.gameObject.AddComponent<RPUIMicroButton>();
            }
        }
    }

    private static void ApplyPanelEntrances(RPGameManager manager)
    {
        AddPanelEntrance(manager.menuPanel);
        AddPanelEntrance(manager.winPanel);

        GameObject shopPanel = GameObject.Find("ShopPanel");
        AddPanelEntrance(shopPanel);
    }

    private static void AddPanelEntrance(GameObject panel)
    {
        if (panel == null)
        {
            return;
        }

        if (panel.GetComponent<RPSoftPanelEntrance>() == null)
        {
            panel.AddComponent<RPSoftPanelEntrance>();
        }
    }

    private static void EnsureOnboarding(Transform canvasTransform, RPGameManager manager)
    {
        GameObject panel = GameObject.Find("OnboardingPanel");

        if (panel == null)
        {
            panel = new GameObject("OnboardingPanel");
            panel.transform.SetParent(canvasTransform, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 145f);
            rect.sizeDelta = new Vector2(820f, 112f);

            Image image = panel.AddComponent<Image>();
            image.color = new Color(0.035f, 0.028f, 0.022f, 0.82f);
            image.raycastTarget = false;
        }

        Text text = GameObject.Find("OnboardingText")?.GetComponent<Text>();

        if (text == null)
        {
            text = CreateText(
                "OnboardingText",
                panel.transform,
                "Pull away from the marble.\nRelease to shoot.",
                24,
                TextAnchor.MiddleCenter,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.98f, 0.92f, 0.76f)
            );
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

    private static void EnsureWinFlow(Transform canvasTransform, RPGameManager manager)
    {
        if (manager.winPanel == null)
        {
            return;
        }

        Text nextPreview = GameObject.Find("NextLevelPreviewText")?.GetComponent<Text>();

        if (nextPreview == null)
        {
            nextPreview = CreateText(
                "NextLevelPreviewText",
                manager.winPanel.transform,
                "Next: Level 2",
                19,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.365f),
                new Vector2(0.5f, 0.365f),
                Vector2.zero,
                new Vector2(840f, 70f),
                new Color(0.78f, 0.90f, 1f)
            );
        }
        else
        {
            SetRect(nextPreview.GetComponent<RectTransform>(), new Vector2(0.5f, 0.365f), new Vector2(840f, 70f));
            nextPreview.fontSize = 19;
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

        MoveButton("RetryButton", new Vector2(0.5f, 0.30f), new Vector2(350f, 58f), 21);
        MoveButton("NextButton", new Vector2(0.5f, 0.225f), new Vector2(390f, 66f), 22);
        MoveButton("MenuButton", new Vector2(0.5f, 0.15f), new Vector2(290f, 54f), 20);
    }

    private static void PolishShop(RPGameManager manager)
    {
        RPShopManager shop = Object.FindFirstObjectByType<RPShopManager>();

        if (shop == null)
        {
            GameObject shopObj = new GameObject("RPShopManager");
            shop = shopObj.AddComponent<RPShopManager>();
        }

        shop.gameManager = manager;
        shop.progression = manager.progression;
        manager.shopManager = shop;

        SetTextObject("ShopSkinNameText", 31, new Vector2(0.5f, 0.72f), new Vector2(800f, 76f));
        SetTextObject("ShopSkinPriceText", 23, new Vector2(0.5f, 0.64f), new Vector2(800f, 56f));
        SetTextObject("ShopStatusText", 20, new Vector2(0.5f, 0.55f), new Vector2(850f, 58f));
        SetTextObject("AdStatusText", 17, new Vector2(0.5f, 0.285f), new Vector2(880f, 66f));

        MoveButton("BuySkinButton", new Vector2(0.5f, 0.45f), new Vector2(390f, 62f), 21);
        MoveButton("RewardCoinsButton", new Vector2(0.5f, 0.37f), new Vector2(430f, 58f), 20);
        MoveButton("RewardHintButton", new Vector2(0.5f, 0.21f), new Vector2(400f, 54f), 20);
        MoveButton("CloseShopButton", new Vector2(0.5f, 0.13f), new Vector2(280f, 50f), 19);
    }

    private static void CleanOldObjects()
    {
        // Intentionally conservative. We avoid deleting scene objects that may still be used.
        // Old RicochetPopSceneBuilder is disabled by replacement file in this package.
    }

    private static void SetTextObject(string name, int fontSize, Vector2 anchor, Vector2 size)
    {
        Text text = GameObject.Find(name)?.GetComponent<Text>();

        if (text == null)
        {
            return;
        }

        text.fontSize = fontSize;
        SetRect(text.GetComponent<RectTransform>(), anchor, size);
    }

    private static void MoveButton(string name, Vector2 anchor, Vector2 size, int fontSize)
    {
        GameObject obj = GameObject.Find(name);
        if (obj == null)
        {
            return;
        }

        RectTransform rect = obj.GetComponent<RectTransform>();

        if (rect != null)
        {
            SetRect(rect, anchor, size);
        }

        Text label = obj.GetComponentInChildren<Text>();

        if (label != null)
        {
            label.fontSize = fontSize;
        }

        if (obj.GetComponent<RPUIMicroButton>() == null)
        {
            obj.AddComponent<RPUIMicroButton>();
        }
    }

    private static void SetRect(RectTransform rect, Vector2 anchor, Vector2 size)
    {
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
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

        Shadow shadow = obj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.86f);
        shadow.effectDistance = new Vector2(2f, -2f);

        return text;
    }
}
#endif