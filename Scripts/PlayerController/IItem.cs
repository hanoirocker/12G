namespace TwelveG.PlayerController
{
    using TwelveG.InteractableObjects;
    using TwelveG.Localization;

    interface IItem
    {
        public InteractionTextSO RetrieveInteractionSO();
        ItemType GetItemType();
        public void TakeItem();
        public bool CanBePicked();
    }
}