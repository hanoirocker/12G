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
        private bool isItemShown = false; // Guardamos el estado del objeto

        private float maxEventIntensity = 0f; // La intensidad "global" del evento
        private float currentTargetIntensity = 0f; // La meta actual (depende del item)
        private float minIntensityCoefficient = 0.5f;

        // Variables para rastrear el estado actual exacto
        private float currentWeight = 0f;
        private float currentVolume = 0f;

        private float effectMaxVolume = 1f;

        private Coroutine updateEffectCoroutine;
        private PostProcessingHandler postProcessingHandler;
        private AudioSource electricFeelAudioSource;
        private AudioSourceState audioSourceState;

        private void OnEnable()
        {
            // Reseteamos valores actuales al habilitar para empezar limpios
            currentWeight = 0f;
            currentVolume = 0f;

            // Calculamos objetivo inicial
            RecalculateTargetIntensity();

            StartTransition(effectFadeInDuration);
        }

        private void OnDisable()
        {
            if (electricFeelAudioSource != null)
            {
                electricFeelAudioSource.Stop();
                electricFeelAudioSource.RestoreSnapshot(audioSourceState);
                electricFeelAudioSource = null;
            }

            if (postProcessingHandler != null)
                postProcessingHandler.SetElectricFeelWeight(0f);

            currentWeight = 0f;
        }

        // Método centralizado para recalcular la meta basado en estado del evento y del item
        private void RecalculateTargetIntensity()
        {
            if (!isEffectEnabled)
            {
                currentTargetIntensity = 0f;
            }
            else
            {
                // Si el item está mostrado, usamos el 100% de la intensidad del evento.
                // Si no, usamos el coeficiente (50%).
                currentTargetIntensity = isItemShown ? maxEventIntensity : maxEventIntensity * minIntensityCoefficient * 0.5f;
            }
        }

        public void StartTransition(float duration)
        {
            if (electricFeelAudioSource == null)
            {
                electricFeelAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
                audioSourceState = electricFeelAudioSource.GetSnapshot();
                electricFeelAudioSource.spatialBlend = 0f;
                electricFeelAudioSource.loop = true;
                electricFeelAudioSource.volume = 0f;
            }

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

            // Calculamos el volumen objetivo proporcional a la intensidad
            // Si la intensidad objetivo es 0, volumen es 0. Si es maxEventIntensity, volumen es effectMaxVolume.
            // Usamos una regla de tres simple basada en la intensidad máxima posible (1.0f)
            float targetVolume = (currentTargetIntensity > 0) ? effectMaxVolume * (currentTargetIntensity / maxEventIntensity) : 0f;

            // Protección contra división por cero o duraciones muy cortas
            if (duration <= 0.01f) duration = 0.1f;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                // Lerp desde donde estábamos (startWeight) hasta la nueva meta
                currentWeight = Mathf.Lerp(startWeight, currentTargetIntensity, t);
                currentVolume = Mathf.Lerp(startVolume, targetVolume, t);

                postProcessingHandler.SetElectricFeelWeight(currentWeight);
                electricFeelAudioSource.volume = currentVolume;

                yield return null;
            }

            // Aseguramos valores finales exactos
            currentWeight = currentTargetIntensity;
            currentVolume = targetVolume;

            if (postProcessingHandler != null)
                postProcessingHandler.SetElectricFeelWeight(currentWeight);

            if (electricFeelAudioSource != null)
                electricFeelAudioSource.volume = currentVolume;
        }

        public void SetAudioSettings(float volume)
        {
            effectMaxVolume = volume;
        }

        // Llamado por VFXManager
        public void SetIntensity(float multiplier)
        {
            Debug.Log($"Updating to ...: {multiplier}");

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
            }
            else if (multiplier == 1f)
            {
                isEffectEnabled = true;

                if (electricFeelAudioSource.isPlaying)
                {
                    electricFeelAudioSource.Stop();
                }
                currentAudioClip = faintingClip;
            }

            RecalculateTargetIntensity();
        }

        // Callback del evento (OnItemShown / Hidden)
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