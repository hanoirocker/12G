using UnityEngine;
using TwelveG.AudioController;

namespace TwelveG.VFXController
{
    public class HeadacheEffectHandler : MonoBehaviour
    {
        [Header("Headache Specific Settings")]
        [SerializeField] private float effectSmoothSpeed = 5f;
        [SerializeField] private float maxEffectDistanceOffset = 0.5f;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private AudioClip headacheAudioClip;

        // Estado interno
        private bool isEffectEnabled = true;
        private float resonanceIntensityMultiplier = 0f;
        private float currentAppliedIntensity = 0f;
        private float resonanceVolumeCoefficient = 1f;

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
                headacheAudioSource.volume = currentAppliedIntensity * resonanceVolumeCoefficient;

                if (currentAppliedIntensity < 0.02f && activeResonanceZone == null)
                {
                    StopAudio();
                }
            }
        }

        // --- Métodos Controlados por el Manager ---
        // El radio real se calculo en el ResonanceZone.cs
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
                isEffectEnabled = false;
                resonanceIntensityMultiplier = 0f;
            }
        }

        public void CalculateEffectCoefficients()
        {
            if (resonanceIntensityMultiplier > 0f && resonanceIntensityMultiplier <= 0.1f)
            {
                resonanceVolumeCoefficient = 0.075f;
            }
            else if (resonanceIntensityMultiplier > 0.1f && resonanceIntensityMultiplier <= 0.3f)
            {
                resonanceVolumeCoefficient = 0.1f;
            }
            else if (resonanceIntensityMultiplier > 0.3f && resonanceIntensityMultiplier <= 0.6f)
            {
                resonanceVolumeCoefficient = 0.4f;
            }
            else if (resonanceIntensityMultiplier > 0.6f && resonanceIntensityMultiplier < 1f)
            {
                resonanceVolumeCoefficient = 0.7f;
            }
            else
            {
                resonanceVolumeCoefficient = 1f;
            }

            // Debug.Log($"[HeadacheEffectHandler]: Coeficiente de volumen ajustado a {resonanceVolumeCoefficient} para multiplicador de intensidad {resonanceIntensityMultiplier}.");
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
                    headacheAudioSource.volume = 0f;
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