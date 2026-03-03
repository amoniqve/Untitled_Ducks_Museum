using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("You Win!");
            StartCoroutine(WinDelay());
        }
    }

    IEnumerator WinDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("WinScene");
    }
}