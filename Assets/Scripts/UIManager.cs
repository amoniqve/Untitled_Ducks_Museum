using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Screen References")]
    public GameObject mainMenuScreen;
    public GameObject pauseMenuScreen;
    public GameObject controlsScreen;
    public GameObject inGameControlsScreen;
    public GameObject gameOverScreen;
    public GameObject winScreen;
    public GameObject hudScreen;

    private bool isPaused = false;

    // Tracks where controls were opened from so we can return correctly
    private bool controlsOpenedFromGame = false;

    // Set to true by RestartGame() so Start() skips the main menu on reload
    private static bool restartToGame = false;

    // Tracks whether gameplay has started so returning to menu triggers a fresh reload
    private static bool gameStarted = false;

    private const string BackToMainMenuText = "> BACK TO MAIN MENU";
    private const string BackToGameText     = "> BACK TO GAME";

    private TextMeshProUGUI controlsBackButtonText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Cache the back button TMP text in the controls screen
        if (controlsScreen != null)
        {
            Transform backText = controlsScreen.transform.Find("Panel/BackButton/Text");
            if (backText != null)
                controlsBackButtonText = backText.GetComponent<TextMeshProUGUI>();
        }

        if (restartToGame)
        {
            restartToGame = false;
            gameStarted = true;
            ShowHUD();
        }
        else
        {
            ShowMainMenu();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // In-game controls screen — Escape resumes game directly
            if (inGameControlsScreen != null && inGameControlsScreen.activeSelf)
            {
                ShowHUD();
                return;
            }

            // Main menu controls screen — Escape returns to main menu
            if (controlsScreen != null && controlsScreen.activeSelf)
            {
                CloseControlsScreen();
                return;
            }

            // Main menu with no overlay — do nothing
            if (mainMenuScreen != null && mainMenuScreen.activeSelf) return;

            TogglePause();
        }
    }

    /// <summary>Shows the main menu screen.</summary>
    public void ShowMainMenu()
    {
        HideAllScreens();
        if (mainMenuScreen != null) mainMenuScreen.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayMainMenuMusic();
    }

    /// <summary>Shows the HUD and starts gameplay.</summary>
    public void ShowHUD()
    {
        HideAllScreens();
        if (hudScreen != null) hudScreen.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayAtmosphere();
    }

    /// <summary>Toggles the pause menu.</summary>
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            if (pauseMenuScreen != null) pauseMenuScreen.SetActive(true);
            if (hudScreen != null) hudScreen.SetActive(false);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            if (pauseMenuScreen != null) pauseMenuScreen.SetActive(false);
            if (controlsScreen != null) controlsScreen.SetActive(false);
            if (hudScreen != null) hudScreen.SetActive(true);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>Shows the game over screen.</summary>
    public void ShowGameOver()
    {
        HideAllScreens();
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOverMusic();
    }

    /// <summary>Shows the win screen.</summary>
    public void ShowWinScreen()
    {
        HideAllScreens();
        if (winScreen != null) winScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayVictoryMusic();
    }

    /// <summary>Opens controls from the main menu — no pause, main menu stays visible behind.</summary>
    public void ShowControlsScreen()
    {
        controlsOpenedFromGame = false;
        if (controlsScreen != null) controlsScreen.SetActive(true);
        if (controlsBackButtonText != null)
            controlsBackButtonText.text = BackToMainMenuText;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    /// <summary>Opens controls from in-game — pauses and resumes directly on close.</summary>
    public void ShowControlsFromGame()
    {
        if (hudScreen             != null) hudScreen.SetActive(false);
        if (pauseMenuScreen       != null) pauseMenuScreen.SetActive(false);
        if (inGameControlsScreen  != null) inGameControlsScreen.SetActive(true);
        Time.timeScale   = 0f;
        isPaused         = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    /// <summary>Closes the controls screen and returns to game or main menu.</summary>
    public void CloseControlsScreen()
    {
        if (controlsOpenedFromGame)
            ShowHUD();
        else
            ShowMainMenu();
    }

    /// <summary>Starts the game from the main menu. Reloads the scene if a game was already played.</summary>
    public void StartGame()
    {
        if (gameStarted)
        {
            restartToGame = true;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            gameStarted = true;
            ShowHUD();
        }
    }

    /// <summary>Reloads the scene and resumes gameplay directly.</summary>
    public void RestartGame()
    {
        restartToGame = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>Quits the application.</summary>
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void HideAllScreens()
    {
        if (mainMenuScreen        != null) mainMenuScreen.SetActive(false);
        if (pauseMenuScreen       != null) pauseMenuScreen.SetActive(false);
        if (controlsScreen        != null) controlsScreen.SetActive(false);
        if (inGameControlsScreen  != null) inGameControlsScreen.SetActive(false);
        if (gameOverScreen        != null) gameOverScreen.SetActive(false);
        if (winScreen             != null) winScreen.SetActive(false);
        if (hudScreen             != null) hudScreen.SetActive(false);
    }
}
