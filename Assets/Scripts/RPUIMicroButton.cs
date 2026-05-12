using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class RPUIMicroButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public float hoverScale = 1.035f;
    public float pressScale = 0.965f;
    public float animationSpeed = 12f;

    private RectTransform rectTransform;
    private Vector3 startScale;
    private Vector3 targetScale;
    private Button button;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        startScale = rectTransform.localScale;
        targetScale = startScale;
    }

    private void OnEnable()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        startScale = rectTransform.localScale;
        targetScale = startScale;
    }

    private void Update()
    {
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.unscaledDeltaTime * animationSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsInteractable())
        {
            targetScale = startScale * hoverScale;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = startScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsInteractable())
        {
            targetScale = startScale * pressScale;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsInteractable())
        {
            targetScale = startScale * hoverScale;
        }
        else
        {
            targetScale = startScale;
        }
    }

    private bool IsInteractable()
    {
        return button == null || button.interactable;
    }
}