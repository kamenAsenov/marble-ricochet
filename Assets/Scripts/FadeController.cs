using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;

    public IEnumerator FadeToBlack(float duration)
    {
        yield return Fade(0f, 1f, duration);
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        yield return Fade(1f, 0f, duration);
    }

    public void ClearInstant()
    {
        if (fadeImage == null)
        {
            return;
        }

        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.SmoothStep(from, to, t);

            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;

            yield return null;
        }

        Color final = fadeImage.color;
        final.a = to;
        fadeImage.color = final;
    }
}