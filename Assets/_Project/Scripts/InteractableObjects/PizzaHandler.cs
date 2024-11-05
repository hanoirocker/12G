namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AudioManager;
    using TwelveG.PlayerController;
    using TwelveG.UIManagement;
    using UnityEngine;

    public class PizzaHandler : MonoBehaviour, IInteractable
    {
        // TODO: LOCALIZATION!
        [Header("Text settings")]
        [SerializeField] private string actionText = "";

        [Header("Objects to Modify")]
        [SerializeField] private List<GameObject> pizzaSlices = new List<GameObject>();
        [SerializeField] private GameObject boxTop = null;

        [Header("Objects needed to interact")]
        [SerializeField] private List<ItemType> objectsNeededType;

        [Header("Resulting Objects")]
        [SerializeField] private List<ItemType> resultingObjectsType;

        [Header("Sound settings")]
        [SerializeField] private AudioClip actionSound;

        public GameEventSO onPlayerControls;

        private List<String> playerItems = new List<string>();
        private bool canBeInteractedWith = true;
        private bool sliceHasBeenTaken = false;
        private bool pizzaBoxIsOpen = false;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return canBeInteractedWith;
        }

        public string GetInteractionPrompt()
        {
            return actionText;
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
            onPlayerControls.Raise(this, "DisablePlayerCapsule");

            yield return StartCoroutine(RotateBoxTop());

            foreach (GameObject gameObject in pizzaSlices)
            {
                Destroy(gameObject);
            }

            RemoveUsedItems(playerCamera);

            onPlayerControls.Raise(this, "EnablePlayerCapsule");
        }

        private IEnumerator RotateBoxTop()
        {
            canBeInteractedWith = false;
            if (actionSound) { audioSource.PlayOneShot(actionSound); }

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
            if (actionSound) { audioSource.Stop(); }

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

        public string GetFallBackText()
        {
            return "Seria bueno primero tener un plato a mano";
        }
    }
}