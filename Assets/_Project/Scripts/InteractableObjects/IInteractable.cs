namespace TwelveG.PlayerController
{
    interface IInteractable
    {
        public bool Interact(PlayerInteraction playerCameraObject);

        public string GetInteractionPrompt();

        public bool CanBeInteractedWith(PlayerInteraction playerCameraObject);

        public bool VerifyIfPlayerCanInteract(PlayerInteraction playerCameraObject);

        public string GetFallBackText();
    }
}