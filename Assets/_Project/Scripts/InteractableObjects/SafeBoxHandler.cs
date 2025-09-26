namespace TwelveG.InteractableObjects
{
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class SafeBoxHandler : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] private RotativeDrawerHandler doorHandler;
        [SerializeField] private ObservationTextSO observationFallbackTextDefault = null;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_open;
        [SerializeField] private InteractionTextSO interactionTextsSO_interact;

        // Cambiar a variable privada luego de tests
        public bool isFirstTimeInteracted = true;

        private ObservationTextSO observationFallbackTextRecieved = null;
        private bool canBeinteractedWith = true;
        private bool isLocked = true;

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
                Debug.LogWarning("No data found on UpdateFallbackTexts - observationFallbackTextsRecieved");
            }
        }

        public InteractionTextSO RetrieveInteractionSO()
        {
            if (isLocked)
            {
                if (isFirstTimeInteracted) { return interactionTextsSO_open; }
                else { return interactionTextsSO_interact; }
            }
            // Al desbloquearse, se apaga este script, por lo que los textos se devuelven
            // desde el RotativeDrawerHandler de la puerta de la caja fuerte
            else
            {
                return null;
            }
        }

        public bool Interact(PlayerInteraction interactor)
        {
            if (isLocked)
            {
                if (isFirstTimeInteracted)
                {
                    isFirstTimeInteracted = false;
                }
                return false;
            }
            else
            {
                GetComponent<BoxCollider>().enabled = false;
                return true;
            }
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new System.NotImplementedException();
        }

        public ObservationTextSO GetFallBackText()
        {
            return observationFallbackTextDefault;
        }
    }
}