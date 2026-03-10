using UnityEngine;

public class GhostWakeTrigger : MonoBehaviour
{
    public GuardAI guard;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            guard.WakeGhost();
            gameObject.SetActive(false); 
        }
    }
}