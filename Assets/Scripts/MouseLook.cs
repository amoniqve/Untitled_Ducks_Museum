using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity      = 100f;
    public float controllerSensitivity = 400f;
    public Transform playerBody;

    private const string RightStickX = "RightStickX"; // horizontal — drives yaw
    private const string RightStickY = "RightStickY"; // vertical   — drives pitch

    private float xRotation = 0f;

    void Update()
    {
        // Only run during gameplay — cursor is locked during play, unlocked during menus and game-over
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity      * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity      * Time.deltaTime;

        // Right stick — software deadzone on top of Input Manager deadzone to suppress controller drift
        float stickYaw   = Stick(RightStickX) * controllerSensitivity * Time.deltaTime;
        float stickPitch = Stick(RightStickY) * controllerSensitivity * Time.deltaTime;

        // Vertical look (pitch)
        xRotation -= mouseY + stickPitch;
        xRotation  = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal look (yaw — rotates player body)
        playerBody.Rotate(Vector3.up * (mouseX + stickYaw));
    }

    /// <summary>Returns the axis value zeroed below the deadzone threshold to suppress stick drift.</summary>
    private static float Stick(string axis, float deadzone = 0.12f)
    {
        float v = Input.GetAxis(axis);
        return Mathf.Abs(v) < deadzone ? 0f : v;
    }
}