using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ControlsScreenManager : MonoBehaviour
{
    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        Button back = buttons.FirstOrDefault(b => b.name == "BackButton");

        if (back != null)
            back.onClick.AddListener(() => UIManager.Instance.ShowMainMenu());
    }
}
