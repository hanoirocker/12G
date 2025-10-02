namespace TwelveG.InteractableObjects
{
  using System;
  using System.Collections.Generic;
  using TwelveG.PlayerController;
  using TwelveG.Localization;
  using UnityEngine;
  using TwelveG.AudioController;
  using System.Collections;

  public class OldRadioHandler : MonoBehaviour, IInteractable
  {
    [Header("Sound settings")]
    [SerializeField] private AudioClip turnOnClip = null;
    [SerializeField] private AudioClip changeDialClip = null;
    [SerializeField] private AudioClip firstClip = null;

    [Header("Texts SO")]
    [SerializeField] private InteractionTextSO interactionTextsSO_turnOn;
    [SerializeField] private InteractionTextSO interactionTextsSO_turnOff;

    [Header("Testing")]
    public bool isTurnedOn;

    private AudioSource audioSource;

    private void Awake()
    {
      audioSource = GetComponent<AudioSource>();
    }

    public bool CanBeInteractedWith(PlayerInteraction playerCamera)
    {
      return true;
    }

    public ObservationTextSO GetFallBackText()
    {
      throw new System.NotImplementedException();
    }

    public InteractionTextSO RetrieveInteractionSO()
    {
      return isTurnedOn ? interactionTextsSO_turnOff : interactionTextsSO_turnOn;
    }

    public bool Interact(PlayerInteraction playerCamera)
    {
      if (isTurnedOn)
      {
        audioSource.Stop();
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
        audioSource.PlayOneShot((AudioClip)data);
        isTurnedOn = true;
      }
    }
  }
}