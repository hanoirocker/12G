namespace TwelveG.InteractableObjects
{
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class MainDoorsHandler : MonoBehaviour, IInteractable
    {
        private ObservationTextSO observationFallbackTextRecieved = null;

        private bool canBeinteractedWith = true;

        public bool CanBeInteractedWith(PlayerInteraction interactor)
        {
            return canBeinteractedWith;
        }

        public void UpdateFallbackTexts(Component sender, object data)
        {
            if (data != null)
            {
                observationFallbackTextRecieved = (ObservationTextSO)data;
            }
            else
            {
                print("No data found on UpdateFallbackTexts - observationFallbackTextsRecieved");
            }
        }

        public InteractionTextSO RetrieveInteractionSO()
        {
            return null;
        }

        public bool Interact(PlayerInteraction interactor)
        {
            return false;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new System.NotImplementedException();
        }

        public ObservationTextSO GetFallBackText()
        {
            return observationFallbackTextRecieved;
        }
    }
}