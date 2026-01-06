using System.Collections;
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

    private void Awake()
    {
      videoCanvas = GetComponent<Canvas>();
    }

    private IEnumerator VideoSequence()
    {
      if (videoPlayer.isPrepared)
      {
        videoCanvas.enabled = true;
        rawImage.enabled = true;
        videoPlayer.Play();

        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        GameEvents.Common.onVideoCanvasFinished.Raise(this, null);

        videoPlayer.Stop();
        videoPlayer.clip = null;

        rawImage.enabled = false;
        videoCanvas.enabled = false;
      }
    }

    public void OnLoadVideo(Component sender, object data)
    {
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
      StartCoroutine(VideoSequence());
    }
  }
}