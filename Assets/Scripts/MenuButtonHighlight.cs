using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Highlights a menu button's text label when hovered or selected via controller.
/// Uses color #807A75 with a subtle brightness pulse. Resets automatically when
/// the screen is hidden or the button is deselected.
/// </summary>
[RequireComponent(typeof(Button))]
public class MenuButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Highlight Settings")]
    public Color highlightColor = new Color(0.502f, 0.478f, 0.459f, 1f); // #807A75

    [Header("Pulse Settings")]
    public float pulseSpeed     = 2.5f;
    public float pulseIntensity = 0.18f; // +/- brightness swing

    private TextMeshProUGUI label;
    private Color           originalColor;
    private Coroutine       pulseRoutine;

    private void Awake()
    {
        Transform oldGlow = transform.Find("SelectionGlow");
        if (oldGlow != null) Destroy(oldGlow.gameObject);

        label = GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
            originalColor = label.color;
    }

    // Fires when the screen is hidden via SetActive(false) — resets lingering highlight
    private void OnDisable() => SetHighlight(false);

    public void OnSelect(BaseEventData eventData)              => SetHighlight(true);
    public void OnDeselect(BaseEventData eventData)            => SetHighlight(false);
    public void OnPointerEnter(PointerEventData eventData)     => SetHighlight(true);
    public void OnPointerExit(PointerEventData eventData)      => SetHighlight(false);

    private void SetHighlight(bool active)
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        if (label == null) return;

        if (active)
        {
            label.color  = highlightColor;
            pulseRoutine = StartCoroutine(Pulse());
        }
        else
        {
            label.color = originalColor;
        }
    }

    private IEnumerator Pulse()
    {
        float t = 0f;
        while (true)
        {
            t += Time.unscaledDeltaTime * pulseSpeed;
            float b = 1f + Mathf.Sin(t) * pulseIntensity;
            label.color = new Color(
                Mathf.Clamp01(highlightColor.r * b),
                Mathf.Clamp01(highlightColor.g * b),
                Mathf.Clamp01(highlightColor.b * b),
                highlightColor.a);
            yield return null;
        }
    }
}


