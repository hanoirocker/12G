namespace TwelveG.InteractableObjects
{
    using TwelveG.PlayerController;
    using TwelveG.Localization;
    using UnityEngine;

    public class LightSwitchHandler : MonoBehaviour, IInteractable
    {
        [Header("Object settings: ")]
        [SerializeField] private GameObject lamp;
        [SerializeField] private Renderer lampRenderer;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOn;
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOff;

        private Light lampLight;
        private Material bulbMaterial;
        private AudioSource audioSource;
        private bool lampIsActive;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            lampLight = lamp.GetComponentInChildren<Light>();
        }

        private void Start()
        {
            if (lampRenderer != null)
            {
                bulbMaterial = lampRenderer.material;
            }

            lampIsActive = lampLight.isActiveAndEnabled;
        }

        private void Tooglelamp()
        {
            if (bulbMaterial != null)
            {
                ToogleEmission();
            }

            lampLight.enabled = !lampLight.enabled;
            lampIsActive = !lampIsActive;
            audioSource.Play();
        }

        private void ToogleEmission()
        {
            if (!lampLight.enabled)
            {
                bulbMaterial.EnableKeyword("_EMISSION");
            }
            else
            {
                bulbMaterial.DisableKeyword("_EMISSION");
            }
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return true;
        }

        public InteractionTextSO RetrieveInteractionSO()
        {
            return lampIsActive? interactionTextsSO_turnOff : interactionTextsSO_turnOn;
        }

        public bool Interact(PlayerInteraction interactor)
        {
            Tooglelamp();
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