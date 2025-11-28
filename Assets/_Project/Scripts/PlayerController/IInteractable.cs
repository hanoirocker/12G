namespace TwelveG.PlayerController
{
using TwelveG.Localization;

    interface IInteractable
    {
        public bool Interact(PlayerInteraction playerCameraObject);

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera);

        public bool CanBeInteractedWith(PlayerInteraction playerCameraObject);

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCameraObject);

        public ObservationTextSO GetFallBackText();
    }
}