#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class LoopRoomGameFeelApplier
{
    [MenuItem("Tools/Loop Room/Apply Game Feel Update 06")]
    public static void ApplyGameFeelUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 06.");
            return;
        }

        LoopManager loopManager = Object.FindFirstObjectByType<LoopManager>();
        GameStateUI ui = Object.FindFirstObjectByType<GameStateUI>();

        if (loopManager == null || ui == null)
        {
            EditorUtility.DisplayDialog(
                "Loop Room Update 06",
                "Could not find LoopManager or GameStateUI. First run Tools > Loop Room > Build MVP Scene, then run this update.",
                "OK"
            );
            return;
        }

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            EditorUtility.DisplayDialog(
                "Loop Room Update 06",
                "Could not find Canvas. First run Tools > Loop Room > Build MVP Scene, then run this update.",
                "OK"
            );
            return;
        }

        Text centerPrompt = GameObject.Find("CenterPromptText")?.GetComponent<Text>();

        if (centerPrompt == null)
        {
            centerPrompt = CreateUIText(
                "CenterPromptText",
                canvas.transform,
                "",
                48,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(980f, 360f)
            );

            centerPrompt.enabled = false;
        }

        ui.centerPromptText = centerPrompt;

        FadeController fadeController = canvas.GetComponent<FadeController>();

        if (fadeController == null)
        {
            fadeController = canvas.gameObject.AddComponent<FadeController>();
        }

        Image fadeImage = GameObject.Find("FadeOverlay")?.GetComponent<Image>();

        if (fadeImage == null)
        {
            fadeImage = CreateOverlayImage("FadeOverlay", canvas.transform, new Color(0f, 0f, 0f, 0f));
        }

        fadeController.fadeImage = fadeImage;
        loopManager.fadeController = fadeController;

        EditorUtility.SetDirty(ui);
        EditorUtility.SetDirty(loopManager);
        EditorUtility.SetDirty(fadeController);
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog(
            "Loop Room Update 06",
            "Game Feel Update 06 applied.\n\nNow press Play.\nCorrect decisions should fade to black and restart smoothly.",
            "OK"
        );

        Debug.Log("Loop Room Game Feel Update 06 applied successfully.");
    }

    private static Image CreateOverlayImage(string name, Transform parent, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = obj.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        return image;
    }

    private static Text CreateUIText(
        string name,
        Transform parent,
        string text,
        int fontSize,
        TextAnchor alignment,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 sizeDelta
    )
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = anchorMin;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        Text uiText = obj.AddComponent<Text>();
        uiText.text = text;
        uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        uiText.fontSize = fontSize;
        uiText.alignment = alignment;
        uiText.color = new Color(0.96f, 0.90f, 0.76f);
        uiText.raycastTarget = false;

        Shadow shadow = obj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.85f);
        shadow.effectDistance = new Vector2(2f, -2f);

        return uiText;
    }
}
#endif