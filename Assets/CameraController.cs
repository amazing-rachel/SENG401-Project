using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float mainSpeed = 10f; // main camera movement speed
    public float shiftMultiplier = 3f; // boost for holding shift
    public float maxSpeed = 100f;

    [Header("Mouse Look")]
    public float camSensitivity = 0.2f;
    public float smoothSpeed = 12f;

    [Header("Scroll")]
    public float scrollSensitivity = 0.1f;
    public float minSpeed = 2f;
    public float maxScrollSpeed = 20f;

    private float currentSpeed;
    private float yaw; // horizontal rotation
    private float pitch; // vertical rotation

    private float smoothYaw;
    private float smoothPitch;

    void Start()
    {
        currentSpeed = mainSpeed;
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        smoothYaw = yaw;
        smoothPitch = pitch;

    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleScrollSpeed();
    }

    // smoother rotation after update
        void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(smoothPitch, smoothYaw, 0f);
    }

    void HandleMouseLook()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.rightButton.isPressed)
        {
            Cursor.lockState = CursorLockMode.Locked; // lock for camera control
            Cursor.visible = false;

            Vector2 delta = Mouse.current.delta.ReadValue();

            float mouseX = delta.x * camSensitivity;
            float mouseY = delta.y * camSensitivity;

            yaw += mouseX;

            pitch -= mouseY;

            pitch = Mathf.Clamp(pitch, -80f, 80f);

            float t = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);

            smoothYaw = Mathf.Lerp(smoothYaw, yaw, t);
            smoothPitch = Mathf.Lerp(smoothPitch, pitch, t);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // WASD Controls
    void HandleMovement()
    {
        Vector3 input = Vector3.zero;

        if (Keyboard.current.wKey.isPressed) input.z += 1;
        if (Keyboard.current.sKey.isPressed) input.z -= 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;

        input.Normalize();

        float speed = currentSpeed;

        if (Keyboard.current.leftShiftKey.isPressed) // speed boost for holding shift
            speed *= shiftMultiplier;

        speed = Mathf.Clamp(speed, 0f, maxSpeed);

        Vector3 move = transform.TransformDirection(input) * speed * Time.deltaTime;
        transform.position += move;    
        
    }

    void HandleScrollSpeed()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        currentSpeed += scroll * scrollSensitivity;
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxScrollSpeed);
    }
}