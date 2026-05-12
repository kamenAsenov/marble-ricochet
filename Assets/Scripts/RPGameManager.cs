using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public enum RPGameState
{
    Menu,
    Aiming,
    BallMoving,
    LevelComplete
}

public class RPGameManager : MonoBehaviour
{
    [Header("Scene References")]
    public Camera mainCamera;
    public Transform levelRoot;
    public RPBulletBall ball;
    public LineRenderer aimLine;
    public LineRenderer previewLine;

    [Header("Prefabs")]
    public GameObject targetPrefab;
    public GameObject wallPrefab;
    public GameObject metalBumperPrefab;
    public GameObject breakableGlassPrefab;

    [Header("Materials")]
    public Material targetMaterial;
    public Material wallMaterial;
    public Material metalBumperMaterial;
    public Material breakableGlassMaterial;
    public Material ballMaterial;

    [Header("UI")]
    public GameObject menuPanel;
    public GameObject winPanel;
    public GameObject hudPanel;
    public Text titleText;
    public Text subtitleText;
    public Text levelText;
    public Text shotsText;
    public Text targetsText;
    public Text hintText;
    public Text winTitleText;
    public Text winStatsText;
    public Text winRewardText;
    public Text coinsText;
    public Text streakText;
    public Text menuProgressText;
    public Text hintTokensText;
    public Slider progressSlider;

    [Header("Gameplay")]
    public float minLaunchPower = 3.5f;
    public float maxLaunchPower = 10.8f;
    public float maxPullDistance = 2.15f;
    public int currentLevelIndex = 0;
    public int shotsUsed = 0;

    [Header("Systems")]
    public RPAudioManager audioManager;
    public RPCameraShake cameraShake;
    public RPProgressionManager progression;
    public RPShopManager shopManager;

    public RPGameState state = RPGameState.Menu;

    private List<RPLevelData> levels;
    private RPLevelData currentLevel;
    private readonly List<RPTarget> activeTargets = new List<RPTarget>();
    private readonly List<GameObject> spawnedObjects = new List<GameObject>();

    private bool isDragging = false;
    private Vector3 currentAimDirection = Vector3.forward;
    private float currentPower = 0f;

    private void Start()
    {
        levels = RPLevelLibrary.CreateLevels();

        if (progression == null)
        {
            progression = FindFirstObjectByType<RPProgressionManager>();

            if (progression == null)
            {
                GameObject progressionObject = new GameObject("RPProgressionManager");
                progression = progressionObject.AddComponent<RPProgressionManager>();
            }
        }

        if (shopManager == null)
        {
            shopManager = FindFirstObjectByType<RPShopManager>();
        }

        if (titleText != null)
        {
            titleText.text = "MARBLE RICOCHET";
        }

        if (subtitleText != null)
        {
            subtitleText.text = "A premium glass-breaking puzzle.";
        }

        ShowMenu();
        ApplySelectedSkin();
    }

    private void Update()
    {
        if (state == RPGameState.Aiming)
        {
            HandleAimInput();
        }

        if (state == RPGameState.BallMoving)
        {
            if (ball != null && ball.IsMovingTooLong())
            {
                ResetShot();
            }
        }
    }

    public void ShowMenu()
    {
        state = RPGameState.Menu;

        if (menuPanel != null) menuPanel.SetActive(true);
        if (winPanel != null) winPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(false);

        if (ball != null)
        {
            ball.gameObject.SetActive(false);
        }

        ClearLevel();
        RefreshMetaUI();
    }

