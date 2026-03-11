using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    /// <summary>True after the game ends (win or game-over). Used to block late trigger callbacks.</summary>
    public static bool IsGameFinished { get; private set; }

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

    /// <summary>True while the in-game controls screen is open. Used by MenuNavigator for context-aware B-button handling.</summary>
    public bool InGameControlsActive => inGameControlsScreen != null && inGameControlsScreen.activeSelf;

    /// <summary>True while the pause menu is open.</summary>
    public bool PauseMenuActive => pauseMenuScreen != null && pauseMenuScreen.activeSelf;

    /// <summary>True while the main-menu controls screen is open.</summary>
    public bool MainMenuControlsActive => controlsScreen != null && controlsScreen.activeSelf;

    /// <summary>
    /// The screen that should exclusively receive controller navigation focus.
    /// MenuNavigator uses this to restrict SelectFirst() and Navigate() to only
    /// the selectables inside the focused screen.
    /// </summary>
    public GameObject NavigationRoot
    {
        get
        {
            if (InGameControlsActive)   return inGameControlsScreen;
            if (MainMenuControlsActive) return controlsScreen;
            if (PauseMenuActive)        return pauseMenuScreen;
            if (mainMenuScreen != null && mainMenuScreen.activeSelf) return mainMenuScreen;
            return null;
        }
    }

    private void Awake()
    {
        Instance = this;
        IsGameFinished = false;
    }

    private void Start()
    {
        // Wire only Win/GameOver buttons programmatically. Other buttons have working Inspector connections.
        WireButtonListeners();

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
        bool pausePressed = Input.GetKeyDown(KeyCode.Escape)
                         || Input.GetKeyDown(KeyCode.JoystickButton7); // Xbox Start / Menu

        if (pausePressed)
        {
            // In-game controls screen — go back to pause menu
            if (inGameControlsScreen != null && inGameControlsScreen.activeSelf)
            {
                CloseInGameControls();
                return;
            }

            // Main-menu controls screen — return to main menu
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
        EventSystem.current?.SetSelectedGameObject(null);
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
            if (pauseMenuScreen      != null) pauseMenuScreen.SetActive(true);
            if (hudScreen            != null) hudScreen.SetActive(false);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            ResumeGame();
        }
    }

    /// <summary>Resumes gameplay from any paused state (pause menu, in-game controls, etc.).</summary>
    public void ResumeGame()
    {
        if (pauseMenuScreen      != null) pauseMenuScreen.SetActive(false);
        if (inGameControlsScreen != null) inGameControlsScreen.SetActive(false);
        if (controlsScreen       != null) controlsScreen.SetActive(false);
        if (hudScreen            != null) hudScreen.SetActive(true);
        isPaused         = false;
        Time.timeScale   = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    /// <summary>Shows the game over screen.</summary>
    public void ShowGameOver()
    {
        if (IsGameFinished) return;
        IsGameFinished = true;
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
        if (IsGameFinished) return;
        IsGameFinished = true;
        HideAllScreens();
        if (winScreen != null) winScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayVictoryMusic();
    }

    /// <summary>Opens controls from the main menu — keeps main menu active so video background stays visible.
    /// Navigation is scoped to controlsScreen only via UIManager.NavigationRoot.</summary>
    public void ShowControlsScreen()
    {
        controlsOpenedFromGame = false;
        // Do NOT hide mainMenuScreen — video background must remain visible behind controls
        if (controlsScreen != null) controlsScreen.SetActive(true);
        if (controlsBackButtonText != null)
            controlsBackButtonText.text = BackToMainMenuText;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    /// <summary>Opens controls from in-game — pauses and resumes directly on close.</summary>
    public void ShowControlsFromGame()
    {
        controlsOpenedFromGame = true;
        HideAllScreens();                                                // clear every canvas including ControlsScreen
        if (inGameControlsScreen != null) inGameControlsScreen.SetActive(true);
        Time.timeScale   = 0f;
        isPaused         = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    /// <summary>Closes the in-game controls screen and returns to the pause menu.</summary>
    public void CloseInGameControls()
    {
        if (inGameControlsScreen != null) inGameControlsScreen.SetActive(false);
        if (pauseMenuScreen      != null) pauseMenuScreen.SetActive(true);
        EventSystem.current?.SetSelectedGameObject(null);
    }

    /// <summary>Closes the main-menu controls screen and returns to the main menu.</summary>
    public void CloseControlsScreen()
    {
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

    /// <summary>
    /// Wires runtime onClick listeners for buttons whose Inspector targets are null or broken.
    /// ControlsButton calls ShowControlsFromGame() (already correct in Inspector).
    /// ResumeButton has a null Inspector target so we wire it here.
    /// Win/GameOver buttons are also wired here.
    /// </summary>
    private void WireButtonListeners()
    {
        // Pause menu — ResumeButton's Inspector target is null; wire it safely here
        AddListener(pauseMenuScreen, "Panel/ResumeButton",  TogglePause);

        // Win / GameOver screens
        AddListener(winScreen,      "Panel/PlayAgainButton", RestartGame);
        AddListener(winScreen,      "Panel/MainMenuButton",  ShowMainMenu);
        AddListener(gameOverScreen, "Panel/RetryButton",     RestartGame);
        AddListener(gameOverScreen, "Panel/MainMenuButton",  ShowMainMenu);
    }

    /// <summary>Finds a Button at relPath inside parent and safely adds a runtime listener.</summary>
    private static void AddListener(GameObject parent, string relPath, UnityEngine.Events.UnityAction action)
    {
        if (parent == null) return;
        Transform t = parent.transform.Find(relPath);
        if (t == null) return;
        UnityEngine.UI.Button b = t.GetComponent<UnityEngine.UI.Button>();
        if (b == null) return;
        // Remove first to prevent duplicates if Awake fires more than once (DontDestroyOnLoad edge case)
        b.onClick.RemoveListener(action);
        b.onClick.AddListener(action);
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
