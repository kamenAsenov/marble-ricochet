using UnityEngine;
using UnityEngine.UI;

public class ScreenMoodController : MonoBehaviour
{
    public Image darkOverlay;
    public Image dangerOverlay;
    public Image vignetteOverlay;

    public float normalDarkAlpha = 0.04f;

    private float targetDarkAlpha;
    private float targetDangerAlpha;
    private float targetVignetteAlpha;

    private void Start()
    {
        targetDarkAlpha = normalDarkAlpha;
        targetDangerAlpha = 0f;
        targetVignetteAlpha = 0.08f;
        ApplyImmediate();
    }

    private void Update()
    {
        LerpImageAlpha(darkOverlay, targetDarkAlpha, 2.5f);
        LerpImageAlpha(dangerOverlay, targetDangerAlpha, 3.5f);
        LerpImageAlpha(vignetteOverlay, targetVignetteAlpha, 2.0f);
    }

    public void SetLoopMood(int loop, bool anomalyActive)
    {
        targetDarkAlpha = Mathf.Clamp(normalDarkAlpha + loop * 0.002f, 0.04f, 0.16f);
        targetDangerAlpha = anomalyActive ? 0.010f : 0f;
        targetVignetteAlpha = Mathf.Clamp(0.08f + loop * 0.002f, 0.08f, 0.20f);
    }

    public void SetDangerMood(bool enabled)
    {
        targetDangerAlpha = enabled ? 0.18f : 0f;
        targetDarkAlpha = enabled ? 0.22f : normalDarkAlpha;
        targetVignetteAlpha = enabled ? 0.25f : 0.08f;
    }

    private void LerpImageAlpha(Image image, float targetAlpha, float speed)
    {
        if (image == null) return;
        Color c = image.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * speed);
        image.color = c;
    }

    private void ApplyImmediate()
    {
        SetImageAlpha(darkOverlay, targetDarkAlpha);
        SetImageAlpha(dangerOverlay, targetDangerAlpha);
        SetImageAlpha(vignetteOverlay, targetVignetteAlpha);
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        if (image == null) return;
        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
}