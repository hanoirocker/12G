using System.Collections;
using TwelveG.AudioController;
using TwelveG.EnvironmentController;
using TwelveG.GameController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;
using UnityEngine.Video;

namespace TwelveG.InteractableObjects
{
    public class FridgeHandler : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] private VideoPlayer videoComponent;

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

        [Tooltip("Ajusta cuánto tarda en girar respecto al audio. Valor Negativo = Gira más rápido que el audio.")]
        [SerializeField, Range(-2f, 2f)] private float openingSoundOffset = 0f;
        [SerializeField, Range(-2f, 2f)] private float closingSoundOffset = 0f;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 1f;

        [Header("Jumpscare Settings")]
        [Tooltip("Multiplicador de velocidad para abrir la puerta en el susto (ej: 3x más rápido)")]
        [SerializeField] private float jumpScareSpeedMultiplier = 4.0f;
        [SerializeField] private EventsEnum jumpScareTriggerEvent;

        [Header("Other Settings: ")]
        [SerializeField] private GameObject LightToToggle;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;

        [Header("Testing Settings: ")]
        [SerializeField] private bool doorIsOpen;
        [SerializeField, Range(1f, 3f)] private float fallbackDuration = 0.9f;

        private bool isMoving;
        private Quaternion initialRotation;
        private bool triggerJumpScareOnNextOpen = false;

        private void Start()
        {
            isMoving = false;

            if (rotatesParent && parentObject == null)
            {
                Debug.LogError("[FridgeHandler]: rotatesParent is true but no parentObject is assigned!", this);
                return;
            }
            initialRotation = rotatesParent ? parentObject.transform.localRotation : gameObject.transform.localRotation;
        }

        private void ToggleDoor(Vector3 playerPosition)
        {
            Quaternion targetRotation = doorIsOpen ? initialRotation : initialRotation * Quaternion.Euler(xOffset, yOffset, zOffset);
            StartCoroutine(RotateDrawerDoor(targetRotation, triggerJumpScareOnNextOpen && !doorIsOpen));
        }

        private IEnumerator RotateDrawerDoor(Quaternion targetRotation, bool isJumpscareAction)
        {
            if (!doorIsOpen && LightToToggle != null && PlayerHouseHandler.Instance.HouseHasPower())
            {
                LightToToggle.SetActive(true);
            }

            isMoving = true;

            (float rawClipLength, AudioClip clip, float offsetUsed) = AudioClipData();
            (AudioSource audioSource, AudioSourceState audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
                parentObject != null ? parentObject.transform : gameObject.transform, clipsVolume);

            float realAudioDuration = fallbackDuration;

            if (audioSource != null)
            {
                if (clip != null)
                {
                    float randomPitch = Random.Range(minPitch, maxPitch);
                    audioSource.pitch = randomPitch;
                    audioSource.clip = clip;
                    audioSource.Play();
                    realAudioDuration = clip.length / randomPitch;
                }
            }

            float baseDuration = realAudioDuration + offsetUsed;
            if (baseDuration < 0.1f) baseDuration = 0.1f;

            float rotationDuration = isJumpscareAction ? (baseDuration / jumpScareSpeedMultiplier) : baseDuration;

            if (isJumpscareAction)
            {
                if (videoComponent != null)
                {
                    videoComponent.enabled = true;
                    videoComponent.Play();
                }
                triggerJumpScareOnNextOpen = false;
            }

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

            if (isJumpscareAction && doorIsOpen)
            {
                isMoving = false;

                if (videoComponent != null)
                {
                    float waitTime = (float)videoComponent.clip.length * 0.80f; // Esperamos el 80% del video para cerrar la puerta
                    yield return new WaitForSeconds(waitTime);
                    ToggleDoor(Vector3.zero);
                    yield return new WaitForSeconds((float)videoComponent.clip.length - waitTime);
                    videoComponent.Stop();
                    videoComponent.enabled = false;
                    videoComponent.gameObject.SetActive(false); // Apagamos el plano para que no quede el ultimo frame del video renderizandose como material
                }
            }
            else
            {
                // Finalización normal
                if (!doorIsOpen && LightToToggle != null && PlayerHouseHandler.Instance.HouseHasPower())
                {
                    LightToToggle.SetActive(false);
                }

                isMoving = false;
                if (audioSource != null)
                {
                    float remainingAudioTime = realAudioDuration - rotationDuration;
                    if (remainingAudioTime > 0f && audioSource.isPlaying)
                    {
                        yield return new WaitForSeconds(remainingAudioTime);
                    }
                    AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
                }
            }
        }

        // Método para cargar el susto desde el evento
        public void LoadJumpScareOnEvent(Component sender, object data)
        {
            if (data != null)
            {
                EventContextData contextData = (EventContextData)data;
                EventsEnum eventEnum = contextData.eventEnum;

                if (eventEnum == jumpScareTriggerEvent)
                {
                    if (!doorIsOpen && !isMoving)
                    {
                        triggerJumpScareOnNextOpen = true;
                    }
                }
            }
        }

        // ... Implementación de Interfaces ...
        public bool DoorIsOpen() => doorIsOpen;
        public bool Interact(PlayerInteraction interactor)
        {
            ToggleDoor(interactor.transform.position);
            return true;
        }
        public bool CanBeInteractedWith(PlayerInteraction playerCamera) => !isMoving;
        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera) => doorIsOpen ? interactionTextsSO_close : interactionTextsSO_open;
        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor) => throw new System.NotImplementedException();
        public (ObservationTextSO, float timeUntilShown) GetFallBackText() => throw new System.NotImplementedException();
        private (float, AudioClip, float) AudioClipData()
        {
            if (doorIsOpen && closingDoorSound != null) return (closingDoorSound.length, closingDoorSound, closingSoundOffset);
            else if (!doorIsOpen && openingDoorSound != null) return (openingDoorSound.length, openingDoorSound, openingSoundOffset);
            else return (fallbackDuration, null, 0f);
        }
    }
}