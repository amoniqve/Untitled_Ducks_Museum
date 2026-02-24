using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    
    public void PlayGame()
    {
        SceneManager.LoadScene("MuseumGame"); 
    }

    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit"); 
    }
}