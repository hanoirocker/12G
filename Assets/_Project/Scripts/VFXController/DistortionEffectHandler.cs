using System.Collections;
using TwelveG.AudioController;
using UnityEngine;

namespace TwelveG.VFXController
{
  public class DistortionEffectHandler : MonoBehaviour
  {
    [Header("Audio Settings")]
    [SerializeField] private AudioClip distortionClip;
    [SerializeField, Range(0f, 1f)] private float distortionVolume = 1f;

    private PostProcessingHandler postProcessingHandler;

    private AudioSource currentAudioSource = null;
    private AudioSourceState currentAudioState;

    public void Initialize(PostProcessingHandler ppHandler)
    {
      this.postProcessingHandler = ppHandler;
    }

    public IEnumerator DistortionEffectRoutine(float newMultiplier, float fadeDuration, bool hasAudio)
    {
      StartCoroutine(postProcessingHandler.SetRedDistortionWeight(newMultiplier, fadeDuration));

      if (!hasAudio)
      {
        yield return new WaitForSeconds(fadeDuration);
        yield break;
      }

      if (newMultiplier > 0f)
      {
        if (currentAudioSource == null)
        {
          currentAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
          currentAudioState = currentAudioSource.GetSnapshot();

          currentAudioSource.clip = distortionClip;
          currentAudioSource.loop = true;
          currentAudioSource.volume = 0f;
          currentAudioSource.Play();
        }
        yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(currentAudioSource, currentAudioSource.volume, distortionVolume, fadeDuration));
        AudioUtils.StopAndRestoreAudioSource(currentAudioSource, currentAudioState);
        currentAudioSource = null;
      }
      else
      {
        if (currentAudioSource == null)
        {
          currentAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
          currentAudioState = currentAudioSource.GetSnapshot();

          currentAudioSource.clip = distortionClip;
          currentAudioSource.loop = true;
          currentAudioSource.volume = distortionVolume;
          currentAudioSource.Play();
        }

        yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(currentAudioSource, fadeDuration));
        AudioUtils.StopAndRestoreAudioSource(currentAudioSource, currentAudioState);
        currentAudioSource = null;
      }
      if (newMultiplier > 0f)
      {
        yield return new WaitForSeconds(fadeDuration);
      }
    }
  }
}