    public void StartGame()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        currentLevelIndex = 0;
        LoadLevel(currentLevelIndex);
    }

    public void ContinueGame()
    {
        int index = progression != null ? progression.highestUnlockedLevel : currentLevelIndex;

        if (levels == null || levels.Count == 0)
        {
            levels = RPLevelLibrary.CreateLevels();
        }

        index = Mathf.Clamp(index, 0, levels.Count - 1);
        currentLevelIndex = index;
        LoadLevel(currentLevelIndex);
    }

    public void ResetProgressAndNewGame()
    {
        if (progression != null)
        {
            progression.ResetAllProgress();
        }

        ApplySelectedSkin();
        StartNewGame();
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    public void NextLevel()
    {
        currentLevelIndex++;

        if (levels == null || levels.Count == 0)
        {
            levels = RPLevelLibrary.CreateLevels();
        }

        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = 0;
        }

        LoadLevel(currentLevelIndex);
    }

    public void UseHint()
    {
        if (state != RPGameState.Aiming)
        {
            return;
        }

        if (progression == null || !progression.SpendHintToken())
        {
            if (hintText != null)
            {
                hintText.text = "No hints. Get one from the shop.";
            }

            RefreshMetaUI();
            return;
        }

        if (hintText != null)
        {
            hintText.text = "Hint: use the blue preview line and aim for the first clean bounce.";
        }

        RefreshMetaUI();
    }

    public void RefreshMetaUI()
    {
        UpdateMetaUI();
    }

    public void ApplySelectedSkin()
    {
        if (ball == null)
        {
            return;
        }

        MeshRenderer renderer = ball.GetComponent<MeshRenderer>();

        if (renderer == null || renderer.sharedMaterial == null)
        {
            return;
        }

        Color color = shopManager != null ? shopManager.GetSelectedSkinColor() : new Color(0.94f, 0.92f, 0.84f);

        Material runtimeMat = renderer.sharedMaterial;
        runtimeMat.color = color;

        if (runtimeMat.HasProperty("_BaseColor"))
        {
            runtimeMat.SetColor("_BaseColor", color);
        }
    }

    private void LoadLevel(int index)
    {
        if (levels == null || levels.Count == 0)
        {
            levels = RPLevelLibrary.CreateLevels();
        }

        ClearLevel();

        state = RPGameState.Aiming;
        shotsUsed = 0;
        currentLevel = levels[index];

        if (menuPanel != null) menuPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);

        SpawnBoundaryWalls();
        SpawnLevelWalls(currentLevel);
        SpawnTargets(currentLevel);

        if (ball != null)
        {
            ball.gameObject.SetActive(true);
            ball.ResetBall(currentLevel.ballStart);
            ball.SetGameManager(this);
        }

        ApplySelectedSkin();

        if (audioManager != null)
        {
            audioManager.PlayLevelStart();
        }

        UpdateUI();
        UpdateMetaUI();
    }

    private void ClearLevel()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        spawnedObjects.Clear();
        activeTargets.Clear();

        if (aimLine != null) aimLine.enabled = false;
        if (previewLine != null) previewLine.enabled = false;
    }

    private void SpawnBoundaryWalls()
    {
        SpawnWall(new RPWallData(new Vector3(0f, 0.45f, 7.0f), new Vector3(10.5f, 0.9f, 0.35f)));
        SpawnWall(new RPWallData(new Vector3(0f, 0.45f, -7.0f), new Vector3(10.5f, 0.9f, 0.35f)));
        SpawnWall(new RPWallData(new Vector3(-5.0f, 0.45f, 0f), new Vector3(0.35f, 0.9f, 14.0f)));
        SpawnWall(new RPWallData(new Vector3(5.0f, 0.45f, 0f), new Vector3(0.35f, 0.9f, 14.0f)));
    }

    private void SpawnLevelWalls(RPLevelData data)
    {
        foreach (RPWallData wall in data.walls)
        {
            SpawnWall(wall);
        }
    }

    private void SpawnWall(RPWallData wall)
    {
        GameObject prefab = wallPrefab;
        Material material = wallMaterial;

        if (wall.type == RPObstacleType.MetalBumper && metalBumperPrefab != null)
        {
            prefab = metalBumperPrefab;
            material = metalBumperMaterial;
        }
        else if (wall.type == RPObstacleType.BreakableGlass && breakableGlassPrefab != null)
        {
            prefab = breakableGlassPrefab;
            material = breakableGlassMaterial;
        }

        GameObject obj = Instantiate(prefab, wall.position, Quaternion.identity, levelRoot);
        obj.name = wall.type.ToString();
        obj.SetActive(true);
        obj.transform.localScale = wall.scale;

        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();

        if (renderer != null && material != null)
        {
            renderer.sharedMaterial = material;
        }

        RPObstacle obstacle = obj.GetComponent<RPObstacle>();
        if (obstacle != null)
        {
            obstacle.type = wall.type;
            obstacle.gameManager = this;
        }

        spawnedObjects.Add(obj);
    }

    private void SpawnTargets(RPLevelData data)
    {
        foreach (Vector3 position in data.targets)
        {
            GameObject targetObject = Instantiate(targetPrefab, position, Quaternion.identity, levelRoot);
            targetObject.name = "Glass_Target";
            targetObject.SetActive(true);

            if (targetMaterial != null)
            {
                MeshRenderer renderer = targetObject.GetComponent<MeshRenderer>();

                if (renderer != null)
                {
                    renderer.sharedMaterial = targetMaterial;
                }
            }

            RPTarget target = targetObject.GetComponent<RPTarget>();
            target.SetGameManager(this);
            activeTargets.Add(target);
            spawnedObjects.Add(targetObject);
        }
    }

    private void HandleAimInput()
    {
        if (ball == null || mainCamera == null)
        {
            return;
        }

        if (PointerDown())
        {
            isDragging = true;
        }

        if (PointerHeld() && isDragging)
        {
            Vector3 pointerWorld = GetPointerWorldPosition();
            Vector3 pullVector = ball.transform.position - pointerWorld;
            pullVector.y = 0f;

            float pullDistance = Mathf.Clamp(pullVector.magnitude, 0f, maxPullDistance);
            float power01 = Mathf.Clamp01(pullDistance / maxPullDistance);

            if (pullDistance > 0.08f)
            {
                currentAimDirection = pullVector.normalized;
                currentPower = Mathf.Lerp(minLaunchPower, maxLaunchPower, power01);
                DrawSlingshotLine(pointerWorld, power01);
                DrawRicochetPreview(currentAimDirection);
            }
        }

        if (PointerUp() && isDragging)
        {
            isDragging = false;

            if (aimLine != null) aimLine.enabled = false;
            if (previewLine != null) previewLine.enabled = false;

            LaunchBall(currentAimDirection, currentPower);
        }
    }

    private bool PointerDown()
    {
#if ENABLE_INPUT_SYSTEM
        bool mouseDown = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool touchDown = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        return mouseDown || touchDown;
#else
        if (Input.GetMouseButtonDown(0)) return true;
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#endif
    }

    private bool PointerHeld()
    {
#if ENABLE_INPUT_SYSTEM
        bool mouseHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;
        bool touchHeld = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        return mouseHeld || touchHeld;
#else
        if (Input.GetMouseButton(0)) return true;
        return Input.touchCount > 0;
#endif
    }

    private bool PointerUp()
    {
#if ENABLE_INPUT_SYSTEM
        bool mouseUp = Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;
        bool touchUp = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;
        return mouseUp || touchUp;
#else
        if (Input.GetMouseButtonUp(0)) return true;
        return Input.touchCount > 0 && (
            Input.GetTouch(0).phase == TouchPhase.Ended ||
            Input.GetTouch(0).phase == TouchPhase.Canceled
        );
#endif
    }

    private Vector3 GetPointerWorldPosition()
    {
        Vector2 screenPosition = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            screenPosition = Mouse.current.position.ReadValue();
        }
