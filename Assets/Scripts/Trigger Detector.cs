using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GuardTrigger"))
        {
            Debug.Log("Player detected by guard!");
        }
    }
}