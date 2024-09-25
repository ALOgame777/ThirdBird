using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.Play(); // 비디오 재생 시작

    }
    public void VideoPlay()
    {
        videoPlayer.Play(); // 비디오 재생 시작
    }
    
}
