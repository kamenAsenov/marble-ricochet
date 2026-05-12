#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class LoopRoomMobileUXApplier
{
    [MenuItem("Tools/Loop Room/Apply Mobile UX Update 07")]
    public static void ApplyMobileUXUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 07.");
            return;
        }

        LoopManager loopManager = Object.FindFirstObjectByType<LoopManager>();
        PlayerMovement playerMovement = Object.FindFirstObjectByType<PlayerMovement>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (loopManager == null || playerMovement == null || canvas == null)
        {
            EditorUtility.DisplayDialog(
                "Loop Room Update 07",
                "Missing LoopManager, PlayerMovement or Canvas.\n\nFirst run Build MVP Scene and Apply Game Feel Update 06.",
                "OK"
            );
            return;
        }

        GameUXManager oldUX = Object.FindFirstObjectByType<GameUXManager>();

        if (oldUX != null)
        {
            Object.DestroyImmediate(oldUX.gameObject);
        }

        GameObject uxObject = new GameObject("GameUXManager");
        GameUXManager ux = uxObject.AddComponent<GameUXManager>();
        ux.loopManager = loopManager;
        ux.playerMovement = playerMovement;

        MobileInputBridge mobileInput = CreateMobileControls(canvas.transform);
        ux.mobileInput = mobileInput;
        playerMovement.mobileInput = mobileInput;

        CreatePanels(canvas.transform, ux);

        EditorUtility.SetDirty(ux);
        EditorUtility.SetDirty(playerMovement);
        EditorUtility.SetDirty(mobileInput);
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog(
            "Loop Room Update 07",
            "Mobile UX Update 07 applied.\n\nPress Play. You should now see a Main Menu.",
            "OK"
        );

        Debug.Log("Loop Room Mobile UX Update 07 applied successfully.");
    }

    private static MobileInputBridge CreateMobileControls(Transform canvasTransform)
    {
        GameObject oldRoot = GameObject.Find("MobileControlsRoot");
        if (oldRoot != null)
        {
            Object.DestroyImmediate(oldRoot);
        }

        GameObject root = new GameObject("MobileControlsRoot");
        root.transform.SetParent(canvasTransform, false);

        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        Image lookZone = CreateImage("LookZone_Right", root.transform, new Color(1f, 1f, 1f, 0.0f));
        RectTransform lookRect = lookZone.GetComponent<RectTransform>();
        lookRect.anchorMin = new Vector2(0.5f, 0f);
        lookRect.anchorMax = new Vector2(1f, 1f);
        lookRect.offsetMin = Vector2.zero;
        lookRect.offsetMax = Vector2.zero;

        Image joyBg = CreateImage("Joystick_Background", root.transform, new Color(1f, 1f, 1f, 0.12f));
        RectTransform joyBgRect = joyBg.GetComponent<RectTransform>();
        joyBgRect.anchorMin = new Vector2(0f, 0f);
        joyBgRect.anchorMax = new Vector2(0f, 0f);
        joyBgRect.pivot = new Vector2(0.5f, 0.5f);
        joyBgRect.anchoredPosition = new Vector2(180f, 170f);
        joyBgRect.sizeDelta = new Vector2(180f, 180f);

        Image joyKnob = CreateImage("Joystick_Knob", root.transform, new Color(1f, 1f, 1f, 0.32f));
        RectTransform joyKnobRect = joyKnob.GetComponent<RectTransform>();
        joyKnobRect.anchorMin = new Vector2(0f, 0f);
        joyKnobRect.anchorMax = new Vector2(0f, 0f);
        joyKnobRect.pivot = new Vector2(0.5f, 0.5f);
        joyKnobRect.anchoredPosition = new Vector2(180f, 170f);
        joyKnobRect.sizeDelta = new Vector2(78f, 78f);

        MobileInputBridge bridge = root.AddComponent<MobileInputBridge>();
        bridge.mobileControlsRoot = root;
        bridge.joystickBackground = joyBgRect;
        bridge.joystickKnob = joyKnobRect;
        bridge.lookZone = lookRect;
        bridge.mobileControlsEnabled = true;

        root.SetActive(false);
        return bridge;
    }

    private static void CreatePanels(Transform canvasTransform, GameUXManager ux)
    {
        GameObject oldPanels = GameObject.Find("UX_PANELS");
        if (oldPanels != null)
        {
            Object.DestroyImmediate(oldPanels);
        }

        GameObject parent = new GameObject("UX_PANELS");
        parent.transform.SetParent(canvasTransform, false);

        RectTransform parentRect = parent.AddComponent<RectTransform>();
        parentRect.anchorMin = Vector2.zero;
        parentRect.anchorMax = Vector2.one;
        parentRect.offsetMin = Vector2.zero;
        parentRect.offsetMax = Vector2.zero;

        ux.mainMenuPanel = CreateMainMenu(parent.transform, ux);
        ux.howToPlayPanel = CreateHowToPlay(parent.transform, ux);
        ux.pausePanel = CreatePauseMenu(parent.transform, ux);
        ux.settingsPanel = CreateSettings(parent.transform, ux);

        ux.howToPlayPanel.SetActive(false);
        ux.pausePanel.SetActive(false);
        ux.settingsPanel.SetActive(false);
    }

    private static GameObject CreateMainMenu(Transform parent, GameUXManager ux)
    {
        GameObject panel = CreatePanel("MainMenuPanel", parent, new Color(0f, 0f, 0f, 0.74f));

        Text title = CreateText("Title", panel.transform, "LOOP ROOM", 72, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), new Vector2(0f, 0f), new Vector2(900f, 120f));
        Text subtitle = CreateText("Subtitle", panel.transform, "Did something change?", 30, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.62f), new Vector2(0.5f, 0.62f), Vector2.zero, new Vector2(800f, 70f));

        Button start = CreateButton("StartButton", panel.transform, "START", new Vector2(0.5f, 0.47f), new Vector2(360f, 72f));
        Button how = CreateButton("HowToPlayButton", panel.transform, "HOW TO PLAY", new Vector2(0.5f, 0.36f), new Vector2(360f, 72f));
        Button settings = CreateButton("SettingsButton", panel.transform, "SETTINGS", new Vector2(0.5f, 0.25f), new Vector2(360f, 72f));

        UnityEventTools.AddPersistentListener(start.onClick, ux.StartGame);
        UnityEventTools.AddPersistentListener(how.onClick, ux.ShowHowToPlay);
        UnityEventTools.AddPersistentListener(settings.onClick, ux.ShowSettings);

        return panel;
    }

    private static GameObject CreateHowToPlay(Transform parent, GameUXManager ux)
    {
        GameObject panel = CreatePanel("HowToPlayPanel", parent, new Color(0f, 0f, 0f, 0.82f));

        CreateText(
            "HowText",
            panel.transform,
            "HOW TO PLAY\n\nWalk through the corridor.\nMemorize what belongs there.\n\nIf everything is normal: continue forward.\nIf something changed: turn back.\n\nOne wrong decision ends the run.",
            34,
            TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.56f),
            new Vector2(0.5f, 0.56f),
            Vector2.zero,
            new Vector2(1100f, 520f)
        );

        Button back = CreateButton("BackButton", panel.transform, "BACK", new Vector2(0.5f, 0.17f), new Vector2(320f, 68f));
        UnityEventTools.AddPersistentListener(back.onClick, ux.BackToMainMenu);

        return panel;
    }

    private static GameObject CreatePauseMenu(Transform parent, GameUXManager ux)
    {
        GameObject panel = CreatePanel("PausePanel", parent, new Color(0f, 0f, 0f, 0.72f));

        CreateText("PauseTitle", panel.transform, "PAUSED", 58, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.68f), new Vector2(0.5f, 0.68f), Vector2.zero, new Vector2(700f, 100f));

        Button resume = CreateButton("ResumeButton", panel.transform, "RESUME", new Vector2(0.5f, 0.50f), new Vector2(340f, 70f));
        Button restart = CreateButton("RestartButton", panel.transform, "RESTART RUN", new Vector2(0.5f, 0.39f), new Vector2(340f, 70f));
        Button menu = CreateButton("MenuButton", panel.transform, "MAIN MENU", new Vector2(0.5f, 0.28f), new Vector2(340f, 70f));

        UnityEventTools.AddPersistentListener(resume.onClick, ux.ResumeGame);
        UnityEventTools.AddPersistentListener(restart.onClick, ux.RestartRun);
        UnityEventTools.AddPersistentListener(menu.onClick, ux.QuitToMenu);

        panel.SetActive(false);
        return panel;
    }

    private static GameObject CreateSettings(Transform parent, GameUXManager ux)
    {
        GameObject panel = CreatePanel("SettingsPanel", parent, new Color(0f, 0f, 0f, 0.82f));

        CreateText("SettingsTitle", panel.transform, "SETTINGS", 58, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.76f), new Vector2(0.5f, 0.76f), Vector2.zero, new Vector2(700f, 100f));

        CreateText("SensitivityLabel", panel.transform, "Sensitivity", 30, TextAnchor.MiddleLeft, new Vector2(0.5f, 0.58f), new Vector2(0.5f, 0.58f), new Vector2(-320f, 0f), new Vector2(260f, 70f));

        Slider sensitivity = CreateSlider("SensitivitySlider", panel.transform, new Vector2(0.5f, 0.58f), new Vector2(420f, 44f));
        Text sensitivityValue = CreateText("SensitivityValue", panel.transform, "2.0", 28, TextAnchor.MiddleLeft, new Vector2(0.5f, 0.58f), new Vector2(0.5f, 0.58f), new Vector2(290f, 0f), new Vector2(130f, 60f));

        Toggle soundToggle = CreateToggle("SoundToggle", panel.transform, "Sound", new Vector2(0.5f, 0.45f));
        Toggle mobileToggle = CreateToggle("MobileControlsToggle", panel.transform, "Mobile controls", new Vector2(0.5f, 0.36f));

        ux.sensitivitySlider = sensitivity;
        ux.sensitivityValueText = sensitivityValue;
        ux.soundToggle = soundToggle;
        ux.mobileControlsToggle = mobileToggle;

        UnityEventTools.AddPersistentListener(sensitivity.onValueChanged, ux.OnSensitivityChanged);
        UnityEventTools.AddPersistentListener(soundToggle.onValueChanged, ux.OnSoundToggleChanged);
        UnityEventTools.AddPersistentListener(mobileToggle.onValueChanged, ux.OnMobileControlsToggleChanged);

        Button back = CreateButton("BackButton", panel.transform, "BACK", new Vector2(0.5f, 0.18f), new Vector2(320f, 68f));
        UnityEventTools.AddPersistentListener(back.onClick, ux.BackToMainMenu);

        return panel;
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

    private static Text CreateText(string name, Transform parent, string content, int fontSize, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
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
        text.color = new Color(0.96f, 0.90f, 0.76f);
        text.raycastTarget = false;

        Shadow shadow = obj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.9f);
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
        image.color = new Color(0.18f, 0.12f, 0.08f, 0.94f);

        Button button = obj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.18f, 0.12f, 0.08f, 0.94f);
        colors.highlightedColor = new Color(0.34f, 0.22f, 0.14f, 0.96f);
        colors.pressedColor = new Color(0.10f, 0.07f, 0.05f, 1f);
        button.colors = colors;

        CreateText("Text", obj.transform, label, 28, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        return button;
    }

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        Image image = obj.AddComponent<Image>();
        image.color = color;

        return image;
    }

    private static Slider CreateSlider(string name, Transform parent, Vector2 anchor, Vector2 size)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);

        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;

        Slider slider = root.AddComponent<Slider>();
        slider.minValue = 0.5f;
        slider.maxValue = 4.0f;
        slider.value = 2f;

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(root.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.20f, 0.16f, 0.12f, 1f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0.35f);
        bgRect.anchorMax = new Vector2(1f, 0.65f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(root.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.72f, 0.48f, 0.25f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(root.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = new Color(0.96f, 0.90f, 0.76f, 1f);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(34f, 34f);

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.direction = Slider.Direction.LeftToRight;

        return slider;
    }

    private static Toggle CreateToggle(string name, Transform parent, string label, Vector2 anchor)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);

        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(420f, 60f);

        Toggle toggle = root.AddComponent<Toggle>();
        toggle.isOn = true;

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(root.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.20f, 0.16f, 0.12f, 1f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0.5f);
        bgRect.anchorMax = new Vector2(0f, 0.5f);
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.anchoredPosition = new Vector2(30f, 0f);
        bgRect.sizeDelta = new Vector2(36f, 36f);

        GameObject check = new GameObject("Checkmark");
        check.transform.SetParent(bg.transform, false);
        Image checkImage = check.AddComponent<Image>();
        checkImage.color = new Color(0.72f, 0.48f, 0.25f, 1f);
        RectTransform checkRect = check.GetComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero;
        checkRect.anchorMax = Vector2.one;
        checkRect.offsetMin = new Vector2(7f, 7f);
        checkRect.offsetMax = new Vector2(-7f, -7f);

        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;

        CreateText("Label", root.transform, label, 28, TextAnchor.MiddleLeft, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(75f, 0f), new Vector2(300f, 60f));

        return toggle;
    }
}
#endif