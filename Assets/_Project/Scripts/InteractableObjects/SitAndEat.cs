namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class SitAndEat : MonoBehaviour, IInteractable
    {
        [Header("Objects needed to interact")]
        [SerializeField] private string actionText = "";

        [Header("Objects needed to interact")]
        [SerializeField] private List<ItemType> objectsNeededType = null;

        [Header("Resulting Objects")]
        [SerializeField] private ItemType resultingObjectType;
        [SerializeField] private GameObject emptyPlate = null;
        [SerializeField] private Transform plateTransform = null;

        public GameEventSO sitAndEatPizza;

        private List<String> playerItems = new List<String>();
        private float heatingTime;

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            if (VerifyIfPlayerCanInteract(playerCamera))
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
                StartCoroutine(SitAndEatPizza(playerCamera));
                return true;
            }
            else
            {
                return false;
            }
        }

        // Responsable de todo lo referido a comer la pizza
        private IEnumerator SitAndEatPizza(PlayerInteraction playerCamera)
        {
            // Avisa a PizzaTimeEvent que el jugador interactuó para sentarse a comer
            sitAndEatPizza.Raise(this, null);

            // Remueve PizzaSliceOnPlate del inventario e instancia el plato vacío en la mesa
            // y avisa al PlayerInventory de instanciar la porción de pizza con su SlicePizzaHandler
            // La espera es para darle tiempo a la corutina de PizzaSliceHandler que haga el fadeout antes de 
            // RemoveUsedItems e Instantiate del plato vacio.
            AddSingleSlideOfHeatedPizzaToInventory(playerCamera);

            yield return new WaitForSeconds(1f);

            RemoveUsedItems(playerCamera);
            Instantiate(emptyPlate, plateTransform);
    
            GetComponent<SphereCollider>().enabled = false;
            yield return null;
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

        private void AddSingleSlideOfHeatedPizzaToInventory(PlayerInteraction playerCamera)
        {
            playerCamera.GetComponentInChildren<PlayerInventory>().AddItem(resultingObjectType);
        }
    }
}