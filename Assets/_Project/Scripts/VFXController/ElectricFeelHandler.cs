using System.Collections;
using TwelveG.AudioController;
using UnityEngine;

namespace TwelveG.VFXController
{
    public class ElectricFeelHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Range(0f, 180)] private float effectFadeInDuration = 5f;
        [Tooltip("Duración de la transición rápida al sacar/guardar el objeto.")]
        [SerializeField, Range(0.1f, 5f)] private float itemTransitionDuration = 2.0f;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip electricFeelClip;
        [SerializeField] private AudioClip faintingClip;

        private AudioClip currentAudioClip;
        private bool isEffectEnabled = false;
        private bool isItemShown = false;

        private float maxEventIntensity = 0f;
        private float currentTargetIntensity = 0f;
        private float minIntensityCoefficient = 0.5f;

        private float currentWeight = 0f;
        private float currentVolume = 0f;

        private float effectMaxVolume = 1f;

        private Coroutine updateEffectCoroutine;
        private PostProcessingHandler postProcessingHandler;

        private AudioSource electricFeelAudioSource;
        private AudioSourceState audioSourceState;

        private void OnEnable()
        {
            currentWeight = 0f;
            currentVolume = 0f;
            RecalculateTargetIntensity();

            // Solo pedimos fuente si realmente vamos a sonar (intensidad > 0)
            if (currentTargetIntensity > 0 || isEffectEnabled)
            {
                StartTransition(effectFadeInDuration);
            }
        }

        private void OnDisable()
        {
            ReleaseAudioSource();

            if (postProcessingHandler != null)
                postProcessingHandler.SetElectricFeelWeight(0f);

            currentWeight = 0f;
        }

        private void ReleaseAudioSource()
        {
            if (electricFeelAudioSource != null)
            {
                AudioUtils.StopAndRestoreAudioSource(electricFeelAudioSource, audioSourceState);
                electricFeelAudioSource = null;
            }
        }

        private void RecalculateTargetIntensity()
        {
            if (!isEffectEnabled)
            {
                currentTargetIntensity = 0f;
            }
            else
            {
                currentTargetIntensity = isItemShown ? maxEventIntensity : maxEventIntensity * minIntensityCoefficient * 0.5f;
            }
        }

        public void StartTransition(float duration)
        {
            // Solo pedimos fuente si no tenemos una
            if (electricFeelAudioSource == null)
            {
                electricFeelAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);

                if (electricFeelAudioSource != null)
                {
                    audioSourceState = electricFeelAudioSource.GetSnapshot();
                    electricFeelAudioSource.spatialBlend = 0f;
                    electricFeelAudioSource.loop = true;
                    electricFeelAudioSource.volume = 0f;
                }
            }

            // Si falló al pedir fuente, salimos para evitar errores
            if (electricFeelAudioSource == null) return;

            electricFeelAudioSource.clip = currentAudioClip;

            if (!electricFeelAudioSource.isPlaying)
            {
                electricFeelAudioSource.Play();
            }

            if (updateEffectCoroutine != null) StopCoroutine(updateEffectCoroutine);
            updateEffectCoroutine = StartCoroutine(UpdateEffectCoroutine(duration));
        }

        private IEnumerator UpdateEffectCoroutine(float duration)
        {
            float startWeight = currentWeight;
            float startVolume = currentVolume;

            float targetVolume = (currentTargetIntensity > 0) ? effectMaxVolume * (currentTargetIntensity / maxEventIntensity) : 0f;

            if (duration <= 0.01f) duration = 0.1f;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                currentWeight = Mathf.Lerp(startWeight, currentTargetIntensity, t);
                currentVolume = Mathf.Lerp(startVolume, targetVolume, t);

                postProcessingHandler.SetElectricFeelWeight(currentWeight);

                if (electricFeelAudioSource != null)
                    electricFeelAudioSource.volume = currentVolume;

                yield return null;
            }

            currentWeight = currentTargetIntensity;
            currentVolume = targetVolume;

            if (postProcessingHandler != null)
                postProcessingHandler.SetElectricFeelWeight(currentWeight);

            if (electricFeelAudioSource != null)
                electricFeelAudioSource.volume = currentVolume;

            // Si el volumen llegó a 0 y la intención es apagarse, liberamos la fuente
            if (currentVolume <= 0.01f && currentTargetIntensity <= 0.01f)
            {
                ReleaseAudioSource();
            }
        }

        public void SetAudioSettings(float volume)
        {
            effectMaxVolume = volume;
        }

        public void SetIntensity(float multiplier)
        {
            maxEventIntensity = multiplier;

            if (multiplier > 0f && multiplier < 1f)
            {
                isEffectEnabled = true;
                currentAudioClip = electricFeelClip;
            }
            else if (multiplier == 0f)
            {
                isEffectEnabled = false;
                currentAudioClip = null;
                // Al poner 0, el UpdateEffectCoroutine bajará el volumen y eventualmente llamará a ReleaseAudioSource
            }
            else if (multiplier == 1f)
            {
                isEffectEnabled = true;

                if (electricFeelAudioSource != null && electricFeelAudioSource.isPlaying)
                {
                    electricFeelAudioSource.Stop();
                }
                currentAudioClip = faintingClip;
            }

            RecalculateTargetIntensity();

            // Si habilitamos el efecto y no hay corrutina corriendo, iniciamos una transición
            if (isEffectEnabled)
            {
                StartTransition(effectFadeInDuration);
            }
        }

        public void ReduceEffectCoefficient(Component sender, object data)
        {
            if (sender.gameObject.name != "WalkieTalkie")
            {
                return;
            }

            isItemShown = (bool)data;
            RecalculateTargetIntensity();

            StartTransition(itemTransitionDuration);
        }

        public void Initialize(PostProcessingHandler ppHandler)
        {
            this.postProcessingHandler = ppHandler;
        }
    }
}