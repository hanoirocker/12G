using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TwelveG.VFXController
{
    [RequireComponent(typeof(Volume))]
    public class PostProcessingHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Volume headacheVolume;
        [SerializeField] private Volume electricFeelVolume;
        [SerializeField] private Volume space1Volume;

        // Variables para guardar las referencias a los Overrides del perfil
        private DepthOfField electricDoF;
        private Vignette electricVignette;

        public void SetHeadacheWeight(float weight)
        {
            if (headacheVolume != null) headacheVolume.weight = Mathf.Clamp01(weight);
        }

        public void SetElectricFeelWeight(float weight)
        {
            if (electricFeelVolume != null) electricFeelVolume.weight = Mathf.Clamp01(weight);
        }

        public IEnumerator SetRedHourWeight(float weight, float duration)
        {
            float initialWeight = space1Volume.weight;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float progress = timer / duration;
                space1Volume.weight = Mathf.Lerp(initialWeight, weight, progress);
                yield return null;
            }

            space1Volume.weight = Mathf.Clamp01(weight);
        }

        public IEnumerator DoFAndVignetteRoutine(float duration, float targetVignetteIntensity, float minFocusDist, float maxFocusDist)
        {

            if (electricVignette != null)
            {
                electricVignette.active = true;
                electricVignette.intensity.value = 0f;
            }

            if (electricDoF != null)
            {
                electricDoF.active = true;
            }

            float timer = 0f;
            float cycles = 6f; // Cantidad de oscilaciones
            float intialFocalLength = electricDoF.focalLength.value;
            float initialAperture = electricDoF.aperture.value;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float progress = timer / duration;

                // --- LÓGICA VIGNETTE (Lineal) ---
                if (electricVignette != null)
                {
                    electricVignette.intensity.value = Mathf.Lerp(0f, targetVignetteIntensity, progress * 2f); // 2 veces mas rapido
                }

                // --- LÓGICA DEPTH OF FIELD (Oscilatoria) ---
                if (electricDoF != null)
                {
                    float sineWave = (Mathf.Sin(progress * cycles * Mathf.PI * 2f) + 1f) / 2f;

                    electricDoF.focusDistance.value = Mathf.Lerp(minFocusDist, maxFocusDist, sineWave);
                    electricDoF.focalLength.value = Mathf.Lerp(intialFocalLength, 100, sineWave);
                    electricDoF.aperture.value = Mathf.Lerp(initialAperture, 0, sineWave);
                }

                yield return null;
            }

            if (electricVignette != null) electricVignette.intensity.value = targetVignetteIntensity;

            if (electricDoF != null) electricDoF.focusDistance.value = minFocusDist;
        }
    }
}