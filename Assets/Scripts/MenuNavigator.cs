using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Enables Xbox controller D-pad / left-stick navigation and A-button confirm
/// on any screen that has selectable UI buttons. Pulses the selected button's
/// text with a custom colour. Attach to any persistent GameObject (e.g. Managers).
/// </summary>
public class MenuNavigator : MonoBehaviour
{
    [Header("Navigation Settings")]
    [Tooltip("Seconds before held input starts repeating.")]
    public float repeatDelay    = 0.4f;
    public float repeatInterval = 0.15f;

    [Header("Selection Highlight")]
    public Color pulseColourA = new Color(0.502f, 0.478f, 0.459f, 1f); // #807A75 dim
    public Color pulseColourB = new Color(1.00f,  0.97f,  0.94f,  1f); // bright
    public float pulseSpeed   = 2.5f;

    // How long after a confirm press to ignore further confirms (prevents A-button bleed-through)
    private const float ConfirmCooldownDuration = 0.25f;
    private float confirmCooldown = 0f;

    // D-pad axes (configured by XboxInputSetup)
    private const string DPadX = "DPadX";
    private const string DPadY = "DPadY";

    // Left stick also navigates menus
    private const string StickY = "Vertical";

    private const KeyCode ConfirmButton = KeyCode.JoystickButton0; // A
    private const KeyCode BackButton    = KeyCode.JoystickButton1; // B

    private const float NavThreshold = 0.5f;

    private float holdTimer   = 0f;
    private float repeatTimer = 0f;
    private bool  isHolding   = false;
    private int   lastNavDir  = 0;

    // Highlight tracking
    private GameObject  lastHighlighted;
    private Color       cachedColour;

    private void Update()
    {
        // Tick confirm cooldown regardless of cursor state
        if (confirmCooldown > 0f) confirmCooldown -= Time.unscaledDeltaTime;

        // Only navigate when a menu is visible (cursor unlocked = menu / pause / game-over)
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // Clear any lingering highlight when returning to gameplay
            ClearHighlight();
            return;
        }

