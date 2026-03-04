using UnityEngine;
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
        if (UIManager.Instance != null)
            UIManager.Instance.ShowWinScreen();
    }
}