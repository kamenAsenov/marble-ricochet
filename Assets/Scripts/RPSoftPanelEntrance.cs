using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RPSoftPanelEntrance : MonoBehaviour
{
    public float startScale = 0.96f;
    public float targetScale = 1.0f;
    public float animationSpeed = 10f;

    private RectTransform rectTransform;
    private Vector3 desiredScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        rectTransform.localScale = Vector3.one * startScale;
        desiredScale = Vector3.one * targetScale;
    }

    private void Update()
    {
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            desiredScale,
            Time.unscaledDeltaTime * animationSpeed
        );
    }
}