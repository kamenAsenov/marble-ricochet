#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class LoopRoomSceneBuilder
{
    [MenuItem("Tools/Loop Room/Build MVP Scene")]
    public static void BuildMvpScene()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Stop Play Mode before building the scene.");
            return;
        }

        bool confirmed = EditorUtility.DisplayDialog(
            "Build Loop Room MVP Scene",
            "This will clear the current scene and create Cinematic Visual Update 05. Continue?",
            "Yes, build it",
            "Cancel"
        );

        if (!confirmed)
        {
            return;
        }

        EnsureFolders();
        SetupTextureImportSettings();
        ClearScene();

        Material wallMat = CreateTexturedMaterial("MAT_Cinematic_Wall_Plaster", "Assets/Textures/T_Wall_Plaster.png", new Color(0.72f, 0.66f, 0.55f), 0.32f);
        Material floorMat = CreateTexturedMaterial("MAT_Cinematic_Floor_Tiles", "Assets/Textures/T_Floor_Tiles.png", new Color(0.68f, 0.60f, 0.45f), 0.38f);
        Material carpetMat = CreateTexturedMaterial("MAT_Cinematic_Carpet", "Assets/Textures/T_Carpet_Red.png", new Color(0.75f, 0.30f, 0.22f), 0.46f);
        Material woodMat = CreateTexturedMaterial("MAT_Cinematic_Dark_Wood", "Assets/Textures/T_Wood_Dark.png", new Color(0.38f, 0.19f, 0.09f), 0.40f);
        Material darkNoiseMat = CreateTexturedMaterial("MAT_Cinematic_Dark_Noise", "Assets/Textures/T_Dark_Noise.png", new Color(0.035f, 0.03f, 0.025f), 0.16f);

        Material ceilingMat = CreateMaterial("MAT_Cinematic_Ceiling", new Color(0.36f, 0.33f, 0.28f), 0.20f);
        Material brassMat = CreateMaterial("MAT_Cinematic_Brass", new Color(0.62f, 0.49f, 0.26f), 0.72f);
        Material plantMat = CreateMaterial("MAT_Cinematic_Dusty_Green", new Color(0.17f, 0.33f, 0.19f), 0.38f);
        Material fabricMat = CreateMaterial("MAT_Cinematic_Fabric_Blue", new Color(0.10f, 0.17f, 0.24f), 0.48f);
        Material whiteMat = CreateMaterial("MAT_Cinematic_Aged_White", new Color(0.78f, 0.74f, 0.63f), 0.30f);
        Material mirrorMat = CreateMaterial("MAT_Cinematic_Mirror_Dull", new Color(0.42f, 0.48f, 0.50f), 0.88f);
        Material lightMat = CreateEmissionMaterial("MAT_Cinematic_Warm_Light", new Color(1.0f, 0.76f, 0.42f), 1.6f);
        Material redMat = CreateEmissionMaterial("MAT_Cinematic_Red_Glow", new Color(0.90f, 0.025f, 0.015f), 1.7f);
        Material blueMat = CreateEmissionMaterial("MAT_Cinematic_Cold_Blue", new Color(0.20f, 0.45f, 0.95f), 1.2f);

        GameObject player = CreatePlayer();

        BuildCinematicCorridor(
            wallMat,
            floorMat,
            carpetMat,
            woodMat,
            ceilingMat,
            brassMat,
            plantMat,
            fabricMat,
            whiteMat,
            mirrorMat,
            lightMat
        );

        GameObject[] anomalies = BuildAnomalies(
            darkNoiseMat,
            woodMat,
            redMat,
            blueMat,
            plantMat,
            fabricMat,
            brassMat,
            whiteMat,
            lightMat,
            mirrorMat
        );

        GameObject systems = new GameObject("SYSTEMS");

        LoopManager loopManager = new GameObject("LoopManager").AddComponent<LoopManager>();
        loopManager.transform.SetParent(systems.transform);
        loopManager.player = player.transform;
        loopManager.spawnPosition = new Vector3(0f, 0f, -11f);
        loopManager.easyLoopsEndAt = 4;
        loopManager.mediumLoopsEndAt = 9;
        loopManager.anomalies = anomalies;

        GameStateUI ui = CreateCanvasUI(loopManager);
        loopManager.gameStateUI = ui;

        HorrorAudioManager audioManager = CreateAudioManager(systems.transform);
        loopManager.audioManager = audioManager;

        ScreenMoodController moodController = CreateMoodController(ui.transform.parent);
        loopManager.screenMood = moodController;

        CreateDecisionTrigger("DecisionTrigger_CONTINUE_FORWARD", new Vector3(0f, 1f, 13.25f), loopManager, PlayerDecision.ContinueForward);
        CreateDecisionTrigger("DecisionTrigger_TURN_BACK", new Vector3(0f, 1f, -13.25f), loopManager, PlayerDecision.TurnBack);

        BuildLights();

        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.13f, 0.105f, 0.075f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.018f;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.12f, 0.10f, 0.075f);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("LOOP ROOM Update 05 created: cinematic corridor, textures, carpet, elevator, fog, richer props, clearer anomalies.");
    }

    private static void BuildCinematicCorridor(
        Material wallMat,
        Material floorMat,
        Material carpetMat,
        Material woodMat,
        Material ceilingMat,
        Material brassMat,
        Material plantMat,
        Material fabricMat,
        Material whiteMat,
        Material mirrorMat,
        Material lightMat
    )
    {
        GameObject env = new GameObject("ENVIRONMENT_CINEMATIC");

        CreateCube("Floor_Textured_Tiles", new Vector3(0f, -0.05f, 0f), new Vector3(10f, 0.1f, 30f), floorMat, env.transform);
        CreateCube("Carpet_Runner_Red", new Vector3(0f, 0.015f, 0f), new Vector3(2.35f, 0.035f, 25.5f), carpetMat, env.transform);

        CreateCube("Wall_Left_Textured", new Vector3(-5f, 2.5f, 0f), new Vector3(0.3f, 5f, 30f), wallMat, env.transform);
        CreateCube("Wall_Right_Textured", new Vector3(5f, 2.5f, 0f), new Vector3(0.3f, 5f, 30f), wallMat, env.transform);
        CreateCube("Ceiling_Dark", new Vector3(0f, 5f, 0f), new Vector3(10f, 0.3f, 30f), ceilingMat, env.transform);
        CreateCube("StartWall_Back", new Vector3(0f, 2.5f, -15f), new Vector3(10f, 5f, 0.3f), wallMat, env.transform);
        CreateCube("EndWall_Elevator_Back", new Vector3(0f, 2.5f, 15f), new Vector3(10f, 5f, 0.3f), wallMat, env.transform);

        CreateWallTrims(woodMat, env.transform);
        CreateDoors(woodMat, brassMat, env.transform);
        CreateElevatorEnd(woodMat, brassMat, lightMat, env.transform);
        CreateDecorProps(woodMat, brassMat, plantMat, fabricMat, whiteMat, mirrorMat, lightMat, env.transform);
        CreateWallSconces(brassMat, lightMat, env.transform);
    }

    private static void CreateWallTrims(Material woodMat, Transform parent)
    {
        CreateCube("Left_Baseboard_Wood", new Vector3(-4.78f, 0.28f, 0f), new Vector3(0.18f, 0.22f, 29f), woodMat, parent);
        CreateCube("Right_Baseboard_Wood", new Vector3(4.78f, 0.28f, 0f), new Vector3(0.18f, 0.22f, 29f), woodMat, parent);
        CreateCube("Left_Mid_Trim_Wood", new Vector3(-4.78f, 2.05f, 0f), new Vector3(0.10f, 0.12f, 29f), woodMat, parent);
        CreateCube("Right_Mid_Trim_Wood", new Vector3(4.78f, 2.05f, 0f), new Vector3(0.10f, 0.12f, 29f), woodMat, parent);
        CreateCube("Left_Top_Trim_Wood", new Vector3(-4.78f, 4.55f, 0f), new Vector3(0.12f, 0.18f, 29f), woodMat, parent);
        CreateCube("Right_Top_Trim_Wood", new Vector3(4.78f, 4.55f, 0f), new Vector3(0.12f, 0.18f, 29f), woodMat, parent);
    }

    private static void CreateDoors(Material woodMat, Material brassMat, Transform parent)
    {
        CreateDoor("Door_101_Left", new Vector3(-4.82f, 1.28f, -8.3f), true, woodMat, brassMat, parent, "101");
        CreateDoor("Door_103_Left", new Vector3(-4.82f, 1.28f, -1.3f), true, woodMat, brassMat, parent, "103");
        CreateDoor("Door_105_Left", new Vector3(-4.82f, 1.28f, 6.1f), true, woodMat, brassMat, parent, "105");

        CreateDoor("Door_102_Right", new Vector3(4.82f, 1.28f, -5.0f), false, woodMat, brassMat, parent, "102");
        CreateDoor("Door_104_Right", new Vector3(4.82f, 1.28f, 2.4f), false, woodMat, brassMat, parent, "104");
        CreateDoor("Door_106_Right", new Vector3(4.82f, 1.28f, 9.1f), false, woodMat, brassMat, parent, "106");
    }

    private static void CreateDoor(string name, Vector3 position, bool leftSide, Material woodMat, Material brassMat, Transform parent, string number)
    {
        CreateCube(name + "_Panel", position, new Vector3(0.12f, 2.55f, 1.18f), woodMat, parent);
        CreateCube(name + "_Inset_Upper", position + new Vector3(leftSide ? 0.07f : -0.07f, 0.45f, 0f), new Vector3(0.05f, 0.72f, 0.78f), woodMat, parent);
        CreateCube(name + "_Inset_Lower", position + new Vector3(leftSide ? 0.07f : -0.07f, -0.55f, 0f), new Vector3(0.05f, 0.72f, 0.78f), woodMat, parent);

        float x = leftSide ? -4.64f : 4.64f;
        CreateCube(name + "_Handle", new Vector3(x, 1.24f, position.z + 0.36f), new Vector3(0.10f, 0.13f, 0.13f), brassMat, parent);
        CreateCube(name + "_NumberPlate_" + number, new Vector3(x, 2.12f, position.z), new Vector3(0.055f, 0.30f, 0.50f), brassMat, parent);
    }

    private static void CreateElevatorEnd(Material woodMat, Material brassMat, Material lightMat, Transform parent)
    {
        CreateCube("Elevator_Frame_Top", new Vector3(0f, 3.75f, 14.78f), new Vector3(3.6f, 0.28f, 0.16f), woodMat, parent);
        CreateCube("Elevator_Frame_Left", new Vector3(-1.9f, 1.95f, 14.78f), new Vector3(0.22f, 3.6f, 0.16f), woodMat, parent);
        CreateCube("Elevator_Frame_Right", new Vector3(1.9f, 1.95f, 14.78f), new Vector3(0.22f, 3.6f, 0.16f), woodMat, parent);
        CreateCube("Elevator_Door_Left", new Vector3(-0.48f, 1.75f, 14.70f), new Vector3(0.92f, 3.2f, 0.12f), brassMat, parent);
        CreateCube("Elevator_Door_Right", new Vector3(0.48f, 1.75f, 14.70f), new Vector3(0.92f, 3.2f, 0.12f), brassMat, parent);
        CreateCube("Elevator_Door_Seam", new Vector3(0f, 1.75f, 14.60f), new Vector3(0.035f, 3.2f, 0.08f), woodMat, parent);
        CreateCube("Elevator_Call_Button_Glow", new Vector3(2.35f, 1.55f, 14.55f), new Vector3(0.22f, 0.22f, 0.08f), lightMat, parent);
    }

    private static void CreateDecorProps(Material woodMat, Material brassMat, Material plantMat, Material fabricMat, Material whiteMat, Material mirrorMat, Material lightMat, Transform parent)
    {
        CreateBench("Bench_Left_Cinematic", new Vector3(-3.72f, 0.42f, 0f), woodMat, fabricMat, brassMat, parent);
        CreatePlant("Plant_Right_Cinematic", new Vector3(3.82f, 0f, -7.2f), woodMat, plantMat, parent);
        CreateSideTable("Small_Table_Right_Cinematic", new Vector3(3.82f, 0.55f, 3.4f), woodMat, brassMat, parent);

        CreatePicture("Picture_Left_Landscape", new Vector3(-4.82f, 2.75f, 4.2f), woodMat, whiteMat, parent);
        CreatePicture("Picture_Right_Abstract", new Vector3(4.82f, 2.75f, -0.2f), woodMat, whiteMat, parent);

        CreateCube("Dull_Mirror_Right_Frame", new Vector3(4.82f, 2.55f, -9.9f), new Vector3(0.08f, 1.35f, 0.85f), woodMat, parent);
        CreateCube("Dull_Mirror_Right_Surface", new Vector3(4.76f, 2.55f, -9.9f), new Vector3(0.035f, 1.05f, 0.62f), mirrorMat, parent);

        CreateCube("Radiator_Left", new Vector3(-4.70f, 0.75f, 9.7f), new Vector3(0.18f, 0.85f, 1.3f), whiteMat, parent);
        for (int i = 0; i < 5; i++)
        {
            CreateCube("Radiator_Left_Rib_" + i, new Vector3(-4.56f, 0.75f, 9.25f + i * 0.2f), new Vector3(0.08f, 0.88f, 0.055f), whiteMat, parent);
        }
    }

    private static void CreateWallSconces(Material brassMat, Material lightMat, Transform parent)
    {
        CreateSconce("Sconce_Left_01", new Vector3(-4.72f, 3.15f, -4.7f), true, brassMat, lightMat, parent);
        CreateSconce("Sconce_Right_01", new Vector3(4.72f, 3.15f, 5.3f), false, brassMat, lightMat, parent);
    }

    private static void CreateSconce(string name, Vector3 position, bool left, Material brassMat, Material lightMat, Transform parent)
    {
        CreateCube(name + "_Base", position, new Vector3(0.12f, 0.42f, 0.35f), brassMat, parent);
        CreateCube(name + "_Glow", position + new Vector3(left ? 0.08f : -0.08f, 0.05f, 0f), new Vector3(0.08f, 0.72f, 0.42f), lightMat, parent);
    }

    private static void CreateBench(string name, Vector3 position, Material woodMat, Material fabricMat, Material brassMat, Transform parent)
    {
        CreateCube(name + "_Seat", position, new Vector3(1.9f, 0.24f, 0.64f), fabricMat, parent);
        CreateCube(name + "_Back", position + new Vector3(-0.18f, 0.58f, 0f), new Vector3(0.18f, 0.9f, 0.68f), fabricMat, parent);
        CreateCube(name + "_Wood_Edge", position + new Vector3(0f, 0.17f, 0f), new Vector3(2.05f, 0.09f, 0.78f), woodMat, parent);
        CreateCube(name + "_Leg_01", position + new Vector3(-0.72f, -0.28f, -0.22f), new Vector3(0.12f, 0.5f, 0.12f), brassMat, parent);
        CreateCube(name + "_Leg_02", position + new Vector3(0.72f, -0.28f, 0.22f), new Vector3(0.12f, 0.5f, 0.12f), brassMat, parent);
    }

    private static void CreatePlant(string name, Vector3 position, Material potMat, Material plantMat, Transform parent)
    {
        CreateCube(name + "_Pot", position + new Vector3(0f, 0.35f, 0f), new Vector3(0.62f, 0.7f, 0.62f), potMat, parent);
        CreateCube(name + "_Stem", position + new Vector3(0f, 1.05f, 0f), new Vector3(0.12f, 0.95f, 0.12f), plantMat, parent);
        CreateCube(name + "_Leaves_A", position + new Vector3(0f, 1.55f, 0f), new Vector3(0.95f, 0.48f, 0.45f), plantMat, parent);
        CreateCube(name + "_Leaves_B", position + new Vector3(0f, 1.65f, 0f), new Vector3(0.45f, 0.42f, 0.95f), plantMat, parent);
    }

    private static void CreateSideTable(string name, Vector3 position, Material topMat, Material legMat, Transform parent)
    {
        CreateCube(name + "_Top", position, new Vector3(0.95f, 0.13f, 0.75f), topMat, parent);
        CreateCube(name + "_Leg_01", position + new Vector3(-0.32f, -0.34f, -0.24f), new Vector3(0.09f, 0.65f, 0.09f), legMat, parent);
        CreateCube(name + "_Leg_02", position + new Vector3(0.32f, -0.34f, 0.24f), new Vector3(0.09f, 0.65f, 0.09f), legMat, parent);
    }

    private static void CreatePicture(string name, Vector3 position, Material frameMat, Material insideMat, Transform parent)
    {
        CreateCube(name + "_Frame", position, new Vector3(0.08f, 0.95f, 1.38f), frameMat, parent);
        CreateCube(name + "_Inside", new Vector3(position.x + (position.x < 0 ? -0.055f : 0.055f), position.y, position.z), new Vector3(0.04f, 0.66f, 1.05f), insideMat, parent);
    }

    private static GameObject[] BuildAnomalies(Material shadowMat, Material woodMat, Material redMat, Material blueMat, Material plantMat, Material fabricMat, Material brassMat, Material whiteMat, Material lightMat, Material mirrorMat)
    {
        GameObject parent = new GameObject("ANOMALIES_READABLE");

        GameObject redGlow = CreateCube("Anomaly_EASY_Red_Glowing_Cube", new Vector3(0f, 0.72f, 5.6f), new Vector3(1.25f, 1.25f, 1.25f), redMat, parent.transform);
        AddDef(redGlow, AnomalyDifficulty.Easy, "Red glowing cube");

        GameObject blueLight = CreateCube("Anomaly_EASY_Blue_Ceiling_Light", new Vector3(0f, 4.58f, -1.0f), new Vector3(2.7f, 0.12f, 0.78f), blueMat, parent.transform);
        AddDef(blueLight, AnomalyDifficulty.Easy, "Blue ceiling light");

        GameObject extraDoor = CreateCube("Anomaly_EASY_Extra_Door_Near_Start", new Vector3(4.82f, 1.28f, -11.2f), new Vector3(0.12f, 2.55f, 1.22f), woodMat, parent.transform);
        AddDef(extraDoor, AnomalyDifficulty.Easy, "Extra door near start");

        GameObject hugePlant = CreateCube("Anomaly_EASY_Huge_Plant", new Vector3(3.75f, 1.7f, -7.2f), new Vector3(1.35f, 3.4f, 1.35f), plantMat, parent.transform);
        AddDef(hugePlant, AnomalyDifficulty.Easy, "Huge plant");

        GameObject elevatorOpen = CreateCube("Anomaly_EASY_Elevator_Red_Interior", new Vector3(0f, 1.75f, 14.52f), new Vector3(1.6f, 3.05f, 0.10f), redMat, parent.transform);
        AddDef(elevatorOpen, AnomalyDifficulty.Easy, "Elevator glowing red");

        GameObject shadow = CreateCube("Anomaly_MEDIUM_Shadow_By_Elevator", new Vector3(-1.55f, 1.45f, 11.2f), new Vector3(0.62f, 2.9f, 0.18f), shadowMat, parent.transform);
        AddDef(shadow, AnomalyDifficulty.Medium, "Shadow by elevator");

        GameObject benchMiddle = CreateCube("Anomaly_MEDIUM_Bench_In_Middle", new Vector3(0f, 0.44f, -0.7f), new Vector3(2.0f, 0.36f, 0.82f), fabricMat, parent.transform);
        AddDef(benchMiddle, AnomalyDifficulty.Medium, "Bench in the middle");

        GameObject floatingMirror = CreateCube("Anomaly_MEDIUM_Floating_Mirror", new Vector3(-0.15f, 2.25f, -3.4f), new Vector3(0.14f, 1.25f, 0.9f), mirrorMat, parent.transform);
        floatingMirror.transform.rotation = Quaternion.Euler(0f, 0f, 14f);
        AddDef(floatingMirror, AnomalyDifficulty.Medium, "Floating mirror");

        GameObject brassBar = CreateCube("Anomaly_MEDIUM_Brass_Bar", new Vector3(0f, 1.7f, 3.1f), new Vector3(7.0f, 0.16f, 0.16f), brassMat, parent.transform);
        brassBar.transform.rotation = Quaternion.Euler(0f, 0f, -8f);
        AddDef(brassBar, AnomalyDifficulty.Medium, "Brass bar");

        GameObject darkEnd = CreateCube("Anomaly_HARD_Dark_Elevator_End", new Vector3(0f, 2.5f, 12.6f), new Vector3(8.5f, 4.6f, 0.12f), shadowMat, parent.transform);
        AddDef(darkEnd, AnomalyDifficulty.Hard, "Dark elevator end");

        GameObject tinyWhite = CreateCube("Anomaly_HARD_Tiny_White_Block_On_Trim", new Vector3(-4.62f, 2.08f, 1.2f), new Vector3(0.18f, 0.18f, 0.18f), whiteMat, parent.transform);
        AddDef(tinyWhite, AnomalyDifficulty.Hard, "Tiny white block");

        GameObject lowerLight = CreateCube("Anomaly_HARD_Low_Hanging_Light", new Vector3(0f, 3.55f, 7f), new Vector3(1.8f, 0.09f, 0.55f), lightMat, parent.transform);
        AddDef(lowerLight, AnomalyDifficulty.Hard, "Low hanging light");

        GameObject thinShadow = CreateCube("Anomaly_HARD_Thin_Shadow_Line", new Vector3(4.74f, 2.55f, -4.9f), new Vector3(0.07f, 1.55f, 0.10f), shadowMat, parent.transform);
        AddDef(thinShadow, AnomalyDifficulty.Hard, "Thin shadow line");

        GameObject[] anomalies = new GameObject[]
        {
            redGlow, blueLight, extraDoor, hugePlant, elevatorOpen,
            shadow, benchMiddle, floatingMirror, brassBar,
            darkEnd, tinyWhite, lowerLight, thinShadow
        };

        foreach (GameObject a in anomalies)
        {
            a.SetActive(false);
        }

        return anomalies;
    }

    private static void AddDef(GameObject obj, AnomalyDifficulty difficulty, string displayName)
    {
        AnomalyDefinition def = obj.AddComponent<AnomalyDefinition>();
        def.difficulty = difficulty;
        def.displayName = displayName;
    }

    private static HorrorAudioManager CreateAudioManager(Transform parent)
    {
        GameObject audioObject = new GameObject("HorrorAudioManager");
        audioObject.transform.SetParent(parent);
        return audioObject.AddComponent<HorrorAudioManager>();
    }

    private static void CreateDecisionTrigger(string name, Vector3 position, LoopManager loopManager, PlayerDecision decision)
    {
        GameObject trigger = CreateCube(name, position, new Vector3(8f, 2f, 0.5f), null, null);
        MeshRenderer renderer = trigger.GetComponent<MeshRenderer>();
        if (renderer != null) renderer.enabled = false;

        BoxCollider collider = trigger.GetComponent<BoxCollider>();
        collider.isTrigger = true;

        DecisionTrigger decisionTrigger = trigger.AddComponent<DecisionTrigger>();
        decisionTrigger.loopManager = loopManager;
        decisionTrigger.decision = decision;
    }

    private static void BuildLights()
    {
        GameObject parent = new GameObject("LIGHTS_CINEMATIC");

        CreateLight("Warm_Key_Start", new Vector3(0f, 3.8f, -9f), 2.2f, 8.0f, parent.transform);
        CreateLight("Warm_Key_Mid", new Vector3(0f, 3.8f, -1f), 2.0f, 8.0f, parent.transform);
        CreateLight("Warm_Key_End", new Vector3(0f, 3.8f, 7f), 1.8f, 8.0f, parent.transform);
        CreateLight("Elevator_Button_Glow_Light", new Vector3(2.15f, 1.55f, 13.8f), 1.2f, 4.0f, parent.transform);
        CreateLight("Sconce_Left_Warm", new Vector3(-4.2f, 3.1f, -4.7f), 1.05f, 4.5f, parent.transform);
        CreateLight("Sconce_Right_Warm", new Vector3(4.2f, 3.1f, 5.3f), 1.05f, 4.5f, parent.transform);
    }

    private static void CreateLight(string name, Vector3 position, float intensity, float range, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = position;
        obj.transform.SetParent(parent);

        Light light = obj.AddComponent<Light>();
        light.type = LightType.Point;
        light.intensity = intensity;
        light.range = range;
        light.color = new Color(1f, 0.76f, 0.45f);
        light.shadows = LightShadows.Soft;
    }

    private static GameStateUI CreateCanvasUI(LoopManager loopManager)
    {
        GameObject canvasObject = new GameObject("Canvas_UI_CINEMATIC");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        Image darkOverlay = CreateOverlayImage("DarkOverlay", canvasObject.transform, new Color(0f, 0f, 0f, 0.04f));
        Image dangerOverlay = CreateOverlayImage("DangerOverlay", canvasObject.transform, new Color(0.45f, 0f, 0f, 0f));
        Image vignetteOverlay = CreateOverlayImage("SoftVignetteOverlay", canvasObject.transform, new Color(0f, 0f, 0f, 0.08f));

        GameObject uiObj = new GameObject("GameStateUI");
        uiObj.transform.SetParent(canvasObject.transform, false);
        GameStateUI ui = uiObj.AddComponent<GameStateUI>();
        ui.loopManager = loopManager;

        ui.loopText = CreateUIText("LoopText", canvasObject.transform, "LOOP 1", 44, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(42f, -32f), new Vector2(420f, 80f));
        ui.streakText = CreateUIText("StreakText", canvasObject.transform, "STREAK 0", 24, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(46f, -88f), new Vector2(420f, 60f));
        ui.difficultyText = CreateUIText("DifficultyText", canvasObject.transform, "EASY", 22, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-42f, -38f), new Vector2(260f, 60f));
        ui.hintText = CreateUIText("HintText", canvasObject.transform, "Look carefully. Normal corridor = continue forward.", 25, TextAnchor.LowerLeft, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(42f, 36f), new Vector2(1200f, 70f));
        ui.feedbackText = CreateUIText("FeedbackText", canvasObject.transform, "", 42, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 110f), new Vector2(560f, 100f));
        ui.feedbackText.enabled = false;

        ui.gameOverPanel = CreatePanel(canvasObject.transform);
        ui.gameOverText = CreateUIText("GameOverText", ui.gameOverPanel.transform, "", 34, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(780f, 480f));
        ui.gameOverPanel.SetActive(false);

        ScreenMoodController mood = canvasObject.AddComponent<ScreenMoodController>();
        mood.darkOverlay = darkOverlay;
        mood.dangerOverlay = dangerOverlay;
        mood.vignetteOverlay = vignetteOverlay;

        return ui;
    }

    private static ScreenMoodController CreateMoodController(Transform canvasTransform)
    {
        return canvasTransform.GetComponent<ScreenMoodController>();
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

    private static Text CreateUIText(string name, Transform parent, string text, int fontSize, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
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

    private static GameObject CreatePanel(Transform parent)
    {
        GameObject panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(860f, 540f);

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.80f);
        return panel;
    }

    private static GameObject CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0f, 0f, -11f);
        player.transform.rotation = Quaternion.identity;

        CharacterController controller = player.AddComponent<CharacterController>();
        controller.center = new Vector3(0f, 1f, 0f);
        controller.radius = 0.35f;
        controller.height = 1.8f;

        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        camObj.transform.SetParent(player.transform);
        camObj.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        camObj.transform.localRotation = Quaternion.identity;

        Camera cam = camObj.AddComponent<Camera>();
        cam.fieldOfView = 68f;
        cam.nearClipPlane = 0.05f;
        cam.farClipPlane = 100f;
        cam.backgroundColor = Color.black;

        camObj.AddComponent<AudioListener>();

        PlayerMovement movement = player.AddComponent<PlayerMovement>();
        movement.playerCamera = camObj.transform;
        movement.moveSpeed = 3.15f;
        movement.mouseSensitivity = 2f;

        camObj.AddComponent<BreathingCamera>();
        return player;
    }

    private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.position = position;
        cube.transform.localScale = scale;
        if (parent != null) cube.transform.SetParent(parent);
        if (material != null) cube.GetComponent<MeshRenderer>().sharedMaterial = material;
        return cube;
    }

    private static void EnsureFolders()
    {
        string[] folders = { "Assets/Scripts", "Assets/Editor", "Assets/Materials", "Assets/Audio", "Assets/Prefabs", "Assets/UI", "Assets/Textures", "Assets/Models", "Assets/PostProcessing", "Assets/Animations", "Assets/Docs" };
        foreach (string folder in folders) EnsureFolder(folder);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string[] parts = path.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    private static void SetupTextureImportSettings()
    {
        string[] texturePaths =
        {
            "Assets/Textures/T_Wall_Plaster.png",
            "Assets/Textures/T_Floor_Tiles.png",
            "Assets/Textures/T_Carpet_Red.png",
            "Assets/Textures/T_Wood_Dark.png",
            "Assets/Textures/T_Dark_Noise.png"
        };

        foreach (string path in texturePaths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.SaveAndReimport();
        }
    }

    private static void ClearScene()
    {
        foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Object.DestroyImmediate(obj);
        }
    }

    private static Material CreateTexturedMaterial(string name, string texturePath, Color color, float smoothness)
    {
        Material mat = CreateMaterial(name, color, smoothness);
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        if (tex != null)
        {
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
            if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", tex);
        }
        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        return mat;
    }

    private static Material CreateMaterial(string name, Color color, float smoothness)
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

        if (existing == null) AssetDatabase.CreateAsset(material, assetPath);

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
}
#endif