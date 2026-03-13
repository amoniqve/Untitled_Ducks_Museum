using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [Header("References")]
    public DetectionMeter detectionMeter;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI interactionPrompt;
    public RectTransform staminaFill;

    [Header("Objective Settings")]
    public string currentObjective = "Steal the artifact without being caught.";
    public float objectiveFadeInDuration  = 1.5f;
    public float objectiveDisplayDuration = 5f;
    public float objectiveFadeOutDuration = 2f;

    [Header("Stamina Settings")]
    public float maxStamina      = 100f;
    public float currentStamina  = 100f;
    public float staminaDrainRate = 35f;
    public float staminaRegenRate = 12f;

    [Header("Interaction Prompt Settings")]
    public float promptFadeSpeed = 4f;

    private float objectiveTimer = 0f;
    private CanvasGroup objectiveCanvasGroup;
    private CanvasGroup promptCanvasGroup;
    private bool promptVisible = false;
    private bool isSprinting   = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // --- Stamina bar ---
        if (staminaFill == null)
        {
            Transform found = transform.Find("StaminaPanel/BarBackground/BarFill");
            if (found != null)
                staminaFill = found.GetComponent<RectTransform>();
        }

        if (staminaFill != null)
        {
            staminaFill.anchorMin        = new Vector2(0f, staminaFill.anchorMin.y);
            staminaFill.anchorMax        = new Vector2(1f, staminaFill.anchorMax.y);
            staminaFill.sizeDelta        = new Vector2(0f, staminaFill.sizeDelta.y);
            staminaFill.anchoredPosition = Vector2.zero;
        }

        // --- Objective panel — center it on screen and set up CanvasGroup ---
        Transform objectivePanel = transform.Find("ObjectivePanel");
        if (objectivePanel != null)
        {
            objectiveCanvasGroup = objectivePanel.GetComponent<CanvasGroup>();
            if (objectiveCanvasGroup == null)
                objectiveCanvasGroup = objectivePanel.gameObject.AddComponent<CanvasGroup>();
            objectiveCanvasGroup.alpha = 0f;

            RectTransform panelRect = objectivePanel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                panelRect.anchorMin        = new Vector2(0.15f, 0.42f);
                panelRect.anchorMax        = new Vector2(0.85f, 0.62f);
                panelRect.anchoredPosition = Vector2.zero;
                panelRect.sizeDelta        = Vector2.zero;
            }
        }

        if (objectiveText != null)
        {
            objectiveText.text      = currentObjective;
            objectiveText.fontSize  = 40f;
            objectiveText.alignment = TMPro.TextAlignmentOptions.Center;
        }

        // --- Interaction prompt — set up CanvasGroup for fade ---
        Transform promptPanel = transform.Find("InteractionPrompt");
        if (promptPanel != null)
        {
            promptCanvasGroup = promptPanel.GetComponent<CanvasGroup>();
            if (promptCanvasGroup == null)
                promptCanvasGroup = promptPanel.gameObject.AddComponent<CanvasGroup>();
            promptCanvasGroup.alpha = 0f;
        }

        currentStamina = maxStamina;
    }

    private void Update()
    {
        UpdateObjectiveFade();
        UpdateStamina();
        UpdatePromptFade();
    }

    private void UpdateObjectiveFade()
    {
        if (objectiveCanvasGroup == null) return;

        objectiveTimer += Time.deltaTime;

        if (objectiveTimer < objectiveFadeInDuration)
            objectiveCanvasGroup.alpha = objectiveTimer / objectiveFadeInDuration;
        else if (objectiveTimer < objectiveFadeInDuration + objectiveDisplayDuration)
            objectiveCanvasGroup.alpha = 1f;
        else
            objectiveCanvasGroup.alpha = Mathf.Clamp01(1f - (objectiveTimer - objectiveFadeInDuration - objectiveDisplayDuration) / objectiveFadeOutDuration);
    }

    private void UpdateStamina()
    {
        // LeftShift = keyboard sprint; joystick button 4 = Xbox LB
        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton4);

        if (isSprinting && currentStamina > 0f)
            currentStamina -= staminaDrainRate * Time.deltaTime;
        else if (!isSprinting && currentStamina < maxStamina)
            currentStamina += staminaRegenRate * Time.deltaTime;

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (staminaFill != null)
        {
            float ratio = currentStamina / maxStamina;
            staminaFill.anchorMin = new Vector2(0f,   staminaFill.anchorMin.y);
            staminaFill.anchorMax = new Vector2(ratio, staminaFill.anchorMax.y);
        }
    }

    private void UpdatePromptFade()
    {
        if (promptCanvasGroup == null) return;
        float target = promptVisible ? 1f : 0f;
        promptCanvasGroup.alpha = Mathf.MoveTowards(promptCanvasGroup.alpha, target, promptFadeSpeed * Time.deltaTime);
    }

    /// <summary>Fades in the interaction prompt with the given text.</summary>
    public void ShowInteractionPrompt(string text)
    {
        if (interactionPrompt != null)
            interactionPrompt.text = text;
        promptVisible = true;
    }

    /// <summary>Fades out the interaction prompt.</summary>
    public void HideInteractionPrompt()
    {
        promptVisible = false;
    }

    /// <summary>Sets a new objective and restarts the center announcement fade.</summary>
    public void SetObjective(string newObjective)
    {
        currentObjective = newObjective;
        if (objectiveText != null) objectiveText.text = newObjective;
        objectiveTimer = 0f;
        if (objectiveCanvasGroup != null) objectiveCanvasGroup.alpha = 0f;
    }
}
