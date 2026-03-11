using UnityEngine;

public class GuardTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (UIManager.Instance != null && UIManager.Instance.IsGameFinished) return;

        Debug.Log("You have been caught!");
        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOver();
    }
}