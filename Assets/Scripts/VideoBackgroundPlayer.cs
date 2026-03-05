using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(RawImage))]
public class VideoBackgroundPlayer : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoClip videoClip;
    
    [Range(0f, 1f)]
    public float videoOpacity = 0.4f;
    
    public bool loopVideo = true;
    public bool muteAudio = true;
    
    [Header("Performance")]
    public bool playOnAwake = true;

    private VideoPlayer videoPlayer;
    private RawImage rawImage;
    private RenderTexture renderTexture;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = GetComponent<RawImage>();
        
        SetupVideoPlayer();
    }

    private void SetupVideoPlayer()
    {
        if (videoClip == null)
        {
            Debug.LogWarning("VideoBackgroundPlayer: No video clip assigned!");
            return;
        }

        renderTexture = new RenderTexture(1920, 1080, 0);
        renderTexture.Create();

        videoPlayer.clip = videoClip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.isLooping = loopVideo;
        videoPlayer.playOnAwake = playOnAwake;
        
        if (muteAudio)
        {
            videoPlayer.SetDirectAudioMute(0, true);
        }

        rawImage.texture = renderTexture;
        
        Color imageColor = rawImage.color;
        imageColor.a = videoOpacity;
        rawImage.color = imageColor;

        if (playOnAwake)
        {
            videoPlayer.Play();
        }
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }

    public void Play()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }

    public void Pause()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Pause();
        }
    }

    public void Stop()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
    }

    public void SetOpacity(float opacity)
    {
        videoOpacity = Mathf.Clamp01(opacity);
        if (rawImage != null)
        {
            Color imageColor = rawImage.color;
            imageColor.a = videoOpacity;
            rawImage.color = imageColor;
        }
    }
}
