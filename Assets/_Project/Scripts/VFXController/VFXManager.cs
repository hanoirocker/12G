using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.VFXController
{
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private VFXGeneralConfigSO vfxGeneralConfig;

        // --- DEPENDENCIAS ---
        private PostProcessingHandler postProcessingHandler;

        // --- EFFECT HANDLERS ---
        private HeadacheEffectHandler headacheHandler;
        // private HallucinationHandler hallucinationHandler; // Futuro efecto

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            headacheHandler = GetComponent<HeadacheEffectHandler>();
            postProcessingHandler = GetComponentInChildren<PostProcessingHandler>();

            if (postProcessingHandler == null)
            {
                Debug.LogError("VFXManager: PostProcessingHandler missing!");
                this.enabled = false;
                return;
            }

            // Inyectamos las dependencias globales a los workers
            if (headacheHandler != null && postProcessingHandler != null)
            {
                headacheHandler.Initialize(postProcessingHandler);
            }
        }

        // Llamado desde EventsHandler.cs al iniciar un nuevo evento para configurar los VFX iniciales
        public void InitializeVFXSettings(EventsEnum eventEnum)
        {
            if (vfxGeneralConfig == null)
            {
                Debug.LogError("[VFXManager]: Falta asignar el VFXGeneralConfigSO.");
                return;
            }

            float initialIntensity = vfxGeneralConfig.GetIntensityForScene(eventEnum);

            if (headacheHandler != null)
            {
                headacheHandler.SetIntensityMultiplier(initialIntensity);
                headacheHandler.CalculateEffectCoefficients();
            }
        }

        public void RegisterPlayer(Transform pTransform)
        {
            headacheHandler?.SetPlayer(pTransform);

            // TODO: a futuro efecto ..
            // hallucinationHandler?.SetPlayer(pTransform);
        }

        // Llamado por ResonanceZone.cs
        public void ResonanceZoneEntered(Transform senderTransform, float zoneRadius)
        {
            headacheHandler?.EnterZone(senderTransform, zoneRadius);
        }

        public void ResonanceZoneExited()
        {
            headacheHandler?.ExitZone();
        }

        public void SetResonanceIntensityMultiplier(float newMultiplier)
        {
            headacheHandler?.SetIntensityMultiplier(newMultiplier);
        }
    }
}