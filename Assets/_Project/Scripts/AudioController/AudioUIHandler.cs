namespace TwelveG.AudioController
{
  using UnityEngine;

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

    private void Start()
    {
      uiAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.UI);
    }

    public void PlayPointerEnterSound()
    {
      uiAudioSource.PlayOneShot(pointerSelectClip, pointerSelectVolume);
    }

    public void PlayPointerClickSound()
    {
      uiAudioSource.PlayOneShot(pointerClickClip, pointerClickVolume);
    }

    public void PlayGameSound()
    {
      // TODO: Asignar placeholder para playGameSound
      Debug.LogWarning("[AudioUIHandler]: Playing playGameSound!");
      // uiAudioSource.PlayOneShot(playGameClip, playGameVolume);
    }

    public void PlayPauseMenuSound()
    {
      if (uiAudioSource == null) { return; }

      if (uiAudioSource)
      {
        if (uiAudioSource.isPlaying) { uiAudioSource.Stop(); }
        uiAudioSource.PlayOneShot(inGameMenuClip, inGameMenuVolume);
      }
    }
  }
}