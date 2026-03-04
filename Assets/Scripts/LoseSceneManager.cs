using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LoseSceneManager : MonoBehaviour
{
    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        Button retry = buttons.FirstOrDefault(b => b.name == "RetryButton");
        Button menu  = buttons.FirstOrDefault(b => b.name == "MainMenuButton");

        if (retry != null) retry.onClick.AddListener(Replay);
        if (menu != null)  menu.onClick.AddListener(GoToMainMenu);
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