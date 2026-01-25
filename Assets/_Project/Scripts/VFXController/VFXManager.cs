using System.Collections;
using TwelveG.AudioController;
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

        [Header("Settings")]
        [SerializeField, Range(0f, 1f)] private float electricFaintCoefficient = 0.1f;

        // --- DEPENDENCIAS ---
        private PostProcessingHandler postProcessingHandler;
        private Transform playerTransform;

        // --- EFFECT HANDLERS ---
        private HeadacheEffectHandler headacheHandler;
        private ElectricFeelHandler electricFeelHandler;
        private HallucinationHandler hallucinationHandler;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            postProcessingHandler = GetComponentInChildren<PostProcessingHandler>();

            if (postProcessingHandler == null)
            {
                Debug.LogError("VFXManager: Falta PostProcessingHandler");
                this.enabled = false;
                return;
            }

            SceneEnum currentScene = SceneUtils.RetrieveCurrentSceneEnum();

            if (currentScene == SceneEnum.Evening)
            {
                headacheHandler = GetComponent<HeadacheEffectHandler>();
                electricFeelHandler = GetComponent<ElectricFeelHandler>();

                if (headacheHandler == null)
                {
                    Debug.LogError("VFXManager: Falta HeadacheEffectHandler");
                    this.enabled = false;
                    return;
                }

                if (electricFeelHandler == null)
                {
                    Debug.LogError("VFXManager: Falta ElectricFeelHandler");
                    this.enabled = false;
                    return;
                }

                headacheHandler.Initialize(postProcessingHandler);
                electricFeelHandler.Initialize(postProcessingHandler);
            }
            else if( currentScene == SceneEnum.Night)
            {
                hallucinationHandler = GetComponent<HallucinationHandler>();

                if (hallucinationHandler == null)
                {
                    Debug.LogError("VFXManager: Falta HallucinationHandler");
                    this.enabled = false;
                    return;
                }

                hallucinationHandler.Initialize(postProcessingHandler);
            }
            else
            {
                Debug.LogWarning("VFXManager: No hay VFX configurados para esta escena.");
                this.enabled = false;
                return;
            }
        }

        // Llamado desde EventsHandler.cs al iniciar un nuevo evento para configurar los VFX iniciales
        public void UpdateSceneVFXSettings(EventsEnum eventEnum)
        {
            if (headacheGeneralConfigsSO == null)
            {
                Debug.LogWarning("[VFXManager]: Falta asignar el HeadacheFXGeneralConfigSO!");
                return;
            }

            if (electricFeelGeneralConfigsSO == null)
            {
                Debug.LogWarning("[VFXManager]: Falta asignar el ElectricFeelFXGeneralConfigSO!");
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
            if (electricFeelHandler.enabled)
            {
                electricFeelHandler.StartTransition(10f);
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

        public void SetFreeRoamElectricFeelIntensity(float newMultiplier, float freeRoamVolumeCoefficient)
        {
            electricFeelHandler.SetIntensity(newMultiplier);
            electricFeelHandler.SetAudioSettings(freeRoamVolumeCoefficient);
        }

        public void SetElectricFeelIntensity(float newMultiplier)
        {
            // Maximo valor en el que comienza el desmayo de Simón
            if (newMultiplier == 1f)
            {
                electricFeelHandler.SetAudioSettings(electricFaintCoefficient);
                // Deshabilita la capacidad de sprint del jugador
                playerTransform.GetComponentInParent<FPController>().EnableSprint(false);

                // Se activa el efecto de DoF, Vignette y el filtro pasa graves
                StartCoroutine(postProcessingHandler.DoFAndVignetteRoutine(9.3f, 0.3f, 1f, 6f));

                StartCoroutine(AudioManager.Instance.LowPassCorutine("inGameLowPassCutOff", 1500f, 12f));
            }

            electricFeelHandler.SetIntensity(newMultiplier);

            if (electricFeelHandler.enabled)
            {
                electricFeelHandler.StartTransition(newMultiplier < 1f ? 15f : 5f);
            }
        }

        public IEnumerator TriggerHallucinationEffect(HallucinationEffectType hallucinationEffectType)
        {
            yield return StartCoroutine(hallucinationHandler.TriggerHallucinationRoutine(hallucinationEffectType));
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