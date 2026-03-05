using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class WinSceneManager : MonoBehaviour
{
    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        Button replay = buttons.FirstOrDefault(b => b.name == "PlayAgainButton");
        Button menu   = buttons.FirstOrDefault(b => b.name == "MainMenuButton");

        if (replay != null) replay.onClick.AddListener(Replay);
        if (menu != null)   menu.onClick.AddListener(GoToMainMenu);
    }

    /// <summary>Restarts the game via UIManager.</summary>
    public void Replay()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.RestartGame();
    }

    /// <summary>Returns to the main menu via UIManager.</summary>
    public void GoToMainMenu()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ShowMainMenu();
    }
}