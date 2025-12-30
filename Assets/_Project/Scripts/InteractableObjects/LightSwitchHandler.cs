using TwelveG.PlayerController;
using TwelveG.Localization;
using UnityEngine;
using TwelveG.AudioController;
using System.Collections;

namespace TwelveG.InteractableObjects
{
    public class LightSwitchHandler : MonoBehaviour, IInteractable
    {
        [Header("Object settings: ")]
        [SerializeField] private bool isTableLamp;
        public bool itWorks = true;
        [SerializeField] private Light[] lights;
        [SerializeField] private Renderer[] bulbsRenderers;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOn;
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOff;

        [Header("Audio settings")]

        public bool makesClickSound = true;
        [SerializeField] private AudioClip clickSound;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 1f;

        private AudioSource audioSource;
        private AudioSourceState audioSourceState;
        private bool lightsAreActive;

        private void Start()
        {
            if (lightsAreActive && itWorks)
            {
                foreach (Light singleLight in lights)
                {
                    singleLight.enabled = true;
                }

                ToogleEmissions();
            }
        }

        private void ToogleLights()
        {
            ToogleEmissions();

            foreach (Light singleLight in lights)
            {
                singleLight.enabled = !singleLight.enabled;
            }

            StartCoroutine(PlayLightsSwitchSoundCoroutine());
            lightsAreActive = !lightsAreActive;
        }

        private IEnumerator PlayLightsSwitchSoundCoroutine()
        {
            (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);
            audioSource.clip = clickSound;
            audioSource.pitch = Random.Range(0.9f, 1.2f);

            audioSource.Play();
            yield return new WaitUntil(() => !audioSource.isPlaying);
            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
            audioSource = null;
        }

        public void ToogleEmissions()
        {
            if (isTableLamp)
            {
                Material bulbMaterial = bulbsRenderers[0].materials[1];
                if (!lightsAreActive)
                {
                    bulbMaterial.EnableKeyword("_EMISSION");
                }
                else
                {
                    bulbMaterial.DisableKeyword("_EMISSION");
                }
            }
            else
            {
                foreach (Renderer bulbRenderer in bulbsRenderers)
                {
                    Material bulbMaterial = bulbRenderer.material;

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
        }

        public bool CanBeInteractedWith(PlayerInteraction playerCamera)
        {
            return true;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            if (!itWorks) return null;

            return lightsAreActive ? interactionTextsSO_turnOff : interactionTextsSO_turnOn;
        }

        public bool Interact(PlayerInteraction interactor)
        {
            if (!itWorks)
            {
                StartCoroutine(PlayLightsSwitchSoundCoroutine());
                return false;
            }
            else
            {
                ToogleLights();
                return true;
            }
        }

        public bool VerifyIfPlayerCanInteract(PlayerInteraction interactor)
        {
            return true;
        }

        public (ObservationTextSO, float timeUntilShown) GetFallBackText()
        {
            return (null, 0f);
        }
    }
}