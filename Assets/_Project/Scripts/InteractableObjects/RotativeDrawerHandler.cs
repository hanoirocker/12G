using System.Collections;
using TwelveG.AudioController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class RotativeDrawerHandler : MonoBehaviour, IInteractable
    {
        [Header("Drawer Settings: ")]
        public bool rotatesParent = false;
        [SerializeField] GameObject parentObject;
        [SerializeField] float xOffset = 0f;
        [SerializeField] float yOffset = 0f;
        [SerializeField] float zOffset = 0f;

        [Header("Audio Settings: ")]
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip closingDoorSound;
        [SerializeField, Range(-2f, 2f)] private float openingSoundOffset = 0f;
        [SerializeField, Range(-2f, 2f)] private float closingSoundOffset = 0f;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 1f;

        [Header("Other Settings: ")]
        [SerializeField] private GameObject consequentObjectsToActivate;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;

        [Header("Testing Settings: ")]
        [SerializeField] private bool doorIsOpen;
        [SerializeField, Range(1f, 3f)] private float rotatingDuration = 0.9f;

        private bool isMoving;
        private Quaternion initialRotation;

        private void Start()
        {
            isMoving = false;

            if (rotatesParent && parentObject == null)
            {
                Debug.LogError("[RotativeDrawerHandler]: rotatesParent is true but no parentObject is assigned!", this);
                return;
            }

            initialRotation = rotatesParent ? parentObject.transform.localRotation : gameObject.transform.localRotation;
        }

        private void ToggleDoor(Vector3 playerPosition)
        {
            Quaternion targetRotation = doorIsOpen ? initialRotation : initialRotation * Quaternion.Euler(xOffset, yOffset, zOffset);
            StartCoroutine(RotateDrowerDoor(targetRotation));
        }


        private IEnumerator RotateDrowerDoor(Quaternion targetRotation)
        {
            if (!doorIsOpen && consequentObjectsToActivate != null)
            {
                consequentObjectsToActivate.SetActive(true);
            }

            isMoving = true;
            float coroutineDuration = PlayDoorSounds();
            float elapsedTime = 0f;

            Transform targetTransform = rotatesParent ? parentObject.transform : gameObject.transform;
            Quaternion startRotation = targetTransform.localRotation;

            while (elapsedTime < coroutineDuration)
            {
                targetTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / coroutineDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            targetTransform.localRotation = targetRotation;
            doorIsOpen = !doorIsOpen;
            isMoving = false;

            if (!doorIsOpen && consequentObjectsToActivate != null)
            {
                consequentObjectsToActivate.SetActive(false);
            }
        }

        private float PlayDoorSounds()
        {
            AudioSource audioSource = AudioUtils.GetAudioSourceForInteractable(gameObject.transform, clipsVolume);

            if (doorIsOpen & closingDoorSound != null)
            {
                audioSource.PlayOneShot(closingDoorSound);
                return closingDoorSound.length + closingSoundOffset;
            }
            else if (!doorIsOpen & openingDoorSound != null)
            {
                audioSource.PlayOneShot(openingDoorSound);
                return openingDoorSound.length + openingSoundOffset;
            }
            else
            {
                return rotatingDuration;
            }
        }

        public bool DoorIsOpen()
        {
            return doorIsOpen;
        }

        public bool Interact(PlayerInteraction interactor)
        {
            ToggleDoor(interactor.transform.position);
            return true;
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            if (isMoving) { return false; }
            else { return true; }
        }

        public InteractionTextSO RetrieveInteractionSO()
        {
            return doorIsOpen ? interactionTextsSO_close : interactionTextsSO_open;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new System.NotImplementedException();
        }

        public ObservationTextSO GetFallBackText()
        {
            throw new System.NotImplementedException();
        }
    }
}