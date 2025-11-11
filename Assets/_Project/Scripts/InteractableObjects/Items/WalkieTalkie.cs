namespace TwelveG.InteractableObjects
{
    using UnityEngine;

    public class WalkieTalkie : MonoBehaviour, IInteractableItem
    {
        private bool canBeToogled = true;

        public void AllowItemToBeToggled(bool allow)
        {
            canBeToogled = allow;
        }

        public bool CanBeToggled()
        {
            return canBeToogled;
        }

        public bool Interact()
        {
            throw new System.NotImplementedException();
        }
    }
}