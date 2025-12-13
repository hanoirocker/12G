using TwelveG.AudioController;
using UnityEngine;

namespace TwelveG.VFXController
{
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private PostProcessingHandler postProcessingHandler;

        [Header("Settings")]
        [SerializeField] private float effectSmoothSpeed = 5f;
        [Tooltip("Distancia total donde el efecto llega a 0.")]
        [SerializeField] private float maxEffectDistance = 5f;
        [Tooltip("Distancia desde el centro donde el efecto se mantiene al máximo antes de empezar a decaer.")]
        [SerializeField] private float maxEffectDistanceOffset = 1.0f;
        [Tooltip("Multiplicador de intensidad del efecto de resonancia")]
        private float resonanceIntensityMultiplier = 1.0f;

        [Header("Audio")]
        [SerializeField] private AudioClip headacheAudioClip;

        private float currentAppliedIntensity = 0f;
        private Transform activeResonanceZone = null;
        private Transform playerTransform;
        private AudioSource headacheAudioSource;
        private AudioSourceState audioSourceState;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            if (postProcessingHandler == null)
            {
                Debug.LogError("VFXManager: PostProcessingHandler reference is missing!");
                this.enabled = false;
            }
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                Debug.LogError("VFXManager: Player object with tag 'Player' not found in the scene!");
                this.enabled = false;
            }
        }

        void Update()
        {
            if (activeResonanceZone != null || currentAppliedIntensity > 0.01f)
            {
                CalculateAndApplyHeadache();
            }
        }

        private void CalculateAndApplyHeadache()
        {
            float targetIntensity = 0f;

            // Calcular Intensidad Objetivo
            if (activeResonanceZone != null && playerTransform != null)
            {
                // Calcular distancia real
                float distance = Vector3.Distance(activeResonanceZone.position, playerTransform.position);

                float rawIntensity = Mathf.InverseLerp(maxEffectDistance, maxEffectDistanceOffset, distance);

                targetIntensity = rawIntensity * resonanceIntensityMultiplier;
            }

            // Interpolación suave (Lerp)
            currentAppliedIntensity = Mathf.Lerp(currentAppliedIntensity, targetIntensity, Time.deltaTime * effectSmoothSpeed);

            // Aplicar al Visual (PostProcessing)
            if (postProcessingHandler != null)
            {
                postProcessingHandler.SetHeadacheWeight(currentAppliedIntensity);
            }
            if (headacheAudioSource != null && headacheAudioSource.isPlaying)
            {
                if (currentAppliedIntensity < 0.01f || activeResonanceZone == null)
                {
                    headacheAudioSource.Stop();
                    headacheAudioSource.RestoreSnapshot(audioSourceState);
                    headacheAudioSource = null;
                }
            }
        }

        public void ResonanceZoneEntered(Transform senderTransform)
        {
            activeResonanceZone = senderTransform;

            if (headacheAudioClip != null)
            {
                headacheAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
                audioSourceState = headacheAudioSource.GetSnapshot();
                headacheAudioSource.transform.position = senderTransform.position;
                headacheAudioSource.clip = headacheAudioClip;
                headacheAudioSource.maxDistance = maxEffectDistance;
                headacheAudioSource.loop = true;
                headacheAudioSource.volume = resonanceIntensityMultiplier * 0.25f;
                headacheAudioSource.Play();
            }

        }

        public void ResonanceZoneExited()
        {
            activeResonanceZone = null;
        }

        // Método a llamar desde eventos corrutina o cualquier otro script
        public void SetResonanceIntensityMultiplier(float newMultiplier)
        {
            resonanceIntensityMultiplier = newMultiplier;
        }

        public void RegisterPlayer(Transform pTransform)
        {
            playerTransform = pTransform;
        }
    }
}