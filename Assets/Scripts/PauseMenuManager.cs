using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PauseMenuManager : MonoBehaviour
{
    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        Button resume   = buttons.FirstOrDefault(b => b.name == "ResumeButton");
        Button restart  = buttons.FirstOrDefault(b => b.name == "RestartButton");
        Button controls = buttons.FirstOrDefault(b => b.name == "ControlsButton");
        Button menu     = buttons.FirstOrDefault(b => b.name == "MainMenuButton");

        if (resume != null)   resume.onClick.AddListener(() => UIManager.Instance.TogglePause());
        if (restart != null)  restart.onClick.AddListener(() => UIManager.Instance.RestartGame());
        if (controls != null) controls.onClick.AddListener(() => UIManager.Instance.ShowControlsScreen());
        if (menu != null)     menu.onClick.AddListener(() => UIManager.Instance.ShowMainMenu());
    }
}
