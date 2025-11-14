using System;
using System.Collections.Generic;
using TwelveG.InteractableObjects;
using UnityEngine;

namespace TwelveG.PlayerController
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Hands transforms ")]
        [SerializeField] Transform leftHandTransform;
        [SerializeField] Transform rightHandTransform;

        [Header("Inventory Objects")]
        [SerializeField] GameObject flashlight;
        [SerializeField] GameObject walkieTalkie;

        [Header("Inventory Prefabs")]
        [SerializeField] GameObject broom;
        [SerializeField] GameObject fullTrashBag;
        [SerializeField] GameObject pizzaInClosedBox;
        [SerializeField] GameObject plate;
        [SerializeField] GameObject pizzaSliceOnPlate;
        [SerializeField] GameObject pizzaSlice;
        [SerializeField] GameObject phone;

        [Header("After used Objects: ")]
        [SerializeField] GameObject usedBroom;

        [Header("Game Event SO's")]
        [SerializeField] private GameEventSO onPizzaPickedUp;


        private List<String> pickedUpItems = new List<String>();

        private GameObject activeBroom = null;
        private GameObject activeFlashlight = null;
        private GameObject activeWalkieTalkie = null;
        private GameObject activeFullTrashBag = null;
        private GameObject activePlate = null;
        private GameObject activePizzaSliceOnPlate = null;
        private GameObject activePizzaSlice = null;
        private GameObject activePhone = null;

        private bool playerCanToggleItems = true;

        private void MakeItemUseable(ItemType itemType, bool instantiateItem, GameObject objectToInstantiate, Transform inventoryTransform)
        {
            if (instantiateItem && objectToInstantiate != null)
            {
                switch (itemType)
                {
                    case (ItemType.Flashlight):
                        flashlight.GetComponent<PlayerItemBase>().AllowItemToBeToggled(true);
                        activeFlashlight = flashlight;
                        break;
                    case (ItemType.WalkieTalkie):
                        walkieTalkie.GetComponent<PlayerItemBase>().AllowItemToBeToggled(true);
                        activeFlashlight = walkieTalkie;
                        break;
                    case (ItemType.Broom):
                        activeBroom = Instantiate(objectToInstantiate, inventoryTransform);
                        break;
                    case (ItemType.FullTrashBag):
                        activeFullTrashBag = Instantiate(objectToInstantiate, inventoryTransform);
                        break;
                    case (ItemType.Plate):
                        activePlate = Instantiate(objectToInstantiate, inventoryTransform);
                        break;
                    case (ItemType.PizzaSliceOnPlate):
                        activePizzaSliceOnPlate = Instantiate(objectToInstantiate, inventoryTransform);
                        break;
                    case (ItemType.HeatedPizzaSliceOnPlate):
                        activePizzaSliceOnPlate = Instantiate(objectToInstantiate, inventoryTransform);
                        break;
                    case (ItemType.PizzaSlice):
                        activePizzaSlice = Instantiate(objectToInstantiate, inventoryTransform);
                        break;
                    case (ItemType.Phone):
                        activePhone = Instantiate(objectToInstantiate, inventoryTransform);
                        break;
                    default:
                        Instantiate(objectToInstantiate, inventoryTransform);
                        break;
                }
            }
        }

        private void Update()
        {
            if (activeFlashlight != null && playerCanToggleItems && Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(activeFlashlight.GetComponent<PlayerItemBase>().ToggleItem(activeFlashlight));
            }
            if (activeWalkieTalkie != null && playerCanToggleItems && Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(activeWalkieTalkie.GetComponent<PlayerItemBase>().ToggleItem(activeWalkieTalkie));
            }
        }

        public void HandleExaminationWhileUsingItems(bool isExamining)
        {
            // Si comienza a examinar, el jugador no puede alternar los objetos en las manos
            playerCanToggleItems = !isExamining;

            if (activeWalkieTalkie != null && activeWalkieTalkie.activeSelf)
            {
                // Primero ocultamos los meshes de cualquier objeto que esté en la mano izquierda
                MeshRenderer mesh = activeWalkieTalkie.GetComponent<MeshRenderer>();
                if (mesh != null)
                {
                    mesh.enabled = !isExamining;
                }
            }
            if (activeFlashlight != null && activeFlashlight.activeSelf)
            {
                activeFlashlight.GetComponent<MeshRenderer>().enabled = !isExamining;
                activeFlashlight.GetComponentInChildren<Light>().enabled = !isExamining;
            }
        }

        public void AddItem(ItemType itemType)
        {
            // Se agrega el item a la lista de items recogidos
            pickedUpItems.Add(itemType.ToString());

            // Se instancia el prefab correspondiente en la mano del jugador para todos los items MENOS Flashlight y WalkieTalkie (ya están en cada mano)
            switch (itemType)
            {
                case ItemType.Flashlight:
                    MakeItemUseable(ItemType.Flashlight, true, flashlight, rightHandTransform);
                    break;
                case ItemType.WalkieTalkie:
                    MakeItemUseable(ItemType.WalkieTalkie, true, walkieTalkie, leftHandTransform);
                    break;
                case ItemType.EmptyTrashBag:
                    MakeItemUseable(ItemType.EmptyTrashBag, false, null, null);
                    break;
                case ItemType.Broom:
                    MakeItemUseable(ItemType.Broom, true, broom, rightHandTransform);
                    break;
                case ItemType.FullTrashBag:
                    MakeItemUseable(ItemType.FullTrashBag, true, fullTrashBag, leftHandTransform);
                    break;
                case ItemType.PizzaInClosedBox:
                    MakeItemUseable(ItemType.PizzaInClosedBox, true, pizzaInClosedBox, rightHandTransform);
                    break;
                case ItemType.Plate:
                    MakeItemUseable(ItemType.Plate, true, plate, rightHandTransform);
                    break;
                case ItemType.PizzaSliceOnPlate:
                    MakeItemUseable(ItemType.PizzaSliceOnPlate, true, pizzaSliceOnPlate, rightHandTransform);
                    onPizzaPickedUp.Raise(this, null);
                    break;
                case ItemType.HeatedPizzaSliceOnPlate:
                    MakeItemUseable(ItemType.HeatedPizzaSliceOnPlate, true, pizzaSliceOnPlate, rightHandTransform);
                    break;
                case ItemType.PizzaSlice:
                    MakeItemUseable(ItemType.PizzaSlice, true, pizzaSlice, rightHandTransform);
                    break;
                case ItemType.Phone:
                    MakeItemUseable(ItemType.Phone, true, phone, leftHandTransform);
                    break;
            }
        }

        public bool PlayerIsUsingFlashlight()
        {
            if (activeFlashlight != null && activeFlashlight.activeSelf)
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
                        Destroy(activeBroom);
                        Instantiate(usedBroom);
                        break;
                    case ItemType.FullTrashBag:
                        Destroy(activeFullTrashBag);
                        break;
                    case ItemType.Flashlight:
                        Destroy(activeFlashlight);
                        break;
                    case ItemType.WalkieTalkie:
                        Destroy(activeWalkieTalkie);
                        break;
                    case ItemType.Plate:
                        Destroy(activePlate);
                        break;
                    case ItemType.PizzaSliceOnPlate:
                        Destroy(activePizzaSliceOnPlate);
                        break;
                    case ItemType.HeatedPizzaSliceOnPlate:
                        Destroy(activePizzaSliceOnPlate);
                        break;
                    case ItemType.PizzaSlice:
                        Destroy(activePizzaSlice);
                        break;
                    case ItemType.Phone:
                        Destroy(activePhone);
                        break;
                }
            }
        }

        public void EnablePlayerItem(Component sender, object data)
        {
            ItemType itemType = (ItemType)data;
            pickedUpItems.Add(itemType.ToString());

            switch (itemType)
            {
                case (ItemType.Flashlight):
                    activeFlashlight = flashlight;
                    activeFlashlight.GetComponent<PlayerItemBase>().AllowItemToBeToggled(true);
                    break;
                case (ItemType.WalkieTalkie):
                    activeWalkieTalkie = walkieTalkie;
                    activeWalkieTalkie.GetComponent<PlayerItemBase>().AllowItemToBeToggled(true);
                    break;
                default:
                    break;
            }
        }
    }
}