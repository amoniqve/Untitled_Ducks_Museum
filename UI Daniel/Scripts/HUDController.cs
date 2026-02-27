using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("References")]
    public DetectionMeter detectionMeter;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI interactionPrompt;
    public UnityEngine.UI.Image staminaFill;

    [Header("Objective Settings")]
    public string currentObjective = "Steal the artifact without being caught.";
    public float objectiveFadeInDuration = 2f;
    public float objectiveDisplayDuration = 8f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 15f;

    private float objectiveTimer = 0f;
    private CanvasGroup objectiveCanvasGroup;
    private bool isSprinting = false;

    private void Start()
    {
        if (objectiveText != null)
        {
            objectiveCanvasGroup = objectiveText.GetComponentInParent<CanvasGroup>();
            if (objectiveCanvasGroup == null)
            {
                objectiveCanvasGroup = objectiveText.gameObject.AddComponent<CanvasGroup>();
            }
            objectiveCanvasGroup.alpha = 0f;
            objectiveText.text = currentObjective;
            objectiveTimer = 0f;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }

        currentStamina = maxStamina;
    }

    private void Update()
    {
        UpdateObjectiveFade();
        UpdateStamina();
    }

    private void UpdateObjectiveFade()
    {
        if (objectiveCanvasGroup == null) return;

        objectiveTimer += Time.deltaTime;

        if (objectiveTimer < objectiveFadeInDuration)
        {
            objectiveCanvasGroup.alpha = objectiveTimer / objectiveFadeInDuration;
        }
        else if (objectiveTimer < objectiveFadeInDuration + objectiveDisplayDuration)
        {
            objectiveCanvasGroup.alpha = 1f;
        }
        else
        {
            float fadeOutDuration = 2f;
            float fadeOutProgress = (objectiveTimer - objectiveFadeInDuration - objectiveDisplayDuration) / fadeOutDuration;
            objectiveCanvasGroup.alpha = 1f - fadeOutProgress;
        }
    }

    private void UpdateStamina()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (isSprinting && currentStamina > 0f)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (staminaFill != null)
        {
            staminaFill.fillAmount = currentStamina / maxStamina;
        }
    }

    public void ShowInteractionPrompt(string text)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.text = text;
            interactionPrompt.gameObject.SetActive(true);
        }
    }

    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    public void SetObjective(string newObjective)
    {
        currentObjective = newObjective;
        if (objectiveText != null)
        {
            objectiveText.text = newObjective;
            objectiveTimer = 0f;
            if (objectiveCanvasGroup != null)
            {
                objectiveCanvasGroup.alpha = 0f;
            }
        }
    }
}
