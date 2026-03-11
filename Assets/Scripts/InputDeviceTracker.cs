using UnityEngine;
using TMPro;

/// <summary>
/// Tracks whether the player is using a gamepad or mouse+keyboard and
/// swaps interaction prompt text accordingly each frame.
/// Attach to any persistent GameObject (e.g. Managers).
/// </summary>
public class InputDeviceTracker : MonoBehaviour
{
    public static InputDeviceTracker Instance { get; private set; }

    /// <summary>True when the last detected input was from a gamepad.</summary>
    public bool IsUsingController { get; private set; }

    [Header("Prompt Override (optional)")]
    [Tooltip("Leave null if prompts are driven by ArtifactInteraction instead.")]
    public TextMeshProUGUI interactionPromptText;

    [Header("Prompt Strings")]
    public string mousePrompt      = "[E] Interact";
    public string controllerPrompt = "[A] Interact";

    // Axes/buttons that indicate controller usage
    private static readonly string[] WatchAxes =
    {
        "RightStickX", "RightStickY", "Horizontal", "Vertical"
    };

    private const float StickThreshold = 0.2f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        bool wasController = IsUsingController;

        // Detect controller input
        foreach (string axis in WatchAxes)
        {
            if (Mathf.Abs(Input.GetAxisRaw(axis)) > StickThreshold)
            {
                IsUsingController = true;
                break;
            }
        }

        // Any joystick button press → controller
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown((KeyCode)(KeyCode.JoystickButton0 + i)))
            {
                IsUsingController = true;
                break;
            }
        }

        // Any mouse or keyboard input → mouse+keyboard
        if (Input.anyKeyDown && !IsUsingController)
            IsUsingController = false;

        if (Input.GetAxisRaw("Mouse X") != 0f || Input.GetAxisRaw("Mouse Y") != 0f)
            IsUsingController = false;

        // Update optional prompt label when device changes
        if (IsUsingController != wasController && interactionPromptText != null)
            interactionPromptText.text = IsUsingController ? controllerPrompt : mousePrompt;
    }

    /// <summary>Returns the correct interact prompt string for the current device.</summary>
    public string GetInteractPrompt() => IsUsingController ? controllerPrompt : mousePrompt;
}
