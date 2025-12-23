using TwelveG.GameController;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.VFXController
{
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private VFXGeneralConfigSO headacheGeneralConfigsSO;
        [SerializeField] private VFXGeneralConfigSO electricFeelGeneralConfigsSO;

        // --- DEPENDENCIAS ---
        private PostProcessingHandler postProcessingHandler;
        private Transform playerTransform;

        // --- EFFECT HANDLERS ---
        private HeadacheEffectHandler headacheHandler;
        private ElectricFeelHandler electricFeelHandler;

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
            headacheHandler.Initialize(postProcessingHandler);
            electricFeelHandler.Initialize(postProcessingHandler);
        }

        // Llamado desde EventsHandler.cs al iniciar un nuevo evento para configurar los VFX iniciales
        public void UpdateSceneVFXSettings(EventsEnum eventEnum)
        {
            if (headacheGeneralConfigsSO == null)
            {
                Debug.LogError("[VFXManager]: Falta asignar el HeadacheFXGeneralConfigSO!");
                return;
            }

            if (electricFeelGeneralConfigsSO == null)
            {
                Debug.LogError("[VFXManager]: Falta asignar el ElectricFeelFXGeneralConfigSO!");
                return;
            }

            SceneVFXSettings headachesSettings = headacheGeneralConfigsSO.GetVFXSettingsForEvenum(eventEnum);
            SceneVFXSettings electricFeelSettings = electricFeelGeneralConfigsSO.GetVFXSettingsForEvenum(eventEnum);

            if (headacheHandler != null)
            {
                headacheHandler.SetIntensityMultiplier(headachesSettings.effectIntensity);
                headacheHandler.SetAudioSettings(headachesSettings.volumeCoefficient);
            }
            if (electricFeelHandler != null)
            {
                electricFeelHandler.SetIntensity(electricFeelSettings.effectIntensity);
                electricFeelHandler.SetAudioSettings(electricFeelSettings.volumeCoefficient);
            }

            // Si el efecto ya estaba habilitado, actualizamos su estado de intensidad y volumen
            if(electricFeelHandler.enabled)
            {
                electricFeelHandler.StartTransition(20f);
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
            playerTransform = pTransform;
            headacheHandler?.SetPlayer(pTransform);
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

        // Ejecución forzada de cambios de intensidad.
        public void SetResonanceIntensityMultiplier(float newMultiplier)
        {
            headacheHandler?.SetIntensityMultiplier(newMultiplier);
        }

        public void SetElectricFeelIntensity(float newMultiplier)
        {
            Debug.Log("[VFXManager] SetElectricFeelIntensity to " + newMultiplier);
            electricFeelHandler.SetIntensity(newMultiplier);
        }

        public void TriggerProceduralFaint()
        {
            ProceduralFaint proceduralFaint = playerTransform?.GetComponentInParent<ProceduralFaint>();
            if (proceduralFaint != null)
            {
                proceduralFaint.enabled = true;
            }
        }
    }
}