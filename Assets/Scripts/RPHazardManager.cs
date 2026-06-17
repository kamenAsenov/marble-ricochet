using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RPHazardManager : MonoBehaviour
{
    [Header("References")]
    public RPGameManager gameManager;
    public RPBallStatsManager ballStats;

    [Header("Prefabs")]
    public GameObject holePrefab;
    public GameObject rotatingObstaclePrefab;

    [Header("UI")]
    public GameObject failPanel;
    public Text failTitleText;
    public Text failReasonText;
    public Text hazardHintText;

    private readonly List<GameObject> spawnedHazards = new List<GameObject>();
    private int lastLevelIndex = -1;
    private bool failed = false;

    private void Start()
    {
        ResolveReferences();
        HideFail();
    }

    private void Update()
    {
        ResolveReferences();

        if (gameManager == null)
        {
            return;
        }

        if (gameManager.currentLevelIndex != lastLevelIndex)
        {
            lastLevelIndex = gameManager.currentLevelIndex;
            failed = false;
            HideFail();
            SpawnHazardsForLevel(lastLevelIndex);
            ShowHazardHint(lastLevelIndex);
        }
    }

    public void OnBallEnteredHole(RPBulletBall ball, Vector3 holePosition)
    {
        if (failed)
        {
            return;
        }

        failed = true;

        if (ball != null)
        {
            ball.StopBall();
            ball.transform.position = holePosition + Vector3.up * 0.05f;
        }

        if (gameManager != null)
        {
            gameManager.state = RPGameState.LevelComplete;
        }

        ShowFail("Marble Lost", "The marble fell into a pocket.\nRetry and plan a safer route.");
    }

    public void Retry()
    {
        failed = false;
        HideFail();

        if (gameManager != null)
        {
            gameManager.RestartLevel();
        }
    }

    public void UseHint()
    {
        if (gameManager != null)
        {
            HideFail();
            gameManager.UseHint();
            gameManager.RestartLevel();
        }
    }

    public void OpenShop()
    {
        RPShopManager shop = FindFirstObjectByType<RPShopManager>();

        if (shop != null)
        {
            shop.OpenShop();
        }
    }

    public void GoToMenu()
    {
        failed = false;
        HideFail();

        if (gameManager != null)
        {
            gameManager.ShowMenu();
        }
    }

    private void SpawnHazardsForLevel(int levelIndex)
    {
        ClearHazards();

        int level = levelIndex + 1;

        if (level < 11)
        {
            return;
        }

        if (holePrefab == null || rotatingObstaclePrefab == null)
        {
            return;
        }

        // Controlled hazard introduction:
        // 11-15: holes only
        // 16-20: rotating obstacles only or simple mix
        // 21+: combined routes
        if (level == 11)
        {
            SpawnHole(new Vector3(0f, 0.11f, 1.25f), 0.72f);
        }
        else if (level == 12)
        {
            SpawnHole(new Vector3(-2.2f, 0.11f, 1.5f), 0.68f);
            SpawnHole(new Vector3(2.2f, 0.11f, 1.5f), 0.68f);
        }
        else if (level == 13)
        {
            SpawnHole(new Vector3(0f, 0.11f, 2.2f), 0.66f);
            SpawnHole(new Vector3(3.2f, 0.11f, -0.9f), 0.62f);
        }
        else if (level == 14)
        {
            SpawnHole(new Vector3(-3.0f, 0.11f, 2.1f), 0.64f);
            SpawnHole(new Vector3(3.0f, 0.11f, 2.1f), 0.64f);
        }
        else if (level == 15)
        {
            SpawnHole(new Vector3(0f, 0.11f, 0.3f), 0.72f);
            SpawnHole(new Vector3(-2.7f, 0.11f, 2.8f), 0.62f);
            SpawnHole(new Vector3(2.7f, 0.11f, 2.8f), 0.62f);
        }
        else if (level == 16)
        {
            SpawnRotator(new Vector3(0f, 0.45f, 1.1f), new Vector3(2.3f, 0.55f, 0.22f), 55f, false);
        }
        else if (level == 17)
        {
            SpawnRotator(new Vector3(-1.8f, 0.45f, 1.6f), new Vector3(2.1f, 0.55f, 0.22f), 65f, false);
            SpawnRotator(new Vector3(1.8f, 0.45f, 0.0f), new Vector3(2.1f, 0.55f, 0.22f), 65f, true);
        }
        else if (level == 18)
        {
            SpawnHole(new Vector3(0f, 0.11f, 2.25f), 0.65f);
            SpawnRotator(new Vector3(0f, 0.45f, 0.0f), new Vector3(2.5f, 0.55f, 0.22f), 70f, false);
        }
        else if (level == 19)
        {
            SpawnHole(new Vector3(-2.8f, 0.11f, 1.6f), 0.60f);
            SpawnHole(new Vector3(2.8f, 0.11f, 1.6f), 0.60f);
            SpawnRotator(new Vector3(0f, 0.45f, 1.6f), new Vector3(2.4f, 0.55f, 0.22f), 75f, false);
        }
        else if (level == 20)
        {
            SpawnHole(new Vector3(0f, 0.11f, 0.4f), 0.70f);
            SpawnRotator(new Vector3(-2.0f, 0.45f, 2.0f), new Vector3(2.2f, 0.55f, 0.22f), 70f, false);
            SpawnRotator(new Vector3(2.0f, 0.45f, 2.0f), new Vector3(2.2f, 0.55f, 0.22f), 70f, true);
        }
        else if (level % 3 == 0)
        {
            SpawnHole(new Vector3(0f, 0.11f, 1.0f), 0.66f);
            SpawnRotator(new Vector3(0f, 0.45f, 2.4f), new Vector3(2.4f, 0.55f, 0.22f), 60f + level, false);
        }
        else if (level % 3 == 1)
        {
            SpawnHole(new Vector3(-2.8f, 0.11f, 2.1f), 0.62f);
            SpawnHole(new Vector3(2.8f, 0.11f, 2.1f), 0.62f);
        }
        else
        {
            SpawnRotator(new Vector3(0f, 0.45f, 1.6f), new Vector3(2.6f, 0.55f, 0.22f), 68f, level % 2 == 0);
        }
    }

    private void SpawnHole(Vector3 position, float diameter)
    {
        GameObject obj = Instantiate(holePrefab, position, Quaternion.identity);
        obj.name = "Hazard_Hole_Pocket";
        obj.SetActive(true);
        obj.transform.localScale = new Vector3(diameter, 0.08f, diameter);

        RPHolePocket hole = obj.GetComponent<RPHolePocket>();

        if (hole != null)
        {
            hole.hazardManager = this;
        }

        spawnedHazards.Add(obj);
    }

    private void SpawnRotator(Vector3 position, Vector3 scale, float speed, bool alternate)
    {
        GameObject obj = Instantiate(rotatingObstaclePrefab, position, Quaternion.identity);
        obj.name = "Hazard_Rotating_Obstacle";
        obj.SetActive(true);
        obj.transform.localScale = scale;

        RPRotatingObstacle rotator = obj.GetComponent<RPRotatingObstacle>();

        if (rotator != null)
        {
            rotator.rotationSpeed = speed;
            rotator.alternateDirection = alternate;
        }

        RPObstacle obstacle = obj.GetComponent<RPObstacle>();

        if (obstacle != null)
        {
            obstacle.type = RPObstacleType.NormalWall;
            obstacle.gameManager = gameManager;
        }

        spawnedHazards.Add(obj);
    }

    private void ClearHazards()
    {
        foreach (GameObject obj in spawnedHazards)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        spawnedHazards.Clear();
    }

    private void ShowFail(string title, string reason)
    {
        if (failPanel != null)
        {
            failPanel.SetActive(true);
        }

        if (failTitleText != null)
        {
            failTitleText.text = title;
        }

        if (failReasonText != null)
        {
            failReasonText.text = reason;
        }
    }

    private void HideFail()
    {
        if (failPanel != null)
        {
            failPanel.SetActive(false);
        }
    }

    private void ShowHazardHint(int levelIndex)
    {
        if (hazardHintText == null)
        {
            return;
        }

        int level = levelIndex + 1;

        if (level == 11)
        {
            hazardHintText.text = "New: pockets. Avoid the holes.";
            hazardHintText.gameObject.SetActive(true);
        }
        else if (level == 16)
        {
            hazardHintText.text = "New: rotating blockers. Time your shot.";
            hazardHintText.gameObject.SetActive(true);
        }
        else if (level >= 18 && level <= 20)
        {
            hazardHintText.text = "Avoid pockets. Use timing and rails.";
            hazardHintText.gameObject.SetActive(true);
        }
        else
        {
            hazardHintText.gameObject.SetActive(false);
        }
    }

    private void ResolveReferences()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<RPGameManager>();
        }

        if (ballStats == null)
        {
            ballStats = FindFirstObjectByType<RPBallStatsManager>();
        }
    }
}