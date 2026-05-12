#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MarbleRicochetProgressionUpdate05Applier
{
    [MenuItem("Tools/Marble Ricochet/Apply Progression Update 05")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 05.");
            return;
        }

        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (manager == null || canvas == null)
        {
            EditorUtility.DisplayDialog(
                "Update 05",
                "Could not find RPGameManager or Canvas.\n\nFirst run:\nTools > Marble Ricochet > Replace Project With Marble Ricochet",
                "OK"
            );
            return;
        }

        RPProgressionManager progression = Object.FindFirstObjectByType<RPProgressionManager>();

        if (progression == null)
        {
            GameObject progressionObject = new GameObject("RPProgressionManager");
            progression = progressionObject.AddComponent<RPProgressionManager>();
        }

        manager.progression = progression;

        GameObject menuPanel = manager.menuPanel != null ? manager.menuPanel : GameObject.Find("MenuPanel");

        if (menuPanel != null)
        {
            Text progressText = GameObject.Find("MenuProgressText")?.GetComponent<Text>();
            if (progressText == null)
            {
                progressText = CreateText(
                    "MenuProgressText",
                    menuPanel.transform,
                    "Level 1   •   Stars 0   •   Coins 0",
                    24,
                    TextAnchor.MiddleCenter,
                    new Vector2(0.5f, 0.52f),
                    new Vector2(0.5f, 0.52f),
                    Vector2.zero,
                    new Vector2(900f, 70f),
                    new Color(1f, 0.76f, 0.36f)
                );
            }

            manager.menuProgressText = progressText;

            Button continueButton = GameObject.Find("ContinueButton")?.GetComponent<Button>();
            if (continueButton == null)
            {
                continueButton = CreateButton(
                    "ContinueButton",
                    menuPanel.transform,
                    "CONTINUE",
                    new Vector2(0.5f, 0.47f),
                    new Vector2(440f, 86f)
                );
            }

            Button newGameButton = GameObject.Find("NewGameButton")?.GetComponent<Button>();
            if (newGameButton == null)
            {
                newGameButton = CreateButton(
                    "NewGameButton",
                    menuPanel.transform,
                    "NEW GAME",
                    new Vector2(0.5f, 0.37f),
                    new Vector2(380f, 76f)
                );
            }

            Button resetButton = GameObject.Find("ResetProgressButton")?.GetComponent<Button>();
            if (resetButton == null)
            {
                resetButton = CreateButton(
                    "ResetProgressButton",
                    menuPanel.transform,
                    "RESET",
                    new Vector2(0.5f, 0.18f),
                    new Vector2(260f, 62f)
                );
            }

            continueButton.onClick.RemoveAllListeners();
            newGameButton.onClick.RemoveAllListeners();
            resetButton.onClick.RemoveAllListeners();

            UnityEventTools.AddPersistentListener(continueButton.onClick, manager.ContinueGame);
            UnityEventTools.AddPersistentListener(newGameButton.onClick, manager.StartNewGame);
            UnityEventTools.AddPersistentListener(resetButton.onClick, manager.ResetProgressAndNewGame);

            GameObject oldStart = GameObject.Find("StartButton");
            if (oldStart != null)
            {
                oldStart.SetActive(false);
            }
        }

        GameObject winPanel = manager.winPanel != null ? manager.winPanel : GameObject.Find("WinPanel");

        if (winPanel != null)
        {
            Button retryButton = GameObject.Find("RetryButton")?.GetComponent<Button>();

            if (retryButton == null)
            {
                retryButton = CreateButton(
                    "RetryButton",
                    winPanel.transform,
                    "RETRY FOR 3★",
                    new Vector2(0.5f, 0.34f),
                    new Vector2(410f, 74f)
                );
            }

            retryButton.onClick.RemoveAllListeners();
            UnityEventTools.AddPersistentListener(retryButton.onClick, manager.RestartLevel);

            Button nextButton = GameObject.Find("NextButton")?.GetComponent<Button>();
            if (nextButton != null)
            {
                RectTransform nextRect = nextButton.GetComponent<RectTransform>();
                nextRect.anchorMin = new Vector2(0.5f, 0.24f);
                nextRect.anchorMax = new Vector2(0.5f, 0.24f);
                nextRect.anchoredPosition = Vector2.zero;
            }
        }

        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(progression);
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog(
            "Update 05 Applied",
            "Progression Update 05 applied.\n\nMain menu now has Continue / New Game / Reset.\nWin screen now has Retry for 3 stars.",
            "OK"
        );
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

        CreateText("Text", obj.transform, label, 28, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(1f, 0.94f, 0.80f));

        return button;
    }
}
#endif