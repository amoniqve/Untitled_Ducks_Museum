using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

    private bool isPaused   = false;
    private bool controlsOpenedFromGame = false;
    private static bool restartToGame = false;
    private static bool gameStarted   = false;

    /// <summary>
    /// True once the player wins or loses. Guards and other gameplay systems
    /// poll this to stop acting after the game has ended.
    /// </summary>
    public bool IsGameFinished { get; private set; } = false;

    private const string BackToMainMenuText = "> BACK TO MAIN MENU";
    private const string BackToGameText     = "> BACK TO GAME";

    private TextMeshProUGUI controlsBackButtonText;

    private void Awake()
    {
        Instance = this;
        IsGameFinished = false;
    }

    private void Start()
    {
        // Wire all screen buttons in code so inspector target=null never silently breaks them
        WireButtonListeners();

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

    /// <summary>
    /// Adds runtime onClick listeners ONLY for Win and GameOver screen buttons,
    /// which are the only buttons confirmed to have genuinely broken inspector targets.
    /// Every other screen (main menu, pause, controls) has valid inspector connections
    /// and must NOT receive extra listeners — that would cause double StartGame() calls.
    /// </summary>
    private void WireButtonListeners()
    {
        AddListener(winScreen,      "Panel/PlayAgainButton", RestartGame);
        AddListener(winScreen,      "Panel/MainMenuButton",  ShowMainMenu);
        AddListener(gameOverScreen, "Panel/RetryButton",     RestartGame);
        AddListener(gameOverScreen, "Panel/MainMenuButton",  ShowMainMenu);
    }

    /// <summary>Finds a Button at relPath inside parent and adds a runtime listener.</summary>
    private static void AddListener(GameObject parent, string relPath, UnityEngine.Events.UnityAction action)
    {
        if (parent == null) return;
        Transform t = parent.transform.Find(relPath);
        if (t == null) return;
        Button b = t.GetComponent<Button>();
        if (b == null) return;
        // Remove the listener first to prevent duplicates if Awake fires twice (DontDestroyOnLoad edge case)
        b.onClick.RemoveListener(action);
        b.onClick.AddListener(action);
    }

    private void Update()
    {
        bool pausePressed = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7);

        if (pausePressed)
        {
            if (inGameControlsScreen != null && inGameControlsScreen.activeSelf)
            {
                ShowHUD();
                return;
            }

            if (controlsScreen != null && controlsScreen.activeSelf)
            {
                CloseControlsScreen();
                return;
            }

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
        SelectFirstButtonIn(mainMenuScreen);
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
        // Clear selection so controller focus leaves menus
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
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
            SelectFirstButtonIn(pauseMenuScreen);
        }
        else
        {
            if (pauseMenuScreen != null) pauseMenuScreen.SetActive(false);
            if (controlsScreen != null) controlsScreen.SetActive(false);
            if (hudScreen != null) hudScreen.SetActive(true);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    /// <summary>Shows the game over screen.</summary>
    public void ShowGameOver()
    {
        if (IsGameFinished) return; // never overwrite win screen
        IsGameFinished = true;
        HideAllScreens();
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOverMusic();
        SelectFirstButtonIn(gameOverScreen);
    }

    /// <summary>Shows the win screen.</summary>
    public void ShowWinScreen()
    {
        IsGameFinished = true;
        HideAllScreens();
        if (winScreen != null) winScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayVictoryMusic();
        SelectFirstButtonIn(winScreen);
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
        SelectFirstButtonIn(controlsScreen);
    }

    /// <summary>Opens controls from in-game — pauses and resumes directly on close.</summary>
    public void ShowControlsFromGame()
    {
        controlsOpenedFromGame = true;  // ensure B/back returns to game, not main menu
        if (hudScreen            != null) hudScreen.SetActive(false);
        if (pauseMenuScreen      != null) pauseMenuScreen.SetActive(false);
        if (inGameControlsScreen != null) inGameControlsScreen.SetActive(true);
        Time.timeScale   = 0f;
        isPaused         = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        SelectFirstButtonIn(inGameControlsScreen);
    }

    /// <summary>Closes the controls screen and returns to game or main menu.</summary>
    public void CloseControlsScreen()
    {
        bool fromGame = controlsOpenedFromGame;
        controlsOpenedFromGame = false;
        if (fromGame)
            ShowHUD();
        else
            ShowMainMenu();
    }

    /// <summary>
    /// Returns the deepest active menu screen so MenuNavigator never navigates
    /// to buttons on screens hidden behind the currently visible one.
    /// Priority: in-game controls > controls > pause > win > game over > main menu.
    /// </summary>
    public GameObject CurrentNavigationRoot
    {
        get
        {
            if (inGameControlsScreen != null && inGameControlsScreen.activeSelf) return inGameControlsScreen;
            if (controlsScreen       != null && controlsScreen.activeSelf)       return controlsScreen;
            if (pauseMenuScreen      != null && pauseMenuScreen.activeSelf)       return pauseMenuScreen;
            if (winScreen            != null && winScreen.activeSelf)             return winScreen;
            if (gameOverScreen       != null && gameOverScreen.activeSelf)        return gameOverScreen;
            if (mainMenuScreen       != null && mainMenuScreen.activeSelf)        return mainMenuScreen;
            return null;
        }
    }

    /// <summary>
    /// Called by MenuNavigator B button — navigates back from whatever is currently open.
    /// </summary>
    public void GoBack()
    {
        if (inGameControlsScreen != null && inGameControlsScreen.activeSelf)
        {
            // Return to pause menu from in-game controls
            inGameControlsScreen.SetActive(false);
            if (pauseMenuScreen != null) pauseMenuScreen.SetActive(true);
            SelectFirstButtonIn(pauseMenuScreen);
            return;
        }

        if (controlsScreen != null && controlsScreen.activeSelf)
        {
            CloseControlsScreen();
            return;
        }

        if (pauseMenuScreen != null && pauseMenuScreen.activeSelf)
        {
            TogglePause(); // unpause
            return;
        }
        // Main menu / win / game over — B does nothing
    }

    /// <summary>Starts the game from the main menu.</summary>
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

    /// <summary>Selects the first interactable button inside a screen for controller navigation.</summary>
    private void SelectFirstButtonIn(GameObject screen)
    {
        if (screen == null || EventSystem.current == null) return;
        Button[] buttons = screen.GetComponentsInChildren<Button>(false);
        foreach (Button b in buttons)
        {
            if (b.gameObject.activeInHierarchy && b.interactable)
            {
                EventSystem.current.SetSelectedGameObject(b.gameObject);
                return;
            }
        }
    }

    private void HideAllScreens()
    {
        if (mainMenuScreen       != null) mainMenuScreen.SetActive(false);
        if (pauseMenuScreen      != null) pauseMenuScreen.SetActive(false);
        if (controlsScreen       != null) controlsScreen.SetActive(false);
        if (inGameControlsScreen != null) inGameControlsScreen.SetActive(false);
        if (gameOverScreen       != null) gameOverScreen.SetActive(false);
        if (winScreen            != null) winScreen.SetActive(false);
        if (hudScreen            != null) hudScreen.SetActive(false);
    }
}

