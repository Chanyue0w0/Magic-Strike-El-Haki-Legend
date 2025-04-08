using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public static VideoPlayerController Instance { get; private set; }

    [Header("影片列表")]
    public VideoClip[] clips; // 可在 Inspector 中掛影片

    private VideoPlayer videoPlayer;
    private int currentClipIndex = 0;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 可選：跨場景保留

        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }

        videoPlayer.playOnAwake = true;
        videoPlayer.isLooping = true;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;

        if (clips != null && clips.Length > 0)
        {
            PlayVideoByIndex(0); // 預設播放第一部
        }
    }

    public void PlayVideoByIndex(int index)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("尚未指定任何影片");
            return;
        }

        if (index < 0 || index >= clips.Length)
        {
            Debug.LogWarning("PlayVideoByIndex: index 超出範圍");
            return;
        }

        currentClipIndex = index;
        PlayVideo(clips[index]);
    }

    public void PlayVideo(VideoClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("PlayVideo: 傳入的 VideoClip 為 null");
            return;
        }

        videoPlayer.Stop();
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    public void PauseVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
    }

    public void ReplayVideo()
    {
        videoPlayer.Stop();
        videoPlayer.Play();
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
    }

    // 下一部影片（循環）
    public void NextVideo()
    {
        if (clips == null || clips.Length == 0) return;

        currentClipIndex = (currentClipIndex + 1) % clips.Length;
        PlayVideo(clips[currentClipIndex]);
    }

    // 上一部影片（循環）
    public void LastVideo()
    {
        if (clips == null || clips.Length == 0) return;

        currentClipIndex = (currentClipIndex - 1 + clips.Length) % clips.Length;
        PlayVideo(clips[currentClipIndex]);
    }
}
