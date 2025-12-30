using System;
using TwelveG.PlayerController;
using TwelveG.Localization;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.GameController;

namespace TwelveG.InteractableObjects
{
  public class OldRadioHandler : MonoBehaviour, IInteractable
  {
    [Header("References")]
    [SerializeField] private GameObject resonanceZone = null;
    [SerializeField] private MeshRenderer meshRenderer = null;
    [Space]
    [Header("Audio settings")]
    [SerializeField] private AudioClip noTuneClip = null;
    [SerializeField] private AudioClip toggleClip = null;
    [SerializeField] private AudioClip randomTunningClip = null;
    [SerializeField, Range(0f, 1f)] private float interactionVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float clipsVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float suddenLoudNoiseVolume = 1f;

    [Header("Texts SO")]
    [Space]
    [SerializeField] private InteractionTextSO interactionTextsSO_turnOn;
    [SerializeField] private InteractionTextSO interactionTextsSO_turnOff;
    [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;

    [Header("Testing")]
    [Space]
    public bool isTurnedOn = false;
    public bool turnedOnByEvent = false;
    private bool turnedOnRandomly = false;

    private Coroutine playingCoroutine = null;
    private Material meterGlassMaterial;
    private bool canBeInteractedWith = true;
    private AudioSource audioSource;
    private AudioSourceState audioSourceState;

    void Awake()
    {
      meterGlassMaterial = meshRenderer.materials[14];
    }

    public bool CanBeInteractedWith(PlayerInteraction playerCamera)
    {
      return canBeInteractedWith;
    }

    public (ObservationTextSO, float timeUntilShown) GetFallBackText()
    {
      throw new System.NotImplementedException();
    }

    public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
    {
      return isTurnedOn ? interactionTextsSO_turnOff : interactionTextsSO_turnOn;
    }

    public bool Interact(PlayerInteraction playerCamera)
    {
      StartCoroutine(ToggleOldRadio());
      return true;
    }

    private IEnumerator ToggleOldRadio()
    {
      if (audioSource == null)
      {
        (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, interactionVolume);
      }

      if (!isTurnedOn)
      {
        meterGlassMaterial.EnableKeyword("_EMISSION");
        audioSource.loop = false;
        audioSource.clip = toggleClip;
        audioSource.Play();

        yield return new WaitForSeconds(toggleClip.length * 0.7f);
        isTurnedOn = true;
        resonanceZone.SetActive(true);

        if (noTuneClip != null)
        {
          audioSource.volume = clipsVolume;
          audioSource.clip = noTuneClip;
          audioSource.loop = true;
          audioSource.Play();
        }
        else
        {
          Debug.LogWarning("OldRadioHandler: No radio sound clip assigned!");
        }
      }
      else if (isTurnedOn)
      {
        meterGlassMaterial.DisableKeyword("_EMISSION");
        audioSource.loop = false;
        audioSource.clip = toggleClip;
        audioSource.Play();

        yield return new WaitForSeconds(toggleClip.length);

        AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
        audioSource = null;

        audioSource = null;
        isTurnedOn = false;
        resonanceZone.SetActive(false);

        if (turnedOnByEvent)
        {
          StartCoroutine(MakeObservation(0, 1.5f));
          turnedOnByEvent = false;
          canBeInteractedWith = false;

          // Vuelve a prenderse luego de apagada de golpe y fuerte
          playingCoroutine = StartCoroutine(SuddenLoudNoiseCoroutine());
        }

        if (turnedOnRandomly)
        {
          if(playingCoroutine != null)
          {
            StopCoroutine(playingCoroutine);
            playingCoroutine = null;
          }

          StartCoroutine(MakeObservation(1, 1.5f));
          turnedOnRandomly = false;
          canBeInteractedWith = false;
        }
      }
    }
    private IEnumerator SuddenLoudNoiseCoroutine()
    {
      yield return new WaitForSeconds(4.5f);

      if (audioSource == null)
      {
        (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, suddenLoudNoiseVolume);
      }

      meterGlassMaterial.EnableKeyword("_EMISSION");
      audioSource.PlayOneShot(randomTunningClip);
      canBeInteractedWith = true;
      resonanceZone.SetActive(true);
      turnedOnRandomly = true;
      isTurnedOn = true;

      yield return new WaitForSeconds(randomTunningClip.length);
      audioSource.loop = true;
      audioSource.clip = noTuneClip;
      audioSource.Play();
    }

    public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
    {
      throw new NotImplementedException();
    }

    public void TriggerOldRadioByEvent(Component sender, object data)
    {
      if (data != null)
      {
        if (audioSource == null)
        {
          (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);
        }

        meterGlassMaterial.EnableKeyword("_EMISSION");
        audioSource.clip = (AudioClip)data;
        audioSource.Play();
        canBeInteractedWith = true;
        resonanceZone.SetActive(true);
        isTurnedOn = true;
        turnedOnByEvent = true;
      }
    }

    private IEnumerator MakeObservation(int observationIndex, float timeBeforeObservation)
    {
      yield return new WaitForSeconds(timeBeforeObservation);
      GameEvents.Common.onObservationCanvasShowText.Raise(
        this,
        eventObservationsTextsSOs[observationIndex]
      );
    }
  }
}