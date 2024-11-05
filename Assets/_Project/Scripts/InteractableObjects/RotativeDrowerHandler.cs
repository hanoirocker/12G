namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class RotativeDrowerHandler : MonoBehaviour, IInteractable
    {
        [Header("Drower Settings: ")]
        [SerializeField] float yOffset = 0f;

        [Header("Audio Settings: ")]
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip closingDoorSound;
        [SerializeField, Range(-2f, 2f)] private float openingSoundOffset = 0f;
        [SerializeField, Range(-2f, 2f)] private float closingSoundOffset = 0f;

        [Header("Testing Settings: ")]
        [SerializeField] private bool doorIsOpen;
        [SerializeField, Range(1f, 3f)] private float rotatingDuration = 1.5f;

        AudioSource audioSource;
        private bool isMoving;
        private Quaternion initialRotation;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            isMoving = false;
            initialRotation = gameObject.transform.localRotation;
        }

        private void ToggleDoor(Vector3 playerPosition)
        {
            Quaternion targetRotation = doorIsOpen ? initialRotation : initialRotation * Quaternion.Euler(0, yOffset, 0);
            StartCoroutine(RotateDrowerDoor(targetRotation));
        }


        private IEnumerator RotateDrowerDoor(Quaternion targetRotation)
        {
            // Right now: Coroutine time depends on the length of the opening/closing audio clips.

            isMoving = true;
            float coroutineDuration = PlayDoorSounds();
            float elapsedTime = 0f;
            Quaternion startRotation = gameObject.transform.localRotation;

            while (elapsedTime < coroutineDuration)
            {
                gameObject.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / coroutineDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            gameObject.transform.localRotation = targetRotation;
            doorIsOpen = !doorIsOpen;
            isMoving = false;
        }

        private float PlayDoorSounds()
        {
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

        private string GetDoorTextForCanvas(bool doorIsOpen)
        {
            return doorIsOpen ? "Close Door" : "Open Door";
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