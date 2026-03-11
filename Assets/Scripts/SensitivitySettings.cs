using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Single unified sensitivity slider. One value scales both mouse and controller
/// sensitivity proportionally. Slider is named "MouseSensSlider" in the scene
/// (the old separate-device ctrl elements are disabled at startup).
/// Settings persist via PlayerPrefs.
/// </summary>
public class SensitivitySettings : MonoBehaviour
{
    public static SensitivitySettings Instance { get; private set; }

    private const string SensKey       = "Sensitivity";
    private const string SliderName    = "MouseSensSlider";
    private const string ValueTextName = "MouseSensValueText";

    private const float SensMin     = 1f;
    private const float SensMax     = 100f;
    private const float SensDefault = 50f;

    // slider × scale = actual sensitivity value fed to MouseLook
    private const float MouseScale = 2f;  // 50 × 2  = 100 (original mouse default)
    private const float CtrlScale  = 3f;  // 50 × 3  = 150 (original controller default)

    private Slider[]          sliders;
    private TextMeshProUGUI[] valueTexts;
    private MouseLook         mouseLook;

    private void Awake() => Instance = this;

    private void Start()
    {
        mouseLook  = FindObjectOfType<MouseLook>();
        sliders    = FindObjectsOfType<Slider>(true).Where(s => s.name == SliderName).ToArray();
        valueTexts = FindObjectsOfType<TextMeshProUGUI>(true).Where(t => t.name == ValueTextName).ToArray();

        // Hide leftover two-slider elements from the previous setup
        DisableBySliderName("CtrlSensSlider");
        DisableByTextName("CtrlSensLabel");
        DisableByTextName("CtrlSensValueText");

        float saved = PlayerPrefs.GetFloat(SensKey, SensDefault);

        foreach (Slider s in sliders)
        {
            s.minValue     = SensMin;
            s.maxValue     = SensMax;
            s.wholeNumbers = true;
            s.SetValueWithoutNotify(saved);
            s.onValueChanged.AddListener(OnSensChanged);
        }

        Apply(saved);
        UpdateValueTexts(saved);
    }

    private void OnSensChanged(float value)
    {
        // Keep both screens' sliders in sync
        foreach (Slider s in sliders)
            if (!Mathf.Approximately(s.value, value)) s.SetValueWithoutNotify(value);

        Apply(value);
        UpdateValueTexts(value);
        PlayerPrefs.SetFloat(SensKey, value);
    }

    /// <summary>Applies the unified sensitivity to both mouse and controller axes.</summary>
    private void Apply(float value)
    {
        if (mouseLook == null) return;
        mouseLook.mouseSensitivity      = value * MouseScale;
        mouseLook.controllerSensitivity = value * CtrlScale;
    }

    private void UpdateValueTexts(float value)
    {
        string display = Mathf.RoundToInt(value).ToString();
        foreach (TextMeshProUGUI t in valueTexts)
            if (t != null) t.text = display;
    }

    private static void DisableBySliderName(string sliderName)
    {
        foreach (Slider s in FindObjectsOfType<Slider>(true))
            if (s.name == sliderName) s.gameObject.SetActive(false);
    }

    private static void DisableByTextName(string textName)
    {
        foreach (TextMeshProUGUI t in FindObjectsOfType<TextMeshProUGUI>(true))
            if (t.name == textName) t.gameObject.SetActive(false);
    }
}
