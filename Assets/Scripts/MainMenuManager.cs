using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene"); 
    }

    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit"); 
    }
}