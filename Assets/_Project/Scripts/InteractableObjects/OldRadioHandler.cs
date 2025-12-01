using System;
using TwelveG.PlayerController;
using TwelveG.Localization;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TwelveG.AudioController;

namespace TwelveG.InteractableObjects
{
  public class OldRadioHandler : MonoBehaviour, IInteractable
  {
    [Header("Audio settings")]
    [SerializeField] private AudioClip turnOnClip = null;
    // [SerializeField] private AudioClip changeDialClip = null;
    [SerializeField] private AudioClip firstClip = null;
    [SerializeField, Range(0f, 1f)] private float clipsVolume = 0.5f;

    [Header("Texts SO")]
    [SerializeField] private InteractionTextSO interactionTextsSO_turnOn;
    [SerializeField] private InteractionTextSO interactionTextsSO_turnOff;
    [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;

    [Header("EventsSO references")]
    [SerializeField] private GameEventSO onObservationCanvasShowText;

    [Header("Testing")]
    public bool isTurnedOn;
    public bool turnedOnByEvent = false;

    private AudioSource audioSource;

    public bool CanBeInteractedWith(PlayerInteraction playerCamera)
    {
      return true;
    }

    public ObservationTextSO GetFallBackText()
    {
      throw new System.NotImplementedException();
    }

    public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
    {
      return isTurnedOn ? interactionTextsSO_turnOff : interactionTextsSO_turnOn;
    }

    public bool Interact(PlayerInteraction playerCamera)
    {
      if (isTurnedOn)
      {
        audioSource.Stop();
        StartCoroutine(MakeObservation(1, 2f));
        turnedOnByEvent = false;
        isTurnedOn = false;
        return true;
      }
      else
      {
        StartCoroutine(TurnOnOldRadio());
        isTurnedOn = true;
        return true;
      }
    }

    private IEnumerator TurnOnOldRadio()
    {
      if (turnOnClip != null)
      {
        audioSource = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);
        audioSource.PlayOneShot(turnOnClip);
        yield return new WaitUntil(() => !audioSource.isPlaying);
      }
      if (firstClip != null)
      {
        audioSource.PlayOneShot(firstClip);
      }
    }

    public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
    {
      throw new NotImplementedException();
    }

    public void TriggerOldRadioByEvent(Component sender, object data)
    {
      if (data != null)
      {
        audioSource = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);
        StartCoroutine(MakeObservation(0, 1f));
        audioSource.PlayOneShot((AudioClip)data);
        isTurnedOn = true;
        turnedOnByEvent = true;
      }
    }

    private IEnumerator MakeObservation(int observationIndex, float timeBeforeObservation)
    {
      yield return new WaitForSeconds(timeBeforeObservation);
      onObservationCanvasShowText.Raise(
        this,
        eventObservationsTextsSOs[observationIndex]
      );
    }
  }
}