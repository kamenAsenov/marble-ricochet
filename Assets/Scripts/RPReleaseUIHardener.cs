using UnityEngine;
using UnityEngine.UI;

public class RPReleaseUIHardener : MonoBehaviour
{
    public GameObject objectivePanel;
    public Text objectiveText;
    public GameObject failPanel;

    private float nextCleanupTime;

    private void Awake()
    {
        RemoveOldDecorativeLineNoise();
        SuppressSecondaryHudNoise();
        ApplyLayoutPass();
    }

    private void Update()
    {
        if (Time.unscaledTime >= nextCleanupTime)
        {
            nextCleanupTime = Time.unscaledTime + 0.35f;
            SuppressSecondaryHudNoise();
            ApplyLayoutPass();
        }
    }

    private void SuppressSecondaryHudNoise()
    {
        RPHazardManager hazards = FindFirstObjectByType<RPHazardManager>();
        if (hazards != null)
        {
            if (hazards.hazardHintText != null)
            {
                hazards.hazardHintText.gameObject.SetActive(false);
                hazards.hazardHintText = null;
            }
        }

        RPBallStatsManager stats = FindFirstObjectByType<RPBallStatsManager>();
        if (stats != null)
        {
            if (stats.statsHudText != null)
            {
                stats.statsHudText.gameObject.SetActive(false);
                stats.statsHudText = null;
            }
        }
    }

    private void ApplyLayoutPass()
    {
        if (objectivePanel != null)
        {
            RectTransform rect = objectivePanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.5f, 0.835f);
                rect.anchorMax = new Vector2(0.5f, 0.835f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(560f, 58f);
            }
        }

        if (objectiveText != null)
        {
            objectiveText.fontSize = 14;
            objectiveText.alignment = TextAnchor.MiddleCenter;
            objectiveText.horizontalOverflow = HorizontalWrapMode.Wrap;
            objectiveText.verticalOverflow = VerticalWrapMode.Truncate;
            objectiveText.color = new Color(0.78f, 0.82f, 0.82f, 1f);
        }

        if (failPanel == null)
        {
            failPanel = GameObject.Find("FailPanel");
        }

        if (failPanel != null && failPanel.activeSelf)
        {
            failPanel.transform.SetAsLastSibling();
            if (objectivePanel != null)
            {
                objectivePanel.SetActive(false);
            }
        }
    }

    private void RemoveOldDecorativeLineNoise()
    {
        Transform[] transforms = FindObjectsOfType<Transform>(true);

        foreach (Transform t in transforms)
        {
            if (t == null || t.gameObject == gameObject)
            {
                continue;
            }

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
                n.Contains("playfield_right_guide")
            )
            {
                Destroy(t.gameObject);
            }
        }
    }
}
