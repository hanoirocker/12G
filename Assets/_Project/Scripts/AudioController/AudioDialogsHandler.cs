using System;
using System.Collections;
using TwelveG.GameController;
using TwelveG.InteractableObjects;
using TwelveG.PlayerController;
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

    private void Start()
    {
      GetAudioSources();
    }

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

    private void GetAudioSources()
    {
      SceneEnum sceneEnum = SceneUtils.RetrieveCurrentSceneEnum();

      if (sceneEnum != SceneEnum.Evening && sceneEnum != SceneEnum.Night)
      {
        return;
      }

      WTSource = PlayerHandler.Instance.GetComponentInChildren<WalkieTalkie>().GetComponent<AudioSource>();

      if (WTSource == null)
      {
        Debug.LogError("[AudioDialogsHandler] WalkieTalkie AudioSource not found!");
      }

      simonSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Dialogs);

      if (simonSource == null)
      {
        Debug.LogError("[AudioDialogsHandler] Couldn't find a free AudioSource for Simon!");
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