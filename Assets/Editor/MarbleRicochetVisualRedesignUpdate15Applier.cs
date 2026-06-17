
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MarbleRicochetVisualRedesignUpdate15Applier
{
    private static readonly Color VoidBlack = new Color(0.018f, 0.022f, 0.026f, 1f);
    private static readonly Color Graphite = new Color(0.055f, 0.068f, 0.073f, 1f);
    private static readonly Color GraphiteLight = new Color(0.085f, 0.105f, 0.112f, 1f);
    private static readonly Color RailBlack = new Color(0.012f, 0.015f, 0.017f, 1f);
    private static readonly Color Brass = new Color(0.86f, 0.58f, 0.25f, 1f);
    private static readonly Color SoftBrass = new Color(1.0f, 0.73f, 0.38f, 1f);
    private static readonly Color IcyCyan = new Color(0.35f, 0.88f, 1.0f, 1f);
    private static readonly Color RubyHazard = new Color(0.98f, 0.20f, 0.30f, 1f);
    private static readonly Color TextIvory = new Color(0.94f, 0.91f, 0.82f, 1f);

    [MenuItem("Tools/Marble Ricochet/Apply Visual Redesign Update 15")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 15.");
            return;
        }

        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (manager == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Update 15", "Could not find RPGameManager or Canvas. Open the Marble Ricochet scene first.", "OK");
            return;
        }

        Material tableMat = CreateMaterial("MR15_Smoked_Graphite_Table", Graphite, 0.78f, 0.0f);
        Material tableEdgeMat = CreateMaterial("MR15_Slate_Table_Edge", GraphiteLight, 0.70f, 0.0f);
        Material railMat = CreateMaterial("MR15_Matte_Black_Rubber_Rail", RailBlack, 0.42f, 0.0f);
        Material brassMat = CreateEmissionMaterial("MR15_Brushed_Brass_Accent", Brass, 0.34f, 0.82f);
        Material cyanMat = CreateEmissionMaterial("MR15_Icy_Cyan_Gameplay_Accent", IcyCyan, 0.85f, 0.72f);
        Material glassMat = CreateTransparentMaterial("MR15_Premium_Icy_Glass_Target", new Color(0.35f, 0.88f, 1.0f, 0.62f), 0.95f);
        Material glassWallMat = CreateTransparentMaterial("MR15_Premium_Clear_Breakable_Glass", new Color(0.70f, 0.94f, 1.0f, 0.38f), 0.92f);
        Material bumperMat = CreateEmissionMaterial("MR15_Gunmetal_Brass_Bumper", new Color(0.95f, 0.56f, 0.22f), 0.52f, 0.72f);
        Material marbleMat = CreateMaterial("MR15_White_Marble_Ball", new Color(0.94f, 0.94f, 0.89f, 1f), 0.92f, 0.0f);
        Material holeMat = CreateMaterial("MR15_Deep_Black_Pocket", new Color(0.000f, 0.002f, 0.004f, 1f), 0.22f, 0.0f);
        Material hazardRingMat = CreateEmissionMaterial("MR15_Ruby_Hazard_Ring", RubyHazard, 0.62f, 0.72f);
        Material rotatorMat = CreateEmissionMaterial("MR15_Gunmetal_Rotating_Blocker", new Color(0.55f, 0.64f, 0.68f, 1f), 0.28f, 0.82f);

        RemoveOldVisualRoots();
        ApplyGameplayMaterials(manager, railMat, glassMat, glassWallMat, bumperMat, marbleMat);
        CreateGraphiteTable(tableMat, tableEdgeMat, railMat, brassMat, cyanMat);
        CreateCinematicAmbience();
        RestyleHazardsIfPresent(manager, holeMat, hazardRingMat, rotatorMat);
        RestyleUI(canvas.transform);
        RestyleImportantText();
        CreateAppIconDirectionNote();

        EditorUtility.SetDirty(manager);
        SaveScene();

        EditorUtility.DisplayDialog("Update 15 Applied", "Visual Redesign Update 15 applied. Graphite/brass/icy-cyan premium tabletop direction is active.", "OK");
    }

    private static void RemoveOldVisualRoots()
    {
        string[] names = { "MR12_Premium_Table_Visuals", "MR12_Premium_Lighting", "Permanent_Visible_Bounce_Rails_Update11", "MR15_Graphite_Premium_Table", "MR15_Cinematic_Ambience" };
        foreach (string name in names)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null) Object.DestroyImmediate(obj);
        }
    }

    private static void ApplyGameplayMaterials(RPGameManager manager, Material railMat, Material glassMat, Material glassWallMat, Material bumperMat, Material marbleMat)
    {
        manager.wallMaterial = railMat;
        manager.targetMaterial = glassMat;
        manager.breakableGlassMaterial = glassWallMat;
        manager.metalBumperMaterial = bumperMat;
        manager.ballMaterial = marbleMat;
        ApplyMaterialToRenderer(manager.wallPrefab, railMat);
        ApplyMaterialToRenderer(manager.targetPrefab, glassMat);
        ApplyMaterialToRenderer(manager.breakableGlassPrefab, glassWallMat);
        ApplyMaterialToRenderer(manager.metalBumperPrefab, bumperMat);
        if (manager.ball != null) ApplyMaterialToRenderer(manager.ball.gameObject, marbleMat);
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

    private static void CreateGraphiteTable(Material tableMat, Material tableEdgeMat, Material railMat, Material brassMat, Material cyanMat)
    {
        GameObject root = new GameObject("MR15_Graphite_Premium_Table");
        CreateCube(root.transform, "Deep_Void_Backdrop", new Vector3(0f, -0.22f, 0f), new Vector3(18.0f, 0.08f, 22.0f), CreateMaterial("MR15_Void_Backdrop", VoidBlack, 0.25f, 0.0f));
        CreateCube(root.transform, "Smoked_Graphite_Surface", new Vector3(0f, -0.035f, 0f), new Vector3(11.65f, 0.060f, 15.65f), tableMat);
        CreateCube(root.transform, "Slate_Bevel_Layer", new Vector3(0f, -0.075f, 0f), new Vector3(11.95f, 0.055f, 15.95f), tableEdgeMat);
        CreateCube(root.transform, "Shadow_Under_Table", new Vector3(0f, -0.125f, 0f), new Vector3(12.35f, 0.035f, 16.35f), CreateMaterial("MR15_Table_Shadow_Layer", new Color(0.005f, 0.006f, 0.007f, 1f), 0.18f, 0.0f));

        Material grainMat = CreateMaterial("MR15_Subtle_Graphite_Grain", new Color(0.105f, 0.125f, 0.132f, 1f), 0.64f, 0.0f);
        for (int i = -5; i <= 5; i++)
        {
            GameObject line = CreateCube(root.transform, "Subtle_Graphite_Grain_Long_" + i, new Vector3(i * 0.95f, 0.006f, 0f), new Vector3(0.012f, 0.010f, 14.25f), grainMat);
            SetNoShadow(line);
        }
        for (int z = -6; z <= 6; z += 3)
        {
            GameObject line = CreateCube(root.transform, "Subtle_Graphite_Grain_Cross_" + z, new Vector3(0f, 0.008f, z), new Vector3(10.35f, 0.010f, 0.012f), grainMat);
            SetNoShadow(line);
        }

        CreateCube(root.transform, "Premium_Top_Rail_Black_Rubber", new Vector3(0f, 0.24f, 7.10f), new Vector3(10.95f, 0.46f, 0.48f), railMat);
        CreateCube(root.transform, "Premium_Bottom_Rail_Black_Rubber", new Vector3(0f, 0.24f, -7.10f), new Vector3(10.95f, 0.46f, 0.48f), railMat);
        CreateCube(root.transform, "Premium_Left_Rail_Black_Rubber", new Vector3(-5.10f, 0.24f, 0f), new Vector3(0.48f, 0.46f, 14.15f), railMat);
        CreateCube(root.transform, "Premium_Right_Rail_Black_Rubber", new Vector3(5.10f, 0.24f, 0f), new Vector3(0.48f, 0.46f, 14.15f), railMat);

        CreateCube(root.transform, "Top_Brass_Lip", new Vector3(0f, 0.515f, 6.77f), new Vector3(10.10f, 0.052f, 0.070f), brassMat);
        CreateCube(root.transform, "Bottom_Brass_Lip", new Vector3(0f, 0.515f, -6.77f), new Vector3(10.10f, 0.052f, 0.070f), brassMat);
        CreateCube(root.transform, "Left_Brass_Lip", new Vector3(-4.77f, 0.515f, 0f), new Vector3(0.070f, 0.052f, 13.35f), brassMat);
        CreateCube(root.transform, "Right_Brass_Lip", new Vector3(4.77f, 0.515f, 0f), new Vector3(0.070f, 0.052f, 13.35f), brassMat);

        CreateCube(root.transform, "Cyan_Playfield_Top_Guide", new Vector3(0f, 0.035f, 6.40f), new Vector3(8.65f, 0.014f, 0.025f), cyanMat);
        CreateCube(root.transform, "Cyan_Playfield_Left_Guide", new Vector3(-4.42f, 0.035f, 0f), new Vector3(0.025f, 0.014f, 11.05f), cyanMat);
        CreateCube(root.transform, "Cyan_Playfield_Right_Guide", new Vector3(4.42f, 0.035f, 0f), new Vector3(0.025f, 0.014f, 11.05f), cyanMat);

        CreateCorner(root.transform, new Vector3(-5.10f, 0.54f, -7.10f), brassMat);
        CreateCorner(root.transform, new Vector3(5.10f, 0.54f, -7.10f), brassMat);
        CreateCorner(root.transform, new Vector3(-5.10f, 0.54f, 7.10f), brassMat);
        CreateCorner(root.transform, new Vector3(5.10f, 0.54f, 7.10f), brassMat);
    }

    private static void CreateCinematicAmbience()
    {
        GameObject root = new GameObject("MR15_Cinematic_Ambience");
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.12f, 0.16f, 0.18f, 1f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.015f, 0.018f, 0.020f, 1f);
        RenderSettings.fogDensity = 0.018f;
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.backgroundColor = VoidBlack;
            EditorUtility.SetDirty(cam);
        }
        CreateLight(root.transform, "MR15_Warm_Key_Light", LightType.Directional, new Vector3(0f, 8f, -3f), Quaternion.Euler(58f, 0f, 28f), new Color(1f, 0.82f, 0.62f, 1f), 1.35f, 10f);
        CreateLight(root.transform, "MR15_Cyan_Glass_Fill", LightType.Point, new Vector3(0f, 4.5f, 1.6f), Quaternion.identity, new Color(0.40f, 0.80f, 1f, 1f), 1.10f, 11f);
        CreateLight(root.transform, "MR15_Brass_Rim_Glow", LightType.Point, new Vector3(0f, 3.3f, -5.6f), Quaternion.identity, new Color(1f, 0.62f, 0.28f, 1f), 0.62f, 8f);
    }

    private static void RestyleHazardsIfPresent(RPGameManager manager, Material holeMat, Material ringMat, Material rotatorMat)
    {
        RPHazardManager hazards = Object.FindFirstObjectByType<RPHazardManager>();
        if (hazards == null) return;
        hazards.holePrefab = CreatePremiumHolePrefab(holeMat, ringMat);
        hazards.rotatingObstaclePrefab = CreatePremiumRotatorPrefab(rotatorMat, manager);
        EditorUtility.SetDirty(hazards);
    }

    private static GameObject CreatePremiumHolePrefab(Material holeMat, Material ringMat)
    {
        GameObject old = GameObject.Find("MR15_PremiumHolePocketPrefab");
        if (old != null) Object.DestroyImmediate(old);
        GameObject root = new GameObject("MR15_PremiumHolePocketPrefab");
        root.transform.position = new Vector3(100f, 100f, 100f);
        root.SetActive(false);

        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disc.name = "Deep_Pocket_Center";
        disc.transform.SetParent(root.transform);
        disc.transform.localPosition = Vector3.zero;
        disc.transform.localScale = new Vector3(1f, 0.035f, 1f);
        ApplyMaterialToRenderer(disc, holeMat);
        Object.DestroyImmediate(disc.GetComponent<Collider>());

        CreateRimBar(root.transform, "Premium_Pocket_Rim_N", new Vector3(0f, 0.062f, 0.52f), new Vector3(0.78f, 0.042f, 0.055f), ringMat);
        CreateRimBar(root.transform, "Premium_Pocket_Rim_S", new Vector3(0f, 0.062f, -0.52f), new Vector3(0.78f, 0.042f, 0.055f), ringMat);
        CreateRimBar(root.transform, "Premium_Pocket_Rim_E", new Vector3(0.52f, 0.062f, 0f), new Vector3(0.055f, 0.042f, 0.78f), ringMat);
        CreateRimBar(root.transform, "Premium_Pocket_Rim_W", new Vector3(-0.52f, 0.062f, 0f), new Vector3(0.055f, 0.042f, 0.78f), ringMat);

        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.48f;
        root.AddComponent<RPHolePocket>();
        return root;
    }

    private static GameObject CreatePremiumRotatorPrefab(Material mat, RPGameManager manager)
    {
        GameObject old = GameObject.Find("MR15_PremiumRotatingObstaclePrefab");
        if (old != null) Object.DestroyImmediate(old);
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = "MR15_PremiumRotatingObstaclePrefab";
        obj.transform.position = new Vector3(100f, 100f, 100f);
        obj.transform.localScale = new Vector3(2.4f, 0.55f, 0.22f);
        obj.SetActive(false);
        ApplyMaterialToRenderer(obj, mat);
        RPObstacle obstacle = obj.AddComponent<RPObstacle>();
        obstacle.type = RPObstacleType.NormalWall;
        obstacle.gameManager = manager;
        obj.AddComponent<RPRotatingObstacle>();
        return obj;
    }

    private static void RestyleUI(Transform canvasTransform)
    {
        StylePanel("MenuPanel", new Color(0.015f, 0.020f, 0.025f, 0.92f));
        StylePanel("WinPanel", new Color(0.015f, 0.020f, 0.025f, 0.94f));
        StylePanel("ShopPanel", new Color(0.012f, 0.018f, 0.023f, 0.96f));
        StylePanel("FailPanel", new Color(0.015f, 0.016f, 0.018f, 0.95f));
        StylePanel("OnboardingPanel", new Color(0.020f, 0.028f, 0.032f, 0.86f));
        StylePanel("LevelObjectivePanel", new Color(0.020f, 0.028f, 0.032f, 0.86f));

        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button button in buttons) StyleButton(button);

        Text[] texts = Object.FindObjectsByType<Text>(FindObjectsSortMode.None);
        foreach (Text text in texts)
        {
            if (text == null) continue;
            string name = text.gameObject.name.ToLowerInvariant();
            if (name.Contains("title") || name.Contains("combo") || name.Contains("reward")) text.color = SoftBrass;
            else if (name.Contains("hint") || name.Contains("skill") || name.Contains("objective") || name.Contains("preview")) text.color = new Color(0.75f, 0.92f, 1f, 1f);
            else text.color = TextIvory;
            EditorUtility.SetDirty(text);
        }
    }

    private static void RestyleImportantText()
    {
        SetTextStyle("TitleText", 46, SoftBrass);
        SetTextStyle("SubtitleText", 21, new Color(0.75f, 0.88f, 0.92f, 1f));
        SetTextStyle("WinTitleText", 34, SoftBrass);
        SetTextStyle("FailTitleText", 42, RubyHazard);
        SetTextStyle("SkillComboText", 40, SoftBrass);
        SetTextStyle("HazardHintText", 20, new Color(1f, 0.62f, 0.38f, 1f));
    }

    private static void StylePanel(string name, Color color)
    {
        GameObject panel = GameObject.Find(name);
        if (panel == null) return;
        Image image = panel.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
            EditorUtility.SetDirty(image);
        }
        EnsureUIBorder(panel.transform, name + "_MR15_BrassBorder", Brass);
    }

    private static void EnsureUIBorder(Transform parent, string name, Color color)
    {
        if (GameObject.Find(name) != null) return;
        GameObject borderRoot = new GameObject(name);
        borderRoot.transform.SetParent(parent, false);
        borderRoot.transform.SetAsFirstSibling();
        RectTransform rootRect = borderRoot.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        CreateUIBorderLine(borderRoot.transform, "Top", 1f, -2f, color);
        CreateUIBorderLine(borderRoot.transform, "Bottom", 0f, 2f, color);
    }

    private static void CreateUIBorderLine(Transform parent, string suffix, float yAnchor, float yOffset, Color color)
    {
        GameObject line = new GameObject("Border_" + suffix);
        line.transform.SetParent(parent, false);
        RectTransform rect = line.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, yAnchor);
        rect.anchorMax = new Vector2(1f, yAnchor);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, yOffset);
        rect.sizeDelta = new Vector2(0f, 4f);
        Image image = line.AddComponent<Image>();
        image.color = new Color(color.r, color.g, color.b, 0.34f);
        image.raycastTarget = false;
    }

    private static void StyleButton(Button button)
    {
        if (button == null) return;
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(0.050f, 0.064f, 0.070f, 0.98f);
            EditorUtility.SetDirty(image);
        }
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.050f, 0.064f, 0.070f, 0.98f);
        colors.highlightedColor = new Color(0.120f, 0.160f, 0.175f, 1f);
        colors.pressedColor = new Color(0.90f, 0.58f, 0.25f, 1f);
        colors.selectedColor = new Color(0.10f, 0.13f, 0.15f, 1f);
        colors.disabledColor = new Color(0.08f, 0.08f, 0.08f, 0.4f);
        button.colors = colors;
        Text label = button.GetComponentInChildren<Text>();
        if (label != null)
        {
            label.color = new Color(0.94f, 0.88f, 0.72f, 1f);
            label.fontStyle = FontStyle.Bold;
            EditorUtility.SetDirty(label);
        }
        Outline outline = button.GetComponent<Outline>();
        if (outline == null) outline = button.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.86f, 0.58f, 0.25f, 0.55f);
        outline.effectDistance = new Vector2(1.2f, -1.2f);
        EditorUtility.SetDirty(button);
    }

    private static void SetTextStyle(string name, int fontSize, Color color)
    {
        Text text = GameObject.Find(name)?.GetComponent<Text>();
        if (text == null) return;
        text.fontSize = fontSize;
        text.color = color;
        text.fontStyle = FontStyle.Bold;
        EditorUtility.SetDirty(text);
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
        corner.name = "Graphite_Table_Brass_Corner";
        corner.transform.SetParent(parent);
        corner.transform.position = position;
        corner.transform.localScale = new Vector3(0.24f, 0.24f, 0.24f);
        ApplyMaterialToRenderer(corner, material);
        Collider collider = corner.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);
    }

    private static void CreateRimBar(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.name = name;
        bar.transform.SetParent(parent);
        bar.transform.localPosition = localPosition;
        bar.transform.localScale = localScale;
        ApplyMaterialToRenderer(bar, material);
        Collider collider = bar.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);
    }

    private static void SetNoShadow(GameObject obj)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
    }

    private static void CreateLight(Transform parent, string name, LightType type, Vector3 position, Quaternion rotation, Color color, float intensity, float range)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        Light light = obj.AddComponent<Light>();
        light.type = type;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
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

    private static Material CreateEmissionMaterial(string name, Color color, float emissionStrength, float smoothness)
    {
        Material material = CreateMaterial(name, color, smoothness, 0.15f);
        if (material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * emissionStrength);
        }
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

    private static void CreateAppIconDirectionNote()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Textures")) AssetDatabase.CreateFolder("Assets", "Textures");
        if (!AssetDatabase.IsValidFolder("Assets/Textures/AppIcon")) AssetDatabase.CreateFolder("Assets/Textures", "AppIcon");
        string notePath = "Assets/Textures/AppIcon/UPDATE_15_VISUAL_DIRECTION_NOTE.txt";
        System.IO.File.WriteAllText(notePath, "Update 15 visual direction: graphite luxury tabletop, matte black rails, brushed brass accents, icy cyan glass/gameplay accents, ruby hazards. Avoid cheap brown, clown colors and neon overload.");
        AssetDatabase.Refresh();
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
