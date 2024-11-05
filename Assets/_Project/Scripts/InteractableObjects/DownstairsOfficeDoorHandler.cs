namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class DownstairsOfficeDoorHandler : MonoBehaviour, IInteractable
    {
        [Header("Object Settings")]
        [SerializeField] private GameObject door;
        [SerializeField] private bool doorIsOpened;
        [SerializeField] private bool doorIsLocked = true;
        [SerializeField] Animation doorPickAnimation;
        [SerializeField] Transform parent;

        [Header("Text Settings")]
        [SerializeField] List<String> lockedDoorTexts = new List<string>();
        [SerializeField] List<String> fallBackTexts = new List<string>();

        [Header("Objects needed to interact")]
        [SerializeField] private List<ItemType> objectsNeededType;

        [Header("Audio settings")]
        // [SerializeField] private AudioClip lockedSound = null;
        // [SerializeField] private AudioClip unclockedSound = null;
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip closingDoorSound;

        List<String> playerItems = new List<String>();
        private Quaternion initialRotation;
        private AudioSource audioSource;
        private int lockedIndex = 0;
        private bool isMoving;

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
            Quaternion targetRotation = doorIsOpened ? initialRotation : initialRotation * Quaternion.Euler(0, 90, 0);
            StartCoroutine(RotateDoor(targetRotation));
            if (doorPickAnimation && !doorIsOpened)
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
            doorIsOpened = !doorIsOpened;
            isMoving = false;
            audioSource.Stop();
        }

        private float PlayDoorSounds()
        {
            if (doorIsOpened)
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

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            if (doorIsLocked) { return true; }
            else
            {
                if (isMoving) { return false; }
                else { return true; }
            }
        }

        public string GetInteractionPrompt()
        {
            return GetDoorTextForCanvas(doorIsLocked, doorIsOpened);
        }

        private string GetDoorTextForCanvas(bool doorIsLocked, bool doorIsOpened)
        {
            if (doorIsLocked)
            {
                if (lockedIndex == 0) { return lockedDoorTexts[0]; }
                else { return lockedDoorTexts[1]; }
            }
            else
            {
                return doorIsOpened ? "Close Door" : "Open Door";
            }
        }

        public bool Interact(PlayerInteraction playerCamera)
        {
            if (doorIsLocked)
            {
                bool playerHasRequiredItems = VerifyIfPlayerCanInteract(playerCamera);
                if (playerHasRequiredItems)
                {
                    StartCoroutine(UnlockDoor(playerCamera));
                    return true;
                }
                else
                {
                    // audioSource.PlayOneShot(lockedSound);
                    print("Deberia sonar a que no puede abrirla");
                    lockedIndex += 1;
                    return false;
                }
            }
            else
            {
                ToggleDoor();
                return true;
            }

        }

        private IEnumerator UnlockDoor(PlayerInteraction playerCamera)
        {
            // audioSource.PlayOneShot(unclockedSound);
            // yield return new WaitUntil(() => !audioSource.isPlaying);
            print("Deberia sonar a QUE ABRIO");

            yield return new WaitForSeconds(1f);

            // Remueve la llave del inventario del jugador y destraba la puerta
            RemoveUsedItems(playerCamera);
            doorIsLocked = false;

            GetComponent<RotativeDoorHandler>().enabled = true;
            // Aca podria dispararse un Unity Event al desbloquear la puerta

            this.gameObject.SetActive(false);
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCamera)
        {
            playerItems = playerCamera.GetComponentInChildren<PlayerInventory>().returnPickedItems();

            bool allItemsPresent = objectsNeededType.All(item => playerItems.Contains(item.ToString()));

            return allItemsPresent;
        }

        private void RemoveUsedItems(PlayerInteraction playerCamera)
        {
            // Accede al inventario del jugador y remueve los objetos necesarios para la tarea
            var playerInventory = playerCamera.GetComponentInChildren<PlayerInventory>();
            foreach (var itemNeeded in objectsNeededType)
            {
                playerInventory.RemoveItem(itemNeeded);
            }
        }

        public string GetFallBackText()
        {
            if (lockedIndex == 1) { return fallBackTexts[0]; }
            else { return fallBackTexts[1]; }
        }
    }
}