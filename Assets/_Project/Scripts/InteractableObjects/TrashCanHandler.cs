using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwelveG.AudioController;
using TwelveG.GameController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class TrashCanHandler : MonoBehaviour, IInteractable, ICheckpointListener
    {
        [Header("Objects needed to interact")]
        [SerializeField] private List<ItemType> objectsNeededType;

        [Header("Visual settings")]
        [SerializeField] private bool fadesImage = false;

        [Header("Audio settings")]
        [SerializeField] private Transform bottomOfTrashCanTransform;
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip dumpingTrashSound;
        [SerializeField] private AudioClip closingDoorSound;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 0.7f;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO trowAwayTrash;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO;

        private AudioSourceState audioSourceState;
        private AudioSource audioSource;
        private Quaternion initialRotation;
        private bool doorIsOpen = false;
        private float defaultRotationTime = 1f;

        private void Start()
        {
            initialRotation = gameObject.transform.localRotation;
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return VerifyIfPlayerCanInteract(playerCamera);
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
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
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));

            Quaternion targetRotation = doorIsOpen ? initialRotation : initialRotation * Quaternion.Euler(-90, 0, 0);
            yield return StartCoroutine(RotateTop(targetRotation));

            if (fadesImage)
            {
                yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeOut, 1f));
            }

            yield return new WaitUntil(() => !audioSource.isPlaying);

            RemoveUsedItems(playerCamera);

            if (dumpingTrashSound != null)
            {
                (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(bottomOfTrashCanTransform, clipsVolume);
                audioSource.PlayOneShot(dumpingTrashSound);
                yield return new WaitUntil(() => !audioSource.isPlaying);
                AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
                audioSource = null;
            }

            yield return StartCoroutine(RotateTop(initialRotation));

            if (fadesImage)
            {
                yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeIn, 1f));
            }

            trowAwayTrash.Raise(this, null);
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
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
        }

        private float PlayDoorSounds()
        {
            (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);

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

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            throw new NotImplementedException();
        }

        public void OnCheckpointReached(string state)
        {
            if(state == "INTERACTION_DISABLED")
            {
                gameObject.GetComponent<Collider>().enabled = false;
            }
        }
    }
}