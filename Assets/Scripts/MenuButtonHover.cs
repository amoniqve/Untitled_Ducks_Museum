using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to any menu button. On pointer enter, sets the EventSystem selection to
/// this button so MenuNavigator's existing pulse-highlight automatically applies.
/// Mouse and controller hover effects are then handled by a single shared system.
/// </summary>
public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler
{
    /// <summary>Sets this button as the EventSystem's selected object on mouse hover,
    /// triggering the same colour-pulse and scale effect used by controller navigation.</summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current?.SetSelectedGameObject(gameObject);
    }
}
