namespace TwelveG.InteractableObjects
{
    using TwelveG.PlayerController;
    using System.Collections;
    using UnityEngine;
    using TwelveG.Localization;

    public class RotativeDoorHandler : MonoBehaviour, IInteractable
    {
        [Header("Object settings: ")]
        [SerializeField] private GameObject door;
        [SerializeField] private bool doorIsOpen;
        [SerializeField] Animation doorPickAnimation;
        [SerializeField] Transform parent;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;

        [Header("Audio settings: ")]
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip closingDoorSound;

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
            // Right now: Coroutine time depends on the length of the opening/closing audio clips.

            isMoving = true;
            float coroutineDuration = PlayDoorSounds();
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
            audioSource.Stop();
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

        public InteractionTextSO RetrieveInteractionSO()
        {
            return doorIsOpen ? interactionTextsSO_close : interactionTextsSO_close;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCamera)
        {
            throw new System.NotImplementedException();
        }

        public string GetFallBackText(string currentLanguage)
        {
            throw new System.NotImplementedException();
        }
    }
}