using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class MainDoorsHandler : MonoBehaviour, IInteractable
    {
        [Header("Settings")]
        [SerializeField] private bool isMainEntranceDoor;
        [SerializeField] private ObservationTextSO observationFallbackTextDefault = null;
        [SerializeField] private ObservationTextSO defaultEventDrivenObservationFallbackText = null;
        [SerializeField, Range(0f, 5f)] private float timeBeforeShowingFallbackText = 0f;

        [Space]
        [Header("Testing")]
        public ObservationTextSO currentObservationFallbackText = null;

        private bool canBeinteractedWith = true;

        void Awake()
        {
            if(isMainEntranceDoor)
            {
                currentObservationFallbackText = defaultEventDrivenObservationFallbackText;
            }
        }

        public bool CanBeInteractedWith(PlayerInteraction interactor)
        {
            return canBeinteractedWith;
        }

        public void UpdateFallbackTexts(Component sender, object data)
        {
            if (data != null)
            {
                currentObservationFallbackText = (ObservationTextSO)data;
            }
            else
            {
                print("No data found on UpdateFallbackTexts - observationFallbackTextsRecieved");
            }
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
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

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            if (isMainEntranceDoor) { return (currentObservationFallbackText, timeBeforeShowingFallbackText); }
            return (observationFallbackTextDefault, timeBeforeShowingFallbackText);
        }

        // onResetEventDrivenTexts: Llamado en eventos para volver a la lista default de textos
        public void ResetFallbackText()
        {
            currentObservationFallbackText = defaultEventDrivenObservationFallbackText;
        }
    }
}