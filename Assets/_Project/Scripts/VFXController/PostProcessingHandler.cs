using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TwelveG.VFXController
{
    [RequireComponent(typeof(Volume))]
    public class PostProcessingHandler : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Si se deja vacío, buscará el componente en este mismo objeto.")]
        [SerializeField] private Volume volume;

        // Referencias cacheadas a los efectos individuales
        // (Por si queremos animar uno solo, como el latido de la viñeta)
        private Vignette vignette;
        private ChromaticAberration chromaticAberration;
        private LensDistortion lensDistortion;

        private void Awake()
        {
            if (volume == null) 
                volume = GetComponent<Volume>();

            if (volume.profile == null)
            {
                Debug.LogError($"[PostProcessingHandler]: El volumen en {gameObject.name} no tiene un Perfil asignado.");
                return;
            }

            // Cachear los efectos individuales para acceso rápido
            // Nota: TryGet devuelve false si el efecto no está añadido en el perfil del editor.
            volume.profile.TryGet(out vignette);
            volume.profile.TryGet(out chromaticAberration);
            volume.profile.TryGet(out lensDistortion);
        }

        /// <summary>
        /// Controla la intensidad global de todo el efecto de dolor de cabeza.
        /// 0 = Apagado (Visión normal).
        /// 1 = Máximo dolor (Todos los efectos al tope configurado en el perfil).
        /// </summary>
        /// <param name="weight">Valor entre 0 y 1.</param>
        public void SetHeadacheWeight(float weight)
        {
            if (volume != null)
            {
                volume.weight = Mathf.Clamp01(weight);
            }
        }

        /// <summary>
        /// Permite modificar la intensidad de la viñeta independientemente del peso global.
        /// Útil para efectos de "latido" (pulsing).
        /// </summary>
        public void SetVignetteIntensity(float intensity)
        {
            if (vignette != null)
            {
                vignette.intensity.value = intensity;
            }
        }

        /// <summary>
        /// Permite modificar la intensidad de la aberración cromática independientemente.
        /// </summary>
        public void SetChromaticAberration(float intensity)
        {
            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.value = intensity;
            }
        }
    }
}