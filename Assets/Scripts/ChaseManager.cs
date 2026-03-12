using System.Collections;
using UnityEngine;

public class ChaseManager : MonoBehaviour
{
    [Header("Lights")]
    public Light[] hallwayLights;
    public Light[] startRoomLights;
    public Light[] goalRoomLights;

    public float startRoomDelay = 5f;

    [Header("Audio")]
    public AudioClip lightsOutSound;   
    public AudioClip flickerSound;     
    public AudioClip lightOnSound;     
    public AudioClip ghostChaseSound;  

    private bool isChasing = false;

    public void StartChase()
    {
        if (isChasing) return;

        isChasing = true;

        if (ghostChaseSound != null)
        {
            AudioSource.PlayClipAtPoint(ghostChaseSound, Camera.main.transform.position);
        }

        if (lightsOutSound != null)
        {
            AudioSource.PlayClipAtPoint(lightsOutSound, Camera.main.transform.position);
        }

      
        foreach (Light light in hallwayLights)
            light.enabled = false;

        foreach (Light light in goalRoomLights)
            light.enabled = false;


        Invoke("TurnOffStartRoom", startRoomDelay);

        StartCoroutine(HallwayFlicker());
    }

    void TurnOffStartRoom()
    {
        foreach (Light light in startRoomLights)
            light.enabled = false;
    }

    public void StopChase()
    {
        isChasing = false;

        StopAllCoroutines();

  
        foreach (Light light in hallwayLights)
            light.enabled = true;

        foreach (Light light in startRoomLights)
            light.enabled = true;

        foreach (Light light in goalRoomLights)
            light.enabled = true;

  
        if (lightOnSound != null)
        {
            foreach (Light light in startRoomLights)
            {
                AudioSource.PlayClipAtPoint(lightOnSound, light.transform.position);
            }
        }
    }

    IEnumerator HallwayFlicker()
    {
        while (isChasing)
        {
            int randomIndex = Random.Range(0, hallwayLights.Length);

            for (int i = 0; i < hallwayLights.Length; i++)
            {
                bool wasEnabled = hallwayLights[i].enabled;
                hallwayLights[i].enabled = (i == randomIndex);

                if (hallwayLights[i].enabled != wasEnabled && hallwayLights[i].enabled && flickerSound != null)
                {
                    AudioSource.PlayClipAtPoint(flickerSound, hallwayLights[i].transform.position);
                }
            }

            yield return new WaitForSeconds(2f); 
        }
    }
}