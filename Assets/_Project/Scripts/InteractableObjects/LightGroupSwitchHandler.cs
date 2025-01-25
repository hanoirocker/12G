namespace TwelveG.InteractableObjects
{
    using TwelveG.PlayerController;
    using TwelveG.Localization;
    using UnityEngine;

    public class LightGroupSwitchHandler : MonoBehaviour, IInteractable
    {
        [Header("Object settings: ")]
        [SerializeField] private Light[] lights;
        [SerializeField] private Renderer[] bulbsRenderers;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOn;
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOff;

        private bool lightsAreActive;
        private Material bulbMaterial;
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            lightsAreActive = false;
        }

        private void ToogleLights()
        {
            ToogleEmissions();

            foreach (Light singleLight in lights)
            {
                singleLight.enabled = !singleLight.enabled;
            }

            audioSource.Play();
            lightsAreActive = !lightsAreActive;
            print(lightsAreActive);
        }

        public void ToogleEmissions()
        {
            foreach (Renderer bulbRenderer in bulbsRenderers)
            {
                bulbMaterial = bulbRenderer.material;

                if (!lightsAreActive)
                {
                    bulbMaterial.EnableKeyword("_EMISSION");
                }
                else
                {
                    bulbMaterial.DisableKeyword("_EMISSION");
                }
            }
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return true;
        }

        public InteractionTextSO RetrieveInteractionSO()
        {
            return lightsAreActive? interactionTextsSO_turnOff : interactionTextsSO_turnOn;
        }

        public bool Interact(PlayerInteraction interactor)
        {
            ToogleLights();
            return true;
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            throw new System.NotImplementedException();
        }

        public string GetFallBackText(string currentLanguage)
        {
            throw new System.NotImplementedException();
        }
    }

}