using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("You have been caught!");
            SceneManager.LoadScene("LoseScene");
        }
    }
}