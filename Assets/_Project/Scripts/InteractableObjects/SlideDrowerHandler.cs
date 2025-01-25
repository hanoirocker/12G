namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class SlideDrowerHandler : MonoBehaviour, IInteractable
    {
        [Header("Drower Settings: ")]
        [SerializeField] private Transform parentTransform;
        [SerializeField] float xOffSet = 0f;
        [SerializeField] float zOffSet = 0f;

        [Header("Audio Settings: ")]
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip closingDoorSound;

        [Header("Testing Settings: ")]
        [SerializeField] private bool doorIsOpen;
        [SerializeField, Range(1f, 3f)] private float slidingDuration = 1.5f;

        [Header("Objects Settings: ")]
        [SerializeField] private CapsuleCollider trashBagCollider;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;

        private Vector3 initialPosition = new Vector3();
        private bool isMoving = false;

        private void Start()
        {
            initialPosition = parentTransform.position;
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
            Vector3 currentPosition = parentTransform.position;
            Vector3 targetPosition = new Vector3(currentPosition.x + xOffSet, currentPosition.y, currentPosition.z + zOffSet);
            StartCoroutine(SlideDoorOpen(targetPosition));
        }

        private void CloseDoor()
        {
            StartCoroutine(SlideDoorClose());
        }

        private float PlayDoorSounds()
        {
            if (doorIsOpen & closingDoorSound != null)
            {
                GetComponent<AudioSource>().PlayOneShot(closingDoorSound);
                return closingDoorSound.length;
            }
            else if (!doorIsOpen & openingDoorSound != null)
            {
                GetComponent<AudioSource>().PlayOneShot(openingDoorSound);
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
            float duration = 1f; // Adjust the duration of the rotation as needed
            float elapsedTime = 0f;
            Vector3 startPosition = parentTransform.position;

            while (elapsedTime < duration)
            {
                parentTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            parentTransform.position = targetPosition;
            doorIsOpen = !doorIsOpen;
            isMoving = false;

            if (trashBagCollider) { trashBagCollider.enabled = true; }
        }

        private IEnumerator SlideDoorClose()
        {
            isMoving = true;
            if (trashBagCollider) { trashBagCollider.enabled = false; }
            float duration = 1f;
            float elapsedTime = 0f;
            Vector3 startPosition = parentTransform.position;

            while (elapsedTime < duration)
            {
                parentTransform.position = Vector3.Lerp(startPosition, initialPosition, elapsedTime / duration);
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
            if (isMoving) { return false; }
            else { return true; }
        }

        public InteractionTextSO RetrieveInteractionSO()
        {
            return doorIsOpen ? interactionTextsSO_close : interactionTextsSO_close;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new System.NotImplementedException();
        }

        public string GetFallBackText(string currentLanguage)
        {
            throw new System.NotImplementedException();
        }
    }
}
