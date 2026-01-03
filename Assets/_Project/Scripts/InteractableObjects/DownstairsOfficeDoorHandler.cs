using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwelveG.AudioController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.GameController;

namespace TwelveG.InteractableObjects
{
    public class DownstairsOfficeDoorHandler : MonoBehaviour, IInteractable, ICheckpointListener
    {
        [Header("Object Settings")]
        [SerializeField] private GameObject door;
        [SerializeField] private bool doorIsOpened;
        [SerializeField] private bool doorIsLocked = true;
        [SerializeField] private bool isNightmare = false;
        [SerializeField] Animation doorPickAnimation;
        [SerializeField, Range(0.5f, 2f)] float rotationTime;

        [Header("Text Settings")]
        [SerializeField] private ObservationTextSO observationFallback;
        [SerializeField, Range(0f, 5f)] private float timeBeforeShowingFallbackText = 0f;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_close;
        [SerializeField] private InteractionTextSO interactionTextsSO_tryToOpen;
        [SerializeField] private InteractionTextSO interactionTextsSO_tryToOpenAgain;

        [Header("Objects needed to interact")]
        [SerializeField] private List<ItemType> objectsNeededType;

        [Header("Audio settings")]
        [SerializeField] private AudioClip[] tryToOpenDesperatelyClips = null;
        [SerializeField] private AudioClip lockedSound = null;
        [SerializeField] private AudioClip unclockedSound = null;
        [SerializeField] private AudioClip eventUnlockedSound = null;
        [SerializeField] private AudioClip openingDoorSound;
        [SerializeField] private AudioClip closingDoorSound;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 1f;

        List<String> playerItems = new List<String>();
        private Quaternion initialRotation;
        private AudioSource audioSource;
        private AudioSourceState audioSourceState;
        private int lockedIndex = 0;
        private bool isMoving;

        private void Start()
        {
            isMoving = false;
            if (door == null) door = gameObject;
            initialRotation = door.transform.localRotation;
        }

        // --- INTERFACE IMPLEMENTATION ---

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return doorIsLocked || !isMoving;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return GetDoorTextForCanvas();
        }

        public bool Interact(PlayerInteraction playerCamera)
        {
            SetupAudioSource();

            if (doorIsLocked)
            {
                if (VerifyIfPlayerCanInteract(playerCamera))
                {
                    StartCoroutine(UnlockDoor(playerCamera));
                    return true;
                }
                else
                {
                    StartCoroutine(PlayLockCoroutine());
                    return false;
                }
            }

            ToggleDoor();
            return true;
        }

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            return lockedIndex == 0 ? (observationFallback, timeBeforeShowingFallbackText) : (null, 0f);
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCamera)
        {
            // Si no se necesitan objetos pero la puerta está cerrada lógicamente, asumimos que no se puede abrir.
            if (objectsNeededType.Count == 0 && doorIsLocked) return false;

            var inventory = playerCamera.GetComponentInChildren<PlayerInventory>();
            if (inventory == null) return false;

            playerItems = inventory.returnPickedItems();

            return objectsNeededType.All(item => playerItems.Contains(item.ToString()));
        }

        public void UnlockDoorByEvent()
        {
            StartCoroutine(UnlockDoorByEventCoroutine());
        }

        private void SetupAudioSource()
        {
            if (audioSource == null)
            {
                (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(transform, clipsVolume);
            }
        }

        private void ToggleDoor()
        {
            Quaternion targetRotation = doorIsOpened
                ? initialRotation
                : initialRotation * Quaternion.Euler(0, 90, 0);

            if (doorPickAnimation && !doorIsOpened)
            {
                doorPickAnimation.Play();
            }

            StartCoroutine(RotateDoor(targetRotation));
        }

        private IEnumerator RotateDoor(Quaternion targetRotation)
        {
            isMoving = true;

            float duration = rotationTime;
            AudioClip clipToPlay = doorIsOpened ? closingDoorSound : openingDoorSound;

            if (clipToPlay != null)
            {
                audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                audioSource.clip = clipToPlay;
                audioSource.Play();
                duration = AudioUtils.CalculateDurationWithPitch(clipToPlay, audioSource.pitch);
            }

            float elapsedTime = 0f;
            Quaternion startRotation = door.transform.localRotation;

            while (elapsedTime < duration)
            {
                door.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            door.transform.localRotation = targetRotation;
            doorIsOpened = !doorIsOpened;
            isMoving = false;

            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
            audioSource = null;
        }

        private IEnumerator PlayLockCoroutine()
        {
            isMoving = true;

            if (doorPickAnimation) doorPickAnimation.Play();

            // Manejo especial si es una pesadilla
            if (isNightmare)
            {
                GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));

                if (tryToOpenDesperatelyClips != null && tryToOpenDesperatelyClips.Length > 0)
                {
                    var clip = tryToOpenDesperatelyClips[UnityEngine.Random.Range(0, tryToOpenDesperatelyClips.Length - 1)];
                    yield return PlaySoundAndWait(clip);
                }

                GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
            }
            else
            {
                if (lockedSound != null) yield return PlaySoundAndWait(lockedSound);
            }

            lockedIndex++;
            isMoving = false;
        }

        private IEnumerator UnlockDoor(PlayerInteraction playerCamera)
        {
            if (unclockedSound != null)
            {
                audioSource.PlayOneShot(unclockedSound);
                yield return new WaitForSeconds(unclockedSound.length);
            }
            else
            {
                yield return new WaitForSeconds(0.5f); // Pequeña espera por default si no hay sonido
            }

            RemoveUsedItems(playerCamera);
            doorIsLocked = false;
        }

        private IEnumerator UnlockDoorByEventCoroutine()
        {
            SetupAudioSource();

            if (eventUnlockedSound != null)
            {
                yield return PlaySoundAndWait(eventUnlockedSound);
            }

            doorIsLocked = false;
        }

        private InteractionTextSO GetDoorTextForCanvas()
        {
            if (doorIsLocked)
            {
                return lockedIndex == 0 ? interactionTextsSO_tryToOpen : interactionTextsSO_tryToOpenAgain;
            }

            return doorIsOpened ? interactionTextsSO_close : interactionTextsSO_open;
        }

        private void RemoveUsedItems(PlayerInteraction playerCamera)
        {
            var inventory = playerCamera.GetComponentInChildren<PlayerInventory>();
            if (inventory != null)
            {
                foreach (var itemNeeded in objectsNeededType)
                {
                    inventory.RemoveItem(itemNeeded);
                }
            }
        }

        private IEnumerator PlaySoundAndWait(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
                yield return new WaitUntil(() => !audioSource.isPlaying);
            }
        }

        public void OnCheckpointReached(string state)
        {
            if (state == "UNLOCKED")
            {
                doorIsLocked = false;
            }
        }
    }
}