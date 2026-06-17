using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RPComboTextFloat : MonoBehaviour
{
    public float floatAmount = 16f;
    public float floatSpeed = 2.0f;

    private RectTransform rectTransform;
    private Vector2 startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        rectTransform.anchoredPosition = startPosition;
    }

    private void Update()
    {
        float y = Mathf.Sin(Time.unscaledTime * floatSpeed) * floatAmount;
        rectTransform.anchoredPosition = startPosition + new Vector2(0f, y);
    }
}