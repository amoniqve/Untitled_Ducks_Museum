using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to a Slider root to show a pulsing, scaling handle when the slider is
/// focused by controller D-pad navigation or mouse hover. MenuNavigator skips its
/// own highlight logic for any selected Slider so there is no conflict.
/// No Inspector wiring required — the handle Image is found by path at startup.
/// </summary>
public class SliderSelectIndicator : MonoBehaviour,
    ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    private const float PulseSpeed    = 2.5f;
    private const float HandleScaleMax = 1.4f;

    private static readonly Color ColourDim    = new Color(0.502f, 0.478f, 0.459f, 1f);
    private static readonly Color ColourBright = new Color(1.00f,  0.97f,  0.94f,  1f);

    private Image handleImage;
    private Color originalHandleColour;
    private bool  isSelected;

    private void Awake()
    {
        // Handle is always at this relative path in a standard Unity Slider
        Transform handle = transform.Find("Handle Slide Area/Handle");
        if (handle == null) return;

        handleImage = handle.GetComponent<Image>();
        if (handleImage != null) originalHandleColour = handleImage.color;
    }

    private void Update()
    {
        if (!isSelected || handleImage == null) return;

        float t = (Mathf.Sin(Time.unscaledTime * PulseSpeed) + 1f) * 0.5f;
        handleImage.color                = Color.Lerp(ColourDim, ColourBright, t);
        handleImage.transform.localScale = Vector3.one * Mathf.Lerp(1f, HandleScaleMax, t);
    }

    /// <summary>Sets this slider as the EventSystem selected object on mouse hover,
    /// triggering OnSelect so the pulse effect starts immediately.</summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current?.SetSelectedGameObject(gameObject);
    }

    /// <summary>Called by the EventSystem when this slider gains focus.</summary>
    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
    }

    /// <summary>Called by the EventSystem when focus leaves this slider.</summary>
    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        if (handleImage == null) return;
        handleImage.color                = originalHandleColour;
        handleImage.transform.localScale = Vector3.one;
    }
}
