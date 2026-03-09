using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public Light linkedLight;

    void OnTriggerStay(Collider other)
    {
        if (linkedLight == null) return;

        if (other.CompareTag("Player") && linkedLight.enabled)
        {
            ChaseManager manager = FindObjectOfType<ChaseManager>();

            if (manager != null)
                manager.StopChase();
        }
    }
}