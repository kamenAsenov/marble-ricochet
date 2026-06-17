#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MarbleRicochetHazardsBallAbilitiesUpdate14Applier
{
    [MenuItem("Tools/Marble Ricochet/Apply Hazards Ball Abilities Update 14")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 14.");
            return;
        }

        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (manager == null || canvas == null)
        {
            EditorUtility.DisplayDialog(
                "Update 14",
                "Could not find RPGameManager or Canvas.\n\nOpen the Marble Ricochet scene first.",
                "OK"
            );
            return;
        }

        Material holeMat = CreateMaterial("MR14_Dark_Pocket_Hole", new Color(0.005f, 0.004f, 0.004f), 0.15f);
        Material holeRingMat = CreateEmissionMaterial("MR14_Pocket_Gold_Ring", new Color(1.0f, 0.62f, 0.20f), 0.35f);
        Material rotatorMat = CreateEmissionMaterial("MR14_Rotating_Blocker_Brass", new Color(1.0f, 0.54f, 0.18f), 0.46f);

        RPBallStatsManager ballStats = EnsureBallStats(manager, canvas.transform);
        RPHazardManager hazards = EnsureHazardManager(manager, canvas.transform, holeMat, holeRingMat, rotatorMat);
        EnsureFailScreen(canvas.transform, manager, hazards);
        EnsureShopStats(manager, canvas.transform, ballStats);
        EnsureHazardHint(canvas.transform, hazards);
        ConnectComboToBallStats(ballStats);

        EditorUtility.SetDirty(manager);
        SaveScene();

        EditorUtility.DisplayDialog(
            "Update 14 Applied",
            "Hazards + Ball Abilities Update 14 applied.\n\nAdded pockets, fail screen, rotating obstacles and ball abilities.",
            "OK"
        );
    }

    private static RPBallStatsManager EnsureBallStats(RPGameManager manager, Transform canvasTransform)
    {
        RPBallStatsManager stats = Object.FindFirstObjectByType<RPBallStatsManager>();

        if (stats == null)
        {
            GameObject obj = new GameObject("RPBallStatsManager");
            stats = obj.AddComponent<RPBallStatsManager>();
        }

        stats.gameManager = manager;
        stats.progression = Object.FindFirstObjectByType<RPProgressionManager>();

        Text hudText = GameObject.Find("BallAbilityHudText")?.GetComponent<Text>();

        if (hudText == null && manager.hudPanel != null)
        {
            hudText = CreateText(
                "BallAbilityHudText",
                manager.hudPanel.transform,
                "Balanced",
                14,
                TextAnchor.UpperCenter,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -150f),
                new Vector2(260f, 44f),
                new Color(0.78f, 0.90f, 1f)
            );
        }

        stats.statsHudText = hudText;
        stats.ApplySelectedBallStats();
        EditorUtility.SetDirty(stats);
        return stats;
    }

    private static RPHazardManager EnsureHazardManager(
        RPGameManager manager,
        Transform canvasTransform,
        Material holeMat,
        Material ringMat,
        Material rotatorMat
    )
    {
        RPHazardManager hazards = Object.FindFirstObjectByType<RPHazardManager>();

        if (hazards == null)
        {
            GameObject obj = new GameObject("RPHazardManager");
            hazards = obj.AddComponent<RPHazardManager>();
        }

        hazards.gameManager = manager;
        hazards.ballStats = Object.FindFirstObjectByType<RPBallStatsManager>();
        hazards.holePrefab = CreateHolePrefab(holeMat, ringMat);
        hazards.rotatingObstaclePrefab = CreateRotatorPrefab(rotatorMat, manager);

        EditorUtility.SetDirty(hazards);
        return hazards;
    }

    private static GameObject CreateHolePrefab(Material holeMat, Material ringMat)
    {
        GameObject old = GameObject.Find("MR14_HolePocketPrefab");

        if (old != null)
        {
            Object.DestroyImmediate(old);
        }

        GameObject root = new GameObject("MR14_HolePocketPrefab");
        root.transform.position = new Vector3(100f, 100f, 100f);
        root.SetActive(false);

        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disc.name = "Hole_Dark_Center";
        disc.transform.SetParent(root.transform);
        disc.transform.localPosition = Vector3.zero;
        disc.transform.localScale = new Vector3(1f, 0.04f, 1f);

        MeshRenderer discRenderer = disc.GetComponent<MeshRenderer>();

        if (discRenderer != null)
        {
            discRenderer.sharedMaterial = holeMat;
        }

        Object.DestroyImmediate(disc.GetComponent<Collider>());

        // Unity has no built-in PrimitiveType.Torus.
        // We create a clean visual gold rim from four thin bars + four small corner cylinders.
        CreateRimBar(root.transform, "Hole_Ring_North", new Vector3(0f, 0.07f, 0.52f), new Vector3(0.78f, 0.045f, 0.055f), ringMat);
        CreateRimBar(root.transform, "Hole_Ring_South", new Vector3(0f, 0.07f, -0.52f), new Vector3(0.78f, 0.045f, 0.055f), ringMat);
        CreateRimBar(root.transform, "Hole_Ring_East", new Vector3(0.52f, 0.07f, 0f), new Vector3(0.055f, 0.045f, 0.78f), ringMat);
        CreateRimBar(root.transform, "Hole_Ring_West", new Vector3(-0.52f, 0.07f, 0f), new Vector3(0.055f, 0.045f, 0.78f), ringMat);

        CreateCornerDot(root.transform, "Hole_Ring_Dot_NE", new Vector3(0.42f, 0.075f, 0.42f), ringMat);
        CreateCornerDot(root.transform, "Hole_Ring_Dot_NW", new Vector3(-0.42f, 0.075f, 0.42f), ringMat);
        CreateCornerDot(root.transform, "Hole_Ring_Dot_SE", new Vector3(0.42f, 0.075f, -0.42f), ringMat);
        CreateCornerDot(root.transform, "Hole_Ring_Dot_SW", new Vector3(-0.42f, 0.075f, -0.42f), ringMat);

        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.48f;

        root.AddComponent<RPHolePocket>();
        return root;
    }

    private static void CreateRimBar(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.name = name;
        bar.transform.SetParent(parent);
        bar.transform.localPosition = localPosition;
        bar.transform.localScale = localScale;

        MeshRenderer renderer = bar.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        Collider collider = bar.GetComponent<Collider>();

        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }
    }

    private static void CreateCornerDot(Transform parent, string name, Vector3 localPosition, Material material)
    {
        GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dot.name = name;
        dot.transform.SetParent(parent);
        dot.transform.localPosition = localPosition;
        dot.transform.localScale = new Vector3(0.13f, 0.035f, 0.13f);

        MeshRenderer renderer = dot.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        Collider collider = dot.GetComponent<Collider>();

        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }
    }

    private static GameObject CreateRotatorPrefab(Material mat, RPGameManager manager)
    {
        GameObject old = GameObject.Find("MR14_RotatingObstaclePrefab");

        if (old != null)
        {
            Object.DestroyImmediate(old);
        }

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "MR14_RotatingObstaclePrefab";
        obj.transform.position = new Vector3(100f, 100f, 100f);
        obj.transform.localScale = new Vector3(2.4f, 0.55f, 0.22f);
        obj.SetActive(false);

        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.sharedMaterial = mat;
        }

        RPObstacle obstacle = obj.AddComponent<RPObstacle>();
        obstacle.type = RPObstacleType.NormalWall;
        obstacle.gameManager = manager;

        obj.AddComponent<RPRotatingObstacle>();

        return obj;
    }

    private static void EnsureFailScreen(Transform canvasTransform, RPGameManager manager, RPHazardManager hazards)
    {
        GameObject panel = GameObject.Find("FailPanel");

        if (panel == null)
        {
            panel = new GameObject("FailPanel");
            panel.transform.SetParent(canvasTransform, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = panel.AddComponent<Image>();
            image.color = new Color(0.025f, 0.018f, 0.014f, 0.93f);
        }

        Text title = GetOrCreateText(panel.transform, "FailTitleText", "Marble Lost", 46, new Vector2(0.5f, 0.68f), new Vector2(850f, 90f), new Color(1f, 0.62f, 0.24f));
        Text reason = GetOrCreateText(panel.transform, "FailReasonText", "The marble fell into a pocket.", 24, new Vector2(0.5f, 0.56f), new Vector2(850f, 120f), new Color(0.95f, 0.88f, 0.74f));

        Button retry = GetOrCreateButton(panel.transform, "FailRetryButton", "RETRY", new Vector2(0.5f, 0.42f), new Vector2(380f, 70f), 23);
        Button hint = GetOrCreateButton(panel.transform, "FailHintButton", "USE HINT", new Vector2(0.5f, 0.33f), new Vector2(340f, 60f), 20);
        Button shop = GetOrCreateButton(panel.transform, "FailShopButton", "SHOP", new Vector2(0.5f, 0.25f), new Vector2(300f, 56f), 19);
        Button menu = GetOrCreateButton(panel.transform, "FailMenuButton", "MENU", new Vector2(0.5f, 0.17f), new Vector2(280f, 52f), 18);

        retry.onClick.RemoveAllListeners();
        hint.onClick.RemoveAllListeners();
        shop.onClick.RemoveAllListeners();
        menu.onClick.RemoveAllListeners();

        UnityEventTools.AddPersistentListener(retry.onClick, hazards.Retry);
        UnityEventTools.AddPersistentListener(hint.onClick, hazards.UseHint);
        UnityEventTools.AddPersistentListener(shop.onClick, hazards.OpenShop);
        UnityEventTools.AddPersistentListener(menu.onClick, hazards.GoToMenu);

        hazards.failPanel = panel;
        hazards.failTitleText = title;
        hazards.failReasonText = reason;

        panel.SetActive(false);
    }

    private static void EnsureShopStats(RPGameManager manager, Transform canvasTransform, RPBallStatsManager ballStats)
    {
        RPShopManager shop = Object.FindFirstObjectByType<RPShopManager>();

        if (shop == null)
        {
            GameObject shopObj = new GameObject("RPShopManager");
            shop = shopObj.AddComponent<RPShopManager>();
        }

        shop.gameManager = manager;
        shop.progression = Object.FindFirstObjectByType<RPProgressionManager>();
        shop.ballStats = ballStats;
        manager.shopManager = shop;

        GameObject panel = GameObject.Find("ShopPanel");

        if (panel != null)
        {
            Text statsText = GameObject.Find("BallStatsText")?.GetComponent<Text>();

            if (statsText == null)
            {
                statsText = CreateText(
                    "BallStatsText",
                    panel.transform,
                    "Ability: Balanced",
                    18,
                    TextAnchor.MiddleCenter,
                    new Vector2(0.5f, 0.515f),
                    new Vector2(0.5f, 0.515f),
                    Vector2.zero,
                    new Vector2(820f, 92f),
                    new Color(0.78f, 0.90f, 1f)
                );
            }

            shop.ballStatsText = statsText;

            Text shopTitle = GameObject.Find("ShopSkinNameText")?.GetComponent<Text>();

            if (shopTitle != null)
            {
                shopTitle.fontSize = 28;
            }

            MoveButton("BuySkinButton", new Vector2(0.5f, 0.405f), new Vector2(360f, 54f), 18);
            MoveButton("RewardCoinsButton", new Vector2(0.5f, 0.335f), new Vector2(390f, 50f), 16);
            MoveButton("RewardHintButton", new Vector2(0.5f, 0.205f), new Vector2(360f, 48f), 16);
        }

        EditorUtility.SetDirty(shop);
    }

    private static void EnsureHazardHint(Transform canvasTransform, RPHazardManager hazards)
    {
        Text hint = GameObject.Find("HazardHintText")?.GetComponent<Text>();

        if (hint == null)
        {
            hint = CreateText(
                "HazardHintText",
                canvasTransform,
                "Avoid pockets.",
                22,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.705f),
                new Vector2(0.5f, 0.705f),
                Vector2.zero,
                new Vector2(820f, 65f),
                new Color(1f, 0.72f, 0.32f)
            );
        }

        hazards.hazardHintText = hint;
        hint.gameObject.SetActive(false);
    }

    private static void ConnectComboToBallStats(RPBallStatsManager ballStats)
    {
        RPComboSkillManager combo = Object.FindFirstObjectByType<RPComboSkillManager>();

        if (combo != null)
        {
            combo.ballStats = ballStats;
            EditorUtility.SetDirty(combo);
        }
    }

    private static Text GetOrCreateText(Transform parent, string name, string content, int fontSize, Vector2 anchor, Vector2 size, Color color)
    {
        Text text = GameObject.Find(name)?.GetComponent<Text>();

        if (text != null)
        {
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            RectTransform rect = text.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            return text;
        }

        return CreateText(name, parent, content, fontSize, TextAnchor.MiddleCenter, anchor, anchor, Vector2.zero, size, color);
    }

    private static Button GetOrCreateButton(Transform parent, string name, string label, Vector2 anchor, Vector2 size, int fontSize)
    {
        Button button = GameObject.Find(name)?.GetComponent<Button>();

        if (button != null)
        {
            MoveButton(name, anchor, size, fontSize);
            return button;
        }

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

        button = obj.AddComponent<Button>();

        CreateText("Text", obj.transform, label, fontSize, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(1f, 0.94f, 0.80f));

        return button;
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

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        material.color = color;

        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", smoothness);
        }

        if (existing == null)
        {
            AssetDatabase.CreateAsset(material, assetPath);
        }

        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();

        return material;
    }

    private static Material CreateEmissionMaterial(string name, Color color, float emissionStrength)
    {
        Material material = CreateMaterial(name, color, 0.60f);

        if (material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * emissionStrength);
        }

        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();

        return material;
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