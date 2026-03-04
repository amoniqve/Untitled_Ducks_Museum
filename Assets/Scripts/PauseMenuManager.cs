using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Button References")]
    public Button resumeButton;
    public Button restartButton;
    public Button controlsButton;
    public Button mainMenuButton;

    private void Start()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => UIManager.Instance.TogglePause());

        if (restartButton != null)
            restartButton.onClick.AddListener(() => UIManager.Instance.RestartGame());

        if (controlsButton != null)
            controlsButton.onClick.AddListener(() => UIManager.Instance.ShowControlsScreen());

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(() => UIManager.Instance.ShowMainMenu());
    }
}