        HandleNavigation();
        HandleConfirm();
        HandleBack();
        ApplyHighlight();
    }

    // ── Navigation ─────────────────────────────────────────────────────────────

    private void HandleNavigation()
    {
        float dpadY  = Input.GetAxisRaw(DPadY);
        float stickY = Input.GetAxisRaw(StickY);

        float rawY  = Mathf.Abs(dpadY) > NavThreshold ? dpadY : stickY;
        float dpadX = Input.GetAxisRaw(DPadX);

        int dir = 0;
        if (rawY >  NavThreshold) dir = -1;
        if (rawY < -NavThreshold) dir =  1;

        int hDir = 0;
        if (dpadX >  NavThreshold) hDir =  1;
        if (dpadX < -NavThreshold) hDir = -1;

        bool anyNav = dir != 0 || hDir != 0;

        if (!anyNav)
        {
            isHolding   = false;
            holdTimer   = 0f;
            repeatTimer = 0f;
            lastNavDir  = 0;
            return;
        }

        if (!isHolding || dir != lastNavDir)
        {
            DispatchNavigation(dir, hDir);
            isHolding   = true;
            lastNavDir  = dir;
            holdTimer   = 0f;
            repeatTimer = 0f;
        }
        else
        {
            holdTimer += Time.unscaledDeltaTime;
            if (holdTimer >= repeatDelay)
            {
                repeatTimer += Time.unscaledDeltaTime;
                if (repeatTimer >= repeatInterval)
                {
                    DispatchNavigation(dir, hDir);
                    repeatTimer = 0f;
                }
            }
        }
    }

    /// <summary>
    /// Routes navigation — horizontal D-pad adjusts a selected Slider's value
    /// instead of moving focus. Vertical always moves between selectables.
    /// </summary>
    private void DispatchNavigation(int vertDir, int horzDir)
    {
        if (horzDir != 0 && AdjustSelectedSlider(horzDir)) return;
        Navigate(vertDir, horzDir);
    }

    /// <summary>
    /// Steps the currently selected Slider with accelerating increments the longer
    /// D-pad is held. Starts at 1 per step, ramps up to 10 at maximum hold.
    /// Returns true when a Slider was adjusted so normal navigation is suppressed.
    /// </summary>
    private bool AdjustSelectedSlider(int horzDir)
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        if (current == null) return false;

        Slider slider = current.GetComponent<Slider>();
        if (slider == null || !slider.interactable) return false;

        // Hold-time acceleration: slow start for fine control, faster for large adjustments
        float step;
        if      (holdTimer < 0.8f) step = 1f;
        else if (holdTimer < 2.0f) step = 2f;
        else if (holdTimer < 4.0f) step = 5f;
        else                       step = 10f;

        slider.value = Mathf.Clamp(slider.value + horzDir * step, slider.minValue, slider.maxValue);
        return true;
    }

    private void Navigate(int vertDir, int horzDir)
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current == null || !current.activeInHierarchy)
        {
            SelectFirst();
            return;
        }

        Selectable sel = current.GetComponent<Selectable>();
        if (sel == null) return;

        Selectable next = null;
        if (vertDir < 0) next = sel.FindSelectableOnUp();
        if (vertDir > 0) next = sel.FindSelectableOnDown();
        if (horzDir > 0) next = sel.FindSelectableOnRight();
        if (horzDir < 0) next = sel.FindSelectableOnLeft();

        if (next == null || !next.gameObject.activeInHierarchy) return;

        // Reject navigation targets outside the current focused screen
        GameObject root = UIManager.Instance?.NavigationRoot;
        if (root != null && !next.transform.IsChildOf(root.transform)) return;

        EventSystem.current.SetSelectedGameObject(next.gameObject);
    }

    private void HandleConfirm()
    {
        if (confirmCooldown > 0f) return;
        if (!Input.GetKeyDown(ConfirmButton)) return;

        GameObject current = EventSystem.current.currentSelectedGameObject;
        if (current == null) { SelectFirst(); return; }

        Button btn = current.GetComponent<Button>();
        if (btn != null && btn.interactable)
        {
            confirmCooldown = ConfirmCooldownDuration;
            // Clear selection so the newly shown screen doesn't inherit this highlight
            EventSystem.current.SetSelectedGameObject(null);
            btn.onClick.Invoke();
        }
    }

    private void HandleBack()
    {
        if (!Input.GetKeyDown(BackButton)) return;
        if (UIManager.Instance == null) return;

        if (UIManager.Instance.InGameControlsActive)
            UIManager.Instance.CloseInGameControls();       // in-game controls → back to pause menu
        else if (UIManager.Instance.PauseMenuActive)
            UIManager.Instance.ResumeGame();                // pause menu → resume game
        else if (UIManager.Instance.MainMenuControlsActive)
            UIManager.Instance.CloseControlsScreen();       // main-menu controls → back to main menu
        // No B action on main menu itself or HUD
    }

    /// <summary>
    /// Selects the first active, interactable selectable within the current navigation
    /// root, ordered by vertical screen position (topmost first). Falls back to any
    /// active selectable if no root is defined.
    /// </summary>
    private void SelectFirst()
    {
        GameObject root = UIManager.Instance?.NavigationRoot;

        Selectable best     = null;
        float      bestTop  = float.NegativeInfinity;

        foreach (Selectable sel in Selectable.allSelectablesArray)
        {
            if (!sel.gameObject.activeInHierarchy || !sel.interactable) continue;
            if (root != null && !sel.transform.IsChildOf(root.transform)) continue;

            // Use the world-space top edge of the RectTransform as the sort key
            RectTransform rt = sel.GetComponent<RectTransform>();
            float top = rt != null ? rt.position.y : sel.transform.position.y;

            if (top > bestTop)
            {
                bestTop = top;
                best    = sel;
            }
        }

        if (best != null)
            EventSystem.current.SetSelectedGameObject(best.gameObject);
    }

    // ── Pulse Highlight ────────────────────────────────────────────────────────

    private void ApplyHighlight()
    {
        GameObject nowSelected = EventSystem.current?.currentSelectedGameObject;

        if (nowSelected != lastHighlighted)
        {
            // Restore old element — skip if it's a Slider (SliderSelectIndicator owns that)
            if (lastHighlighted != null && lastHighlighted.GetComponent<Slider>() == null)
            {
                TextMeshProUGUI old = lastHighlighted.GetComponentInChildren<TextMeshProUGUI>();
                if (old != null) old.color = cachedColour;
                lastHighlighted.transform.localScale = Vector3.one;
            }

            lastHighlighted = nowSelected;

            // Cache colour of the newly selected element — skip Sliders
            if (lastHighlighted != null && lastHighlighted.GetComponent<Slider>() == null)
            {
                TextMeshProUGUI txt = lastHighlighted.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) cachedColour = txt.color;
            }
        }

        if (lastHighlighted == null) return;

        // Sliders handle their own visual feedback via SliderSelectIndicator
        if (lastHighlighted.GetComponent<Slider>() != null) return;

        TextMeshProUGUI label = lastHighlighted.GetComponentInChildren<TextMeshProUGUI>();
        if (label == null) return;

        // Colour pulse between dim #807A75 and bright near-white
        float t = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f;
        label.color = Color.Lerp(pulseColourA, pulseColourB, t);

        // Subtle scale swell
        float scale = 1.0f + 0.06f * t;
        lastHighlighted.transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void ClearHighlight()
    {
        if (lastHighlighted == null) return;

        if (lastHighlighted.GetComponent<Slider>() == null)
        {
            TextMeshProUGUI txt = lastHighlighted.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.color = cachedColour;
            lastHighlighted.transform.localScale = Vector3.one;
        }

        lastHighlighted = null;
    }
}
