using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Tracks whether the player is using a gamepad or mouse+keyboard.
/// Controller input sets the flag immediately.
/// Switching back to keyboard requires sustained mouse/key input for
/// KeyboardFrameGrace consecutive frames after controller axes settle —
/// this prevents right-stick release momentum from flipping the flag back.
/// Subscribe to OnDeviceChanged to react to switches anywhere in the codebase.
/// </summary>
public class InputDeviceTracker : MonoBehaviour
{
    public static InputDeviceTracker Instance { get; private set; }

    /// <summary>True when the last detected input was from a gamepad.</summary>
    public bool IsUsingController { get; private set; }

    /// <summary>Fired whenever the active input device changes. Arg is true = controller.</summary>
    public event Action<bool> OnDeviceChanged;

    [Header("Prompt Override (optional)")]
    [Tooltip("Leave null if prompts are driven by ArtifactInteraction instead.")]
    public TextMeshProUGUI interactionPromptText;

    [Header("Prompt Strings")]
    public string mousePrompt      = "[E] Interact";
    public string controllerPrompt = "[A] Interact";

    // Only unambiguous controller-only axes — not shared with WASD
    // LeftStickX/Y are joystick-only bindings on the same physical axes as Horizontal/Vertical
    private static readonly string[] ControllerAxes =
    {
        "LeftStickX", "LeftStickY", "RightStickX", "RightStickY", "DPadX", "DPadY"
    };

    private const float StickThreshold = 0.2f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        bool wasController    = IsUsingController;
        bool controllerSignal = false;
        bool keyboardSignal   = false;

        // ── Controller signals ────────────────────────────────────────────────

        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown((KeyCode)(KeyCode.JoystickButton0 + i)))
            {
                controllerSignal = true;
                break;
            }
        }

        if (!controllerSignal)
        {
            foreach (string axis in ControllerAxes)
            {
                if (Mathf.Abs(Input.GetAxisRaw(axis)) > StickThreshold)
                {
                    controllerSignal = true;
                    break;
                }
            }
        }

        // ── Keyboard signals — key presses ONLY, never mouse movement ─────────
        // Mouse X/Y deltas are deliberately excluded: many systems report tiny
        // non-zero mouse deltas every frame even when the mouse is untouched,
        // which would constantly fight controller detection.

        if (!controllerSignal && Input.anyKeyDown)
        {
            bool isJoystick = false;
            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown((KeyCode)(KeyCode.JoystickButton0 + i)))
                {
                    isJoystick = true;
                    break;
                }
            }
            if (!isJoystick) keyboardSignal = true;
        }

        // ── State transitions ─────────────────────────────────────────────────

        if (controllerSignal)
        {
            IsUsingController = true;
        }
        else if (keyboardSignal)
        {
            IsUsingController = false;
        }
        // No input at all — hold current device unchanged

        // ── Broadcast change ──────────────────────────────────────────────────

        if (IsUsingController != wasController)
        {
            OnDeviceChanged?.Invoke(IsUsingController);

            if (interactionPromptText != null)
                interactionPromptText.text = IsUsingController ? controllerPrompt : mousePrompt;
        }
    }
}
