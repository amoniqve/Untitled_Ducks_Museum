using System.Collections;
using UnityEngine;

public class ChaseManager : MonoBehaviour
{
    public Light[] hallwayLights;
    public Light[] startRoomLights;
    public Light[] goalRoomLights;

    public float startRoomDelay = 5f;

    private bool isChasing = false;

    public void StartChase()
    {
        if (isChasing) return;

        isChasing = true;

        // turnsoff hallway floor and goal room lights 
        foreach (Light light in hallwayLights)
            light.enabled = false;

        foreach (Light light in goalRoomLights)
            light.enabled = false;

        // start room turns off after delay
        Invoke("TurnOffStartRoom", startRoomDelay);

        // start hallway flicker (slow cuz prof mentioned seizures)
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

        // turns everything back on
        foreach (Light light in hallwayLights)
            light.enabled = true;

        foreach (Light light in startRoomLights)
            light.enabled = true;

        foreach (Light light in goalRoomLights)
            light.enabled = true;
    }

    IEnumerator HallwayFlicker()
    {
        while (isChasing)
        {
            int randomIndex = Random.Range(0, hallwayLights.Length);

            for (int i = 0; i < hallwayLights.Length; i++)
                hallwayLights[i].enabled = (i == randomIndex);

            yield return new WaitForSeconds(2f);
        }
    }
}