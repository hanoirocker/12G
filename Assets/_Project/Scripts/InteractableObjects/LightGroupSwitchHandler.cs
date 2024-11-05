namespace TwelveG.InteractableObjects
{
    using TwelveG.PlayerController;
    using UnityEngine;

    public class LightGroupSwitchHandler : MonoBehaviour, IInteractable
    {
        [SerializeField] Light[] lights;
        [SerializeField] Renderer[] bulbsRenderers;

        bool lightsAreActive;
        Material bulbMaterial;
        AudioSource audioSource;

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

        public string GetInteractionPrompt()
        {
            return lightsAreActive ? "Turn off lights" : "Turn lights on";
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

        public string GetFallBackText()
        {
            throw new System.NotImplementedException();
        }
    }

}