using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        Button play     = buttons.FirstOrDefault(b => b.name == "PlayButton");
        Button controls = buttons.FirstOrDefault(b => b.name == "OptionsButton");
        Button quit     = buttons.FirstOrDefault(b => b.name == "QuitButton");

        if (play != null)     play.onClick.AddListener(PlayGame);
        if (controls != null) controls.onClick.AddListener(ShowControls);
        if (quit != null)     quit.onClick.AddListener(QuitGame);
    }

    /// <summary>Starts the game by switching to the HUD via UIManager.</summary>
    public void PlayGame()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.StartGame();
    }

    /// <summary>Shows the controls screen via UIManager.</summary>
    public void ShowControls()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ShowControlsScreen();
    }

    /// <summary>Quits the application.</summary>
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}