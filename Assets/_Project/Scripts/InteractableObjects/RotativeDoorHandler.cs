using TwelveG.PlayerController;
using System.Collections;
using UnityEngine;
using TwelveG.Localization;
using TwelveG.AudioController;

namespace TwelveG.InteractableObjects
{
    public class RotativeDoorHandler : MonoBehaviour, IInteractable
    {
        [Header("Object settings: ")]
        [SerializeField] private GameObject door;
        [SerializeField] private bool doorIsOpen;
        [SerializeField] Animation doorPickAnimation;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;

        [Header("Audio settings: ")]
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip closingDoorSound;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 0.7f;

        private bool isMoving;
        private Quaternion initialRotation;

        private void Start()
        {
            isMoving = false;
            initialRotation = door.transform.localRotation;
        }

        private void ToggleDoor()
        {
            Quaternion targetRotation = doorIsOpen ? initialRotation : initialRotation * Quaternion.Euler(0, 90, 0);
            StartCoroutine(RotateDoor(targetRotation));
            if (doorPickAnimation && !doorIsOpen)
            {
                doorPickAnimation.Play();
            }
        }

        private IEnumerator RotateDoor(Quaternion targetRotation)
        {
            isMoving = true;

            AudioClip clip = doorIsOpen ? closingDoorSound : openingDoorSound;
            AudioManager.Instance.PoolsHandler.GetFreeTemporarySourceByType(
                AudioPoolType.Interaction,
                clip.length,
                (source) =>
                {
                    source.transform.position = door.transform.position;
                    source.volume = clipsVolume;
                    source.clip = clip;
                    source.Play();
                }
            );

            float coroutineDuration = clip.length;
            float elapsedTime = 0f;
            Quaternion startRotation = door.transform.localRotation;

            while (elapsedTime < coroutineDuration)
            {
                door.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / coroutineDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            door.transform.localRotation = targetRotation;
            doorIsOpen = !doorIsOpen;
            isMoving = false;
        }

        public bool Interact(PlayerInteraction interactor)
        {
            ToggleDoor();
            return true;
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            if (isMoving) { return false; }
            else { return true; }
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return doorIsOpen ? interactionTextsSO_close : interactionTextsSO_open;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCamera)
        {
            throw new System.NotImplementedException();
        }

        public ObservationTextSO GetFallBackText()
        {
            throw new System.NotImplementedException();
        }
    }
}