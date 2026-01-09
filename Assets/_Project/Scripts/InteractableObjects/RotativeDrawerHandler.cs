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
        [SerializeField, Range(0.5f, 1f)] private float minPitch = 0.8f;
        [SerializeField, Range(1f, 2f)] private float maxPitch = 1.5f;

        [Tooltip("Ajusta cuánto tarda en girar respecto al audio. Valor Negativo = Gira más rápido que el audio (Ideal para portazos).")]
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
        [SerializeField, Range(1f, 3f)] private float fallbackDuration = 0.9f;

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
            StartCoroutine(RotateDrawerDoor(targetRotation));
        }


        private IEnumerator RotateDrawerDoor(Quaternion targetRotation)
        {
            // 1. Configuración Inicial
            if (!doorIsOpen && consequentObjectsToActivate != null)
            {
                consequentObjectsToActivate.SetActive(true);
            }

            isMoving = true;

            (float rawClipLength, AudioClip clip, float offsetUsed) = AudioClipData();

            (AudioSource audioSource, AudioSourceState audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
                parentObject != null ? parentObject.transform : gameObject.transform, clipsVolume);

            float randomPitch = Random.Range(minPitch, maxPitch);
            audioSource.pitch = randomPitch;
            audioSource.clip = clip;
            audioSource.Play();

            float realAudioDuration = (clip != null) ? (clip.length / randomPitch) : fallbackDuration;

            float rotationDuration = realAudioDuration + offsetUsed;

            if (rotationDuration < 0.1f) rotationDuration = 0.1f;

            float elapsedTime = 0f;
            Transform targetTransform = rotatesParent ? parentObject.transform : gameObject.transform;
            Quaternion startRotation = targetTransform.localRotation;

            while (elapsedTime < rotationDuration)
            {
                targetTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            targetTransform.localRotation = targetRotation;
            doorIsOpen = !doorIsOpen;

            if (!doorIsOpen && consequentObjectsToActivate != null)
            {
                consequentObjectsToActivate.SetActive(false);
            }

            isMoving = false;

            float remainingAudioTime = realAudioDuration - rotationDuration;

            if (remainingAudioTime > 0f)
            {
                yield return new WaitForSeconds(remainingAudioTime);
            }

            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
        }

        private (float, AudioClip, float) AudioClipData()
        {
            if (doorIsOpen && closingDoorSound != null)
            {
                return (closingDoorSound.length, closingDoorSound, closingSoundOffset);
            }
            else if (!doorIsOpen && openingDoorSound != null)
            {
                return (openingDoorSound.length, openingDoorSound, openingSoundOffset);
            }
            else
            {
                return (fallbackDuration, null, 0f);
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
            // Solo bloqueamos si se está moviendo visualmente.
            return !isMoving;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return doorIsOpen ? interactionTextsSO_close : interactionTextsSO_open;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new System.NotImplementedException();
        }

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            throw new System.NotImplementedException();
        }
    }
}