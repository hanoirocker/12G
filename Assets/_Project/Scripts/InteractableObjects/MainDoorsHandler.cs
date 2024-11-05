namespace TwelveG.InteractableObjects
{
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class MainDoorsHandler : MonoBehaviour, IInteractable
    {
        private EventsFallbacksTextsSO eventsFallbacksTextsSO;

        public bool CanBeInteractedWith(PlayerInteraction interactor)
        {
            return true;
        }

        public void UpdateFallbackTexts(Component sender, object data)
        {
            if (data != null)
            {
                eventsFallbacksTextsSO = (EventsFallbacksTextsSO)data;
            }
            else
            {
                print("No data found on UpdateFallbackTexts");
            }
        }

        public string GetFallBackText()
        {
            // Si no se encuentra el idioma, retorna un valor por defecto
            return "Texto no disponible";
        }

        public string GetInteractionPrompt()
        {
            // Si no se encuentra el idioma, retorna un valor por defecto
            return "Texto no disponible";
        }

        public bool Interact(PlayerInteraction interactor)
        {
            return false;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new System.NotImplementedException();
        }
    }
}