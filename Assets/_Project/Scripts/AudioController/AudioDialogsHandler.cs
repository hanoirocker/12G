namespace TwelveG.AudioController
{
  using UnityEngine;
  public class AudioDialogsHandler : MonoBehaviour
  {
    public void PlayDialogClip(AudioClip dialogClip, float charPitch)
    {
      if (dialogClip != null)
      {
        AudioSource dialogSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Dialogs);
        dialogSource.pitch = charPitch;
        dialogSource.PlayOneShot(dialogClip);
      }
    }
  }
}