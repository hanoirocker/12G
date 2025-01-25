using TwelveG.Localization;

namespace TwelveG.PlayerController
{
    interface IInteractable
    {
        public bool Interact(PlayerInteraction playerCameraObject);

        public InteractionTextSO RetrieveInteractionSO();

        public bool CanBeInteractedWith(PlayerInteraction playerCameraObject);

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCameraObject);

        public string GetFallBackText(string currentLanguage);
    }
}