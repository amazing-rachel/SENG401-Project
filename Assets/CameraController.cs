using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float mainSpeed = 8f;
    public float shiftMultiplier = 3f;
    public float maxSpeed = 20f;

    [Header("Mouse Look")]
    public float camSensitivity = 0.15f;

    [Header("Scroll")]
    public float scrollSensitivity = 2f;
    public float minSpeed = 2f;
    public float maxScrollSpeed = 20f;

    private float currentSpeed;
    private float yaw;
    private float pitch;

    void Start()
    {
        currentSpeed = mainSpeed;
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleScrollSpeed();
    }

    void HandleMouseLook()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.rightButton.isPressed)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Vector2 delta = Mouse.current.delta.ReadValue();
            delta = Vector2.ClampMagnitude(delta, 10f);

            float mouseX = delta.x * camSensitivity * 60f * Time.deltaTime;
            float mouseY = delta.y * camSensitivity * 60f * Time.deltaTime;

            yaw += mouseX;

            pitch -= mouseY;

            pitch = Mathf.Clamp(pitch, -80f, 80f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleMovement()
    {
        Vector3 input = Vector3.zero;

        if (Keyboard.current.wKey.isPressed) input.z += 1;
        if (Keyboard.current.sKey.isPressed) input.z -= 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;

        input.Normalize();

        float speed = currentSpeed;

        if (Keyboard.current.leftShiftKey.isPressed)
            speed *= shiftMultiplier;

        speed = Mathf.Clamp(speed, 0f, maxSpeed);

        transform.Translate(input * speed * Time.deltaTime, Space.Self);
    }

    void HandleScrollSpeed()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        scroll = Mathf.Clamp(scroll, -1f, 1f);

        currentSpeed += scroll * scrollSensitivity;
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxScrollSpeed);
    }
}