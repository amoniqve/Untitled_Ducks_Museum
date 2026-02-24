using UnityEngine;

public class GuardTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("You have been caught!");
        }
    }
}