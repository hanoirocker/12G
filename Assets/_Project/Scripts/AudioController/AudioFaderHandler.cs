namespace TwelveG.AudioController
{
  using System.Collections;
  using UnityEngine;
  using UnityEngine.Audio;

  public class AudioFaderHandler : MonoBehaviour
  {
    [SerializeField] private AudioMixer masterMixer;

    public IEnumerator AudioSourceFadeIn(AudioSource source, float from, float to, float duration)
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

    public IEnumerator AudioSourceFadeOut(AudioSource source, float duration)
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

    public void FadeAudioGroup(AudioGroup audioGroup, float from, float to, float duration)
    {
      StartCoroutine(FadeAudioCoroutine(audioGroup.ToString(), from, to, duration));
    }

    public IEnumerator FadeAudioCoroutine(string audioGroup, float from, float to, float duration)
    {
      float elapsed = 0f;

      while (elapsed < duration)
      {
        elapsed += Time.deltaTime;
        float newVolume = Mathf.Lerp(from, to, elapsed / duration);
        masterMixer.SetFloat(audioGroup, Mathf.Log10(newVolume) * 20);
        yield return null;
      }

      masterMixer.SetFloat(audioGroup, Mathf.Log10(to) * 20);
    }
  }
}