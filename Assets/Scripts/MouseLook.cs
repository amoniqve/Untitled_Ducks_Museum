using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Sensitivity")]
    public float mouseSensitivity      = 100f;
    public float controllerSensitivity = 150f;
    public Transform playerBody;

    [Header("Stick Feel — tune in Play Mode")]
    [Tooltip("Stick must exceed this before any output is produced. Eliminates drift.")]
    [SerializeField] [Range(0f, 0.4f)]  private float stickDeadzone    = 0.10f;

    [Tooltip("1 = linear. Higher values compress slow speeds and expand fast ones. Try 1.2–1.5.")]
    [SerializeField] [Range(1f, 3f)]    private float stickCurvePower  = 1.3f;

    [Tooltip("0 = no smoothing (instant). Higher values ease in/out more. Try 0.05–0.15.")]
    [SerializeField] [Range(0f, 0.4f)]  private float stickSmoothing   = 0.07f;

    private const string RightStickX = "RightStickX";
    private const string RightStickY = "RightStickY";

    private float xRotation   = 0f;
    private float smoothYaw   = 0f;
    private float smoothPitch = 0f;

    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // ── Mouse ─────────────────────────────────────────────────────────────
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ── Right stick ───────────────────────────────────────────────────────
        float rawYaw   = Stick(RightStickX);
        float rawPitch = Stick(RightStickY);

        // Non-linear curve: preserves sign, compresses low end, opens up high end.
        float curvedYaw   = Mathf.Sign(rawYaw)   * Mathf.Pow(Mathf.Abs(rawYaw),   stickCurvePower);
        float curvedPitch = Mathf.Sign(rawPitch)  * Mathf.Pow(Mathf.Abs(rawPitch), stickCurvePower);

        // Light lerp to smooth snap-back on release without adding perceptible lag.
        float lerpT     = 1f - stickSmoothing;
        smoothYaw       = Mathf.Lerp(smoothYaw,   curvedYaw,   lerpT);
        smoothPitch     = Mathf.Lerp(smoothPitch, curvedPitch, lerpT);

        float stickYaw   = smoothYaw   * controllerSensitivity * Time.deltaTime;
        float stickPitch = smoothPitch * controllerSensitivity * Time.deltaTime;

        // ── Apply rotation ────────────────────────────────────────────────────
        xRotation -= mouseY + stickPitch;
        xRotation  = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * (mouseX + stickYaw));
    }

    /// <summary>
    /// Reads the raw hardware axis value (no Unity smoothing), applies a software
    /// deadzone, then remaps the live range [deadzone, 1] to [0, 1] so there is no
    /// output jump at the deadzone boundary.
    /// </summary>
    private float Stick(string axis)
    {
        float v = Input.GetAxisRaw(axis);
        float abs = Mathf.Abs(v);
        if (abs < stickDeadzone) return 0f;
        return Mathf.Sign(v) * (abs - stickDeadzone) / (1f - stickDeadzone);
    }
}
