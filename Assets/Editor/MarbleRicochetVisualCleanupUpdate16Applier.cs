#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MarbleRicochetVisualCleanupUpdate16Applier
{
    private static readonly Color VoidBlack = new Color(0.015f, 0.016f, 0.018f, 1f);
    private static readonly Color CardBlack = new Color(0.05f, 0.055f, 0.06f, 0.96f);
    private static readonly Color SoftGraphite = new Color(0.11f, 0.12f, 0.125f, 1f);
    private static readonly Color DeepGraphite = new Color(0.055f, 0.060f, 0.066f, 1f);
    private static readonly Color MatteBlack = new Color(0.018f, 0.020f, 0.022f, 1f);
    private static readonly Color SatinBrass = new Color(0.76f, 0.58f, 0.33f, 1f);
    private static readonly Color SoftCyan = new Color(0.57f, 0.86f, 0.95f, 1f);
    private static readonly Color MutedRuby = new Color(0.82f, 0.28f, 0.36f, 1f);
    private static readonly Color TextIvory = new Color(0.94f, 0.92f, 0.86f, 1f);
    private static readonly Color TextMuted = new Color(0.73f, 0.79f, 0.82f, 1f);

    [MenuItem("Tools/Marble Ricochet/Apply Visual Cleanup Update 16")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 16.");
            return;
        }

        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (manager == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Update 16", "Could not find RPGameManager or Canvas.\n\nOpen the Marble Ricochet scene first.", "OK");
            return;
        }

        RemoveVisualNoiseRoots();
        SetupCameraAndAmbient();

        Material backdropMat = CreateMaterial("MR16_Backdrop_Matte_Black", VoidBlack, 0.18f, 0.0f);
        Material surfaceMat = CreateMaterial("MR16_Playfield_Soft_Graphite", DeepGraphite, 0.78f, 0.05f);
        Material frameMat = CreateMaterial("MR16_Frame_Matte_Black", MatteBlack, 0.42f, 0.0f);
        Material brassMat = CreateEmissionMaterial("MR16_Brass_Satin", SatinBrass, 0.18f, 0.82f);
        Material guideMat = CreateEmissionMaterial("MR16_Guide_Soft_Cyan", new Color(0.42f,0.72f,0.82f,1f), 0.06f, 0.72f);
        Material glassMat = CreateTransparentMaterial("MR16_Target_Icy_Glass", new Color(0.55f, 0.90f, 1.0f, 0.58f), 0.95f);
        Material glassWallMat = CreateTransparentMaterial("MR16_Breakable_Glass", new Color(0.82f, 0.96f, 1.0f, 0.32f), 0.92f);
        Material bumperMat = CreateEmissionMaterial("MR16_Bumper_Gunmetal_Brass", new Color(0.78f,0.58f,0.32f,1f), 0.22f, 0.75f);
        Material ballMat = CreateMaterial("MR16_Marble_Ball_Clean", new Color(0.95f,0.95f,0.92f,1f), 0.94f, 0.0f);
        Material holeMat = CreateMaterial("MR16_Pocket_Black", new Color(0.005f,0.005f,0.006f,1f), 0.12f, 0.0f);
        Material holeAccentMat = CreateEmissionMaterial("MR16_Pocket_Accent_Ruby", MutedRuby, 0.18f, 0.65f);
        Material rotatorMat = CreateEmissionMaterial("MR16_Rotator_Gunmetal", new Color(0.44f,0.50f,0.54f,1f), 0.08f, 0.82f);

        BuildCleanPremiumTable(backdropMat, surfaceMat, frameMat, brassMat, guideMat);
        ApplyGameplayMaterials(manager, frameMat, glassMat, glassWallMat, bumperMat, ballMat);
        RefreshHazardPrefabs(manager, holeMat, holeAccentMat, rotatorMat);
        CleanupAndRestyleUI(canvas.transform);
        TidyGameplayText();

        SaveScene();
        EditorUtility.DisplayDialog("Update 16 Applied", "Visual Cleanup Update 16 applied.\n\nReduced line noise, improved premium feel, cleaner hierarchy and smoother UI styling.", "OK");
    }

    private static void RemoveVisualNoiseRoots()
    {
        string[] roots = {
            "MR15_Graphite_Premium_Table",
            "MR15_Cinematic_Ambience",
            "MR12_Premium_Table_Visuals",
            "MR12_Premium_Lighting",
            "MR16_Clean_Premium_Table"
        };

        foreach (string rootName in roots)
        {
            GameObject root = GameObject.Find(rootName);
            if (root != null) Object.DestroyImmediate(root);
        }
    }

    private static void SetupCameraAndAmbient()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.backgroundColor = VoidBlack;
            EditorUtility.SetDirty(cam);
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.12f, 0.13f, 0.14f, 1f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.02f, 0.022f, 0.026f, 1f);
        RenderSettings.fogDensity = 0.013f;
    }

    private static void BuildCleanPremiumTable(Material backdropMat, Material surfaceMat, Material frameMat, Material brassMat, Material guideMat)
    {
        GameObject root = new GameObject("MR16_Clean_Premium_Table");

        CreateCube(root.transform, "DeepBackdrop", new Vector3(0f, -0.26f, 0f), new Vector3(24f, 0.10f, 28f), backdropMat);
        CreateCube(root.transform, "SoftGraphiteSurface", new Vector3(0f, -0.035f, 0f), new Vector3(11.55f, 0.06f, 15.55f), surfaceMat);
        CreateCube(root.transform, "TableBase", new Vector3(0f, -0.095f, 0f), new Vector3(12.10f, 0.05f, 16.10f), CreateMaterial("MR16_TableBase", SoftGraphite, 0.58f, 0.0f));
        CreateCube(root.transform, "OuterShadowBase", new Vector3(0f, -0.14f, 0f), new Vector3(12.7f, 0.03f, 16.7f), CreateMaterial("MR16_OuterShadowBase", new Color(0.01f,0.01f,0.012f,1f), 0.12f, 0.0f));

        // Clean frame. No full grid noise.
        CreateCube(root.transform, "TopRail", new Vector3(0f, 0.24f, 7.10f), new Vector3(10.95f, 0.46f, 0.48f), frameMat);
        CreateCube(root.transform, "BottomRail", new Vector3(0f, 0.24f, -7.10f), new Vector3(10.95f, 0.46f, 0.48f), frameMat);
        CreateCube(root.transform, "LeftRail", new Vector3(-5.10f, 0.24f, 0f), new Vector3(0.48f, 0.46f, 14.15f), frameMat);
        CreateCube(root.transform, "RightRail", new Vector3(5.10f, 0.24f, 0f), new Vector3(0.48f, 0.46f, 14.15f), frameMat);

        CreateCube(root.transform, "TopBrassLip", new Vector3(0f, 0.505f, 6.77f), new Vector3(10.0f, 0.045f, 0.06f), brassMat);
        CreateCube(root.transform, "BottomBrassLip", new Vector3(0f, 0.505f, -6.77f), new Vector3(10.0f, 0.045f, 0.06f), brassMat);
        CreateCube(root.transform, "LeftBrassLip", new Vector3(-4.77f, 0.505f, 0f), new Vector3(0.06f, 0.045f, 13.35f), brassMat);
        CreateCube(root.transform, "RightBrassLip", new Vector3(4.77f, 0.505f, 0f), new Vector3(0.06f, 0.045f, 13.35f), brassMat);

        // Minimal launch guide only, instead of annoying full-board lines.
        CreateCube(root.transform, "BottomLaunchGuide", new Vector3(0f, 0.014f, -5.15f), new Vector3(1.9f, 0.01f, 0.04f), guideMat);
        CreateCube(root.transform, "BottomLaunchGuideLeft", new Vector3(-0.95f, 0.014f, -4.65f), new Vector3(0.04f, 0.01f, 0.85f), guideMat);
        CreateCube(root.transform, "BottomLaunchGuideRight", new Vector3(0.95f, 0.014f, -4.65f), new Vector3(0.04f, 0.01f, 0.85f), guideMat);

        // Very subtle center glow pad for table richness.
        GameObject glow = CreateCube(root.transform, "CenterSoftGlow", new Vector3(0f, 0.002f, 0.8f), new Vector3(4.0f, 0.002f, 5.4f), CreateTransparentMaterial("MR16_CenterSoftGlow", new Color(0.10f,0.14f,0.16f,0.16f), 0.0f));
        SetNoShadow(glow);

        CreateCorner(root.transform, new Vector3(-5.08f, 0.53f, -7.08f), brassMat);
        CreateCorner(root.transform, new Vector3(5.08f, 0.53f, -7.08f), brassMat);
        CreateCorner(root.transform, new Vector3(-5.08f, 0.53f, 7.08f), brassMat);
        CreateCorner(root.transform, new Vector3(5.08f, 0.53f, 7.08f), brassMat);

        CreateLight(root.transform, "WarmKey", LightType.Directional, new Vector3(0f, 6f, -3f), Quaternion.Euler(56f, 0f, 24f), new Color(1f, 0.86f, 0.72f, 1f), 1.10f, 10f);
        CreateLight(root.transform, "CoolFill", LightType.Point, new Vector3(0f, 4.2f, 1.5f), Quaternion.identity, new Color(0.56f, 0.85f, 0.96f, 1f), 0.56f, 9f);
    }

    private static void ApplyGameplayMaterials(RPGameManager manager, Material wall, Material target, Material breakable, Material bumper, Material ball)
    {
        manager.wallMaterial = wall;
        manager.targetMaterial = target;
        manager.breakableGlassMaterial = breakable;
        manager.metalBumperMaterial = bumper;
        manager.ballMaterial = ball;

        ApplyMaterialToRenderer(manager.wallPrefab, wall);
        ApplyMaterialToRenderer(manager.targetPrefab, target);
        ApplyMaterialToRenderer(manager.breakableGlassPrefab, breakable);
        ApplyMaterialToRenderer(manager.metalBumperPrefab, bumper);
        if (manager.ball != null) ApplyMaterialToRenderer(manager.ball.gameObject, ball);
    }

    private static void RefreshHazardPrefabs(RPGameManager manager, Material hole, Material accent, Material rotator)
    {
        RPHazardManager hazards = Object.FindFirstObjectByType<RPHazardManager>();
        if (hazards == null) return;

        hazards.holePrefab = CreateHolePrefab(hole, accent);
        hazards.rotatingObstaclePrefab = CreateRotatorPrefab(rotator, manager);
        EditorUtility.SetDirty(hazards);
    }

    private static GameObject CreateHolePrefab(Material hole, Material accent)
    {
        GameObject existing = GameObject.Find("MR16_PremiumHolePrefab");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject root = new GameObject("MR16_PremiumHolePrefab");
        root.transform.position = new Vector3(100f,100f,100f);
        root.SetActive(false);

        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disc.name = "PocketCore";
        disc.transform.SetParent(root.transform);
        disc.transform.localPosition = Vector3.zero;
        disc.transform.localScale = new Vector3(1f, 0.035f, 1f);
        ApplyMaterialToRenderer(disc, hole);
        Object.DestroyImmediate(disc.GetComponent<Collider>());

        CreateRimBar(root.transform, "PocketAccentN", new Vector3(0f, 0.055f, 0.50f), new Vector3(0.70f, 0.03f, 0.04f), accent);
        CreateRimBar(root.transform, "PocketAccentS", new Vector3(0f, 0.055f, -0.50f), new Vector3(0.70f, 0.03f, 0.04f), accent);
        CreateRimBar(root.transform, "PocketAccentE", new Vector3(0.50f, 0.055f, 0f), new Vector3(0.04f, 0.03f, 0.70f), accent);
        CreateRimBar(root.transform, "PocketAccentW", new Vector3(-0.50f, 0.055f, 0f), new Vector3(0.04f, 0.03f, 0.70f), accent);

        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.48f;
        root.AddComponent<RPHolePocket>();
        return root;
    }

    private static GameObject CreateRotatorPrefab(Material rotator, RPGameManager manager)
    {
        GameObject existing = GameObject.Find("MR16_RotatorPrefab");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "MR16_RotatorPrefab";
        obj.transform.position = new Vector3(100f,100f,100f);
        obj.transform.localScale = new Vector3(2.4f, 0.55f, 0.22f);
        obj.SetActive(false);
        ApplyMaterialToRenderer(obj, rotator);
        RPObstacle obstacle = obj.AddComponent<RPObstacle>();
        obstacle.type = RPObstacleType.NormalWall;
        obstacle.gameManager = manager;
        obj.AddComponent<RPRotatingObstacle>();
        return obj;
    }

    private static void CleanupAndRestyleUI(Transform canvasTransform)
    {
        StylePanelAndCard("MenuPanel");
        StylePanelAndCard("WinPanel");
        StylePanelAndCard("ShopPanel");
        StylePanelAndCard("FailPanel");
        StylePanelAndCard("LevelObjectivePanel");

        RectTransform objectiveRect = GameObject.Find("LevelObjectivePanel")?.GetComponent<RectTransform>();
        if (objectiveRect != null)
        {
            objectiveRect.anchorMin = new Vector2(0.5f, 0.79f);
            objectiveRect.anchorMax = new Vector2(0.5f, 0.79f);
            objectiveRect.pivot = new Vector2(0.5f, 0.5f);
            objectiveRect.anchoredPosition = Vector2.zero;
            objectiveRect.sizeDelta = new Vector2(640f, 62f);
        }

        RectTransform hazardRect = GameObject.Find("HazardHintText")?.GetComponent<RectTransform>();
        if (hazardRect != null)
        {
            hazardRect.anchorMin = new Vector2(0.5f, 0.73f);
            hazardRect.anchorMax = new Vector2(0.5f, 0.73f);
            hazardRect.pivot = new Vector2(0.5f, 0.5f);
            hazardRect.anchoredPosition = Vector2.zero;
            hazardRect.sizeDelta = new Vector2(620f, 36f);
        }

        RectTransform abilityRect = GameObject.Find("BallAbilityHudText")?.GetComponent<RectTransform>();
        if (abilityRect != null)
        {
            abilityRect.anchorMin = new Vector2(0.82f, 0.92f);
            abilityRect.anchorMax = new Vector2(0.82f, 0.92f);
            abilityRect.pivot = new Vector2(0.5f, 0.5f);
            abilityRect.anchoredPosition = Vector2.zero;
            abilityRect.sizeDelta = new Vector2(160f, 26f);
        }

        // Top info cleanup
        Text hintsText = FindTextByNameContains("hint");
        if (hintsText != null)
        {
            hintsText.fontSize = 15;
            hintsText.color = TextMuted;
            RectTransform rt = hintsText.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 0.95f);
                rt.anchorMax = new Vector2(0.5f, 0.95f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(180f, 22f);
            }
        }

        Text abilityText = GameObject.Find("BallAbilityHudText")?.GetComponent<Text>();
        if (abilityText != null)
        {
            abilityText.fontSize = 13;
            abilityText.alignment = TextAnchor.MiddleRight;
            abilityText.color = SoftCyan;
        }

        Text objectiveText = GameObject.Find("LevelObjectiveText")?.GetComponent<Text>();
        if (objectiveText != null)
        {
            objectiveText.fontSize = 16;
            objectiveText.color = TextMuted;
            objectiveText.alignment = TextAnchor.MiddleCenter;
        }

        Text hazardText = GameObject.Find("HazardHintText")?.GetComponent<Text>();
        if (hazardText != null)
        {
            hazardText.fontSize = 16;
            hazardText.color = new Color(0.95f,0.82f,0.64f,1f);
            hazardText.alignment = TextAnchor.MiddleCenter;
        }

        // Clean fail overlay and win/menu/shop hierarchy.
        RestyleText("FailTitleText", 32, new Color(0.96f,0.75f,0.55f,1f), FontStyle.Bold);
        RestyleText("FailReasonText", 18, TextIvory, FontStyle.Normal);
        RestyleText("WinTitleText", 30, new Color(0.96f,0.80f,0.60f,1f), FontStyle.Bold);
        RestyleText("SkillComboText", 30, new Color(0.94f,0.86f,0.70f,1f), FontStyle.Bold);
        RestyleText("TitleText", 40, new Color(0.96f,0.84f,0.68f,1f), FontStyle.Bold);
        RestyleText("SubtitleText", 17, TextMuted, FontStyle.Normal);

        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button btn in buttons)
        {
            StyleButton(btn);
        }

        Text[] texts = Object.FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text t in texts)
        {
            if (t == null) continue;
            string n = t.gameObject.name.ToLowerInvariant();
            if (n.Contains("button") || n == "text") continue;
            if (n.Contains("title") || n.Contains("reward") || n.Contains("coins") || n.Contains("combo"))
                t.color = new Color(0.95f,0.84f,0.68f,1f);
            else if (n.Contains("hint") || n.Contains("ability") || n.Contains("objective") || n.Contains("stats") || n.Contains("preview"))
                t.color = TextMuted;
            else
                t.color = TextIvory;
        }
    }

    private static void TidyGameplayText()
    {
        // Try to make top-of-screen UI breathe more.
        Text skillBreakdown = GameObject.Find("SkillScoreBreakdownText")?.GetComponent<Text>();
        if (skillBreakdown != null)
        {
            skillBreakdown.fontSize = 16;
            skillBreakdown.color = TextMuted;
            RectTransform rt = skillBreakdown.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 0.42f);
                rt.anchorMax = new Vector2(0.5f, 0.42f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(620f, 72f);
            }
        }

        Text reward = FindTextByNameContains("reward");
        if (reward != null) reward.fontSize = 16;
    }

    private static void StylePanelAndCard(string panelName)
    {
        GameObject panel = GameObject.Find(panelName);
        if (panel == null) return;

        Image panelImage = panel.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = new Color(0.008f,0.010f,0.012f,0.84f);
            EditorUtility.SetDirty(panelImage);
        }

        string cardName = panelName + "_MR16Card";
        GameObject existingCard = GameObject.Find(cardName);
        if (existingCard == null)
        {
            GameObject card = new GameObject(cardName);
            card.transform.SetParent(panel.transform, false);
            card.transform.SetAsFirstSibling();
            RectTransform rt = card.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = CardSizeForPanel(panelName);
            Image img = card.AddComponent<Image>();
            img.color = CardBlack;
            img.raycastTarget = false;
            Outline outline = card.AddComponent<Outline>();
            outline.effectColor = new Color(SatinBrass.r, SatinBrass.g, SatinBrass.b, 0.24f);
            outline.effectDistance = new Vector2(2f, -2f);
        }
    }

    private static Vector2 CardSizeForPanel(string panelName)
    {
        if (panelName == "FailPanel") return new Vector2(470f, 420f);
        if (panelName == "WinPanel") return new Vector2(470f, 460f);
        if (panelName == "ShopPanel") return new Vector2(520f, 520f);
        if (panelName == "MenuPanel") return new Vector2(500f, 450f);
        return new Vector2(620f, 70f);
    }

    private static void StyleButton(Button button)
    {
        if (button == null) return;
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(0.11f,0.12f,0.14f,0.98f);
            EditorUtility.SetDirty(image);
        }
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.11f,0.12f,0.14f,0.98f);
        colors.highlightedColor = new Color(0.16f,0.17f,0.19f,1f);
        colors.pressedColor = new Color(0.74f,0.56f,0.32f,1f);
        colors.selectedColor = new Color(0.16f,0.17f,0.19f,1f);
        colors.disabledColor = new Color(0.08f,0.08f,0.08f,0.38f);
        button.colors = colors;

        Outline outline = button.GetComponent<Outline>();
        if (outline == null) outline = button.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(SatinBrass.r, SatinBrass.g, SatinBrass.b, 0.42f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        Text label = button.GetComponentInChildren<Text>();
        if (label != null)
        {
            label.color = TextIvory;
            label.fontStyle = FontStyle.Bold;
            label.fontSize = Mathf.Max(15, label.fontSize);
        }
        EditorUtility.SetDirty(button);
    }

    private static void RestyleText(string name, int size, Color color, FontStyle style)
    {
        Text t = GameObject.Find(name)?.GetComponent<Text>();
        if (t == null) return;
        t.fontSize = size;
        t.color = color;
        t.fontStyle = style;
        EditorUtility.SetDirty(t);
    }

    private static Text FindTextByNameContains(string key)
    {
        Text[] texts = Object.FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text t in texts)
        {
            if (t != null && t.gameObject.name.ToLowerInvariant().Contains(key.ToLowerInvariant()))
                return t;
        }
        return null;
    }

    private static void ApplyMaterialToRenderer(GameObject obj, Material mat)
    {
        if (obj == null || mat == null) return;
        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sharedMaterial = mat;
            EditorUtility.SetDirty(mr);
        }
    }

    private static GameObject CreateCube(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.position = pos;
        obj.transform.localScale = scale;
        ApplyMaterialToRenderer(obj, mat);
        Collider c = obj.GetComponent<Collider>();
        if (c != null) Object.DestroyImmediate(c);
        return obj;
    }

    private static void CreateCorner(Transform parent, Vector3 pos, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = "FrameCorner";
        obj.transform.SetParent(parent);
        obj.transform.position = pos;
        obj.transform.localScale = new Vector3(0.18f,0.18f,0.18f);
        ApplyMaterialToRenderer(obj, mat);
        Collider c = obj.GetComponent<Collider>();
        if (c != null) Object.DestroyImmediate(c);
    }

    private static void CreateRimBar(Transform parent, string name, Vector3 localPos, Vector3 localScale, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.localPosition = localPos;
        obj.transform.localScale = localScale;
        ApplyMaterialToRenderer(obj, mat);
        Collider c = obj.GetComponent<Collider>();
        if (c != null) Object.DestroyImmediate(c);
    }

    private static void SetNoShadow(GameObject obj)
    {
        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
        }
    }

    private static void CreateLight(Transform parent, string name, LightType type, Vector3 pos, Quaternion rot, Color color, float intensity, float range)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        Light l = obj.AddComponent<Light>();
        l.type = type;
        l.color = color;
        l.intensity = intensity;
        l.range = range;
    }

    private static Material CreateMaterial(string name, Color color, float smoothness, float metallic)
    {
        string folder = "Assets/Materials";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets", "Materials");
        string path = folder + "/" + name + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Simple Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material m = existing != null ? existing : new Material(shader);
        m.shader = shader;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
        m.color = color;
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smoothness);
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
        if (existing == null) AssetDatabase.CreateAsset(m, path);
        EditorUtility.SetDirty(m);
        AssetDatabase.SaveAssets();
        return m;
    }

    private static Material CreateEmissionMaterial(string name, Color color, float emissionStrength, float smoothness)
    {
        Material m = CreateMaterial(name, color, smoothness, 0.15f);
        if (m.HasProperty("_EmissionColor"))
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", color * emissionStrength);
        }
        EditorUtility.SetDirty(m);
        AssetDatabase.SaveAssets();
        return m;
    }

    private static Material CreateTransparentMaterial(string name, Color color, float smoothness)
    {
        Material m = CreateMaterial(name, color, smoothness, 0.0f);
        if (m.HasProperty("_Surface")) m.SetFloat("_Surface", 1f);
        if (m.HasProperty("_AlphaClip")) m.SetFloat("_AlphaClip", 0f);
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
        m.color = color;
        m.renderQueue = 3000;
        EditorUtility.SetDirty(m);
        AssetDatabase.SaveAssets();
        return m;
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
