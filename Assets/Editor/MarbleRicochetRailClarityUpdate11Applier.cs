#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MarbleRicochetRailClarityUpdate11Applier
{
    [MenuItem("Tools/Marble Ricochet/Apply Rail Clarity Update 11")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 11.");
            return;
        }

        RPGameManager manager = Object.FindFirstObjectByType<RPGameManager>();

        if (manager == null)
        {
            EditorUtility.DisplayDialog(
                "Update 11",
                "Could not find RPGameManager.\n\nFirst open the Marble Ricochet scene.",
                "OK"
            );
            return;
        }

        Material visibleRailMaterial = CreateMaterial(
            "MR_Visible_Rubber_Rail_Update11",
            new Color(0.070f, 0.055f, 0.040f),
            0.38f
        );

        Material railGoldMaterial = CreateEmissionMaterial(
            "MR_Rail_Gold_Edge_Update11",
            new Color(1.00f, 0.62f, 0.20f),
            0.35f
        );

        Material railBlueHintMaterial = CreateEmissionMaterial(
            "MR_Rail_Blue_Hint_Update11",
            new Color(0.25f, 0.78f, 1.0f),
            0.55f
        );

        ImproveRuntimeWallPrefab(manager, visibleRailMaterial);
        CreatePermanentRailVisuals(visibleRailMaterial, railGoldMaterial, railBlueHintMaterial);
        AddRailEducationHint(manager);
        ImproveHintText(manager);
        SaveScene();

        EditorUtility.DisplayDialog(
            "Update 11 Applied",
            "Rail Clarity Update 11 applied.\n\nThe table edges should now look like clear bounce rails.",
            "OK"
        );
    }

    private static void ImproveRuntimeWallPrefab(RPGameManager manager, Material railMaterial)
    {
        if (manager.wallPrefab == null)
        {
            return;
        }

        MeshRenderer renderer = manager.wallPrefab.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.sharedMaterial = railMaterial;
        }

        manager.wallMaterial = railMaterial;
        EditorUtility.SetDirty(manager);
    }

    private static void CreatePermanentRailVisuals(Material railMaterial, Material goldMaterial, Material blueHintMaterial)
    {
        GameObject old = GameObject.Find("Permanent_Visible_Bounce_Rails_Update11");

        if (old != null)
        {
            Object.DestroyImmediate(old);
        }

        GameObject root = new GameObject("Permanent_Visible_Bounce_Rails_Update11");

        CreateRail(root.transform, "Top_Bounce_Rail", new Vector3(0f, 0.10f, 7.05f), new Vector3(10.7f, 0.28f, 0.32f), railMaterial);
        CreateRail(root.transform, "Bottom_Bounce_Rail", new Vector3(0f, 0.10f, -7.05f), new Vector3(10.7f, 0.28f, 0.32f), railMaterial);
        CreateRail(root.transform, "Left_Bounce_Rail", new Vector3(-5.05f, 0.10f, 0f), new Vector3(0.32f, 0.28f, 14.1f), railMaterial);
        CreateRail(root.transform, "Right_Bounce_Rail", new Vector3(5.05f, 0.10f, 0f), new Vector3(0.32f, 0.28f, 14.1f), railMaterial);

        CreateRail(root.transform, "Top_Gold_Edge", new Vector3(0f, 0.28f, 6.80f), new Vector3(10.2f, 0.08f, 0.08f), goldMaterial);
        CreateRail(root.transform, "Bottom_Gold_Edge", new Vector3(0f, 0.28f, -6.80f), new Vector3(10.2f, 0.08f, 0.08f), goldMaterial);
        CreateRail(root.transform, "Left_Gold_Edge", new Vector3(-4.80f, 0.28f, 0f), new Vector3(0.08f, 0.08f, 13.4f), goldMaterial);
        CreateRail(root.transform, "Right_Gold_Edge", new Vector3(4.80f, 0.28f, 0f), new Vector3(0.08f, 0.08f, 13.4f), goldMaterial);

        GameObject topHint = CreateRail(root.transform, "Subtle_Blue_Rail_Hint_Top", new Vector3(0f, 0.34f, 6.55f), new Vector3(8.5f, 0.035f, 0.035f), blueHintMaterial);
        GameObject leftHint = CreateRail(root.transform, "Subtle_Blue_Rail_Hint_Left", new Vector3(-4.55f, 0.34f, 0f), new Vector3(0.035f, 0.035f, 11.0f), blueHintMaterial);
        GameObject rightHint = CreateRail(root.transform, "Subtle_Blue_Rail_Hint_Right", new Vector3(4.55f, 0.34f, 0f), new Vector3(0.035f, 0.035f, 11.0f), blueHintMaterial);

        topHint.AddComponent<RPRailHintPulse>();
        leftHint.AddComponent<RPRailHintPulse>();
        rightHint.AddComponent<RPRailHintPulse>();

        CreateCornerMarker(root.transform, new Vector3(-5.05f, 0.35f, -7.05f), goldMaterial);
        CreateCornerMarker(root.transform, new Vector3(5.05f, 0.35f, -7.05f), goldMaterial);
        CreateCornerMarker(root.transform, new Vector3(-5.05f, 0.35f, 7.05f), goldMaterial);
        CreateCornerMarker(root.transform, new Vector3(5.05f, 0.35f, 7.05f), goldMaterial);
    }

    private static GameObject CreateRail(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rail.name = name;
        rail.transform.SetParent(parent);
        rail.transform.position = position;
        rail.transform.localScale = scale;

        MeshRenderer renderer = rail.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        Collider collider = rail.GetComponent<Collider>();

        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }

        return rail;
    }

    private static void CreateCornerMarker(Transform parent, Vector3 position, Material material)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = "Rail_Corner_Marker";
        marker.transform.SetParent(parent);
        marker.transform.position = position;
        marker.transform.localScale = new Vector3(0.20f, 0.20f, 0.20f);

        MeshRenderer renderer = marker.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        Collider collider = marker.GetComponent<Collider>();

        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }
    }

    private static void AddRailEducationHint(RPGameManager manager)
    {
        RPRailEducationHint existing = Object.FindFirstObjectByType<RPRailEducationHint>();

        if (existing == null)
        {
            GameObject obj = new GameObject("RPRailEducationHint");
            existing = obj.AddComponent<RPRailEducationHint>();
        }

        existing.gameManager = manager;
        EditorUtility.SetDirty(existing);
    }

    private static void ImproveHintText(RPGameManager manager)
    {
        if (manager.hintText != null)
        {
            manager.hintText.text = "Rails bounce the marble.";
            manager.hintText.fontSize = Mathf.Min(manager.hintText.fontSize, 18);
            EditorUtility.SetDirty(manager.hintText);
        }

        Text onboardingText = GameObject.Find("OnboardingText")?.GetComponent<Text>();

        if (onboardingText != null)
        {
            onboardingText.text = "Pull back.\nRails bounce the marble.";
            onboardingText.fontSize = 19;
            EditorUtility.SetDirty(onboardingText);
        }
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
        Material material = CreateMaterial(name, color, 0.55f);

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