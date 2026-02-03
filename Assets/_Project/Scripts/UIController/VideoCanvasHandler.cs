using System.Collections;
using TwelveG.AudioController;
using TwelveG.GameController;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
namespace TwelveG.UIController
{
  public class VideoCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;

    public VideoClip videoClip;

    private Canvas videoCanvas;
    private AudioSource videoAudioSource;
    private AudioSourceState videoAudioSourceState;

    private void Awake()
    {
      videoCanvas = GetComponent<Canvas>();
    }

    private IEnumerator VideoSequence(float volume)
    {
      if (videoPlayer.isPrepared)
      {
        videoAudioSource.volume = volume;
        videoCanvas.enabled = true;
        rawImage.enabled = true;
        videoPlayer.Play();

        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        GameEvents.Common.onVideoCanvasFinished.Raise(this, null);

        videoPlayer.Stop();
        videoPlayer.clip = null;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;

        if (videoAudioSource != null)
        {
          AudioUtils.StopAndRestoreAudioSource(videoAudioSource, videoAudioSourceState);
          videoAudioSource = null;
        }

        rawImage.enabled = false;
        videoCanvas.enabled = false;
      }
    }

    public void OnLoadVideo(Component sender, object data)
    {
      videoAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
      videoAudioSourceState = videoAudioSource.GetSnapshot();
      videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
      videoPlayer.SetTargetAudioSource(0, videoAudioSource);

      VideoClip clip = data as VideoClip;

      if (clip == null) { return; }

      if (videoPlayer == null)
      {
        videoPlayer = GetComponentInChildren<VideoPlayer>();
      }

      videoPlayer.clip = clip;
      videoPlayer.Prepare();
    }

    public void OnPlayVideo(Component sender, object data)
    {
      float volume = (float)data;

      StartCoroutine(VideoSequence(volume));
    }
  }
}