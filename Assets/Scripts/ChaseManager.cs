using System.Collections;
using UnityEngine;

public class ChaseManager : MonoBehaviour
{
    public Light[] hallwayLights;
    public Light[] startRoomLights;
    public Light[] goalRoomLights;

    public float startRoomDelay = 5f;

    private bool isChasing = false;

    private void OnDestroy()
    {
        // Scene is reloading — stop everything so coroutines don't access destroyed Lights
        StopAllCoroutines();
        CancelInvoke();
    }

    public void StartChase()
    {
        if (isChasing) return;

        isChasing = true;

        SetLights(hallwayLights, false);
        SetLights(goalRoomLights, false);

        Invoke(nameof(TurnOffStartRoom), startRoomDelay);
        StartCoroutine(HallwayFlicker());
    }

    void TurnOffStartRoom() => SetLights(startRoomLights, false);

    public void StopChase()
    {
        isChasing = false;
        StopAllCoroutines();
        CancelInvoke();

        SetLights(hallwayLights, true);
        SetLights(startRoomLights, true);
        SetLights(goalRoomLights, true);
    }

    IEnumerator HallwayFlicker()
    {
        while (isChasing)
        {
            // Bail out if any light has been destroyed (scene reload mid-coroutine)
            if (hallwayLights == null || hallwayLights.Length == 0) yield break;

            int randomIndex = Random.Range(0, hallwayLights.Length);

            for (int i = 0; i < hallwayLights.Length; i++)
            {
                if (hallwayLights[i] == null) yield break; // destroyed Light — stop cleanly
                hallwayLights[i].enabled = (i == randomIndex);
            }

            yield return new WaitForSeconds(2f);
        }
    }

    /// <summary>Sets enabled state on a light array, skipping any destroyed references.</summary>
    private static void SetLights(Light[] lights, bool enabled)
    {
        if (lights == null) return;
        foreach (Light l in lights)
        {
            if (l != null) l.enabled = enabled;
        }
    }
}
