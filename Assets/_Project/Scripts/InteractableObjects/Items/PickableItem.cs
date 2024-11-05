namespace TwelveG.InteractableObjects
{
    using TwelveG.PlayerController;
    using UnityEngine;

    public class PickableItem : MonoBehaviour, IItem
    {
        [SerializeField] private ItemType itemType;

        public bool canBePicked;

        public void AllowToBePicked(Component sender, object data)
        {
            canBePicked = (bool)data;
        }

        public bool CanBePicked()
        {
            return canBePicked;
        }

        public string GetItemText()
        {
            return "PICKABLE LOCALIZATION!";
        }

        public ItemType GetItemType()
        {
            return itemType;
        }

        // Funcion principal
        public void TakeItem()
        {
            Destroy(gameObject);
        }
    }
}