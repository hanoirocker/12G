namespace TwelveG.AudioController
{
  using UnityEngine;
  public class AudioDialogsHandler : MonoBehaviour
  {
    public void PlayDialogClip(AudioSource audioSource, AudioClip dialogClip, float charPitch, float charVolume)
    {
      if (dialogClip != null)
      {
        audioSource.volume = charVolume;
        audioSource.pitch = charPitch;
        audioSource.PlayOneShot(dialogClip);
      }
    }

    public void PlayBeepSound(AudioSource audioSource, AudioClip dialogClip, float beepVolume)
    {
      if (dialogClip != null && audioSource != null)
      {
        audioSource.volume = beepVolume;
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(dialogClip);
      }
    }
  }
}