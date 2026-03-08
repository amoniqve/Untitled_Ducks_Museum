using UnityEngine;

public class ArtifactInteraction : MonoBehaviour
{
    private const string PromptText = "[ E ]  Pick up artifact";

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
            if (Hud != null) Hud.ShowInteractionPrompt(PromptText);
        }
        else if (!inRange && isInRange)
        {
            isInRange = false;
            if (Hud != null) Hud.HideInteractionPrompt();
        }

        if (isInRange && Input.GetKeyDown(KeyCode.E))
            PickUp();
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
