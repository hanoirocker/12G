namespace TwelveG.InteractableObjects
{
    using TwelveG.PlayerController;
    using UnityEngine;

    public class LightsHandler : MonoBehaviour, IInteractable
    {
        AudioSource audioSource;

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

        private string GetLampTextForCanvas(bool isOn)
        {
            return isOn ? "Apagar las luces" : "Enceder luces";
        }

        public bool Interact(PlayerInteraction interactor)
        {
            ToogleLight();
            return true;
        }

        public string GetInteractionPrompt()
        {
            return GetLampTextForCanvas(lightIsOn);
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return false;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new System.NotImplementedException();
        }

        public string GetFallBackText()
        {
            throw new System.NotImplementedException();
        }
    }

}