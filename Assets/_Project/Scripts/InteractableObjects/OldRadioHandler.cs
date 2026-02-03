using System;
using System.Collections;
using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.EnvironmentController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
  public class OldRadioHandler : MonoBehaviour, IInteractable
  {
    [Header("References")]
    [SerializeField] private GameObject resonanceZone = null;
    [SerializeField] private MeshRenderer meshRenderer = null;
    [SerializeField, Tooltip("Índice del material del vidrio en el MeshRenderer")]
    private int meterGlassMaterialIndex = 14;

    [Space]
    [Header("Audio settings")]
    [SerializeField] private AudioClip noTuneClip = null;
    [SerializeField] private AudioClip toggleClip = null;
    [SerializeField] private AudioClip randomTunningClip = null;

    [Space]
    [SerializeField, Range(0f, 1f)] private float interactionVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float clipsVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float suddenLoudNoiseVolume = 1f;

    [Header("Texts SO")]
    [Space]
    [SerializeField] private InteractionTextSO interactionTextsSO_turnOn;
    [SerializeField] private InteractionTextSO interactionTextsSO_turnOff;
    [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;

    [Header("Runtime State")]
    public bool isTurnedOn = false;

    // Flags de estado de eventos
    private bool turnedOnByEvent = false;
    private bool turnedOnRandomly = false;
    private bool canBeInteractedWith = true;

    private Material meterGlassMaterial;
    private AudioSource audioSource;
    private AudioSourceState audioSourceState;
    private Coroutine activeCoroutine;
    private bool isTogglingSwitch = false;

    void Awake()
    {
      if (meshRenderer != null && meterGlassMaterialIndex < meshRenderer.materials.Length)
      {
        meterGlassMaterial = meshRenderer.materials[meterGlassMaterialIndex];
      }
      else
      {
        Debug.LogError($"[OldRadioHandler] Material index {meterGlassMaterialIndex} fuera de rango o Renderer nulo.");
      }
    }

    public bool CanBeInteractedWith(PlayerInteraction playerCamera) => canBeInteractedWith;

    public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
    {
      return isTurnedOn ? interactionTextsSO_turnOff : interactionTextsSO_turnOn;
    }

    public bool Interact(PlayerInteraction playerCamera)
    {
      if (activeCoroutine != null) StopCoroutine(activeCoroutine);

      if (PlayerHouseHandler.Instance.HouseHasPower() == false)
      {
        activeCoroutine = StartCoroutine(InteractionClickSoundCoroutine());
        return true;
      }
  
      if (isTurnedOn)
      {
        activeCoroutine = StartCoroutine(TurnOffRoutine());
      }
      else
      {
        activeCoroutine = StartCoroutine(TurnOnRoutine());
      }
      return true;
    }

    private IEnumerator PerformPhysicalToggle()
    {
      isTogglingSwitch = true;
      EnsureAudioSource(interactionVolume);

      if (toggleClip != null)
      {
        audioSource.loop = false;
        audioSource.clip = toggleClip;
        audioSource.volume = interactionVolume;
        audioSource.Play();
        yield return new WaitForSeconds(toggleClip.length * 0.7f); // Pequeña espera para sincronía
      }
      else
      {
        yield return new WaitForSeconds(0.2f);
      }
      isTogglingSwitch = false;
    }

    private IEnumerator InteractionClickSoundCoroutine()
    {
      // Simplemente reproduce el sonido del botón y libera el audio al terminar
      yield return StartCoroutine(PerformPhysicalToggle());
      ReleaseAudio();
    }

    private IEnumerator TurnOnRoutine()
    {
      // Sonido físico del botón (Se espera a que termine el click)
      yield return StartCoroutine(PerformPhysicalToggle());

      // Encendido Visual
      ToggleVisuals(true);
      isTurnedOn = true;
      if (resonanceZone != null) resonanceZone.SetActive(true);

      // Ruido de fondo (Estática/Radio)
      EnsureAudioSource(clipsVolume);

      if (noTuneClip != null)
      {
        audioSource.clip = noTuneClip;
        audioSource.loop = true;
        audioSource.volume = clipsVolume;
        audioSource.Play();
      }
      else
      {
        Debug.LogWarning("[OldRadioHandler] No radio sound clip assigned!");
      }
    }

    private IEnumerator TurnOffRoutine()
    {
      ToggleVisuals(false);
      if (resonanceZone != null) resonanceZone.SetActive(false);
      isTurnedOn = false;

      yield return StartCoroutine(PerformPhysicalToggle());

      ReleaseAudio();

      HandlePostTurnOffEvents();
    }

    private void HandlePostTurnOffEvents()
    {
      if (turnedOnByEvent)
      {
        StartCoroutine(MakeObservation(0, 1.5f));
        turnedOnByEvent = false;
        canBeInteractedWith = false;

        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(SuddenLoudNoiseCoroutine());
      }
      else if (turnedOnRandomly)
      {
        StartCoroutine(MakeObservation(1, 1.5f));
        turnedOnRandomly = false;
        canBeInteractedWith = false;
      }
    }

    private IEnumerator SuddenLoudNoiseCoroutine()
    {
      yield return new WaitForSeconds(4.5f);

      EnsureAudioSource(suddenLoudNoiseVolume);

      ToggleVisuals(true);
      if (resonanceZone != null) resonanceZone.SetActive(true);

      turnedOnRandomly = true;
      isTurnedOn = true;
      canBeInteractedWith = true;

      if (randomTunningClip != null)
      {
        audioSource.PlayOneShot(randomTunningClip);
        yield return new WaitForSeconds(randomTunningClip.length);
      }

      if (audioSource != null)
      {
        audioSource.loop = true;
        audioSource.clip = noTuneClip;
        audioSource.volume = clipsVolume;
        audioSource.Play();
      }
    }

    public void TriggerOldRadioByEvent(Component sender, object data)
    {
      if (data == null || !(data is AudioClip)) return;

      AudioClip clip = (AudioClip)data;
      EnsureAudioSource(clipsVolume);

      ToggleVisuals(true);
      if (resonanceZone != null) resonanceZone.SetActive(true);

      audioSource.clip = clip;
      audioSource.loop = true;
      audioSource.Play();

      canBeInteractedWith = true;
      isTurnedOn = true;
      turnedOnByEvent = true;
    }

    // ----- HELPERS -----

    private void ToggleVisuals(bool on)
    {
      if (meterGlassMaterial != null)
      {
        if (on) meterGlassMaterial.EnableKeyword("_EMISSION");
        else meterGlassMaterial.DisableKeyword("_EMISSION");
      }
    }

    private void EnsureAudioSource(float volume)
    {
      if (audioSource == null)
      {
        (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(transform, volume);
      }

      if (audioSource != null)
      {
        audioSource.volume = volume;
        audioSource.transform.position = transform.position;
      }
    }

    private void ReleaseAudio()
    {
      if (audioSource != null)
      {
        AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
        audioSource = null;
      }
    }

    private IEnumerator MakeObservation(int observationIndex, float timeBeforeObservation)
    {
      yield return new WaitForSeconds(timeBeforeObservation);

      if (observationIndex >= 0 && observationIndex < eventObservationsTextsSOs.Count)
      {
        UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
            eventObservationsTextsSOs[observationIndex]
        );
      }
    }
    public (ObservationTextSO, float timeUntilShown) GetFallBackText() => throw new NotImplementedException();
    public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor) => throw new NotImplementedException();
  }
}