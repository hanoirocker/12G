namespace TwelveG.AudioController
{
  using UnityEngine;

  public class AudioUIHandler : MonoBehaviour
  {
    [SerializeField] public AudioClip pointerEnterSound;
    [SerializeField] public AudioClip pointerClickSound;
    [SerializeField] public AudioClip playGameSound;

    private AudioSource uiAudioSource;

    private void Start()
    {
      uiAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.UI);
    }

    public void PlayPointerEnterSound()
    {
      uiAudioSource.PlayOneShot(pointerEnterSound);
    }

    public void PlayPointerClickSound()
    {
      uiAudioSource.PlayOneShot(pointerClickSound);
    }

    public void PlayGameSound()
    {
      // TODO: Asignar placeholder para playGameSound
      Debug.LogWarning("[AudioUIHandler]: Playing playGameSound!");
      // uiAudioSource.PlayOneShot(playGameSound);
    }
  }
}