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
        [SerializeField] private float maxEffectDistanceOffset = 1.0f;
        [Tooltip("Multiplicador de intensidad del efecto de resonancia")]
        private float resonanceIntensityMultiplier = 1.0f;

        [Header("Occlusion Settings")]
        [Tooltip("Capas que bloquean el efecto (Paredes, Suelos, Puertas Cerradas).")]
        [SerializeField] private LayerMask obstacleLayer;

        [Header("Audio")]
        [SerializeField] private AudioClip headacheAudioClip;

        private float currentAppliedIntensity = 0f;
        private float currentMaxEffectDistance = 5f;
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

            if (activeResonanceZone != null && playerTransform != null)
            {
                // CHEQUEO DE OCLUSIÓN POR LAYERS COMO "DEFAULT" (PAREDES, SUELOS, ETC)
                Vector3 directionToPlayer = playerTransform.position - activeResonanceZone.position;
                float distance = directionToPlayer.magnitude;

                // Rayo entre zona de resonancia y jugador. Si pega en algo del layer, no hay efecto
                if (Physics.Raycast(activeResonanceZone.position, directionToPlayer.normalized, out RaycastHit hit, distance, obstacleLayer))
                {
                    targetIntensity = 0f;
                }
                else
                {
                    // Usamos currentMaxEffectDistance que viene del radio de la esfera actual
                    float rawIntensity = Mathf.InverseLerp(currentMaxEffectDistance, maxEffectDistanceOffset, distance);
                    targetIntensity = rawIntensity * resonanceIntensityMultiplier;
                }
            }

            currentAppliedIntensity = Mathf.Lerp(currentAppliedIntensity, targetIntensity, Time.deltaTime * effectSmoothSpeed);

            if (postProcessingHandler != null)
            {
                postProcessingHandler.SetHeadacheWeight(currentAppliedIntensity);
            }

            if (headacheAudioSource != null && headacheAudioSource.isPlaying)
            {
                if (currentAppliedIntensity < 0.1f && activeResonanceZone == null)
                {
                    StopHeadacheAudio();
                }
            }
        }

        public void ResonanceZoneEntered(Transform senderTransform, float zoneRadius)
        {
            activeResonanceZone = senderTransform;

            // ACTUALIZAMOS LA DISTANCIA MÁXIMA BASADA EN EL COLLIDER
            currentMaxEffectDistance = zoneRadius;

            if (headacheAudioClip != null && (headacheAudioSource == null || !headacheAudioSource.isPlaying))
            {
                headacheAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
                if (headacheAudioSource != null)
                {
                    audioSourceState = headacheAudioSource.GetSnapshot();
                    headacheAudioSource.transform.position = senderTransform.position;
                    headacheAudioSource.maxDistance = currentMaxEffectDistance;
                    headacheAudioSource.clip = headacheAudioClip;
                    headacheAudioSource.loop = true;
                    headacheAudioSource.volume = resonanceIntensityMultiplier * 0.75f;
                    headacheAudioSource.Play();
                }
            }
        }

        public void ResonanceZoneExited()
        {
            activeResonanceZone = null;
        }

        private void StopHeadacheAudio()
        {
            if (headacheAudioSource != null)
            {
                headacheAudioSource.Stop();
                headacheAudioSource.RestoreSnapshot(audioSourceState);
            }
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