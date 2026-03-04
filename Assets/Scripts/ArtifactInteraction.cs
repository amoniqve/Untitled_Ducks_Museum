using UnityEngine;

public class ArtifactInteraction : MonoBehaviour
{
    private const string PromptText = "Press [E] to pick up";

    [Header("Settings")]
    public float interactRange = 2.5f;

    private Transform player;
    private bool isInRange = false;
    private bool isPickedUp = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private HUDController Hud => HUDController.Instance;

    private void Update()
    {
        if (isPickedUp || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
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
