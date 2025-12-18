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
    
        // private Vignette vignette;
        // private ChromaticAberration chromaticAberration;

        private void Awake()
        {
            if (headacheVolume != null)
            {
                Debug.Log("[PostProcessingHandler]: Headache Volume assigned via Inspector.");
            }
            if (electricFeelVolume != null)
            {
                Debug.Log("[PostProcessingHandler]: Electric Feel Volume assigned via Inspector.");
            }
        }

        // Control global de intensidad para efecto Headache
        public void SetHeadacheWeight(float weight)
        {
            if (headacheVolume != null)
            {
                headacheVolume.weight = Mathf.Clamp01(weight);
            }
        }

        // Control global de intensidad para efecto Electric Feel
        public void SetElectricFeelWeight(float weight)
        {
            if (electricFeelVolume != null)
            {
                electricFeelVolume.weight = Mathf.Clamp01(weight);
            }
        }

        // Control individual de efecto Vignette
        // public void SetVignetteIntensity(float intensity)
        // {
        //     if (vignette != null)
        //     {
        //         vignette.intensity.value = intensity;
        //     }
        // }
    }
}