using UnityEngine;

/// <summary>
/// Tracks whether the player last used keyboard/mouse or an Xbox controller.
/// Poll InputDeviceTracker.UsingController to switch UI prompts dynamically.
/// </summary>
public class InputDeviceTracker : MonoBehaviour
{
    public static InputDeviceTracker Instance { get; private set; }

    public bool UsingController { get; private set; } = false;

    // Axes that indicate controller use when non-zero
    private const string RightStickX = "RightStickX";
    private const string RightStickY = "RightStickY";
    private const float  StickThreshold = 0.2f;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Any keyboard or mouse activity → keyboard mode
        if (Input.anyKey || Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
        {
            // But joystick buttons count as controller, not keyboard
            bool joystickButton = false;
            for (int i = 0; i <= 19; i++)
            {
                if (Input.GetKey((KeyCode)(350 + i))) // JoystickButton0..19
                {
                    joystickButton = true;
                    break;
                }
            }

            if (!joystickButton)
            {
                UsingController = false;
                return;
            }
        }

        // Any stick / joystick button activity → controller mode
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float rx = Input.GetAxis(RightStickX);
        float ry = Input.GetAxis(RightStickY);

        bool stickActive = Mathf.Abs(h)  > StickThreshold
                        || Mathf.Abs(v)  > StickThreshold
                        || Mathf.Abs(rx) > StickThreshold
                        || Mathf.Abs(ry) > StickThreshold;

        bool buttonActive = false;
        for (int i = 0; i <= 19; i++)
        {
            if (Input.GetKey((KeyCode)(350 + i)))
            {
                buttonActive = true;
                break;
            }
        }

        if (stickActive || buttonActive)
            UsingController = true;
    }
}
