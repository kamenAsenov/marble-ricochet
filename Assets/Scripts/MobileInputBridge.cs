using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileInputBridge : MonoBehaviour
{
    [Header("State")]
    public bool mobileControlsEnabled = true;

    [Header("Virtual Input")]
    public Vector2 moveInput;
    public Vector2 lookInput;

    [Header("UI")]
    public GameObject mobileControlsRoot;
    public RectTransform joystickKnob;
    public RectTransform joystickBackground;
    public RectTransform lookZone;

    [Header("Tuning")]
    public float joystickRadius = 90f;
    public float lookSensitivityMultiplier = 1.0f;

    private int moveFingerId = -1;
    private int lookFingerId = -1;
    private Vector2 joystickStartScreenPosition;
    private Vector2 lastLookPosition;

    private void Update()
    {
        if (!mobileControlsEnabled)
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
            return;
        }

        lookInput = Vector2.zero;

        if (Input.touchSupported && Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        else
        {
            ResetTouchState();
        }
    }

    public void SetMobileControlsVisible(bool visible)
    {
        if (mobileControlsRoot != null)
        {
            mobileControlsRoot.SetActive(visible);
        }

        mobileControlsEnabled = visible;
    }

    private void HandleTouchInput()
    {
        bool moveFingerStillExists = false;
        bool lookFingerStillExists = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.fingerId == moveFingerId)
            {
                moveFingerStillExists = true;
                UpdateMoveTouch(touch);
                continue;
            }

            if (touch.fingerId == lookFingerId)
            {
                lookFingerStillExists = true;
                UpdateLookTouch(touch);
                continue;
            }

            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x < Screen.width * 0.5f && moveFingerId == -1)
                {
                    BeginMoveTouch(touch);
                    moveFingerStillExists = true;
                }
                else if (touch.position.x >= Screen.width * 0.5f && lookFingerId == -1)
                {
                    BeginLookTouch(touch);
                    lookFingerStillExists = true;
                }
            }
        }

        if (!moveFingerStillExists)
        {
            EndMoveTouch();
        }

        if (!lookFingerStillExists)
        {
            EndLookTouch();
        }
    }

    private void BeginMoveTouch(Touch touch)
    {
        moveFingerId = touch.fingerId;
        joystickStartScreenPosition = touch.position;

        if (joystickBackground != null)
        {
            joystickBackground.position = joystickStartScreenPosition;
        }

        if (joystickKnob != null)
        {
            joystickKnob.position = joystickStartScreenPosition;
        }

        UpdateMoveTouch(touch);
    }

    private void UpdateMoveTouch(Touch touch)
    {
        Vector2 delta = touch.position - joystickStartScreenPosition;
        Vector2 clamped = Vector2.ClampMagnitude(delta, joystickRadius);

        moveInput = clamped / joystickRadius;

        if (joystickKnob != null)
        {
            joystickKnob.position = joystickStartScreenPosition + clamped;
        }
    }

    private void EndMoveTouch()
    {
        moveFingerId = -1;
        moveInput = Vector2.zero;

        if (joystickKnob != null && joystickBackground != null)
        {
            joystickKnob.position = joystickBackground.position;
        }
    }

    private void BeginLookTouch(Touch touch)
    {
        lookFingerId = touch.fingerId;
        lastLookPosition = touch.position;
    }

    private void UpdateLookTouch(Touch touch)
    {
        Vector2 currentPosition = touch.position;
        lookInput = (currentPosition - lastLookPosition) * lookSensitivityMultiplier;
        lastLookPosition = currentPosition;
    }

    private void EndLookTouch()
    {
        lookFingerId = -1;
        lookInput = Vector2.zero;
    }

    private void ResetTouchState()
    {
        moveFingerId = -1;
        lookFingerId = -1;
        moveInput = Vector2.zero;
        lookInput = Vector2.zero;
    }
}