using System.Collections;
using TwelveG.AudioController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class SlideDrowerHandler : MonoBehaviour, IInteractable
    {
        [Header("Drawer Settings: ")]
        public bool slidesParent = false;
        [SerializeField] GameObject parentObject;
        [SerializeField] float xOffSet = 0f;
        [SerializeField] float zOffSet = 0f;

        [Header("Audio Settings: ")]
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip closingDoorSound;
        [SerializeField, Range(-2f, 2f)] private float openingSoundOffset = 0f;
        [SerializeField, Range(-2f, 2f)] private float closingSoundOffset = 0f;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 1f;

        [Header("Testing Settings: ")]
        [SerializeField] private bool doorIsOpen;
        [SerializeField, Range(1f, 3f)] private float slidingDuration = 1.5f;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;

        private AudioSourceState audioSourceState;
        private AudioSource audioSource;
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


        private void ToggleDrawer(Vector3 playerPosition)
        {
            Vector3 targetPosition = doorIsOpen ? initialPosition : new Vector3(initialPosition.x + xOffSet, initialPosition.y, initialPosition.z + zOffSet);
            StartCoroutine(SlideDrawerDoor(targetPosition));
        }

        private float PlaySlidingDrawerSounds()
        {
            (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);

            audioSource.pitch = Random.Range(0.9f, 1.2f);
            if (doorIsOpen & closingDoorSound != null)
            {
                audioSource.clip = closingDoorSound;
                audioSource.Play();
                return AudioUtils.CalculateDurationWithPitch(null, audioSource.pitch, closingDoorSound.length + closingSoundOffset);
            }
            else if (!doorIsOpen & openingDoorSound != null)
            {
                audioSource.clip = openingDoorSound;
                audioSource.Play();
                return AudioUtils.CalculateDurationWithPitch(null, audioSource.pitch, openingDoorSound.length + openingSoundOffset);
            }
            else
            {
                return slidingDuration;
            }
        }


        private IEnumerator SlideDrawerDoor(Vector3 targetPosition)
        {
            isMoving = true;
            float duration = PlaySlidingDrawerSounds();
            float elapsedTime = 0f;

            Transform targetTransform = slidesParent ? parentObject.transform : gameObject.transform;
            Vector3 startPosition = targetTransform.localPosition;

            while (elapsedTime < duration)
            {
                targetTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            targetTransform.localPosition = targetPosition;
            doorIsOpen = !doorIsOpen;
            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
            audioSource = null;

            isMoving = false;
        }

        public bool Interact(PlayerInteraction interactor)
        {
            ToggleDrawer(interactor.transform.position);
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
