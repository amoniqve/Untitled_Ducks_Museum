using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity      = 100f;
    public float controllerSensitivity = 200f;
    public Transform playerBody;

    // Axes configured by XboxInputSetup (axis 4 = right stick X, axis 5 = right stick Y)
    private const string RightStickX = "RightStickX"; // horizontal — drives yaw
    private const string RightStickY = "RightStickY"; // vertical   — drives pitch

    private float xRotation = 0f;

    void Update()
    {
        // Mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity      * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity      * Time.deltaTime;

        // Right stick — XboxInputSetup handles inversion per-axis in the Input Manager
        float stickYaw   = Input.GetAxis(RightStickX) * controllerSensitivity * Time.deltaTime;
        float stickPitch = Input.GetAxis(RightStickY) * controllerSensitivity * Time.deltaTime;

        // Vertical look (pitch)
        xRotation -= mouseY + stickPitch;
        xRotation  = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal look (yaw — rotates player body)
        playerBody.Rotate(Vector3.up * (mouseX + stickYaw));
    }
}

