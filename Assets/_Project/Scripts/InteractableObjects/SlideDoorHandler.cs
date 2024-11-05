namespace TwelveG.InteractableObjects
{
    using TwelveG.PlayerController;
    using System.Collections;
    using UnityEngine;

    public class SlideDoorHandler : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject door;
        [SerializeField] private bool doorIsOpen;
        [SerializeField] Transform parent;
        [SerializeField] AudioClip openingDoorSound;
        [SerializeField] AudioClip closingDoorSound;

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
            audioSource = GetComponent<AudioSource>();
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
            if (doorIsOpen)
            {
                audioSource.PlayOneShot(closingDoorSound);
                return closingDoorSound.length;
            }
            else
            {
                audioSource.PlayOneShot(openingDoorSound);
                return openingDoorSound.length;
            }
        }

        private IEnumerator SlideDoorOpen(Vector3 targetPosition)
        {
            isMoving = true;
            float duration = 1f; // Adjust the duration of the rotation as needed
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

        private string GetDoorTextForCanvas(bool doorIsOpen)
        {
            return doorIsOpen ? "Close Door" : "Open Door";
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

        public string GetInteractionPrompt()
        {
            return GetDoorTextForCanvas(doorIsOpen);
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new System.NotImplementedException();
        }

        public string GetFallBackText()
        {
            throw new System.NotImplementedException();
        }
    }

}