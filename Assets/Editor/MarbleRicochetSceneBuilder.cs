#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public static class MarbleRicochetSceneBuilder
{
    [MenuItem("Tools/Marble Ricochet/Replace Project With Marble Ricochet")]
    public static void ReplaceProjectWithMarbleRicochet()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before replacing the scene.");
            return;
        }

        bool confirmed = EditorUtility.DisplayDialog(
            "Marble Ricochet",
            "This will clear the current scene and build Premium Engagement Update 04.\n\nContinue?",
            "Yes, replace it",
            "Cancel"
        );

        if (!confirmed)
        {
            return;
        }

        ClearScene();
        BuildScene();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog(
            "Marble Ricochet",
            "Premium Engagement Update 04 created.\n\nPress Play.",
            "OK"
        );
    }

    private static void ClearScene()
    {
        foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Object.DestroyImmediate(obj);
        }
    }

    private static void BuildScene()
    {
        CreateEventSystem();

        Material tableMat = CreateMaterial("MR_Wood_Table_Premium", new Color(0.30f, 0.15f, 0.06f), 0.52f);
        Material tableLineMat = CreateMaterial("MR_Wood_Table_Gold_Lines", new Color(0.70f, 0.46f, 0.20f), 0.55f);
        Material wallMat = CreateMaterial("MR_Dark_Rubber_Rails", new Color(0.035f, 0.033f, 0.030f), 0.32f);
        Material metalMat = CreateMaterial("MR_Brushed_Metal_Premium", new Color(0.61f, 0.56f, 0.46f), 0.88f);
        Material bumperMat = CreateEmissionMaterial("MR_Bumper_Brass_Glow", new Color(0.95f, 0.63f, 0.25f), 0.65f);
        Material marbleMat = CreateMaterial("MR_White_Marble_Ball", new Color(0.94f, 0.92f, 0.84f), 0.82f);
        Material glassMat = CreateTransparentMaterial("MR_Glass_Target_Premium", new Color(0.42f, 0.90f, 1.0f, 0.58f), 0.96f);
        Material breakGlassMat = CreateTransparentMaterial("MR_Breakable_Glass_Wall", new Color(0.65f, 0.95f, 1.0f, 0.42f), 0.88f);
        Material aimMat = CreateEmissionMaterial("MR_Slingshot_Line_Warm", new Color(1.0f, 0.76f, 0.34f), 1.25f);
        Material previewMat = CreateEmissionMaterial("MR_Preview_Line_Blue", new Color(0.25f, 0.85f, 1.0f), 1.45f);
        Material particleMat = CreateEmissionMaterial("MR_Glass_Shard_Material", new Color(0.60f, 0.95f, 1.0f), 1.2f);

        Camera camera = CreateCamera();
        RPCameraShake shake = camera.gameObject.AddComponent<RPCameraShake>();

        CreateLighting();
        CreateTable(tableMat, tableLineMat, metalMat);

        GameObject root = new GameObject("MARBLE_RICOCHET_GAME");
        GameObject levelRoot = new GameObject("LevelRoot");
        levelRoot.transform.SetParent(root.transform);

        GameObject wallPrefab = CreateWallPrefab(wallMat);
        GameObject bumperPrefab = CreateObstaclePrefab("MetalBumperPrefab", bumperMat, RPObstacleType.MetalBumper);
        GameObject breakablePrefab = CreateObstaclePrefab("BreakableGlassPrefab", breakGlassMat, RPObstacleType.BreakableGlass);
        GameObject popEffectPrefab = CreateGlassPopEffectPrefab(particleMat);
        GameObject targetPrefab = CreateTargetPrefab(glassMat, popEffectPrefab);
        RPBulletBall ball = CreateBall(marbleMat);
        LineRenderer aimLine = CreateLine("SlingshotLine", aimMat, 0.08f, 0.02f);
        LineRenderer previewLine = CreateLine("RicochetPreviewLine", previewMat, 0.045f, 0.018f);

        CreateCanvas(
            out GameObject menuPanel,
            out GameObject hudPanel,
            out GameObject winPanel,
            out Text titleText,
            out Text subtitleText,
            out Text levelText,
            out Text shotsText,
            out Text targetsText,
            out Text hintText,
            out Text winTitleText,
            out Text winStatsText,
            out Text winRewardText,
            out Text coinsText,
            out Text streakText,
            out Slider progressSlider,
            out Button startButton,
            out Button restartButton,
            out Button nextButton,
            out Button menuButton
        );

        GameObject audioObject = new GameObject("RPAudioManager");
        RPAudioManager audioManager = audioObject.AddComponent<RPAudioManager>();

        GameObject progressionObject = new GameObject("RPProgressionManager");
        RPProgressionManager progression = progressionObject.AddComponent<RPProgressionManager>();

        GameObject managerObject = new GameObject("RPGameManager");
        RPGameManager manager = managerObject.AddComponent<RPGameManager>();
        manager.mainCamera = camera;
        manager.levelRoot = levelRoot.transform;
        manager.ball = ball;
        manager.aimLine = aimLine;
        manager.previewLine = previewLine;
        manager.targetPrefab = targetPrefab;
        manager.wallPrefab = wallPrefab;
        manager.metalBumperPrefab = bumperPrefab;
        manager.breakableGlassPrefab = breakablePrefab;
        manager.targetMaterial = glassMat;
        manager.wallMaterial = wallMat;
        manager.metalBumperMaterial = bumperMat;
        manager.breakableGlassMaterial = breakGlassMat;
        manager.ballMaterial = marbleMat;
        manager.menuPanel = menuPanel;
        manager.hudPanel = hudPanel;
        manager.winPanel = winPanel;
        manager.titleText = titleText;
        manager.subtitleText = subtitleText;
        manager.levelText = levelText;
        manager.shotsText = shotsText;
        manager.targetsText = targetsText;
        manager.hintText = hintText;
        manager.winTitleText = winTitleText;
        manager.winStatsText = winStatsText;
        manager.winRewardText = winRewardText;
        manager.coinsText = coinsText;
        manager.streakText = streakText;
        manager.progressSlider = progressSlider;
        manager.audioManager = audioManager;
        manager.cameraShake = shake;
        manager.progression = progression;

        UnityEventTools.AddPersistentListener(startButton.onClick, manager.StartGame);
        UnityEventTools.AddPersistentListener(restartButton.onClick, manager.RestartLevel);
        UnityEventTools.AddPersistentListener(nextButton.onClick, manager.NextLevel);
        UnityEventTools.AddPersistentListener(menuButton.onClick, manager.ShowMenu);

        Selection.activeObject = managerObject;
    }

    private static void CreateEventSystem()
    {
        GameObject existing = GameObject.Find("EventSystem");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();

#if ENABLE_INPUT_SYSTEM
        eventSystem.AddComponent<InputSystemUIInputModule>();
#else
        eventSystem.AddComponent<StandaloneInputModule>();
#endif
    }

    private static Camera CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 12.5f, -0.35f);
        cameraObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 8.0f;
        camera.backgroundColor = new Color(0.055f, 0.043f, 0.036f);
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 40f;

        cameraObject.AddComponent<AudioListener>();

        return camera;
    }

    private static void CreateLighting()
    {
        GameObject key = new GameObject("Warm_Key_Light");
        key.transform.position = new Vector3(0f, 8f, -3f);
        key.transform.rotation = Quaternion.Euler(60f, 0f, 25f);

        Light keyLight = key.AddComponent<Light>();
        keyLight.type = LightType.Directional;
        keyLight.intensity = 1.85f;
        keyLight.color = new Color(1f, 0.84f, 0.62f);

        GameObject fill = new GameObject("Soft_Table_Fill_Light");
        fill.transform.position = new Vector3(0f, 6f, 0f);

        Light fillLight = fill.AddComponent<Light>();
        fillLight.type = LightType.Point;
        fillLight.intensity = 1.25f;
        fillLight.range = 11f;
        fillLight.color = new Color(0.55f, 0.75f, 1f);

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.31f, 0.26f, 0.20f);
    }

    private static void CreateTable(Material tableMat, Material lineMat, Material metalMat)
    {
        GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cube);
        edge.name = "Premium_Metal_Table_Trim";
        edge.transform.position = new Vector3(0f, -0.14f, 0f);
        edge.transform.localScale = new Vector3(11.8f, 0.08f, 15.8f);
        edge.GetComponent<MeshRenderer>().sharedMaterial = metalMat;

        GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cube);
        table.name = "Premium_Wooden_Table_Surface";
        table.transform.position = new Vector3(0f, -0.08f, 0f);
        table.transform.localScale = new Vector3(11.4f, 0.10f, 15.4f);
        table.GetComponent<MeshRenderer>().sharedMaterial = tableMat;

        for (int i = -5; i <= 5; i++)
        {
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = "Gold_Inlay_Line_" + i;
            line.transform.position = new Vector3(i * 0.9f, -0.015f, 0f);
            line.transform.localScale = new Vector3(0.030f, 0.025f, 14.5f);
            line.GetComponent<MeshRenderer>().sharedMaterial = lineMat;
        }

        for (int z = -6; z <= 6; z += 3)
        {
            GameObject cross = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cross.name = "Subtle_Cross_Inlay_" + z;
            cross.transform.position = new Vector3(0f, -0.012f, z);
            cross.transform.localScale = new Vector3(10.2f, 0.022f, 0.025f);
            cross.GetComponent<MeshRenderer>().sharedMaterial = lineMat;
        }
    }

    private static GameObject CreateWallPrefab(Material wallMat)
    {
        GameObject prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        prefab.name = "WallPrefab";
        prefab.transform.position = new Vector3(100f, 100f, 100f);
        prefab.GetComponent<MeshRenderer>().sharedMaterial = wallMat;
        prefab.AddComponent<RPObstacle>().type = RPObstacleType.NormalWall;

        PhysicsMaterial bounce = CreateBounceMaterial("MR_Normal_Wall_Physics", 1f);
        prefab.GetComponent<Collider>().material = bounce;

        prefab.SetActive(false);
        return prefab;
    }

    private static GameObject CreateObstaclePrefab(string name, Material material, RPObstacleType type)
    {
        GameObject prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        prefab.name = name;
        prefab.transform.position = new Vector3(100f, 100f, 100f);
        prefab.GetComponent<MeshRenderer>().sharedMaterial = material;

        RPObstacle obstacle = prefab.AddComponent<RPObstacle>();
        obstacle.type = type;

        PhysicsMaterial physics = CreateBounceMaterial(type == RPObstacleType.MetalBumper ? "MR_Metal_Bumper_Physics" : "MR_Glass_Wall_Physics", type == RPObstacleType.MetalBumper ? 1.15f : 0.92f);
        prefab.GetComponent<Collider>().material = physics;

        prefab.SetActive(false);
        return prefab;
    }

    private static GameObject CreateTargetPrefab(Material glassMat, GameObject popEffectPrefab)
    {
        GameObject target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        target.name = "GlassTargetPrefab";
        target.transform.position = new Vector3(100f, 100f, 100f);
        target.transform.localScale = new Vector3(0.72f, 0.72f, 0.72f);
        target.GetComponent<MeshRenderer>().sharedMaterial = glassMat;

        SphereCollider collider = target.GetComponent<SphereCollider>();
        collider.isTrigger = true;

        RPTarget targetScript = target.AddComponent<RPTarget>();
        targetScript.popEffectPrefab = popEffectPrefab;

        target.AddComponent<RPPulse>();

        target.SetActive(false);
        return target;
    }

    private static GameObject CreateGlassPopEffectPrefab(Material particleMat)
    {
        GameObject effect = new GameObject("GlassPopEffectPrefab");
        effect.transform.position = new Vector3(100f, 100f, 100f);

        ParticleSystem particles = effect.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.25f;
        main.loop = false;
        main.startLifetime = 0.55f;
        main.startSpeed = 3.2f;
        main.startSize = 0.07f;
        main.maxParticles = 60;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 34) });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.35f;

        ParticleSystemRenderer renderer = effect.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = particleMat;

        effect.AddComponent<RPEffectAutoDestroy>().lifetime = 1.4f;
        effect.SetActive(false);

        return effect;
    }

    private static RPBulletBall CreateBall(Material marbleMat)
    {
        GameObject ballObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ballObject.name = "White_Marble_Ball";
        ballObject.transform.position = new Vector3(0f, 0.35f, -6.15f);
        ballObject.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
        ballObject.GetComponent<MeshRenderer>().sharedMaterial = marbleMat;

        Rigidbody rb = ballObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 1f;
        rb.linearDamping = 0.0f;
        rb.angularDamping = 0.0f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        PhysicsMaterial bounce = CreateBounceMaterial("MR_Bouncy_Ball_Physics", 1f);
        ballObject.GetComponent<Collider>().material = bounce;

        return ballObject.AddComponent<RPBulletBall>();
    }

    private static LineRenderer CreateLine(string name, Material material, float startWidth, float endWidth)
    {
        GameObject lineObject = new GameObject(name);
        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = startWidth;
        line.endWidth = endWidth;
        line.sharedMaterial = material;
        line.useWorldSpace = true;
        line.enabled = false;
        return line;
    }

    private static void CreateCanvas(
        out GameObject menuPanel,
        out GameObject hudPanel,
        out GameObject winPanel,
        out Text titleText,
        out Text subtitleText,
        out Text levelText,
        out Text shotsText,
        out Text targetsText,
        out Text hintText,
        out Text winTitleText,
        out Text winStatsText,
        out Text winRewardText,
        out Text coinsText,
        out Text streakText,
        out Slider progressSlider,
        out Button startButton,
        out Button restartButton,
        out Button nextButton,
        out Button menuButton
    )
    {
        GameObject canvasObject = new GameObject("Canvas_UI");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        menuPanel = CreatePremiumPanel("MenuPanel", canvasObject.transform, new Color(0.030f, 0.024f, 0.020f, 0.92f));
        menuPanel.AddComponent<RPPanelJuice>();
        CreateText("SmallBrandText", menuPanel.transform, "PREMIUM PUZZLE TABLE", 22, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.79f), new Vector2(0.5f, 0.79f), Vector2.zero, new Vector2(900f, 50f), new Color(0.78f, 0.60f, 0.34f));
        titleText = CreateText("TitleText", menuPanel.transform, "MARBLE RICOCHET", 68, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.70f), new Vector2(0.5f, 0.70f), Vector2.zero, new Vector2(980f, 160f), new Color(0.98f, 0.92f, 0.76f));
        subtitleText = CreateText("SubtitleText", menuPanel.transform, "A premium glass-breaking puzzle.", 32, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.60f), new Vector2(0.5f, 0.60f), Vector2.zero, new Vector2(900f, 90f), new Color(0.82f, 0.78f, 0.68f));
        startButton = CreatePremiumButton("StartButton", menuPanel.transform, "PLAY", new Vector2(0.5f, 0.43f), new Vector2(440f, 96f));
        CreateText("HowToPlayText", menuPanel.transform, "Pull away from the marble.\nRelease to break every glass target.", 25, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.27f), new Vector2(0.5f, 0.27f), Vector2.zero, new Vector2(850f, 120f), new Color(0.86f, 0.82f, 0.72f));

        hudPanel = new GameObject("HUDPanel");
        hudPanel.transform.SetParent(canvasObject.transform, false);
        RectTransform hudRect = hudPanel.AddComponent<RectTransform>();
        hudRect.anchorMin = Vector2.zero;
        hudRect.anchorMax = Vector2.one;
        hudRect.offsetMin = Vector2.zero;
        hudRect.offsetMax = Vector2.zero;

        levelText = CreateText("LevelText", hudPanel.transform, "LEVEL 1", 27, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(35f, -28f), new Vector2(560f, 70f), new Color(0.96f, 0.91f, 0.78f));
        shotsText = CreateText("ShotsText", hudPanel.transform, "SHOTS 0", 24, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-35f, -32f), new Vector2(340f, 70f), new Color(0.96f, 0.91f, 0.78f));
        targetsText = CreateText("TargetsText", hudPanel.transform, "GLASS 0", 24, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -32f), new Vector2(320f, 70f), new Color(0.78f, 0.95f, 1f));
        coinsText = CreateText("CoinsText", hudPanel.transform, "COINS 0", 22, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(35f, -82f), new Vector2(260f, 60f), new Color(1f, 0.76f, 0.36f));
        streakText = CreateText("StreakText", hudPanel.transform, "STREAK —", 22, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-35f, -82f), new Vector2(300f, 60f), new Color(1f, 0.76f, 0.36f));
        hintText = CreateText("HintText", hudPanel.transform, "Pull back from the marble, then release.", 24, TextAnchor.LowerCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 34f), new Vector2(900f, 70f), new Color(0.90f, 0.86f, 0.76f));

        progressSlider = CreateProgressSlider("ProgressSlider", hudPanel.transform, new Vector2(0.5f, 0.955f), new Vector2(380f, 16f));

        restartButton = CreatePremiumButton("RestartButton", hudPanel.transform, "↻", new Vector2(0.08f, 0.08f), new Vector2(105f, 75f));

        winPanel = CreatePremiumPanel("WinPanel", canvasObject.transform, new Color(0.030f, 0.024f, 0.020f, 0.93f));
        winPanel.AddComponent<RPPanelJuice>();
        winTitleText = CreateText("WinTitleText", winPanel.transform, "Perfect shot!", 54, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.68f), new Vector2(0.5f, 0.68f), Vector2.zero, new Vector2(950f, 110f), new Color(1f, 0.78f, 0.36f));
        winStatsText = CreateText("WinStatsText", winPanel.transform, "LEVEL COMPLETE", 35, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.53f), new Vector2(0.5f, 0.53f), Vector2.zero, new Vector2(900f, 260f), new Color(0.96f, 0.91f, 0.78f));
        winRewardText = CreateText("WinRewardText", winPanel.transform, "+25 coins", 30, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.39f), new Vector2(0.5f, 0.39f), Vector2.zero, new Vector2(900f, 140f), new Color(1f, 0.76f, 0.36f));
        nextButton = CreatePremiumButton("NextButton", winPanel.transform, "NEXT LEVEL", new Vector2(0.5f, 0.25f), new Vector2(460f, 90f));
        menuButton = CreatePremiumButton("MenuButton", winPanel.transform, "MENU", new Vector2(0.5f, 0.16f), new Vector2(360f, 72f));

        winPanel.SetActive(false);
        hudPanel.SetActive(false);
    }

    private static GameObject CreatePremiumPanel(string name, Transform parent, Color color)
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

        GameObject card = new GameObject(name + "_Card");
        card.transform.SetParent(panel.transform, false);
        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = Vector2.zero;
        cardRect.sizeDelta = new Vector2(940f, 1180f);

        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(0.11f, 0.075f, 0.045f, 0.64f);

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

    private static Button CreatePremiumButton(string name, Transform parent, string label, Vector2 anchor, Vector2 size)
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

        CreateText("Text", obj.transform, label, 32, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(1f, 0.94f, 0.80f));

        return button;
    }

    private static Slider CreateProgressSlider(string name, Transform parent, Vector2 anchor, Vector2 size)
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
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        slider.interactable = false;

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(root.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.12f, 0.085f, 0.055f, 0.9f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(root.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.95f, 0.62f, 0.25f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;

        return slider;
    }

    private static Material CreateMaterial(string name, Color color, float smoothness)
    {
        string folderPath = "Assets/Materials";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        string assetPath = folderPath + "/" + name + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Simple Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material material = existing != null ? existing : new Material(shader);
        material.shader = shader;

        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        material.color = color;
        if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);

        if (existing == null) AssetDatabase.CreateAsset(material, assetPath);

        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();

        return material;
    }

    private static Material CreateTransparentMaterial(string name, Color color, float smoothness)
    {
        Material material = CreateMaterial(name, color, smoothness);

        if (material.HasProperty("_Surface")) material.SetFloat("_Surface", 1f);
        if (material.HasProperty("_Blend")) material.SetFloat("_Blend", 0f);
        if (material.HasProperty("_AlphaClip")) material.SetFloat("_AlphaClip", 0f);
        material.renderQueue = 3000;

        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        material.color = color;

        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();

        return material;
    }

    private static Material CreateEmissionMaterial(string name, Color color, float emissionStrength)
    {
        Material material = CreateMaterial(name, color, 0.45f);

        if (material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * emissionStrength);
        }

        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();

        return material;
    }

    private static PhysicsMaterial CreateBounceMaterial(string name, float bounciness)
    {
        string folderPath = "Assets/Materials";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        string assetPath = folderPath + "/" + name + ".physicMaterial";
        PhysicsMaterial existing = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(assetPath);

        PhysicsMaterial material = existing != null ? existing : new PhysicsMaterial(name);
        material.bounciness = bounciness;
        material.dynamicFriction = 0f;
        material.staticFriction = 0f;
        material.frictionCombine = PhysicsMaterialCombine.Minimum;
        material.bounceCombine = PhysicsMaterialCombine.Maximum;

        if (existing == null)
        {
            AssetDatabase.CreateAsset(material, assetPath);
        }

        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();

        return material;
    }
}
#endif