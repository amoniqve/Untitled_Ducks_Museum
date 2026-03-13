using UnityEngine;

/// <summary>
/// Pause menu lifecycle. Button wiring is handled entirely by inspector persistent
/// calls — duplicate runtime listeners must NOT be added here to prevent double-firing.
/// ESC / Start controller handling lives in UIManager.Update() for the same reason.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    // Intentionally empty — all button events are wired in the Inspector
    // and all keyboard/controller input is handled in UIManager.Update().
}