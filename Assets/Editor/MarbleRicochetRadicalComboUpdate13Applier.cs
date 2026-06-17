#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MarbleRicochetRadicalComboUpdate13Applier
{
    [MenuItem("Tools/Marble Ricochet/Apply Radical Combo Update 13")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 13.");
            return;
        }

        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (manager == null || canvas == null)
        {
            EditorUtility.DisplayDialog(
                "Update 13",
                "Could not find RPGameManager or Canvas.\n\nOpen the Marble Ricochet scene first.",
                "OK"
            );
            return;
        }

        RPComboSkillManager combo = EnsureComboManager(manager);
        CreateComboUI(canvas.transform, manager, combo);
        ImproveWinScreenForScoreBreakdown(manager);
        ImproveMenuCampaignFeel(manager);
        SaveScene();

        EditorUtility.DisplayDialog(
            "Update 13 Applied",
            "Radical Combo Update 13 applied.\n\nAdded skill-shot recognition, combos, bank shot bonuses, level objectives and score breakdown.",
            "OK"
        );
    }

    private static RPComboSkillManager EnsureComboManager(RPGameManager manager)
    {
        RPComboSkillManager combo = Object.FindFirstObjectByType<RPComboSkillManager>();

        if (combo == null)
        {
            GameObject obj = new GameObject("RPComboSkillManager");
            combo = obj.AddComponent<RPComboSkillManager>();
        }

        combo.gameManager = manager;
        combo.progression = Object.FindFirstObjectByType<RPProgressionManager>();

        EditorUtility.SetDirty(combo);
        return combo;
    }

    private static void CreateComboUI(Transform canvasTransform, RPGameManager manager, RPComboSkillManager combo)
    {
        Text comboText = GameObject.Find("SkillComboText")?.GetComponent<Text>();

        if (comboText == null)
        {
            comboText = CreateText(
                "SkillComboText",
                canvasTransform,
                "Bank Shot!",
                42,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.64f),
                new Vector2(0.5f, 0.64f),
                Vector2.zero,
                new Vector2(900f, 90f),
                new Color(1f, 0.78f, 0.30f)
            );

            comboText.gameObject.AddComponent<RPComboTextFloat>();
        }

        comboText.gameObject.SetActive(false);

        Text scoreBreakdown = GameObject.Find("SkillScoreBreakdownText")?.GetComponent<Text>();

        if (scoreBreakdown == null && manager.winPanel != null)
        {
            scoreBreakdown = CreateText(
                "SkillScoreBreakdownText",
                manager.winPanel.transform,
                "Skill score: 0",
                17,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.405f),
                new Vector2(0.5f, 0.405f),
                Vector2.zero,
                new Vector2(780f, 86f),
                new Color(0.82f, 0.95f, 1f)
            );
        }

        GameObject objectivePanel = GameObject.Find("LevelObjectivePanel");

        if (objectivePanel == null)
        {
            objectivePanel = new GameObject("LevelObjectivePanel");
            objectivePanel.transform.SetParent(canvasTransform, false);

            RectTransform rect = objectivePanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.74f);
            rect.anchorMax = new Vector2(0.5f, 0.74f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(850f, 118f);

            Image image = objectivePanel.AddComponent<Image>();
            image.color = new Color(0.03f, 0.024f, 0.020f, 0.84f);
            image.raycastTarget = false;
        }

        Text objectiveText = GameObject.Find("LevelObjectiveText")?.GetComponent<Text>();

        if (objectiveText == null)
        {
            objectiveText = CreateText(
                "LevelObjectiveText",
                objectivePanel.transform,
                "Chapter 1\nBreak the glass.",
                22,
                TextAnchor.MiddleCenter,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.98f, 0.91f, 0.72f)
            );
        }

        objectivePanel.SetActive(false);

        combo.comboText = comboText;
        combo.scoreBreakdownText = scoreBreakdown;
        combo.objectivePanel = objectivePanel;
        combo.objectiveText = objectiveText;

        EditorUtility.SetDirty(combo);
    }

    private static void ImproveWinScreenForScoreBreakdown(RPGameManager manager)
    {
        if (manager.winStatsText != null)
        {
            RectTransform rect = manager.winStatsText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.66f);
            rect.anchorMax = new Vector2(0.5f, 0.66f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(760f, 132f);
            manager.winStatsText.fontSize = 20;
            EditorUtility.SetDirty(manager.winStatsText);
        }

        if (manager.winRewardText != null)
        {
            RectTransform rect = manager.winRewardText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.50f);
            rect.anchorMax = new Vector2(0.5f, 0.50f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(760f, 70f);
            manager.winRewardText.fontSize = 18;
            EditorUtility.SetDirty(manager.winRewardText);
        }

        MoveButton("RetryButton", new Vector2(0.5f, 0.29f), new Vector2(310f, 50f), 16);
        MoveButton("NextButton", new Vector2(0.5f, 0.22f), new Vector2(350f, 58f), 18);
        MoveButton("MenuButton", new Vector2(0.5f, 0.15f), new Vector2(270f, 48f), 16);
    }

    private static void ImproveMenuCampaignFeel(RPGameManager manager)
    {
        Text campaignText = GameObject.Find("CampaignTaglineText")?.GetComponent<Text>();

        if (campaignText == null && manager.menuPanel != null)
        {
            campaignText = CreateText(
                "CampaignTaglineText",
                manager.menuPanel.transform,
                "6 chapters • 30 handmade skill levels",
                17,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.285f),
                new Vector2(0.5f, 0.285f),
                Vector2.zero,
                new Vector2(820f, 45f),
                new Color(0.78f, 0.90f, 1f)
            );
        }

        Text howTo = GameObject.Find("HowToPlayText")?.GetComponent<Text>();

        if (howTo != null)
        {
            howTo.text = "Master bank shots, combos and perfect routes.";
            howTo.fontSize = 16;
            EditorUtility.SetDirty(howTo);
        }
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

    private static void SaveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.IsValid())
        {
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveOpenScenes();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif