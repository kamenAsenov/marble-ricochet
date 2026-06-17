#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MarbleRicochetFinalPreReleaseCleanupUpdate17Applier
{
    private static readonly Color Background = new Color(0.012f, 0.013f, 0.015f, 1f);
    private static readonly Color Board = new Color(0.075f, 0.082f, 0.088f, 1f);
    private static readonly Color BoardSoft = new Color(0.095f, 0.103f, 0.110f, 1f);
    private static readonly Color Rail = new Color(0.020f, 0.022f, 0.025f, 1f);
    private static readonly Color Edge = new Color(0.185f, 0.195f, 0.205f, 1f);
    private static readonly Color Glass = new Color(0.57f, 0.91f, 1.0f, 0.56f);
    private static readonly Color TextMain = new Color(0.93f, 0.91f, 0.86f, 1f);
    private static readonly Color TextSub = new Color(0.70f, 0.75f, 0.76f, 1f);
    private static readonly Color Accent = new Color(0.72f, 0.76f, 0.74f, 1f);
    private static readonly Color Danger = new Color(0.72f, 0.28f, 0.32f, 1f);

    [MenuItem("Tools/Marble Ricochet/Apply Final Pre-Release Cleanup Update 17")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 17.");
            return;
        }

        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (manager == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Update 17", "Could not find RPGameManager or Canvas.\n\nOpen the Marble Ricochet scene first.", "OK");
            return;
        }

        AggressiveVisualNoiseCleanup();
        DisableOldNoisyTexts();
        SetupMinimalScene();

        Material boardMat = CreateMaterial("MR17_Minimal_Slate_Board", Board, 0.74f, 0.02f);
        Material boardSoftMat = CreateMaterial("MR17_Minimal_Slate_Soft", BoardSoft, 0.62f, 0.0f);
        Material railMat = CreateMaterial("MR17_Minimal_Matte_Rail", Rail, 0.46f, 0.0f);
        Material edgeMat = CreateMaterial("MR17_Minimal_Subtle_Edge", Edge, 0.58f, 0.0f);
        Material targetMat = CreateTransparentMaterial("MR17_Clean_Glass_Target", Glass, 0.93f);
        Material glassWallMat = CreateTransparentMaterial("MR17_Clean_Breakable_Glass", new Color(0.76f, 0.95f, 1f, 0.30f), 0.90f);
        Material ballMat = CreateMaterial("MR17_Clean_Marble_Ball", new Color(0.96f, 0.96f, 0.92f, 1f), 0.94f, 0.0f);
        Material bumperMat = CreateMaterial("MR17_Simple_Bumper_Gunmetal", new Color(0.38f, 0.42f, 0.45f, 1f), 0.68f, 0.08f);
        Material pocketMat = CreateMaterial("MR17_Simple_Pocket_Black", new Color(0.002f, 0.002f, 0.003f, 1f), 0.10f, 0.0f);
        Material rotatorMat = CreateMaterial("MR17_Simple_Rotator", new Color(0.25f, 0.28f, 0.30f, 1f), 0.56f, 0.04f);

        BuildMinimalPremiumBoard(boardMat, boardSoftMat, railMat, edgeMat);
        ApplyGameplayMaterials(manager, railMat, targetMat, glassWallMat, bumperMat, ballMat);
        ApplySimpleHazards(manager, pocketMat, rotatorMat);
        BuildReleaseUI(canvas.transform, manager);
        InstallRuntimeHardener(manager);

        SaveScene();

        EditorUtility.DisplayDialog(
            "Update 17 Applied",
            "Final Pre-Release Cleanup Update 17 applied.\n\nOrange board lines removed, pockets simplified, UI overlap reduced, and release-style layout applied.",
            "OK"
        );
    }

    private static void AggressiveVisualNoiseCleanup()
    {
        string[] rootNames =
        {
            "MR12_Premium_Table_Visuals",
            "MR12_Premium_Lighting",
            "MR15_Graphite_Premium_Table",
            "MR15_Cinematic_Ambience",
            "MR16_Clean_Premium_Table",
            "MR17_Minimal_Release_Board",
            "Permanent_Visible_Bounce_Rails_Update11",
            "Rail_Clarity_Root",
            "Premium_Rail_Clarity_Root"
        };

        foreach (string rootName in rootNames)
        {
            GameObject root = GameObject.Find(rootName);
            if (root != null)
            {
                Object.DestroyImmediate(root);
            }
        }

        Transform[] all = Object.FindObjectsOfType<Transform>(true);
        List<GameObject> toDelete = new List<GameObject>();

        foreach (Transform t in all)
        {
            if (t == null) continue;
            string n = t.gameObject.name.ToLowerInvariant();

            if (
                n.Contains("fine_gold_inlay") ||
                n.Contains("subtle_graphite_grain") ||
                n.Contains("cyan_playfield") ||
                n.Contains("bottomlaunchguide") ||
                n.Contains("launchguideleft") ||
                n.Contains("launchguideright") ||
                n.Contains("gold_lip") ||
                n.Contains("brass_lip") ||
                n.Contains("rail_gold") ||
                n.Contains("playfield_top_guide") ||
                n.Contains("playfield_left_guide") ||
                n.Contains("playfield_right_guide") ||
                n.Contains("premium_rail_gold_lip") ||
                n.Contains("fine_gold") ||
                n.Contains("center_soft_glow") ||
                n.Contains("center_softglow")
            )
            {
                toDelete.Add(t.gameObject);
            }
        }

        foreach (GameObject obj in toDelete)
        {
            if (obj != null)
            {
                Object.DestroyImmediate(obj);
            }
        }
    }

    private static void DisableOldNoisyTexts()
    {
        string[] names =
        {
            "HazardHintText",
            "BallAbilityHudText",
            "CampaignTaglineText"
        };

        foreach (string name in names)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    private static void SetupMinimalScene()
    {
        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.backgroundColor = Background;
            EditorUtility.SetDirty(camera);
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.125f, 0.130f, 0.135f, 1f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = Background;
        RenderSettings.fogDensity = 0.010f;
    }

    private static void BuildMinimalPremiumBoard(Material boardMat, Material boardSoftMat, Material railMat, Material edgeMat)
    {
        GameObject root = new GameObject("MR17_Minimal_Release_Board");

        Material backdropMat = CreateMaterial("MR17_Backdrop_Black", Background, 0.20f, 0.0f);
        Material shadowMat = CreateMaterial("MR17_Table_Shadow", new Color(0.006f, 0.006f, 0.007f, 1f), 0.12f, 0.0f);

        CreateCube(root.transform, "Backdrop", new Vector3(0f, -0.30f, 0f), new Vector3(24f, 0.10f, 28f), backdropMat);
        CreateCube(root.transform, "TableShadow", new Vector3(0f, -0.145f, 0f), new Vector3(12.8f, 0.05f, 16.9f), shadowMat);
        CreateCube(root.transform, "BoardBase", new Vector3(0f, -0.090f, 0f), new Vector3(12.05f, 0.055f, 16.05f), boardSoftMat);
        CreateCube(root.transform, "BoardSurface", new Vector3(0f, -0.035f, 0f), new Vector3(11.45f, 0.058f, 15.45f), boardMat);

        // No decorative grid, no orange/yellow lines, no loud accents.
        CreateCube(root.transform, "TopRail", new Vector3(0f, 0.24f, 7.10f), new Vector3(10.95f, 0.46f, 0.50f), railMat);
        CreateCube(root.transform, "BottomRail", new Vector3(0f, 0.24f, -7.10f), new Vector3(10.95f, 0.46f, 0.50f), railMat);
        CreateCube(root.transform, "LeftRail", new Vector3(-5.10f, 0.24f, 0f), new Vector3(0.50f, 0.46f, 14.15f), railMat);
        CreateCube(root.transform, "RightRail", new Vector3(5.10f, 0.24f, 0f), new Vector3(0.50f, 0.46f, 14.15f), railMat);

        // Subtle neutral edge, not orange/yellow.
        CreateCube(root.transform, "TopNeutralEdge", new Vector3(0f, 0.506f, 6.77f), new Vector3(10.0f, 0.034f, 0.045f), edgeMat);
        CreateCube(root.transform, "BottomNeutralEdge", new Vector3(0f, 0.506f, -6.77f), new Vector3(10.0f, 0.034f, 0.045f), edgeMat);
        CreateCube(root.transform, "LeftNeutralEdge", new Vector3(-4.77f, 0.506f, 0f), new Vector3(0.045f, 0.034f, 13.35f), edgeMat);
        CreateCube(root.transform, "RightNeutralEdge", new Vector3(4.77f, 0.506f, 0f), new Vector3(0.045f, 0.034f, 13.35f), edgeMat);

        CreateCorner(root.transform, new Vector3(-5.08f, 0.52f, -7.08f), edgeMat);
        CreateCorner(root.transform, new Vector3(5.08f, 0.52f, -7.08f), edgeMat);
        CreateCorner(root.transform, new Vector3(-5.08f, 0.52f, 7.08f), edgeMat);
        CreateCorner(root.transform, new Vector3(5.08f, 0.52f, 7.08f), edgeMat);

        CreateLight(root.transform, "ReleaseKeyLight", LightType.Directional, new Vector3(0f, 6f, -3f), Quaternion.Euler(56f, 0f, 22f), new Color(0.96f, 0.92f, 0.86f, 1f), 1.00f, 10f);
        CreateLight(root.transform, "ReleaseSoftFill", LightType.Point, new Vector3(0f, 4.2f, 1.2f), Quaternion.identity, new Color(0.62f, 0.75f, 0.82f, 1f), 0.42f, 9f);
    }

    private static void ApplyGameplayMaterials(RPGameManager manager, Material wallMat, Material targetMat, Material glassWallMat, Material bumperMat, Material ballMat)
    {
        manager.wallMaterial = wallMat;
        manager.targetMaterial = targetMat;
        manager.breakableGlassMaterial = glassWallMat;
        manager.metalBumperMaterial = bumperMat;
        manager.ballMaterial = ballMat;

        ApplyMaterialToRenderer(manager.wallPrefab, wallMat);
        ApplyMaterialToRenderer(manager.targetPrefab, targetMat);
        ApplyMaterialToRenderer(manager.breakableGlassPrefab, glassWallMat);
        ApplyMaterialToRenderer(manager.metalBumperPrefab, bumperMat);

        if (manager.ball != null)
        {
            ApplyMaterialToRenderer(manager.ball.gameObject, ballMat);
        }
    }

    private static void ApplySimpleHazards(RPGameManager manager, Material pocketMat, Material rotatorMat)
    {
        RPHazardManager hazards = Object.FindFirstObjectByType<RPHazardManager>();
        if (hazards == null)
        {
            return;
        }

        hazards.holePrefab = CreateSimplePocketPrefab(pocketMat);
        hazards.rotatingObstaclePrefab = CreateSimpleRotatorPrefab(rotatorMat, manager);

        if (hazards.hazardHintText != null)
        {
            hazards.hazardHintText.gameObject.SetActive(false);
            hazards.hazardHintText = null;
        }

        EditorUtility.SetDirty(hazards);
    }

    private static GameObject CreateSimplePocketPrefab(Material pocketMat)
    {
        GameObject old = GameObject.Find("MR17_SimplePocketPrefab");
        if (old != null)
        {
            Object.DestroyImmediate(old);
        }

        GameObject root = new GameObject("MR17_SimplePocketPrefab");
        root.transform.position = new Vector3(100f, 100f, 100f);
        root.SetActive(false);

        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disc.name = "SimplePocketVisual";
        disc.transform.SetParent(root.transform);
        disc.transform.localPosition = Vector3.zero;
        disc.transform.localScale = new Vector3(1.0f, 0.032f, 1.0f);
        ApplyMaterialToRenderer(disc, pocketMat);
        Object.DestroyImmediate(disc.GetComponent<Collider>());

        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.48f;

        root.AddComponent<RPHolePocket>();
        return root;
    }

    private static GameObject CreateSimpleRotatorPrefab(Material rotatorMat, RPGameManager manager)
    {
        GameObject old = GameObject.Find("MR17_SimpleRotatorPrefab");
        if (old != null)
        {
            Object.DestroyImmediate(old);
        }

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "MR17_SimpleRotatorPrefab";
        obj.transform.position = new Vector3(100f, 100f, 100f);
        obj.transform.localScale = new Vector3(2.4f, 0.50f, 0.18f);
        obj.SetActive(false);
        ApplyMaterialToRenderer(obj, rotatorMat);

        RPObstacle obstacle = obj.AddComponent<RPObstacle>();
        obstacle.type = RPObstacleType.NormalWall;
        obstacle.gameManager = manager;
        obj.AddComponent<RPRotatingObstacle>();

        return obj;
    }

    private static void BuildReleaseUI(Transform canvasTransform, RPGameManager manager)
    {
        GameObject releasePanel = GameObject.Find("ReleaseObjectivePanel");
        if (releasePanel == null)
        {
            releasePanel = new GameObject("ReleaseObjectivePanel");
            releasePanel.transform.SetParent(canvasTransform, false);
            RectTransform rect = releasePanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.835f);
            rect.anchorMax = new Vector2(0.5f, 0.835f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(560f, 58f);
            Image img = releasePanel.AddComponent<Image>();
            img.color = new Color(0.018f, 0.020f, 0.023f, 0.62f);
            img.raycastTarget = false;
        }

        Text releaseText = GameObject.Find("ReleaseObjectiveText")?.GetComponent<Text>();
        if (releaseText == null)
        {
            releaseText = CreateText("ReleaseObjectiveText", releasePanel.transform, "Clear the table.", 14, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextSub);
        }

        RPComboSkillManager combo = Object.FindFirstObjectByType<RPComboSkillManager>();
        if (combo != null)
        {
            if (combo.objectivePanel != null && combo.objectivePanel != releasePanel)
            {
                combo.objectivePanel.SetActive(false);
            }

            combo.objectivePanel = releasePanel;
            combo.objectiveText = releaseText;
            combo.objectiveVisibleTime = 2.0f;
            EditorUtility.SetDirty(combo);
        }

        RestyleHudAndPanels(manager, canvasTransform);
        RestyleAllButtons();
        RestyleAllText();
    }

    private static void RestyleHudAndPanels(RPGameManager manager, Transform canvasTransform)
    {
        StyleFullPanel("FailPanel", new Color(0.006f, 0.007f, 0.008f, 0.985f));
        StyleFullPanel("WinPanel", new Color(0.006f, 0.007f, 0.008f, 0.965f));
        StyleFullPanel("MenuPanel", new Color(0.006f, 0.007f, 0.008f, 0.965f));
        StyleFullPanel("ShopPanel", new Color(0.006f, 0.007f, 0.008f, 0.970f));

        EnsureCard("FailPanel", "FailReleaseCard", new Vector2(430f, 390f));
        EnsureCard("WinPanel", "WinReleaseCard", new Vector2(440f, 430f));
        EnsureCard("MenuPanel", "MenuReleaseCard", new Vector2(450f, 420f));
        EnsureCard("ShopPanel", "ShopReleaseCard", new Vector2(500f, 510f));

        MoveText("FailTitleText", new Vector2(0.5f, 0.63f), new Vector2(380f, 54f), 30, TextMain, FontStyle.Bold);
        MoveText("FailReasonText", new Vector2(0.5f, 0.53f), new Vector2(360f, 78f), 17, TextSub, FontStyle.Normal);
        MoveButton("FailRetryButton", new Vector2(0.5f, 0.405f), new Vector2(300f, 54f), 18);
        MoveButton("FailHintButton", new Vector2(0.5f, 0.325f), new Vector2(280f, 48f), 16);
        MoveButton("FailShopButton", new Vector2(0.5f, 0.250f), new Vector2(260f, 44f), 15);
        MoveButton("FailMenuButton", new Vector2(0.5f, 0.180f), new Vector2(240f, 42f), 15);

        MoveText("WinTitleText", new Vector2(0.5f, 0.70f), new Vector2(380f, 52f), 30, TextMain, FontStyle.Bold);
        MoveText("SkillScoreBreakdownText", new Vector2(0.5f, 0.46f), new Vector2(360f, 82f), 15, TextSub, FontStyle.Normal);
        MoveText("ShopSkinNameText", new Vector2(0.5f, 0.71f), new Vector2(420f, 50f), 26, TextMain, FontStyle.Bold);
        MoveText("ShopSkinPriceText", new Vector2(0.5f, 0.63f), new Vector2(420f, 38f), 17, TextSub, FontStyle.Normal);
        MoveText("BallStatsText", new Vector2(0.5f, 0.53f), new Vector2(420f, 78f), 15, TextSub, FontStyle.Normal);
        MoveText("ShopStatusText", new Vector2(0.5f, 0.83f), new Vector2(420f, 34f), 14, TextSub, FontStyle.Normal);
        MoveText("AdStatusText", new Vector2(0.5f, 0.19f), new Vector2(420f, 52f), 13, TextSub, FontStyle.Normal);

        MoveButton("BuySkinButton", new Vector2(0.5f, 0.405f), new Vector2(310f, 48f), 16);
        MoveButton("RewardCoinsButton", new Vector2(0.5f, 0.330f), new Vector2(320f, 44f), 15);
        MoveButton("RewardHintButton", new Vector2(0.5f, 0.260f), new Vector2(300f, 42f), 15);

        GameObject fail = GameObject.Find("FailPanel");
        if (fail != null)
        {
            fail.transform.SetAsLastSibling();
        }

        GameObject ability = GameObject.Find("BallAbilityHudText");
        if (ability != null)
        {
            ability.SetActive(false);
        }

        GameObject hazard = GameObject.Find("HazardHintText");
        if (hazard != null)
        {
            hazard.SetActive(false);
        }
    }

    private static void InstallRuntimeHardener(RPGameManager manager)
    {
        RPReleaseUIHardener hardener = Object.FindFirstObjectByType<RPReleaseUIHardener>();
        if (hardener == null)
        {
            GameObject obj = new GameObject("RPReleaseUIHardener");
            hardener = obj.AddComponent<RPReleaseUIHardener>();
        }

        hardener.objectivePanel = GameObject.Find("ReleaseObjectivePanel");
        hardener.objectiveText = GameObject.Find("ReleaseObjectiveText")?.GetComponent<Text>();
        hardener.failPanel = GameObject.Find("FailPanel");
        EditorUtility.SetDirty(hardener);
    }

    private static void StyleFullPanel(string name, Color color)
    {
        GameObject obj = GameObject.Find(name);
        if (obj == null) return;

        Image img = obj.GetComponent<Image>();
        if (img == null) img = obj.AddComponent<Image>();
        img.color = color;
        EditorUtility.SetDirty(img);
    }

    private static void EnsureCard(string panelName, string cardName, Vector2 size)
    {
        GameObject panel = GameObject.Find(panelName);
        if (panel == null) return;

        GameObject card = GameObject.Find(cardName);
        if (card == null)
        {
            card = new GameObject(cardName);
            card.transform.SetParent(panel.transform, false);
            card.transform.SetAsFirstSibling();
            RectTransform rect = card.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            Image img = card.AddComponent<Image>();
            img.color = new Color(0.035f, 0.038f, 0.042f, 0.98f);
            img.raycastTarget = false;
            Outline outline = card.AddComponent<Outline>();
            outline.effectColor = new Color(0.22f, 0.24f, 0.25f, 0.80f);
            outline.effectDistance = new Vector2(1.4f, -1.4f);
        }
        else
        {
            RectTransform rect = card.GetComponent<RectTransform>();
            if (rect != null) rect.sizeDelta = size;
        }
    }

    private static void RestyleAllButtons()
    {
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            Image img = button.GetComponent<Image>();
            if (img != null)
            {
                img.color = new Color(0.105f, 0.112f, 0.120f, 1f);
                EditorUtility.SetDirty(img);
            }

            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.105f, 0.112f, 0.120f, 1f);
            colors.highlightedColor = new Color(0.150f, 0.160f, 0.168f, 1f);
            colors.pressedColor = new Color(0.235f, 0.250f, 0.260f, 1f);
            colors.selectedColor = new Color(0.150f, 0.160f, 0.168f, 1f);
            colors.disabledColor = new Color(0.06f, 0.06f, 0.06f, 0.35f);
            button.colors = colors;

            Outline outline = button.GetComponent<Outline>();
            if (outline == null) outline = button.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.30f, 0.32f, 0.33f, 0.65f);
            outline.effectDistance = new Vector2(1f, -1f);

            Text label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.color = TextMain;
                label.fontStyle = FontStyle.Bold;
                label.horizontalOverflow = HorizontalWrapMode.Wrap;
                label.verticalOverflow = VerticalWrapMode.Truncate;
                EditorUtility.SetDirty(label);
            }

            EditorUtility.SetDirty(button);
        }
    }

    private static void RestyleAllText()
    {
        Text[] texts = Object.FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text text in texts)
        {
            if (text == null) continue;
            string n = text.gameObject.name.ToLowerInvariant();
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            if (n.Contains("title") || n.Contains("combo"))
            {
                text.color = TextMain;
                text.fontStyle = FontStyle.Bold;
            }
            else if (n.Contains("hint") || n.Contains("stats") || n.Contains("objective") || n.Contains("reason") || n.Contains("status"))
            {
                text.color = TextSub;
                text.fontStyle = FontStyle.Normal;
            }
            else
            {
                text.color = TextMain;
            }
            EditorUtility.SetDirty(text);
        }
    }

    private static void MoveText(string name, Vector2 anchor, Vector2 size, int fontSize, Color color, FontStyle style)
    {
        Text text = GameObject.Find(name)?.GetComponent<Text>();
        if (text == null) return;
        RectTransform rect = text.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
        }
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        EditorUtility.SetDirty(text);
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
            label.color = TextMain;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
        }
    }

    private static Text CreateText(string name, Transform parent, string value, int fontSize, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Color color)
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
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private static GameObject CreateCube(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        ApplyMaterialToRenderer(obj, material);
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);
        return obj;
    }

    private static void CreateCorner(Transform parent, Vector3 position, Material material)
    {
        GameObject corner = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        corner.name = "MinimalCorner";
        corner.transform.SetParent(parent);
        corner.transform.position = position;
        corner.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
        ApplyMaterialToRenderer(corner, material);
        Collider collider = corner.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);
    }

    private static void CreateLight(Transform parent, string name, LightType type, Vector3 position, Quaternion rotation, Color color, float intensity, float range)
    {
        GameObject lightObject = new GameObject(name);
        lightObject.transform.SetParent(parent);
        lightObject.transform.position = position;
        lightObject.transform.rotation = rotation;
        Light light = lightObject.AddComponent<Light>();
        light.type = type;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
    }

    private static void ApplyMaterialToRenderer(GameObject obj, Material material)
    {
        if (obj == null || material == null) return;
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
            EditorUtility.SetDirty(renderer);
        }
    }

    private static Material CreateMaterial(string name, Color color, float smoothness, float metallic)
    {
        string folderPath = "Assets/Materials";
        if (!AssetDatabase.IsValidFolder(folderPath)) AssetDatabase.CreateFolder("Assets", "Materials");
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
        if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
        if (existing == null) AssetDatabase.CreateAsset(material, assetPath);
        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();
        return material;
    }

    private static Material CreateTransparentMaterial(string name, Color color, float smoothness)
    {
        Material material = CreateMaterial(name, color, smoothness, 0.0f);
        if (material.HasProperty("_Surface")) material.SetFloat("_Surface", 1f);
        if (material.HasProperty("_AlphaClip")) material.SetFloat("_AlphaClip", 0f);
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        material.color = color;
        material.renderQueue = 3000;
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
