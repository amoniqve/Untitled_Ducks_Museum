using UnityEngine;
using UnityEngine.UI;

public class LoseSceneManager : MonoBehaviour
{
    [Header("Button References")]
    public Button defaultButton;
    public Button mainMenuButton;

    private void Start()
    {
        if (defaultButton != null)
            defaultButton.onClick.AddListener(Replay);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
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
