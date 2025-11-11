namespace TwelveG.InteractableObjects
{
    public interface IInteractableItem
    {
        public bool Interact();

        public bool CanBeToggled();

        public void AllowItemToBeToggled(bool allow);
    }
}
