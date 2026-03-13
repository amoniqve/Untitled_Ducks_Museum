using UnityEngine;
using TMPro;

/// <summary>
/// Drives the pulsing glow effect on the Back button text in the controls screens,
/// and highlights it with a brighter colour when the controller selects it.
/// Attach to the BackButton GameObject on both ControlsScreen and InGameControlsScreen.
/// </summary>
public class ControlsScreenAnimator : MonoBehaviour
{
    [Header("Text Reference")]
    public TextMeshProUGUI backButtonText;

    [Header("Pulse Settings")]
    public float pulseSpeed     = 1.8f;
    public Color colourDim      = new Color(0.85f, 0.83f, 0.80f, 0.55f);
    public Color colourBright   = new Color(1.00f, 0.97f, 0.88f, 1.00f);

    [Header("Hover / Selected Settings")]
    public Color colourSelected = new Color(1.00f, 0.90f, 0.50f, 1.00f);

    private bool isSelected = false;

    private void Reset()
    {
        backButtonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (backButtonText == null) return;

        if (isSelected)
        {
            backButtonText.color = colourSelected;
        }
        else
        {
            // Sine-wave pulse between dim and bright
            float t = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f;
            backButtonText.color = Color.Lerp(colourDim, colourBright, t);
        }
    }

    /// <summary>Called by MenuNavigator or EventSystem when this button gains focus.</summary>
    public void OnSelect()   => isSelected = true;

    /// <summary>Called when focus leaves this button.</summary>
    public void OnDeselect() => isSelected = false;
}
