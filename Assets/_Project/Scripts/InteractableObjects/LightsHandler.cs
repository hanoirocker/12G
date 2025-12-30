using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class LightsHandler : MonoBehaviour, IInteractable
    {
        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOff;
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOn;

        private AudioSource audioSource;

        private bool lightIsOn;
        // _light since 'light' is a reserved word
        Light _light;

        private void Start()
        {
            _light = GetComponentInChildren<Light>();
            _light.enabled = false;
            lightIsOn = false;
        }

        private void ToogleLight()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            _light.enabled = !_light.enabled;
            lightIsOn = !lightIsOn;
        }

        public bool Interact(PlayerInteraction interactor)
        {
            ToogleLight();
            return true;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return lightIsOn ? interactionTextsSO_turnOff : interactionTextsSO_turnOn;
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return false;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new System.NotImplementedException();
        }

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            throw new System.NotImplementedException();
        }
    }

}