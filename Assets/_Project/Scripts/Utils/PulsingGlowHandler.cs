using UnityEngine;

namespace TwelveG.Utils
{
    public class PulsingGlowHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Color glowColor = Color.red;
        [SerializeField, Range(0f, 100f)] private float minIntensity = 3f;
        [SerializeField, Range(0f, 100f)] private float maxIntensity = 5f;
        [SerializeField, Range(0.1f, 25f)] private float pulseSpeed = 3f;

        [Header("Optional References")]
        [Tooltip("Si se deja vacío, busca el Renderer en este objeto.")]
        [SerializeField] private Renderer targetRenderer;
        
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private Material targetMaterial;
        private float currentIntensity;

        private void OnEnable()
        {
            if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();

            targetMaterial = targetRenderer.material;
            
            targetMaterial.EnableKeyword("_EMISSION");
        }

        private void Update()
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

            currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, t);

            Color finalColor = glowColor * currentIntensity;

            targetMaterial.SetColor(EmissionColor, finalColor);
        }
    }
}