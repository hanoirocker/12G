using TwelveG.PlayerController;
using System.Collections;
using UnityEngine;
using TwelveG.Localization;
using TwelveG.AudioController;

namespace TwelveG.InteractableObjects
{
    public class SlideDoorHandler : MonoBehaviour, IInteractable
    {
        [Header("Object Settings: ")]
        [SerializeField] private GameObject door;
        [SerializeField] private bool doorIsOpen;
        [SerializeField] Transform parent;
        [SerializeField, Range(0.5f, 1.5f)] float slidingDuration;

        [Header("Audio Settings: ")]
        [SerializeField] AudioClip openingDoorSound;
        [SerializeField] AudioClip closingDoorSound;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 0.8f;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;

        AudioSource audioSource;
        private bool isMoving;
        private string doorTag;
        private Vector3 initialPosition;

        private void Awake()
        {
            isMoving = false;
            initialPosition = door.transform.localPosition;
        }

        private void Start()
        {
            doorTag = door.tag;
        }

        private void ToggleDoor(Vector3 playerPosition)
        {
            if (!doorIsOpen)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }

        private void OpenDoor()
        {
            float xOffset;
            xOffset = doorIsOpen ? (1.45f) * parent.localScale.x : (-1.45f) * parent.localScale.x;
            Vector3 currentPosition = door.transform.position;
            Vector3 targetPosition = new Vector3(currentPosition.x + xOffset, currentPosition.y, currentPosition.z);
            StartCoroutine(SlideDoorOpen(targetPosition));
        }

        private void CloseDoor()
        {
            StartCoroutine(SlideDoorClose());
        }


        private float PlayDoorSounds()
        {
            audioSource = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);

            if (doorIsOpen && closingDoorSound != null)
            {
                audioSource.PlayOneShot(closingDoorSound);
                return closingDoorSound.length;
            }
            else if (!doorIsOpen && openingDoorSound != null)
            {
                audioSource.PlayOneShot(openingDoorSound);
                return openingDoorSound.length;
            }
            else
            {
                return slidingDuration;
            }
        }

        private IEnumerator SlideDoorOpen(Vector3 targetPosition)
        {
            isMoving = true;
            float duration = PlayDoorSounds();

            float elapsedTime = 0f;
            Vector3 startPosition = door.transform.position;

            while (elapsedTime < duration)
            {
                door.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            door.transform.position = targetPosition;
            doorIsOpen = !doorIsOpen;
            isMoving = false;
        }

        private IEnumerator SlideDoorClose()
        {
            isMoving = true;
            float duration = 1f; // Adjust the duration of the rotation as needed
            float elapsedTime = 0f;
            Vector3 startPosition = door.transform.position;

            while (elapsedTime < duration)
            {
                door.transform.position = Vector3.Lerp(startPosition, initialPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            doorIsOpen = !doorIsOpen;
            isMoving = false;
        }

        public bool Interact(PlayerInteraction interactor)
        {
            ToggleDoor(interactor.transform.position);
            return true;
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return isMoving;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return doorIsOpen ? interactionTextsSO_close : interactionTextsSO_close;
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