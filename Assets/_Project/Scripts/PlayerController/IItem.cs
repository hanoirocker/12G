namespace TwelveG.PlayerController
{
    using TwelveG.InteractableObjects;

    interface IItem
    {
        public string GetItemText(string languageCode);
        ItemType GetItemType();
        public void TakeItem();
        public bool CanBePicked();
    }
}