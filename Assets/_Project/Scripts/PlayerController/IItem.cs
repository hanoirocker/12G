namespace TwelveG.PlayerController
{
    using TwelveG.InteractableObjects;
    using TwelveG.Localization;

    interface IItem
    {
        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera);
        ItemType GetItemType();
        public void TakeItem();
        public bool CanBePicked();
    }
}