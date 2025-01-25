namespace TwelveG.InteractableObjects
{
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class PickableItem : MonoBehaviour, IItem
    {
        [Header("Item settings")]
        [SerializeField] private ItemType itemType;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO;

        public bool canBePicked;

        public void AllowToBePicked(Component sender, object data)
        {
            canBePicked = (bool)data;
        }

        public bool CanBePicked()
        {
            return canBePicked;
        }

        public string GetItemText(string languageCode)
        {
            return Utils.TextFunctions.RetrieveInteractionText(languageCode, interactionTextsSO);
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