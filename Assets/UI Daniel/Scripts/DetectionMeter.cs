using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetectionMeter : MonoBehaviour
{
    [Header("UI References")]
    public Image fillImage;
    public TextMeshProUGUI detectionLabel;
    public Image alertIcon;

    [Header("Colors")]
    public Color normalColor = new Color(0.42f, 0.43f, 0.46f, 1f);
    public Color partialColor = new Color(0.55f, 0.55f, 0.55f, 1f);
    public Color alertColor = new Color(0.45f, 0.15f, 0.15f, 1f);

    [Header("Detection Settings")]
    [Range(0f, 1f)]
    public float detectionLevel = 0f;
    public float detectionThreshold = 0.8f;

    private bool isAlert = false;
    private float pulseTimer = 0f;

    private void Update()
    {
        UpdateMeter();
        UpdateAlertIcon();
    }

    private void UpdateMeter()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = detectionLevel;

            if (detectionLevel >= 1f)
            {
                fillImage.color = alertColor;
            }
            else if (detectionLevel > detectionThreshold)
            {
                fillImage.color = Color.Lerp(partialColor, alertColor, (detectionLevel - detectionThreshold) / (1f - detectionThreshold));
            }
            else if (detectionLevel > 0.3f)
            {
                fillImage.color = partialColor;
            }
            else
            {
                fillImage.color = normalColor;
            }
        }
    }

    private void UpdateAlertIcon()
    {
        if (alertIcon == null) return;

        bool shouldAlert = detectionLevel >= detectionThreshold;

        if (shouldAlert)
        {
            alertIcon.enabled = true;
            alertIcon.color = detectionLevel >= 1f ? alertColor : partialColor;

            pulseTimer += Time.deltaTime * 3f;
            float pulse = Mathf.Lerp(0.7f, 1f, (Mathf.Sin(pulseTimer) + 1f) * 0.5f);
            alertIcon.transform.localScale = Vector3.one * pulse;
        }
        else
        {
            alertIcon.enabled = false;
            pulseTimer = 0f;
        }
    }

    public void SetDetectionLevel(float level)
    {
        detectionLevel = Mathf.Clamp01(level);
    }

    public void IncreaseDetection(float amount)
    {
        detectionLevel = Mathf.Clamp01(detectionLevel + amount);
    }

    public void DecreaseDetection(float amount)
    {
        detectionLevel = Mathf.Clamp01(detectionLevel - amount);
    }

    public void ResetDetection()
    {
        detectionLevel = 0f;
    }
}
