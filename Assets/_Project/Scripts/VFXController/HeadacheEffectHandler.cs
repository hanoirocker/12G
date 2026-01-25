using UnityEngine;
using TwelveG.AudioController;
using TwelveG.PlayerController;
using System.Collections;

namespace TwelveG.VFXController
{
    public class HeadacheEffectHandler : MonoBehaviour
    {
        [Header("Specific Settings")]
        [SerializeField] private float effectSmoothSpeed = 5f;
        [Tooltip("Intensity threshold to start applying dizziness effect (Includes disabling sprint).")]
        [SerializeField, Range(0f, 1f)] private float dizzinessThreshold = 0.5f;
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

            if (activeResonanceZone != null && isEffectEnabled && playerTransform != null)
            {
                Vector3 directionToPlayer = playerTransform.position - activeResonanceZone.position;
                float distance = directionToPlayer.magnitude;

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

            currentAppliedIntensity = Mathf.Lerp(currentAppliedIntensity, targetIntensity, Time.deltaTime * effectSmoothSpeed);

            if (postProcessingHandler != null)
            {
                postProcessingHandler.SetHeadacheWeight(currentAppliedIntensity);
            }

            if (currentAppliedIntensity > dizzinessThreshold)
            {
                if (!dizzinessEffectRunning)
                {
                    dizzinessEffectRunning = true;
                    dizzinessHandler.enabled = true;

                    HandleHeartBeatAudio(true);
                }

                if (fpController.IsSprinting())
                {
                    fpController.ToggleSprint(false);
                }

                dizzinessHandler.SetDizzinessIntensity(currentAppliedIntensity);
            }

            // Comunicar al PlayerController para detener el efecto de mareo
            if (currentAppliedIntensity <= dizzinessThreshold && dizzinessEffectRunning)
            {
                dizzinessEffectRunning = false;
                dizzinessHandler.enabled = false;

                HandleHeartBeatAudio(false);
            }

            HandleResonanceAudio();
        }

        private void HandleHeartBeatAudio(bool enable)
        {
            if (heartbeatCoroutine != null)
            {
                StopCoroutine(heartbeatCoroutine);
                heartbeatCoroutine = null;
            }

            if (enable)
            {
                // Solo pedimos fuente si no tenemos ya una
                if (heartBeatAudioSource == null)
                {
                    // Asumiendo que ReturnFreeAudioSource devuelve una fuente YA reservada/claimed
                    heartBeatAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);

                    if (heartBeatAudioSource != null)
                    {
                        heartBeatSourceState = heartBeatAudioSource.GetSnapshot();
                        heartBeatAudioSource.clip = heartbeatClip;
                        heartBeatAudioSource.loop = true;
                        heartBeatAudioSource.volume = 0f;
                        heartBeatAudioSource.Play();
                    }
                }

                if (heartBeatAudioSource != null)
                {
                    float targetVol = resonanceIntensityMultiplier * heartBeatVolumeMultiplier;
                    heartbeatCoroutine = StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(
                        heartBeatAudioSource, heartBeatAudioSource.volume, targetVol, heartBeatFadeInSpeed
                    ));
                }
            }
            else
            {
                if (heartBeatAudioSource != null)
                {
                    heartbeatCoroutine = StartCoroutine(FadeOutAndReturnRoutine());
                }
            }
        }

        private IEnumerator FadeOutAndReturnRoutine()
        {
            if (heartBeatAudioSource == null) yield break;

            yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(
                heartBeatAudioSource, heartBeatFadeOutSpeed
            ));

            if (heartBeatAudioSource != null)
            {
                AudioUtils.StopAndRestoreAudioSource(heartBeatAudioSource, heartBeatSourceState);
                heartBeatAudioSource = null;
            }
            heartbeatCoroutine = null;
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

        public void EnterZone(Transform zone, float radius, float minDistanceForMaxImpact)
        {
            if (!isEffectEnabled) return;

            maxEffectDistanceOffset = minDistanceForMaxImpact;
            activeResonanceZone = zone;
            currentMaxEffectDistance = radius;
            PlayResonanceAudio(zone.position, radius);
        }

        public void ExitZone()
        {
            activeResonanceZone = null;
            maxEffectDistanceOffset = 0.5f;

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
        }

        public void SetAudioSettings(float coefficient)
        {
            resonanceVolumeCoefficient = coefficient;
        }

        private void PlayResonanceAudio(Vector3 position, float maxDist)
        {
            if (resonanceAudioSource == null)
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
            else
            {
                // Si ya teníamos una fuente (ej: salimos y entramos rápido), actualizamos posición
                resonanceAudioSource.transform.position = position;
                resonanceAudioSource.maxDistance = maxDist;
                if (!resonanceAudioSource.isPlaying) resonanceAudioSource.Play();
            }
        }

        private void StopResonanceAudio()
        {
            if (resonanceAudioSource != null)
            {
                AudioUtils.StopAndRestoreAudioSource(resonanceAudioSource, resonanceSourceState);
                resonanceAudioSource = null;
            }
        }
    }
}