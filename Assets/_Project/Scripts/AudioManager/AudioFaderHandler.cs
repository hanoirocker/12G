namespace TwelveG.AudioController
{
  using System.Collections;
  using UnityEngine;

  public class AudioFaderHandler : MonoBehaviour
  {
    public IEnumerator RunAudioFadeIn(AudioSource source, float from, float to, float duration)
    {
      source.Play();

      float elapsed = 0f;

      while (elapsed < duration)
      {
        source.volume = Mathf.Lerp(from, to, elapsed / duration);
        elapsed += Time.deltaTime;
        yield return null;
      }

      source.volume = to;
    }

    public IEnumerator AudioFadeOutSequence(AudioSource source, float duration)
    {
      if (source == null || !source.isPlaying)
        yield break;

      float elapsed = 0f;
      float startVolume = source.volume;

      while (elapsed < duration)
      {
        elapsed += Time.deltaTime;
        source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
        yield return null;
      }

      source.volume = 0f;
      source.Stop();
    }
  }
}