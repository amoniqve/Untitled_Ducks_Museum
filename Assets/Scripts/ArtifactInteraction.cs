using UnityEngine;

public class ArtifactInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 4f;

    private const KeyCode KeyboardInteract    = KeyCode.E;
    private const KeyCode ControllerInteract  = KeyCode.JoystickButton0; // Xbox A

    private Transform player;
    private bool isInRange  = false;
    private bool isPickedUp = false;

    private HUDController Hud => HUDController.Instance;

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Subscribe to device-change events so the prompt updates instantly on switch
        if (InputDeviceTracker.Instance != null)
            InputDeviceTracker.Instance.OnDeviceChanged += OnDeviceChanged;
    }

    private void OnDestroy()
    {
        if (InputDeviceTracker.Instance != null)
            InputDeviceTracker.Instance.OnDeviceChanged -= OnDeviceChanged;
    }

    private void Update()
    {
        if (isPickedUp || player == null) return;

        // Flatten Y so vertical height difference doesn't shrink the effective range
        float distance = Vector3.Distance(
            new Vector3(transform.position.x, player.position.y, transform.position.z),
            player.position);

        bool inRange = distance <= interactRange;

        if (inRange && !isInRange)
        {
            isInRange = true;
        }
        else if (!inRange && isInRange)
        {
            isInRange = false;
            if (Hud != null) Hud.HideInteractionPrompt();
        }

        // Re-read device flag every frame while in range — guarantees the prompt
        // is always correct regardless of when device detection settles
        if (isInRange) UpdatePrompt();

        if (isInRange && (Input.GetKeyDown(KeyboardInteract) || Input.GetKeyDown(ControllerInteract)))
            PickUp();
    }

    /// <summary>Called by InputDeviceTracker when the active device changes — refreshes prompt if in range.</summary>
    private void OnDeviceChanged(bool isController)
    {
        if (isInRange) UpdatePrompt();
    }

    /// <summary>Shows the correct prompt based on the active input device.</summary>
    private void UpdatePrompt()
    {
        if (Hud == null) return;
        bool isController = InputDeviceTracker.Instance != null && InputDeviceTracker.Instance.IsUsingController;
        string key = isController ? "[ A ]" : "[ E ]";
        Hud.ShowInteractionPrompt($"{key}  Pick up artifact");
    }

    /// <summary>Picks up the artifact and triggers the win screen.</summary>
    private void PickUp()
    {
        isPickedUp = true;
        if (Hud != null) Hud.HideInteractionPrompt();
        gameObject.SetActive(false);
        if (UIManager.Instance != null)
            UIManager.Instance.ShowWinScreen();
    }

    private void OnDisable()
    {
        if (Hud != null) Hud.HideInteractionPrompt();
    }
}
