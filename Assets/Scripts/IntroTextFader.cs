using UnityEngine;
using UnityEngine.UI; 

public class IntroTextFader : MonoBehaviour
{
    public CanvasGroup canvasGroup; 
    public float fadeDuration = 1.5f; 
    public float displayTime = 3f; 

    void Start()
    {
        StartCoroutine(FadeTextRoutine());
    }

    System.Collections.IEnumerator FadeTextRoutine()
    {
        // fade in
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1;

        // hold
        yield return new WaitForSeconds(displayTime);

        // fade out
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;

        
    }
}