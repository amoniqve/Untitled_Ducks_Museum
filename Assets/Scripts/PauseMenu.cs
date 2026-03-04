using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject menuUI; 
    private bool isPaused = false;

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        isPaused = !isPaused;

        menuUI.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }
}