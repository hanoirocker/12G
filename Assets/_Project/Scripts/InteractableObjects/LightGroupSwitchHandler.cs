using TwelveG.PlayerController;
using TwelveG.Localization;
using UnityEngine;
using TwelveG.AudioController;

namespace TwelveG.InteractableObjects
{
    public class LightGroupSwitchHandler : MonoBehaviour, IInteractable
    {
        [Header("Object settings: ")]
        [SerializeField] private Light[] lights;
        [SerializeField] private Renderer[] bulbsRenderers;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOn;
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOff;

        [SerializeField] private AudioClip clickSound;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 1f;

        private bool lightsAreActive;
        private Material bulbMaterial;
        private AudioSource audioSource;

        private void Start()
        {
            lightsAreActive = false;
        }

        private void ToogleLights()
        {
            ToogleEmissions();

            audioSource = AudioUtils.GetAudioSourceForInteractable(gameObject.transform, clipsVolume);
            audioSource.clip = clickSound;

            foreach (Light singleLight in lights)
            {
                singleLight.enabled = !singleLight.enabled;
            }

            audioSource.Play();
            lightsAreActive = !lightsAreActive;
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
            return lightsAreActive ? interactionTextsSO_turnOff : interactionTextsSO_turnOn;
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

        public ObservationTextSO GetFallBackText()
        {
            throw new System.NotImplementedException();
        }
    }

}