#else
        screenPosition = Input.mousePosition;

        if (Input.touchCount > 0)
        {
            screenPosition = Input.GetTouch(0).position;
        }
#endif

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return ball.transform.position + Vector3.back;
    }

    private void DrawSlingshotLine(Vector3 pointerWorld, float power01)
    {
        if (aimLine == null)
        {
            return;
        }

        aimLine.enabled = true;
        aimLine.positionCount = 2;
        aimLine.startWidth = Mathf.Lerp(0.04f, 0.11f, power01);
        aimLine.endWidth = 0.025f;

        Vector3 start = ball.transform.position + Vector3.up * 0.08f;
        Vector3 end = pointerWorld + Vector3.up * 0.08f;

        aimLine.SetPosition(0, start);
        aimLine.SetPosition(1, end);
    }

    private void DrawRicochetPreview(Vector3 direction)
    {
        if (previewLine == null || ball == null)
        {
            return;
        }

        previewLine.enabled = true;

        const int maxSegments = 4;
        const float maxSegmentLength = 5.0f;
        const float skin = 0.05f;

        List<Vector3> points = new List<Vector3>();
        Vector3 origin = ball.transform.position + Vector3.up * 0.08f;
        Vector3 dir = direction.normalized;

        points.Add(origin);

        for (int i = 0; i < maxSegments; i++)
        {
            Ray ray = new Ray(origin, dir);

            if (Physics.Raycast(ray, out RaycastHit hit, maxSegmentLength))
            {
                Vector3 hitPoint = hit.point + Vector3.up * 0.08f;
                points.Add(hitPoint);

                dir = Vector3.Reflect(dir, hit.normal).normalized;
                origin = hit.point + dir * skin + Vector3.up * 0.08f;
            }
            else
            {
                points.Add(origin + dir * maxSegmentLength);
                break;
            }
        }

        previewLine.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            previewLine.SetPosition(i, points[i]);
        }
    }

    private void LaunchBall(Vector3 direction, float power)
    {
        shotsUsed++;
        state = RPGameState.BallMoving;

        if (audioManager != null)
        {
            audioManager.PlayShoot();
        }

        if (ball != null)
        {
            ball.Launch(direction, power);
        }

        UpdateUI();
    }

    public void OnTargetHit(RPTarget target)
    {
        if (!activeTargets.Contains(target))
        {
            return;
        }

        activeTargets.Remove(target);

        if (audioManager != null)
        {
            audioManager.PlayGlassPop();
        }

        if (cameraShake != null)
        {
            cameraShake.Shake(0.08f, 0.08f);
        }

        target.Pop();

        UpdateUI();

        if (activeTargets.Count == 0)
        {
            CompleteLevel();
        }
    }

    public void OnObstacleBroken(RPObstacle obstacle)
    {
        if (audioManager != null)
        {
            audioManager.PlayGlassPop();
        }

        if (cameraShake != null)
        {
            cameraShake.Shake(0.06f, 0.055f);
        }
    }

    public void OnBumperHit()
    {
        if (audioManager != null)
        {
            audioManager.PlayBumper();
        }

        if (cameraShake != null)
        {
            cameraShake.Shake(0.035f, 0.035f);
        }
    }

    private void CompleteLevel()
    {
        state = RPGameState.LevelComplete;

        if (ball != null)
        {
            ball.StopBall();
        }

        int stars = CalculateStars();
        int previousBest = progression != null ? progression.GetBestStarsForLevel(currentLevelIndex) : 0;
        int coinsEarned = progression != null ? progression.RegisterLevelResult(currentLevelIndex, stars, shotsUsed, currentLevel.parShots) : 0;
        int newBest = progression != null ? progression.GetBestStarsForLevel(currentLevelIndex) : stars;
        bool improved = newBest > previousBest;

        string motivation = progression != null ? progression.GetMotivationLine(stars, shotsUsed, currentLevel.parShots) : "Cleared!";

        if (audioManager != null)
        {
            audioManager.PlayWin();
        }

        if (winPanel != null) winPanel.SetActive(true);

        if (winTitleText != null)
        {
            winTitleText.text = motivation;
        }

        if (winStatsText != null)
        {
            winStatsText.text =
                currentLevel.title + "\n\n" +
                "Shots: " + shotsUsed + " / Par: " + currentLevel.parShots + "\n" +
                GetStarString(stars) + "\n" +
                "Best: " + GetStarString(newBest);
        }

        if (winRewardText != null)
        {
            int streak = progression != null ? progression.perfectStreak : 0;
            string improvedText = improved ? "\nNew best!" : "\nReplay for a better score.";
            winRewardText.text = "+" + coinsEarned + " coins" + improvedText + (streak >= 2 ? "\nPerfect streak x" + streak : "");
        }

        UpdateMetaUI();
    }

    private int CalculateStars()
    {
        if (currentLevel == null)
        {
            return 1;
        }

        if (shotsUsed <= currentLevel.parShots) return 3;
        if (shotsUsed <= currentLevel.parShots + 2) return 2;
        return 1;
    }

    private string GetStarString(int stars)
    {
        if (stars >= 3) return "★ ★ ★";
        if (stars == 2) return "★ ★ ☆";
        if (stars == 1) return "★ ☆ ☆";
        return "☆ ☆ ☆";
    }

    public void ResetShot()
    {
        if (state != RPGameState.BallMoving)
        {
            return;
        }

        state = RPGameState.Aiming;

        if (currentLevel != null && ball != null)
        {
            ball.ResetBall(currentLevel.ballStart);
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentLevel == null)
        {
            return;
        }

        if (levelText != null)
        {
            levelText.text = "LEVEL " + (currentLevelIndex + 1) + "\n" + currentLevel.title;
        }

        if (shotsText != null)
        {
            shotsText.text = "SHOTS\n" + shotsUsed + "/" + currentLevel.parShots;
        }

        if (targetsText != null)
        {
            targetsText.text = "GLASS\n" + activeTargets.Count;
        }

        if (hintText != null)
        {
            if (currentLevelIndex == 0)
                hintText.text = "Pull back. Release. Break the glass.";
            else if (currentLevelIndex == 1)
                hintText.text = "Use the rail. Preview shows the bounce.";
            else if (currentLevelIndex == 6)
                hintText.text = "Metal bumpers bounce harder.";
            else if (currentLevelIndex == 7)
                hintText.text = "Glass walls break when hit.";
            else
                hintText.text = "Clear it under par for a perfect shot.";
        }

        if (progressSlider != null && levels != null && levels.Count > 0)
        {
            progressSlider.value = Mathf.Clamp01((currentLevelIndex + 1) / (float)levels.Count);
        }
    }

    private void UpdateMetaUI()
    {
        if (coinsText != null)
        {
            int coins = progression != null ? progression.totalCoins : 0;
            coinsText.text = "COINS\n" + coins;
        }

        if (streakText != null)
        {
            int streak = progression != null ? progression.perfectStreak : 0;
            streakText.text = streak > 0 ? "STREAK\nx" + streak : "STREAK\n—";
        }

        if (hintTokensText != null)
        {
            int hints = progression != null ? progression.hintTokens : 0;
            hintTokensText.text = "HINTS\n" + hints;
        }

        if (menuProgressText != null && progression != null)
        {
            menuProgressText.text = progression.GetMenuProgressLine();
        }
    }
}