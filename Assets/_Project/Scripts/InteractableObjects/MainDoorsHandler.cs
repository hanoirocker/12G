namespace TwelveG.InteractableObjects
{
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class MainDoorsHandler : MonoBehaviour, IInteractable
    {   
        [Header("Event Observations Texts SO")]
        private EventsFallbacksTextsSO mainDoorFallbackTexts;

        private int currentFallbackTextIndex = 0;
        private bool hasReachedMaxContemplations = false;
        private bool canBeinteractedWith = true;

        public bool CanBeInteractedWith(PlayerInteraction interactor)
        {
            return canBeinteractedWith;
        }

        public void UpdateFallbackTexts(Component sender, object data)
        {
            if (data != null)
            {
                mainDoorFallbackTexts = (EventsFallbacksTextsSO)data;
            }
            else
            {
                print("No data found on UpdateFallbackTexts");
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

        // TODO: move observation from text to SO!
        public string GetFallBackText(string languageCode)
        {
            if(hasReachedMaxContemplations) { canBeinteractedWith = false; }
            var (contemplationText, updatedContemplationsStatus, updatedIndex) = 
                Utils.TextFunctions.RetrieveEventFallbackText(
                    languageCode,
                    currentFallbackTextIndex,
                    mainDoorFallbackTexts
                );

            hasReachedMaxContemplations = updatedContemplationsStatus;
            currentFallbackTextIndex = updatedIndex;
            return contemplationText;
        }
    }
}