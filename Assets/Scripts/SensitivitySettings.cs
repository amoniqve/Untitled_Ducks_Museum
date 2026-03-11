using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Single unified sensitivity slider. One value scales both mouse and controller
/// sensitivity proportionally. Settings persist via PlayerPrefs.
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
    private const float MouseScale = 2f;  // 50 × 2 = 100
    private const float CtrlScale  = 3f;  // 50 × 3 = 150

    private Slider[]          sliders;
    private TextMeshProUGUI[] valueTexts;
    private MouseLook         mouseLook;

    private void Awake() => Instance = this;

    private void Start()
    {
        mouseLook  = FindObjectOfType<MouseLook>();
        sliders    = FindObjectsOfType<Slider>(true).Where(s => s.name == SliderName).ToArray();
        valueTexts = FindObjectsOfType<TextMeshProUGUI>(true).Where(t => t.name == ValueTextName).ToArray();

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
}

