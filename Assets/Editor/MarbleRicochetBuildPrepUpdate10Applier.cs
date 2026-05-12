#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MarbleRicochetBuildPrepUpdate10Applier
{
    [MenuItem("Tools/Marble Ricochet/Apply Build Prep Update 10")]
    public static void ApplyUpdate()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before applying Update 10.");
            return;
        }

        ApplyProjectSettings();
        EnsureRuntimeObjects();
        EnsureBuildInfo();
        SaveScene();

        EditorUtility.DisplayDialog(
            "Update 10 Applied",
            "Build & Release Preparation Update 10 applied.\n\nProject is now configured for portrait mobile test builds.\n\nNext step: File > Build Profiles / Build Settings > Android > Build.",
            "OK"
        );
    }

    private static void ApplyProjectSettings()
    {
        PlayerSettings.productName = "Marble Ricochet";
        PlayerSettings.companyName = "Indie Marble Studio";
        PlayerSettings.bundleVersion = "0.1.0";

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;

        PlayerSettings.runInBackground = false;
        PlayerSettings.SplashScreen.show = true;
        PlayerSettings.SplashScreen.showUnityLogo = true;

        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.indiemarblestudio.marblericochet");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.indiemarblestudio.marblericochet");

#if UNITY_ANDROID
        PlayerSettings.Android.bundleVersionCode = 1;
#endif

#if UNITY_IOS
        PlayerSettings.iOS.buildNumber = "1";
#endif

        Debug.Log("Marble Ricochet project settings applied: portrait, version 0.1.0, package id com.indiemarblestudio.marblericochet.");
    }

    private static void EnsureRuntimeObjects()
    {
        GameObject runtime = GameObject.Find("MobileRuntimeSettings");

        if (runtime == null)
        {
            runtime = new GameObject("MobileRuntimeSettings");
        }

        RPMobileRuntimeSettings settings = runtime.GetComponent<RPMobileRuntimeSettings>();

        if (settings == null)
        {
            settings = runtime.AddComponent<RPMobileRuntimeSettings>();
        }

        settings.targetFrameRate = 60;
        settings.neverSleep = true;

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (canvas != null)
        {
            GameObject marker = GameObject.Find("MobileSafeAreaMarker");

            if (marker == null)
            {
                marker = new GameObject("MobileSafeAreaMarker");
                marker.transform.SetParent(canvas.transform, false);
            }

            RectTransform rect = marker.GetComponent<RectTransform>();

            if (rect == null)
            {
                rect = marker.AddComponent<RectTransform>();
            }

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            RPSafeAreaHelper safeArea = marker.GetComponent<RPSafeAreaHelper>();

            if (safeArea == null)
            {
                safeArea = marker.AddComponent<RPSafeAreaHelper>();
            }

            safeArea.target = rect;
            safeArea.applyOnUpdate = true;
        }

        EditorUtility.SetDirty(runtime);
    }

    private static void EnsureBuildInfo()
    {
        GameObject infoObject = GameObject.Find("BuildInfo");

        if (infoObject == null)
        {
            infoObject = new GameObject("BuildInfo");
        }

        RPBuildInfo info = infoObject.GetComponent<RPBuildInfo>();

        if (info == null)
        {
            info = infoObject.AddComponent<RPBuildInfo>();
        }

        info.gameName = "Marble Ricochet";
        info.version = "0.1.0";
        info.buildChannel = "Internal Test";
        info.notes = "Soft-launch preparation build.";

        EditorUtility.SetDirty(infoObject);
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