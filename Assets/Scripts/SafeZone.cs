using UnityEngine;

public class SafeZone : MonoBehaviour
{
    [Header("Light Settings")]
    public Light linkedLight;

    [Header("Audio Settings")]
    public AudioClip safeZoneSound; 

    void OnTriggerStay(Collider other)
    {
        if (linkedLight == null) return;

        if (other.CompareTag("Player") && linkedLight.enabled)
        {
            ChaseManager manager = FindObjectOfType<ChaseManager>();
            if (manager != null)
                manager.StopChase();

            if (safeZoneSound != null)
                AudioSource.PlayClipAtPoint(safeZoneSound, linkedLight.transform.position);
        }
    }
}