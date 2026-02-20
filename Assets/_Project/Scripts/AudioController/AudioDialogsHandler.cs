using System;
using System.Collections;
using TwelveG.InteractableObjects;
using UnityEngine;

namespace TwelveG.AudioController
{
  public class AudioDialogsHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private AudioClip WTBeepClip;
    [Space(5)]
    [Header("Audio Settings")]
    [SerializeField, Range(0f, 1f)] private float WTBeepVolume = 0.55f;
    [Space(5)]

    [Space(10)]
    [Header("Debug")]
    public AudioSource currentSource;
    public AudioClip currentClip;

    private AudioSource WTSource;
    private AudioSource simonSource;

    public IEnumerator PlayDialogClip(AudioClip dialogClip, float clipVolume, bool isSimon)
    {
      currentSource = isSimon ? simonSource : WTSource;

      if (dialogClip != null)
      {
        currentClip = dialogClip;
        currentSource.volume = clipVolume;
        currentSource.loop = false;
        currentSource.clip = dialogClip;
        currentSource.Play();
        yield return new WaitForSeconds(dialogClip.length);
        currentSource.clip = null;
      }
    }

    // Llamado desde PlayerHandler en Start() 
    // para asignar referencias a WT y Simon.
    public void Initialize(WalkieTalkie walkieTalkieRef)
    {
      // walkieTalkieRef puede ser null por ejemplo en Afternoon Scene, donde el objeto WT está desactivado
      if (walkieTalkieRef != null)
      {
        WTSource = walkieTalkieRef.GetComponent<AudioSource>();
      }

      if(simonSource == null)
      {
        simonSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Dialogs);
      }
    }

    public void StopAllDialogAudio()
    {
      if (WTSource != null && WTSource.isPlaying)
      {
        WTSource.Stop();
        WTSource.clip = null;
      }

      if (simonSource != null && simonSource.isPlaying)
      {
        simonSource.Stop();
        simonSource.clip = null;
      }
    }

    public IEnumerator PlayBeepSound()
    {
      if (WTBeepClip != null && WTSource != null)
      {
        WTSource.volume = WTBeepVolume;
        WTSource.pitch = 1f;
        WTSource.loop = false;
        WTSource.clip = WTBeepClip;
        WTSource.Play();
        yield return new WaitForSeconds(WTBeepClip.length);
        WTSource.clip = null;
      }
    }
  }
}