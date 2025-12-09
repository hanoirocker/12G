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
        [SerializeField] private GameObject lamp;
        [SerializeField] private Renderer lampRenderer;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOn;
        [SerializeField] private InteractionTextSO interactionTextsSO_turnOff;

        [Header("Audio settings")]

        public bool makesClickSound = true;
        [SerializeField] private AudioClip clickSound;
        [SerializeField, Range(0f, 1f)] private float clipsVolume = 1f;

        private Light lampLight;
        private Material bulbMaterial;
        private AudioSource audioSource;
        private AudioSourceState audioSourceState;
        private bool lampIsActive;

        private void Awake()
        {
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

            if (makesClickSound && clickSound != null)
            {
               (audioSource, audioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(gameObject.transform, clipsVolume);
                audioSource.clip = clickSound;
                audioSource.pitch = Random.Range(0.78f, 1.15f);
            }

            lampLight.enabled = !lampLight.enabled;
            lampIsActive = !lampIsActive;
            StartCoroutine(MakeClickSoundRoutine());
        }

        private IEnumerator MakeClickSoundRoutine()
        {
            audioSource.Play();
            yield return new WaitUntil(() => !audioSource.isPlaying);
            AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
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

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return lampIsActive ? interactionTextsSO_turnOff : interactionTextsSO_turnOn;
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