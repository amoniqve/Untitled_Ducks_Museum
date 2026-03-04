using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Button References")]
    public Button playButton;
    public Button controlsButton;
    public Button quitButton;

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);

        if (controlsButton != null)
            controlsButton.onClick.AddListener(ShowControls);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
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