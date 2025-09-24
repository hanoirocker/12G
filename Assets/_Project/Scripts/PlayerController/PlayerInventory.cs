namespace TwelveG.PlayerController
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.InteractableObjects;
    using UnityEngine;

    public class PlayerInventory : MonoBehaviour
    {
        [Header("Hands transforms ")]
        [SerializeField] Transform leftHandTransform;
        [SerializeField] Transform rightHandTransform;
        [Header("Inventory Objects (hand-portable): ")]
        [SerializeField] GameObject flashlight;
        [SerializeField] GameObject walkieTalkie;
        [SerializeField] GameObject broom;
        [SerializeField] GameObject fullTrashBag;
        [SerializeField] GameObject pizzaInClosedBox;
        [SerializeField] GameObject plate;
        [SerializeField] GameObject pizzaSliceOnPlate;
        [SerializeField] GameObject pizzaSlice;
        [SerializeField] GameObject phone;

        [Header("After used Objects: ")]
        [SerializeField] GameObject usedBroom;

        [Header("Testing settings: ")]
        [SerializeField] private bool enableAllItems;

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

        private void Start()
        {
            if (enableAllItems) { handleAllItems(); }
        }

        private void handleAllItems()
        {
            MakeItemUseable(ItemType.Flashlight, true, flashlight, rightHandTransform);
            MakeItemUseable(ItemType.WalkieTalkie, true, walkieTalkie, leftHandTransform);
        }

        private void MakeItemUseable(ItemType itemType, bool instantiateItem, GameObject objectToInstantiate, Transform inventoryTransform)
        {
            pickedUpItems.Add(itemType.ToString());
            if (instantiateItem && objectToInstantiate != null)
            {
                switch (itemType)
                {
                    case (ItemType.Flashlight):
                        activeFlashlight = Instantiate(objectToInstantiate, inventoryTransform);
                        break;
                    case (ItemType.WalkieTalkie):
                        activeWalkieTalkie = Instantiate(objectToInstantiate, inventoryTransform);
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
            if (activeFlashlight != null && Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(ToggleItem(activeFlashlight));
            }
            if (activeWalkieTalkie != null && Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(ToggleItem(activeWalkieTalkie));
            }
        }

        private IEnumerator ToggleItem(GameObject item)
        {
            // Si el item está activo, se ejecuta primero la animación de guardado
            if (item.activeSelf)
            {
                item.GetComponent<Animator>().SetTrigger("HideItem");
                yield return new WaitForSeconds(1.5f);
                item.SetActive(false);
            }
            else
            {
                item.SetActive(true);
            }

        }

        public void AddItem(ItemType itemType)
        {
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
                        flashlight.SetActive(false);
                        break;
                    case ItemType.WalkieTalkie:
                        walkieTalkie.SetActive(false);
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
    }
}