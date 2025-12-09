    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TwelveG.AudioController;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;


namespace TwelveG.InteractableObjects
{
    public class PizzaHandler : MonoBehaviour, IInteractable
    {
        [Header("Objects to Modify")]
        [SerializeField] private List<GameObject> pizzaSlices = new List<GameObject>();
        [SerializeField] private GameObject boxTop = null;

        [Header("Objects needed to interact")]
        [SerializeField] private List<ItemType> objectsNeededType;

        [Header("Resulting Objects")]
        [SerializeField] private List<ItemType> resultingObjectsType;

        [Header("Audio settings")]
        [SerializeField] private AudioClip actionSound;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 1f;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onPlayerControls;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO;

        [Header("Fallback Texts SO")]
        [SerializeField] private ObservationTextSO observationTextSO;

        private List<String> playerItems = new List<string>();
        private bool canBeInteractedWith = true;
        private bool sliceHasBeenTaken = false;
        private bool pizzaBoxIsOpen = false;

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return canBeInteractedWith;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            if (sliceHasBeenTaken) { return null; }
            return interactionTextsSO;
        }

        public bool Interact(PlayerInteraction playerCamera)
        {
            bool playerHasRequiredItems = VerifyIfPlayerCanInteract(playerCamera);

            if (playerHasRequiredItems && !sliceHasBeenTaken)
            {
                StartCoroutine(GrabPizza(pizzaSlices, playerCamera));
                return true;
            }
            else if (sliceHasBeenTaken)
            {
                StartCoroutine(RotateBoxTop());
                canBeInteractedWith = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator GrabPizza(List<GameObject> pizzaSlices, PlayerInteraction playerCamera)
        {
            onPlayerControls.Raise(this, new EnablePlayerControllers(false));

            yield return StartCoroutine(RotateBoxTop());

            foreach (GameObject gameObject in pizzaSlices)
            {
                Destroy(gameObject);
            }

            RemoveUsedItems(playerCamera);

            onPlayerControls.Raise(this, new EnablePlayerControllers(true));
        }

        private IEnumerator RotateBoxTop()
        {
            canBeInteractedWith = false;
            (AudioSource audioSource, AudioSourceState audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);

            if (actionSound)
            {
                audioSource.PlayOneShot(actionSound);
            }

            Quaternion initialRotation = boxTop.transform.localRotation;
            Quaternion targetRotation;
            float coroutineDuration = 0.4f;

            if (!pizzaBoxIsOpen)
            {
                targetRotation = initialRotation * Quaternion.Euler(-20f, 0, 0);
            }
            else
            {
                targetRotation = initialRotation * Quaternion.Euler(20f, 0, 0);
            }

            float elapsedTime = 0f;

            while (elapsedTime < coroutineDuration)
            {
                boxTop.transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / coroutineDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            boxTop.transform.localRotation = targetRotation;
            if (actionSound) { AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState); }

            pizzaBoxIsOpen = !pizzaBoxIsOpen;
            canBeInteractedWith = true;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCamera)
        {
            playerItems = playerCamera.GetComponentInChildren<PlayerInventory>().returnPickedItems();

            bool playerHasNeededItems = objectsNeededType.All(item => playerItems.Contains(item.ToString()));

            return playerHasNeededItems;
        }

        private void RemoveUsedItems(PlayerInteraction playerCamera)
        {
            // Accede al inventario del jugador y remueve los objetos necesarios para la tarea
            var playerInventory = playerCamera.GetComponentInChildren<PlayerInventory>();
            foreach (var itemNeeded in objectsNeededType)
            {
                playerInventory.RemoveItem(itemNeeded);
            }
            // Agrega el ItemType resultante de la interacción al inventario del usuario.
            // Dependiendo del tipo de item, `RemoveItem` en PlayerIventory hará cosas distantas.
            if (resultingObjectsType != null && resultingObjectsType.Count > 0)
            {
                foreach (var resultingItem in resultingObjectsType)
                {
                    playerInventory.AddItem(resultingItem);
                    sliceHasBeenTaken = true;
                }
            }
        }

        public ObservationTextSO GetFallBackText()
        {
            return observationTextSO;
        }
    }
}