using System.Collections;
using UnityEngine;

/// <summary>
/// Manages all background music for the game.
/// Attach this to a GameObject in GameScene and assign audio clips via the Inspector.
/// UIManager calls the public methods on state changes.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip mainMenuMusic;      // Main Menu and Game Over Music.mp3
    public AudioClip atmosphereMusic;    // Atmosphere.mp3
    public AudioClip victoryMusic;       // Calm surreal victory music.mp3
    public AudioClip gameOverMusic;      // Game Over music with sound.wav

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    public float fastFadeDuration = 0.5f;

    private AudioSource audioSource;
    private Coroutine crossFadeCoroutine;

    private void Awake()
    {
        Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop   = true;
        audioSource.volume = musicVolume;
    }

    /// <summary>Hard cuts to main menu music.</summary>
    public void PlayMainMenuMusic()  => Play(mainMenuMusic);

    /// <summary>Hard cuts to gameplay atmosphere music.</summary>
    public void PlayAtmosphere()     => Play(atmosphereMusic);

    /// <summary>Fast-fades out atmosphere then plays victory music.</summary>
    public void PlayVictoryMusic()   => CrossFadeTo(victoryMusic);

    /// <summary>Fast-fades out atmosphere then plays game over music.</summary>
    public void PlayGameOverMusic()  => CrossFadeTo(gameOverMusic);

    /// <summary>Stops all music immediately.</summary>
    public void StopMusic()
    {
        if (crossFadeCoroutine != null) StopCoroutine(crossFadeCoroutine);
        audioSource.Stop();
    }

    private void Play(AudioClip clip)
    {
        if (clip == null) return;
        if (crossFadeCoroutine != null) StopCoroutine(crossFadeCoroutine);
        audioSource.volume = musicVolume;
        audioSource.clip   = clip;
        audioSource.Play();
    }

    private void CrossFadeTo(AudioClip clip)
    {
        if (clip == null) return;
        if (crossFadeCoroutine != null) StopCoroutine(crossFadeCoroutine);
        crossFadeCoroutine = StartCoroutine(CrossFadeRoutine(clip));
    }

    private IEnumerator CrossFadeRoutine(AudioClip nextClip)
    {
        // Fast fade out
        float startVolume = audioSource.volume;
        float elapsed     = 0f;

        while (elapsed < fastFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fastFadeDuration);
            yield return null;
        }

        // Hard cut to new clip, fade in
        audioSource.clip   = nextClip;
        audioSource.volume = 0f;
        audioSource.Play();
        elapsed = 0f;

        while (elapsed < fastFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, musicVolume, elapsed / fastFadeDuration);
            yield return null;
        }

        audioSource.volume = musicVolume;
        crossFadeCoroutine = null;
    }
}
