using UnityEngine;
using TwelveG.AudioController;
using TwelveG.PlayerController;
using System;
using System.Collections;

namespace TwelveG.VFXController
{
    public class HeadacheEffectHandler : MonoBehaviour
    {
        [Header("Specific Settings")]
        [SerializeField] private float effectSmoothSpeed = 5f;
        [Tooltip("Intensity threshold to start applying dizziness effect (Includes disabling sprint).")]
        [SerializeField, Range(0f, 1f)] private float dizzinessThreshold = 0.6f;
        [SerializeField] private LayerMask obstacleLayer;
        [Space]
        [Header("Audio Settings")]
        [SerializeField] private AudioClip resonanceClip;
        [SerializeField] private AudioClip heartbeatClip;
        [SerializeField, Range(2f, 10f)] private float heartBeatFadeInSpeed = 8f;
        [SerializeField, Range(2f, 10f)] private float heartBeatFadeOutSpeed = 5f;
        [SerializeField, Range(0f, 1f)] private float heartBeatVolumeMultiplier = 0.6f;

        // Estado interno
        private float maxEffectDistanceOffset = 0.5f;
        private bool isEffectEnabled = true;
        private float resonanceIntensityMultiplier = 0f;
        private float currentAppliedIntensity = 0f;
        private float resonanceVolumeCoefficient = 1f;
        private bool dizzinessEffectRunning = false;

        // Datos de la zona actual
        private Transform activeResonanceZone;
        private float currentMaxEffectDistance;

        // Dependencias inyectadas por el Manager
        private Transform playerTransform;
        private DizzinessHandler dizzinessHandler;
        private FPController fpController;
        private PostProcessingHandler postProcessingHandler;

        private AudioSource resonanceAudioSource;
        private AudioSourceState resonanceSourceState;
        private AudioSource heartBeatAudioSource;
        private AudioSourceState heartBeatSourceState;
        private Coroutine heartbeatCoroutine;

        public void Initialize(PostProcessingHandler ppHandler)
        {
            this.postProcessingHandler = ppHandler;
        }

        public void SetPlayer(Transform player)
        {
            playerTransform = player;
            dizzinessHandler = playerTransform.GetComponentInParent<DizzinessHandler>();
            fpController = playerTransform.GetComponentInParent<FPController>();
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

                    // Loggear distancia actual entre zona y player
                    // (Ideal para modificar minDistanceForMaxImpact en ResonanceZone.cs)
                    // Debug.Log($"Distancia a centro de zona: {distance}");
                }
            }

            // Interpolación
            currentAppliedIntensity = Mathf.Lerp(currentAppliedIntensity, targetIntensity, Time.deltaTime * effectSmoothSpeed);

            // Aplicar al PostProcessing
            if (postProcessingHandler != null)
            {
                postProcessingHandler.SetHeadacheWeight(currentAppliedIntensity);
            }

            // Comunicar al PlayerController para iniciar el efecto de mareo
            if (currentAppliedIntensity > dizzinessThreshold)
            {
                if (!dizzinessEffectRunning)
                {
                    dizzinessEffectRunning = true;
                    dizzinessHandler.enabled = true;

                    StartCoroutine(HandleHeartBeatAudio(true));
                }

                if (fpController.IsSprinting())
                {
                    fpController.EnableSprint(false);
                }

                dizzinessHandler.SetDizzinessIntensity(currentAppliedIntensity);
            }

            // Comunicar al PlayerController para detener el efecto de mareo
            if (currentAppliedIntensity <= dizzinessThreshold && dizzinessEffectRunning)
            {
                dizzinessEffectRunning = false;
                dizzinessHandler.enabled = false;

                StartCoroutine(HandleHeartBeatAudio(false));
            }
            
            HandleResonanceAudio();
        }

        private IEnumerator HandleHeartBeatAudio(bool enable)
        {
            if (enable)
            {
                AudioSource heartbeatSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
                if (heartbeatSource != null && !heartbeatSource.isPlaying)
                {
                    heartBeatAudioSource = heartbeatSource;
                    heartBeatSourceState = heartBeatAudioSource.GetSnapshot();
                    heartBeatAudioSource.volume = 0;
                    heartBeatAudioSource.clip = heartbeatClip;
                    heartBeatAudioSource.loop = true;
                    heartBeatAudioSource.Play();
                }

                if (heartbeatCoroutine != null)
                {
                    StopCoroutine(heartbeatCoroutine);
                    heartbeatCoroutine = null;
                }

                heartbeatCoroutine = StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(
                    heartBeatAudioSource, 0f, resonanceIntensityMultiplier * heartBeatVolumeMultiplier, heartBeatFadeInSpeed
                    ));
            }
            else
            {
                if (heartbeatCoroutine != null)
                {
                    StopCoroutine(heartbeatCoroutine);
                    heartbeatCoroutine = null;
                }

                if (heartBeatAudioSource != null)
                {
                    heartbeatCoroutine = StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(heartBeatAudioSource, heartBeatFadeOutSpeed));
                    yield return new WaitUntil(() => !heartBeatAudioSource.isPlaying);
                    heartBeatAudioSource.RestoreSnapshot(heartBeatSourceState);
                    heartBeatAudioSource = null;
                }
            }
        }

        private void HandleResonanceAudio()
        {
            if (resonanceAudioSource != null && resonanceAudioSource.isPlaying)
            {
                resonanceAudioSource.volume = currentAppliedIntensity * resonanceVolumeCoefficient;

                if (currentAppliedIntensity < 0.02f && activeResonanceZone == null)
                {
                    StopResonanceAudio();
                }
            }
        }

        // --- Métodos Controlados por el Manager ---
        // El radio real se calculo en el ResonanceZone.cs
        public void EnterZone(Transform zone, float radius, float minDistanceForMaxImpact)
        {
            if (!isEffectEnabled) return;

            maxEffectDistanceOffset = minDistanceForMaxImpact;
            activeResonanceZone = zone;
            currentMaxEffectDistance = radius;
            PlayAudio(zone.position, radius);
        }

        public void ExitZone()
        {
            activeResonanceZone = null;
            maxEffectDistanceOffset = 0.5f;
            fpController.EnableSprint(false);

            StopResonanceAudio();
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

            Debug.Log("[HeadacheEffectHandler]: SetIntensityMultiplier to " + resonanceIntensityMultiplier);
        }

        public void SetVolumeCoefficient(float coefficient)
        {
            resonanceVolumeCoefficient = coefficient;
            Debug.Log("[HeadacheEffectHandler]: SetVolumeCoefficient to " + resonanceVolumeCoefficient);
        }

        private void PlayAudio(Vector3 position, float maxDist)
        {
            if (resonanceClip != null && (resonanceAudioSource == null || !resonanceAudioSource.isPlaying))
            {
                resonanceAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
                if (resonanceAudioSource != null)
                {
                    resonanceSourceState = resonanceAudioSource.GetSnapshot();
                    resonanceAudioSource.transform.position = position;
                    resonanceAudioSource.maxDistance = maxDist;
                    resonanceAudioSource.clip = resonanceClip;
                    resonanceAudioSource.loop = true;
                    resonanceAudioSource.volume = 0f;
                    resonanceAudioSource.Play();
                }
            }
        }

        private void StopResonanceAudio()
        {
            if (resonanceAudioSource != null)
            {
                resonanceAudioSource.Stop();
                resonanceAudioSource.RestoreSnapshot(resonanceSourceState);
                resonanceAudioSource = null;
            }
        }
    }
}