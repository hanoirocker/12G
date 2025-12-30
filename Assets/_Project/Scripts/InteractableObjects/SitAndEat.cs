using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;
using UnityEngine.UIElements;

namespace TwelveG.InteractableObjects
{
    public class SitAndEat : MonoBehaviour, IInteractable
    {
        [Header("Objects needed to interact")]
        [SerializeField] private List<ItemType> objectsNeededType = null;

        [Header("Resulting Objects")]
        [SerializeField] private ItemType resultingObjectType;
        [SerializeField] private GameObject emptyPlate = null;
        [SerializeField] private Transform plateTransform = null;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO;

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

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            throw new System.NotImplementedException();
        }

        public bool Interact(PlayerInteraction playerCamera)
        {
            bool playerHasRequiredItems = VerifyIfPlayerCanInteract(playerCamera);

            if (playerHasRequiredItems)
            {
                StartCoroutine(SitAndEatPizza(playerCamera));
                GetComponent<BoxCollider>().enabled = false;
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
            yield return new WaitForSeconds(2.5f);

            AddSingleSlideOfHeatedPizzaToInventory(playerCamera);
            RemoveUsedItems(playerCamera);
            Instantiate(emptyPlate, plateTransform);

            GetComponent<BoxCollider>().enabled = false;
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

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return interactionTextsSO;
        }
    }
}