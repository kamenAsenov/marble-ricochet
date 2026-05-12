using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.15f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;

    [Header("Mobile Input")]
    public MobileInputBridge mobileInput;

    [Header("State")]
    public bool canMove = true;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovement();
            HandleMouseLook();
        }

        if (UnlockPressed())
        {
            UnlockCursor();
        }

        if (LeftMousePressed())
        {
            LockCursor();
        }
    }

    private void HandleMovement()
    {
        Vector2 input = GetMoveInput();
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        if (playerCamera == null)
        {
            return;
        }

        Vector2 lookDelta = GetLookDelta();

        float mouseX = lookDelta.x * mouseSensitivity * 0.08f;
        float mouseY = lookDelta.y * mouseSensitivity * 0.08f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -75f, 75f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private Vector2 GetMoveInput()
    {
        Vector2 input = Vector2.zero;

        if (mobileInput != null && mobileInput.mobileControlsEnabled && mobileInput.moveInput.sqrMagnitude > 0.001f)
        {
            input = mobileInput.moveInput;
        }
        else
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
            }
#else
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
#endif
        }

        if (input.magnitude > 1f)
        {
            input.Normalize();
        }

        return input;
    }

    private Vector2 GetLookDelta()
    {
        if (mobileInput != null && mobileInput.mobileControlsEnabled && mobileInput.lookInput.sqrMagnitude > 0.001f)
        {
            return mobileInput.lookInput;
        }

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            return Mouse.current.delta.ReadValue();
        }

        return Vector2.zero;
#else
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 12f;
#endif
    }

    private bool UnlockPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Tab);
#endif
    }

    private bool LeftMousePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
    }

    public void ResetLook()
    {
        xRotation = 0f;

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.identity;
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}