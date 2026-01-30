using System.Collections;
using TwelveG.AudioController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class SlideDrawerHandler : MonoBehaviour, IInteractable
    {
        [Header("Drawer Settings: ")]
        public bool slidesParent = false;
        [SerializeField] GameObject parentObject;
        [SerializeField] float xOffSet = 0f;
        [SerializeField] float zOffSet = 0f;

        [Header("Audio Settings: ")]
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip closingDoorSound;
        
        [SerializeField, Range(0.9f, 1.2f)] private float minPitch = 0.9f;
        [SerializeField, Range(1.2f, 1.5f)] private float maxPitch = 1.2f;

        [Tooltip("Ajusta cuánto tarda en deslizarse respecto al audio. Valor Negativo = Desliza más rápido que el audio.")]
        [SerializeField, Range(-2f, 2f)] private float openingSoundOffset = 0f;
        [SerializeField, Range(-2f, 2f)] private float closingSoundOffset = 0f;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 1f;

        [Header("Testing Settings: ")]
        [SerializeField] private bool doorIsOpen;
        [SerializeField, Range(1f, 3f)] private float fallbackDuration = 1.5f;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;

        private Vector3 initialPosition = new Vector3();
        private bool isMoving = false;

        private void Start()
        {
            isMoving = false;

            if (slidesParent && parentObject == null)
            {
                Debug.LogError("[SlideDrawerHandler]: slidesParent is true but no parentObject is assigned!", this);
                return;
            }

            Transform targetTransform = slidesParent ? parentObject.transform : transform;
            initialPosition = targetTransform.localPosition;

            if (doorIsOpen)
            {
                initialPosition -= new Vector3(xOffSet, 0, zOffSet);
            }
        }

        private void ToggleDrawer()
        {
            Vector3 targetPosition = doorIsOpen 
                ? initialPosition 
                : new Vector3(initialPosition.x + xOffSet, initialPosition.y, initialPosition.z + zOffSet);
            
            StartCoroutine(SlideDrawerDoor(targetPosition));
        }

        private IEnumerator SlideDrawerDoor(Vector3 targetPosition)
        {
            isMoving = true;

            (float rawClipLength, AudioClip clip, float offsetUsed) = AudioClipData();

            (AudioSource audioSource, AudioSourceState audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
                gameObject.transform, clipsVolume);

            float randomPitch = Random.Range(minPitch, maxPitch);
            audioSource.pitch = randomPitch;
            audioSource.clip = clip;
            audioSource.Play();

            float realAudioDuration = (clip != null) ? (clip.length / randomPitch) : fallbackDuration;

            float slidingDuration = realAudioDuration + offsetUsed;
            
            if (slidingDuration < 0.1f) slidingDuration = 0.1f;

            float elapsedTime = 0f;
            Transform targetTransform = slidesParent ? parentObject.transform : gameObject.transform;
            Vector3 startPosition = targetTransform.localPosition;

            while (elapsedTime < slidingDuration)
            {
                targetTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / slidingDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            targetTransform.localPosition = targetPosition;
            doorIsOpen = !doorIsOpen;

            isMoving = false;

            float remainingAudioTime = realAudioDuration - slidingDuration;

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

        public bool Interact(PlayerInteraction interactor)
        {
            ToggleDrawer();
            return true;
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
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