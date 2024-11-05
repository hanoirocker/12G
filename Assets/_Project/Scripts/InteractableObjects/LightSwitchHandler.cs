namespace TwelveG.InteractableObjects
{
    using TwelveG.PlayerController;
    using UnityEngine;

    public class LightSwitchHandler : MonoBehaviour, IInteractable
    {
        [Header("Object settings: ")]
        [SerializeField] private InteractionTextsSO interactionTextsSO;
        [SerializeField] private GameObject lamp;
        [SerializeField] private Renderer lampRenderer;

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

        public void ToogleEmission()
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

        public string GetInteractionPrompt()
        {
            return lampIsActive ? "Apagar" : "Encender";
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

        public string GetFallBackText()
        {
            throw new System.NotImplementedException();
        }
    }

}