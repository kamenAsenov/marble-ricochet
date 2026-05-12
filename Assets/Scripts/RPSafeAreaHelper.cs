using UnityEngine;

[ExecuteAlways]
public class RPSafeAreaHelper : MonoBehaviour
{
    public RectTransform target;
    public bool applyOnUpdate = true;

    private Rect lastSafeArea = new Rect(0, 0, 0, 0);
    private ScreenOrientation lastOrientation;

    private void Awake()
    {
        if (target == null)
        {
            target = GetComponent<RectTransform>();
        }

        ApplySafeArea();
    }

    private void OnEnable()
    {
        ApplySafeArea();
    }

    private void Update()
    {
        if (!applyOnUpdate)
        {
            return;
        }

        if (Screen.safeArea != lastSafeArea || Screen.orientation != lastOrientation)
        {
            ApplySafeArea();
        }
    }

    public void ApplySafeArea()
    {
        if (target == null)
        {
            return;
        }

        Rect safeArea = Screen.safeArea;

        if (Screen.width <= 0 || Screen.height <= 0)
        {
            return;
        }

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        target.anchorMin = anchorMin;
        target.anchorMax = anchorMax;

        lastSafeArea = safeArea;
        lastOrientation = Screen.orientation;
    }
}