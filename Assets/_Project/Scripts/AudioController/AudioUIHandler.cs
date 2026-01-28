using System.Collections;
using UnityEngine;

namespace TwelveG.AudioController
{
  public class AudioUIHandler : MonoBehaviour
  {
    [Header("UI Audio Clips")]
    [SerializeField] public AudioClip pointerSelectClip;
    [SerializeField, Range(0f, 1f)] public float pointerSelectVolume = 1f;
    [SerializeField] public AudioClip pointerClickClip;
    [SerializeField, Range(0f, 1f)] public float pointerClickVolume = 1f;
    [SerializeField] public AudioClip playGameClip;
    [SerializeField, Range(0f, 1f)] public float playGameVolume = 1f;

    [Header("Specific Canvas clips")]
    [SerializeField] private AudioClip inGameMenuClip;
    [SerializeField, Range(0f, 1f)] private float inGameMenuVolume = 1f;

    private AudioSource uiAudioSource;
    private Coroutine uiSoundCoroutine;

    private void Start()
    {
      uiAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.UI);
    }

    public void PlayPointerEnterSound()
    {
      uiSoundCoroutine = StartCoroutine(PlayUISound(pointerSelectClip, pointerSelectVolume));
    }

    public void PlayPointerClickSound()
    {
      uiSoundCoroutine = StartCoroutine(PlayUISound(pointerClickClip, pointerClickVolume));
    }

    public void PlayGameSound()
    {
      uiSoundCoroutine = StartCoroutine(PlayUISound(playGameClip, playGameVolume));
    }

    private IEnumerator PlayUISound(AudioClip clip, float volume)
    {
      if (uiSoundCoroutine != null) { StopCoroutine(uiSoundCoroutine); }

      uiAudioSource.PlayOneShot(clip, volume);
      yield return new WaitForSeconds(clip.length);
      AudioManager.Instance.PoolsHandler.ReleaseAudioSource(uiAudioSource);
      uiSoundCoroutine = null;
    }

    public void PlayPauseMenuSound()
    {
      if (uiAudioSource == null) { return; }

      if (uiAudioSource)
      {
        if (uiAudioSource.isPlaying) { uiAudioSource.Stop(); }
        uiSoundCoroutine = StartCoroutine(PlayUISound(inGameMenuClip, inGameMenuVolume));
      }
    }
  }
}