using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Handles Xbox controller navigation in menus.
/// Left stick and D-pad move selection. A confirms. B cancels.
/// Navigation is always scoped to UIManager.CurrentNavigationRoot so buttons
/// on screens behind an overlay can never be reached.
/// </summary>
public class MenuNavigator : MonoBehaviour
{
    private const KeyCode ConfirmButton = KeyCode.JoystickButton0; // A
    private const KeyCode CancelButton  = KeyCode.JoystickButton1; // B

    private const float NavigationCooldown = 0.5f;
    private const float StickDeadZone      = 0.6f;
    private const float ConfirmCooldown    = 0.4f;

    private float navTimer     = 0f;
    private float confirmTimer = 0f;
    private bool  wasNeutral   = true;

    private void Update()
    {
        navTimer     -= Time.unscaledDeltaTime;
        confirmTimer -= Time.unscaledDeltaTime;

        // Evict selection if it has drifted outside the current navigation root
        EnforceNavigationScope();

        // --- Navigation ---
        float h = Mathf.Clamp(Input.GetAxis("Horizontal") + Input.GetAxis("DPadX"), -1f, 1f);
        float v = Mathf.Clamp(Input.GetAxis("Vertical")   + Input.GetAxis("DPadY"), -1f, 1f);

        bool isNeutral = Mathf.Abs(h) < StickDeadZone && Mathf.Abs(v) < StickDeadZone;

        if (!isNeutral && (wasNeutral || navTimer <= 0f))
        {
            navTimer   = NavigationCooldown;
            wasNeutral = false;

            if (EventSystem.current.currentSelectedGameObject == null)
                SelectFirstButton();
            else
            {
                AxisEventData axisData = new AxisEventData(EventSystem.current);
                axisData.moveDir    = GetMoveDirection(h, v);
                axisData.moveVector = new Vector2(h, v);
                ExecuteEvents.Execute(
                    EventSystem.current.currentSelectedGameObject,
                    axisData,
                    ExecuteEvents.moveHandler);

                // After move, make sure we haven't drifted outside the root
                EnforceNavigationScope();
            }
        }

        if (isNeutral)
        {
            wasNeutral = true;
            navTimer   = 0f;
        }

        // --- Confirm (A) ---
        if (Input.GetKeyDown(ConfirmButton) && confirmTimer <= 0f)
        {
            confirmTimer = ConfirmCooldown;

            if (EventSystem.current.currentSelectedGameObject == null)
                SelectFirstButton();
            else
            {
                Button selected = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                if (selected != null && selected.interactable)
                    selected.onClick.Invoke();
            }
        }

        // --- Cancel (B) — always goes back, never activates the selected button ---
        if (Input.GetKeyDown(CancelButton) && confirmTimer <= 0f)
        {
            confirmTimer = ConfirmCooldown;
            if (UIManager.Instance != null)
                UIManager.Instance.GoBack();
        }
    }

    /// <summary>
    /// Selects the first interactable button inside the current navigation root.
    /// Falls back to scene-wide search if UIManager is unavailable.
    /// </summary>
    private void SelectFirstButton()
    {
        GameObject root = UIManager.Instance != null
            ? UIManager.Instance.CurrentNavigationRoot
            : null;

        Button[] buttons = root != null
            ? root.GetComponentsInChildren<Button>(false)
            : FindObjectsOfType<Button>(false);

        foreach (Button b in buttons)
        {
            if (b.gameObject.activeInHierarchy && b.interactable)
            {
                EventSystem.current.SetSelectedGameObject(b.gameObject);
                return;
            }
        }
    }

    /// <summary>
    /// If the currently selected object is not a descendant of the active navigation root,
    /// clear the selection and re-select within the root.
    /// </summary>
    private void EnforceNavigationScope()
    {
        if (UIManager.Instance == null || EventSystem.current == null) return;

        GameObject root     = UIManager.Instance.CurrentNavigationRoot;
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (root == null || selected == null) return;

        // Walk up the selected object's hierarchy to check containment
        Transform t = selected.transform;
        while (t != null)
        {
            if (t.gameObject == root) return; // within scope — all good
            t = t.parent;
        }

        // Out of scope — snap back into the active screen
        EventSystem.current.SetSelectedGameObject(null);
        SelectFirstButton();
    }

    private MoveDirection GetMoveDirection(float h, float v)
    {
        if (Mathf.Abs(v) > Mathf.Abs(h))
            return v > 0 ? MoveDirection.Up : MoveDirection.Down;
        return h > 0 ? MoveDirection.Right : MoveDirection.Left;
    }
}





