namespace TwelveG.PlayerController
{
    using TwelveG.InteractableObjects;

    interface IItem
    {
        public string GetItemText();
        ItemType GetItemType();
        public void TakeItem();
        public bool CanBePicked();
    }
}