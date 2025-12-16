using UnityEngine;
using TwelveG.AudioController;

namespace TwelveG.VFXController
{
    public class HeadacheEffectHandler : MonoBehaviour
    {
        [Header("Headache Specific Settings")]
        [SerializeField] private float effectSmoothSpeed = 5f;
        [SerializeField] private float maxEffectDistanceOffset = 1.0f;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private AudioClip headacheAudioClip;

        // Estado interno
        private bool isEffectEnabled = true;
        private float resonanceIntensityMultiplier = 1.0f;
        private float currentAppliedIntensity = 0f;
        
        // Datos de la zona actual
        private Transform activeResonanceZone;
        private float currentMaxEffectDistance;

        // Dependencias inyectadas por el Manager
        private Transform playerTransform;
        private PostProcessingHandler postProcessingHandler;
        
        private AudioSource headacheAudioSource;
        private AudioSourceState audioSourceState;

        public void Initialize(PostProcessingHandler ppHandler)
        {
            this.postProcessingHandler = ppHandler;
        }

        public void SetPlayer(Transform player)
        {
            this.playerTransform = player;
        }

        private void Update()
        {
            if (activeResonanceZone == null && currentAppliedIntensity <= 0.001f) 
                return;

            CalculateLogic();
        }

        private void CalculateLogic()
        {
            float targetIntensity = 0f;

            // Solo calculamos objetivo si hay zona activa, el efecto está habilitado y tenemos player
            if (activeResonanceZone != null && isEffectEnabled && playerTransform != null)
            {
                Vector3 directionToPlayer = playerTransform.position - activeResonanceZone.position;
                float distance = directionToPlayer.magnitude;

                // Raycast de oclusión
                if (Physics.Raycast(activeResonanceZone.position, directionToPlayer.normalized, out RaycastHit hit, distance, obstacleLayer))
                {
                    targetIntensity = 0f;
                }
                else
                {
                    float rawIntensity = Mathf.InverseLerp(currentMaxEffectDistance, maxEffectDistanceOffset, distance);
                    targetIntensity = rawIntensity * resonanceIntensityMultiplier;
                }
            }

            // Interpolación
            currentAppliedIntensity = Mathf.Lerp(currentAppliedIntensity, targetIntensity, Time.deltaTime * effectSmoothSpeed);

            // Aplicar al PostProcessing
            if (postProcessingHandler != null)
            {
                postProcessingHandler.SetHeadacheWeight(currentAppliedIntensity);
            }

            HandleAudio();
        }

        private void HandleAudio()
        {
            if (headacheAudioSource != null && headacheAudioSource.isPlaying)
            {
                if (currentAppliedIntensity < 0.1f && activeResonanceZone == null)
                {
                    StopAudio();
                }
            }
        }

        // --- Métodos Controlados por el Manager ---

        public void EnterZone(Transform zone, float radius)
        {
            if (!isEffectEnabled) return;

            activeResonanceZone = zone;
            currentMaxEffectDistance = radius;
            PlayAudio(zone.position, radius);
        }

        public void ExitZone()
        {
            activeResonanceZone = null;
        }

        public void SetIntensityMultiplier(float multiplier)
        {
            if (multiplier > 0f)
            {
                isEffectEnabled = true;
                resonanceIntensityMultiplier = multiplier;
            }
            else
            {
                if (headacheAudioSource != null) StopAudio();
                isEffectEnabled = false;
                resonanceIntensityMultiplier = 0f;
            }
        }

        private void PlayAudio(Vector3 position, float maxDist)
        {
            if (headacheAudioClip != null && (headacheAudioSource == null || !headacheAudioSource.isPlaying))
            {
                headacheAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
                if (headacheAudioSource != null)
                {
                    audioSourceState = headacheAudioSource.GetSnapshot();
                    headacheAudioSource.transform.position = position;
                    headacheAudioSource.maxDistance = maxDist;
                    headacheAudioSource.clip = headacheAudioClip;
                    headacheAudioSource.loop = true;
                    headacheAudioSource.volume = resonanceIntensityMultiplier * 0.75f;
                    headacheAudioSource.Play();
                }
            }
        }

        private void StopAudio()
        {
            if (headacheAudioSource != null)
            {
                headacheAudioSource.Stop();
                headacheAudioSource.RestoreSnapshot(audioSourceState);
                headacheAudioSource = null;
            }
        }
    }
}