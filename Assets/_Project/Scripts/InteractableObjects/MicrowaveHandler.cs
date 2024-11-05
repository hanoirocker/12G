namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TwelveG.PlayerController;
    using UnityEngine;
    using UnityEngine.Events;

    public class MicrowaveHandler : MonoBehaviour, IInteractable
    {
        [Header("Text Settings")]
        [SerializeField] private string actionText = "";

        [Header("Objects needed to interact")]
        [SerializeField] private List<ItemType> objectsNeededType = null;

        [Header("Resulting Objects")]
        [SerializeField] private ItemType resultingObjectType;
        [SerializeField] private GameObject pizzaSlice = null;
        [SerializeField] private Transform plateTransform = null;

        [Header("Sound settings")]
        [SerializeField] private AudioClip heatingSound = null;
        [SerializeField] private AudioClip managePlate = null;

        [Header("Other components settings")]
        [SerializeField] private RotativeDrowerHandler rotativeDrowerHandler;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Light microwaveLight;
        
        public GameEventSO pizzaHeatingFinished;

        private List<String> playerItems = new List<String>();
        private float heatingTime;

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            if (rotativeDrowerHandler.DoorIsOpen() && VerifyIfPlayerCanInteract(playerCamera))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetFallBackText()
        {
            throw new System.NotImplementedException();
        }

        public string GetInteractionPrompt()
        {
            return actionText;
        }

        public bool Interact(PlayerInteraction playerCamera)
        {
            bool playerHasRequiredItems = VerifyIfPlayerCanInteract(playerCamera);

            if (playerHasRequiredItems)
            {
                StartCoroutine(HeatPizza(playerCamera));
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerator HeatPizza(PlayerInteraction playerCamera)
        {
            RemoveUsedItems(playerCamera);
            audioSource.PlayOneShot(managePlate);
            GameObject heatedPizza = Instantiate(pizzaSlice, plateTransform);
            GetComponentInChildren<RotativeDrowerHandler>().Interact(playerCamera);

            yield return new WaitUntil(() => !rotativeDrowerHandler.DoorIsOpen());
            GetComponentInChildren<SphereCollider>().enabled = false;
            microwaveLight.enabled = true;
            audioSource.PlayOneShot(heatingSound);

            yield return new WaitUntil(() => !audioSource.isPlaying);
            microwaveLight.enabled = false;
            GetComponentInChildren<SphereCollider>().enabled = true;

            yield return new WaitUntil(() => rotativeDrowerHandler.DoorIsOpen());
            Destroy(heatedPizza);
            AddHeatedPizzaToInventory(playerCamera);
            audioSource.PlayOneShot(managePlate);
            GetComponent<BoxCollider>().enabled = false;

            // Avisa a PizzaTimeEvent para que despliegue texto.
            pizzaHeatingFinished.Raise(this, null);
        }

        private void AddHeatedPizzaToInventory(PlayerInteraction playerCamera)
        {
            print("Trying to add: " + resultingObjectType + " to player Inventory");
            playerCamera.GetComponentInChildren<PlayerInventory>().AddItem(resultingObjectType);
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
        }
    }
}