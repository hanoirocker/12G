using System;
using System.Collections.Generic;
using TwelveG.EnvironmentController;
using TwelveG.GameController;
using TwelveG.InteractableObjects;
using TwelveG.VFXController;
using UnityEngine;

namespace TwelveG.PlayerController
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Inventory Prefabs")]
        [SerializeField] GameObject flashlight;
        [SerializeField] GameObject walkieTalkie;

        [Space(5)]
        [SerializeField] GameObject broom;
        [SerializeField] GameObject fullTrashBag;
        [SerializeField] GameObject plate;
        [SerializeField] GameObject pizzaSliceOnPlate;
        [SerializeField] GameObject pizzaSlice;
        [SerializeField] GameObject phone;

        private List<String> pickedUpItems = new List<String>();

        private bool playerCanToggleItems = true;

        public void HandleExaminationWhileUsingItems(bool isExamining)
        {
            // Si comienza a examinar, el jugador no puede alternar los objetos en las manos
            playerCanToggleItems = !isExamining;

            if (walkieTalkie != null && walkieTalkie.activeSelf)
            {
                // Primero ocultamos los meshes de cualquier objeto que esté en la mano izquierda
                MeshRenderer mesh = walkieTalkie.GetComponent<MeshRenderer>();
                if (mesh != null)
                {
                    mesh.enabled = !isExamining;
                }
                Canvas wtCanvas = walkieTalkie.GetComponentInChildren<Canvas>();
                if (wtCanvas != null)
                {
                    wtCanvas.enabled = !isExamining;
                }
            }
            if (flashlight != null && flashlight.activeSelf)
            {
                flashlight.GetComponent<MeshRenderer>().enabled = !isExamining;
                flashlight.GetComponentInChildren<Light>().enabled = !isExamining;
            }
        }

        public void AddItem(ItemType itemType)
        {
            // Se agrega el item a la lista de items recogidos
            pickedUpItems.Add(itemType.ToString());

            switch (itemType)
            {
                case ItemType.Flashlight:
                    flashlight.GetComponent<PlayerItemBase>().AllowItemToBeToggled(true);
                    break;
                case ItemType.WalkieTalkie:
                    walkieTalkie.GetComponent<PlayerItemBase>().AllowItemToBeToggled(true);
                    break;
                case ItemType.EmptyTrashBag:
                    break;
                case ItemType.Broom:
                    broom.SetActive(true);
                    break;
                case ItemType.FullTrashBag:
                    fullTrashBag.SetActive(true);
                    break;
                case ItemType.Plate:
                    plate.SetActive(true);
                    break;
                case ItemType.PizzaSliceOnPlate:
                    pizzaSliceOnPlate.SetActive(true);
                    GameEvents.Common.onPizzaPickedUp.Raise(this, null);
                    break;
                case ItemType.HeatedPizzaSliceOnPlate:
                    pizzaSliceOnPlate.SetActive(true);
                    break;
                case ItemType.PizzaSlice:
                    pizzaSlice.SetActive(true);
                    break;
                case ItemType.Phone:
                    phone.SetActive(true);
                    break;
            }
        }

        public bool PlayerIsUsingFlashlight()
        {
            if (flashlight != null && flashlight.activeSelf)
                return true;
            return false;
        }

        public List<String> returnPickedItems()
        {
            return pickedUpItems;
        }

        public string ListOfPickedItems()
        {
            string list = string.Join(", ", pickedUpItems);
            return list;
        }

        public void RemoveItem(ItemType itemType)
        {
            if (pickedUpItems.Contains(itemType.ToString()))
            {
                pickedUpItems.Remove(itemType.ToString());

                switch (itemType)
                {
                    case ItemType.EmptyTrashBag:
                        break;
                    case ItemType.Broom:
                        broom.SetActive(false);
                        FindAnyObjectByType<PlayerHouseHandler>()?.ToggleCheckpointPrefabs(new ObjectData("Used - Broom", true));
                        break;
                    case ItemType.FullTrashBag:
                        fullTrashBag.SetActive(false);
                        break;
                    case ItemType.Flashlight:
                        flashlight.GetComponent<PlayerItemBase>().AllowItemToBeToggled(false);
                        flashlight.SetActive(false);
                        break;
                    case ItemType.WalkieTalkie:
                        walkieTalkie.GetComponent<PlayerItemBase>().AllowItemToBeToggled(false);
                        walkieTalkie.SetActive(false);
                        VFXManager.Instance?.EnableElectricFeelVFX(false);
                        break;
                    case ItemType.Plate:
                        plate.SetActive(false);
                        break;
                    case ItemType.PizzaSliceOnPlate:
                        pizzaSliceOnPlate.SetActive(false);
                        break;
                    case ItemType.HeatedPizzaSliceOnPlate:
                        pizzaSliceOnPlate.SetActive(false);
                        break;
                    case ItemType.PizzaSlice:
                        pizzaSlice.SetActive(false);
                        break;
                    case ItemType.Phone:
                        phone.SetActive(false);
                        break;
                }
            }
        }

        public void RemovePlayerItem(Component sender, object data)
        {
            ItemType itemType = (ItemType)data;
            RemoveItem(itemType);
        }

        public void EnablePlayerItem(Component sender, object data)
        {
            ItemType itemType = (ItemType)data;
            pickedUpItems.Add(itemType.ToString());

            switch (itemType)
            {
                case (ItemType.Flashlight):
                    flashlight.GetComponent<PlayerItemBase>().AllowItemToBeToggled(true);
                    break;
                case (ItemType.WalkieTalkie):
                    walkieTalkie.GetComponent<PlayerItemBase>().AllowItemToBeToggled(true);
                    VFXManager.Instance?.EnableElectricFeelVFX(true);
                    break;
                default:
                    break;
            }
        }
    }
}