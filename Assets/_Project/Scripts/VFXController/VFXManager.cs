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
        private ElectricFeelHandler electricFeelHandler;
        // private HallucinationHandler hallucinationHandler; // Futuro efecto

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            headacheHandler = GetComponent<HeadacheEffectHandler>();
            electricFeelHandler = GetComponent<ElectricFeelHandler>();
            postProcessingHandler = GetComponentInChildren<PostProcessingHandler>();

            if (postProcessingHandler == null)
            {
                Debug.LogError("VFXManager: PostProcessingHandler missing!");
                this.enabled = false;
                return;
            }

            if (headacheHandler == null)
            {
                Debug.LogError("VFXManager: HeadacheEffectHandler missing!");
                this.enabled = false;
                return;
            }

            if (electricFeelHandler == null)
            {
                Debug.LogError("VFXManager: ElectricFeelHandler missing!");
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

            SceneVFXSettings settings = vfxGeneralConfig.GetVFXSettingsForEvenum(eventEnum);

            if (headacheHandler != null)
            {
                headacheHandler.SetIntensityMultiplier(settings.initialHeadacheIntensity);
                headacheHandler.SetVolumeCoefficient(settings.resonanceCoefficient);
            }
        }

        // Llamado desde PlayerInventory.cs al habilitar WalkieTalkie luego de recibir SO onEnablePlayerItem 
        public void EnableElectricFeelVFX(bool enable)
        {
            if (electricFeelHandler != null)
            {
                electricFeelHandler.enabled = enable;
            }
        }

        public void RegisterPlayer(Transform pTransform)
        {
            headacheHandler?.SetPlayer(pTransform);

            // TODO: a futuro efecto ..
            // hallucinationHandler?.SetPlayer(pTransform);
        }

        // Llamado por ResonanceZone.cs
        public void ResonanceZoneEntered(Transform senderTransform, float zoneRadius, float minDistanceForMaxImpact)
        {
            headacheHandler?.EnterZone(senderTransform, zoneRadius, minDistanceForMaxImpact);
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