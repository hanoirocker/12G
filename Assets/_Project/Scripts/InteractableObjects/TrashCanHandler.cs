namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using TwelveG.UIController;
    using UnityEngine;

    public class TrashCanHandler : MonoBehaviour, IInteractable
    {
        [Header("Objects needed to interact")]
        [SerializeField] private List<ItemType> objectsNeededType;

        [Header("Resulting Objects")]
        [SerializeField] private List<ItemType> resultingObjectsType;

        [Header("Visual settings")]
        [SerializeField] private bool fadesImage = false;

        [Header("Top door settings")]
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip closingDoorSound;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO trowAwayTrash;
        [SerializeField] private GameEventSO onPlayerControls;
        [SerializeField] private GameEventSO onImageCanvasControls;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO;

        private AudioSource audioSource;
        private Quaternion initialRotation;
        private bool doorIsOpen = false;
        private float defaultRotationTime = 1f;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            initialRotation = gameObject.transform.localRotation;
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return VerifyIfPlayerCanInteract(playerCamera);
        }

        public InteractionTextSO RetrieveInteractionSO()
        {
            return interactionTextsSO;
        }

        public bool Interact(PlayerInteraction playerCamera)
        {
            bool playerCanInteract = VerifyIfPlayerCanInteract(playerCamera);

            if (playerCanInteract)
            {
                StartCoroutine(TrowObjectsIntoCan(playerCamera));
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator TrowObjectsIntoCan(PlayerInteraction playerCamera)
        {
            onPlayerControls.Raise(this, new EnablePlayerControllers(false));

            Quaternion targetRotation = doorIsOpen ? initialRotation : initialRotation * Quaternion.Euler(-90, 0, 0);
            yield return StartCoroutine(RotateTop(targetRotation));

            if (fadesImage)
            {
                onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitUntil(() => !audioSource.isPlaying);

            RemoveUsedItems(playerCamera);

            yield return StartCoroutine(RotateTop(initialRotation));

            if (fadesImage)
            {
                onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
                yield return new WaitForSeconds(1f);
            }

            trowAwayTrash.Raise(this, null);
            onPlayerControls.Raise(this, new EnablePlayerControllers(true));
        }

        private void ToggleTop()
        {
            Quaternion targetRotation = doorIsOpen ? initialRotation : initialRotation * Quaternion.Euler(0, 90, 0);
            StartCoroutine(RotateTop(targetRotation));
        }

        private IEnumerator RotateTop(Quaternion targetRotation)
        {
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
            audioSource.Stop();
        }

        private float PlayDoorSounds()
        {
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
                return defaultRotationTime;
            }
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCamera)
        {
            List<String> playerItems = playerCamera.GetComponentInChildren<PlayerInventory>().returnPickedItems();

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

        public ObservationTextSO GetFallBackText()
        {
            throw new NotImplementedException();
        }
    }
}