using UnityEngine;

public class ArtifactInteraction : MonoBehaviour
{
    private const string KeyboardPrompt   = "[ E ]  Pick up artifact";
    private const string ControllerPrompt = "[ A ]  Pick up artifact";

    [Header("Settings")]
    public float interactRange = 4f;

    private Transform player;
    private bool isInRange  = false;
    private bool isPickedUp = false;

    private HUDController Hud => HUDController.Instance;

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
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
            UpdatePrompt();
        }
        else if (!inRange && isInRange)
        {
            isInRange = false;
            if (Hud != null) Hud.HideInteractionPrompt();
        }

        // Update prompt text if device changed while in range
        if (isInRange)
            UpdatePrompt();

        if (isInRange && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton0)))
            PickUp();
    }

    /// <summary>Refreshes the prompt text based on the active input device.</summary>
    private void UpdatePrompt()
    {
        if (Hud == null) return;
        bool controller = InputDeviceTracker.Instance != null && InputDeviceTracker.Instance.UsingController;
        Hud.ShowInteractionPrompt(controller ? ControllerPrompt : KeyboardPrompt);
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

