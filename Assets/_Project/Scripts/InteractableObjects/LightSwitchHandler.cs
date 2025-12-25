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
            if (lightsAreActive)
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

            (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);
            audioSource.clip = clickSound;
            audioSource.pitch = Random.Range(0.9f, 1.2f);

            foreach (Light singleLight in lights)
            {
                singleLight.enabled = !singleLight.enabled;
            }

            StartCoroutine(PlayLightsSwitchSoundCoroutine());
            lightsAreActive = !lightsAreActive;
        }

        private IEnumerator PlayLightsSwitchSoundCoroutine()
        {
            audioSource.Play();
            yield return new WaitUntil(() => !audioSource.isPlaying);
            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
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
                return;
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