using System.Collections;
using Cinemachine;
using TwelveG.AudioController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.GameController
{
  public class VoicesEvent : GameEventBase
  {
    [Header("Timing")]
    [SerializeField, Range(1, 10)] private int initialTime = 2;

    [Space(5)]
    [Header("Text & Dialogs")]
    [SerializeField] private UIOptionsTextSO[] playerHelperDataTextSO;

    [Space(5)]
    [Header("Audio - Sequence")]
    [Tooltip("Música de tensión que incrementa mientras el enemigo avanza")]
    [SerializeField] private AudioClip tensionRampClip;

    [Space(5)]
    [Header("Timing - Sequence")]
    [SerializeField, Range(0f, 5f)] private float GARAGE_OPEN_WAIT = 3f;
    [SerializeField, Range(0f, 5f)] private float DRAMATIC_PAUSE = 1.5f;

    [Space(5)]
    [Header("Audio - Outcome")]
    [Tooltip("Sonido de miedo constante (cuando el enemigo ya es omnipresente)")]
    [SerializeField] private AudioClip constantFearClip;
    [Tooltip("Sonido de alivio/recuperar aliento (al entrar al depot a tiempo)")]
    [SerializeField] private AudioClip breathRecoveryClip;
    [SerializeField] private AudioClip jumpscareClip;

    private bool allowNextAction = false;
    private bool enemyIsOmnipresent = false;
    private bool playerIsSafe = false;

    private AudioSource tensionSource;

    public override IEnumerator Execute()
    {
      PlayerHandler playerHandler = PlayerHandler.Instance;
      GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

      yield return new WaitForSeconds(initialTime);

      // FIX: No sirve de nada cargar un dialogo si debe estar en loop
      // y el jugador debe poder guardar el walkie talkie.
      // TODO: Quizas un sonido en el player con una voz de fondo en loop?

      // Esperar a que el jugador salga de zonas seguras de arriba
      yield return new WaitUntil(
        () => playerHandler.GetCurrentHouseArea() != HouseArea.MiddleStairs
        && playerHandler.GetCurrentHouseArea() != HouseArea.LivingRoom
        && playerHandler.GetCurrentHouseArea() != HouseArea.None
      );

      // Luz y Puerta Depot
      ToggleDepotState(true);

      StartCoroutine(EnemyInvasionSequence());

      // Monitor de estado (Controla Audio, Muerte y Exito)
      yield return StartCoroutine(MonitorInvasionRoutine());

      // ------------------------------------------------------------
      // SI LLEGAMOS HASTA ACÁ, EL JUGADOR SUPERO LA INVASIÓN
      // ------------------------------------------------------------

      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
      GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, 3));
      GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.KitchenDepot, true));

      yield return new WaitForSeconds(3.5f);
      ToggleDepotState(false);

      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();
    }

    // --- RUTINA DEL ENEMIGO ---
    private IEnumerator EnemyInvasionSequence()
    {
      Coroutine enemyWalkingCoroutine;
      enemyIsOmnipresent = false;

      // Posicionar en Garage (o donde empiece la animación)
      EnvironmentHandler.Instance.EnemyHandler.ShowEnemy(EnemyPositions.GarageMainDoor);

      // Apertura forzada de la puerta del garage
      DownstairsOfficeDoorHandler garageMainDoorHandler =
        PlayerHouseHandler.Instance.GetStoredObjectByID("Garage MainDoor Lock")
        .GetComponent<DownstairsOfficeDoorHandler>();
      garageMainDoorHandler.ForceOpenDoor();
      garageMainDoorHandler.isNightmare = false;

      // Esperar hasta que se abra la puerta del garage
      yield return new WaitForSeconds(GARAGE_OPEN_WAIT);

      // "ALGUIEN HA INGRESADO A TU HOGAR"
      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

      // Garage -> Entrada
      enemyWalkingCoroutine = StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyWalkingRoutine(FSMaterial.MosaicGarage)
      );
      yield return StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyAnimation(EnemyAnimations.Voices1, false)
      );

      StopCoroutine(enemyWalkingCoroutine);

      // Esperar hasta que se abra la puerta del garage al hall
      InteractWithDoor("Garage Door Lock");
      yield return new WaitForSeconds(GARAGE_OPEN_WAIT);

      // Entrada --> DownstairsHall
      enemyWalkingCoroutine = StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyWalkingRoutine(FSMaterial.MosaicGarage)
      );
      yield return StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyAnimation(EnemyAnimations.Voices2, true)
      );
      StopCoroutine(enemyWalkingCoroutine);

      // Abrir Puerta Hall
      InteractWithDoor("Main Hall Door Lock");
      yield return new WaitForSeconds(GARAGE_OPEN_WAIT);

      // El enemigo desaparece visualmente pero está "en todos lados"
      // Desactivar modelo enemigo acá si es necesario
      // Pausa Dramática
      yield return new WaitForSeconds(DRAMATIC_PAUSE);
      enemyIsOmnipresent = true;
    }

    // Monitor de supervivencia del jugador y audio de tensión
    // Monitor de supervivencia del jugador y audio de tensión
    private IEnumerator MonitorInvasionRoutine()
    {
      // ... (Cálculo de duración y setup inicial igual que antes) ...
      float invasionDuration = EnvironmentHandler.Instance.EnemyHandler.GetInvasionSequenceDuration(
          (GARAGE_OPEN_WAIT * 3) + DRAMATIC_PAUSE
      );

      PlayerHandler player = PlayerHandler.Instance;

      // Configurar Música de Tensión
      tensionSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
      tensionSource.clip = tensionRampClip;
      tensionSource.loop = true;
      tensionSource.volume = 0f;
      tensionSource.Play();

      StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(tensionSource, 0f, 1f, invasionDuration));

      HouseArea lastArea = player.GetCurrentHouseArea();

      bool omnipresentPhaseTriggered = false;

      while (!playerIsSafe)
      {
        // 1. FASE: JUGADOR LOGRA ENTRAR AL DEPOT (EVENTO SUPERADO)
        if (player.GetCurrentHouseArea() == HouseArea.KitchenDepot)
        {
          playerIsSafe = true;
          HandleWinAudio();
          yield break;
        }

        // 2. FASE: EL ENEMIGO AVANZA
        if (!enemyIsOmnipresent)
        {
          // Sin logica por ahora .. quizas luego? Quizas no.
        }
        // 3. FASE: ENEMIGO OMNIPRESENTE (Llegó)
        else
        {
          if (!omnipresentPhaseTriggered)
          {
            omnipresentPhaseTriggered = true; // Bajamos la bandera para que no entre más

            StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(tensionSource, 1f));

            StartCoroutine(AudioManager.Instance.PlayerSoundsHandler.PlayPlayerSound(PlayerSoundsType.ScaredReactionLong));

            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[1]); // "SIEMPRE FUISTE UNA VICTIMA"
          }

          // --- BLOQUE DE EJECUCIÓN CONTINUA (MONITOR DE MUERTE) ---
          HouseArea currentArea = player.GetCurrentHouseArea();

          // A: Jugador cambia de cuarto -> MUERTE
          if (currentArea != lastArea && currentArea != HouseArea.None && currentArea != HouseArea.KitchenDepot)
          {
            yield return StartCoroutine(TriggerDeath());
            yield break;
          }

          // B: Jugador en Kitchen -> MUERTE INSTANTÁNEA
          if (currentArea == HouseArea.Kitchen)
          {
            yield return StartCoroutine(TriggerDeath());
            yield break;
          }

          // C: Jugador en Living -> MUERTE CON DELAY
          if (currentArea == HouseArea.LivingRoom)
          {
            yield return new WaitForSeconds(2.5f);

            // Si tras la espera no entró al depot (safety check), muere.
            if (!playerIsSafe)
            {
              yield return StartCoroutine(TriggerDeath());
              yield break;
            }
          }
          if (currentArea != HouseArea.None) lastArea = currentArea;
        }
        yield return null; // Esperar al siguiente frame
      }
    }

    private IEnumerator TriggerDeath()
    {
      // Detener lógicas
      StopAllCoroutines();
      if (tensionSource)
      {
        tensionSource.Stop();
        AudioManager.Instance.PoolsHandler.ReleaseAudioSource(tensionSource);
        tensionSource = null;
      }

      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));

      if (jumpscareClip)
      {
        AudioSource vfxSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
        vfxSource.PlayOneShot(jumpscareClip);

        // TODO: Animación de muerte del jugador (pantalla, zoom, etc.)
      }

      Debug.Log("<color=red><b>JUMPSCARE! YOU DIED.</b></color>");
      yield return new WaitForSeconds(2f);

      // UI de muerte
    }

    private void HandleWinAudio()
    {
      // Fade out de la tensión
      if (tensionSource != null)
      {
        StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(tensionSource, 1.5f));
      }
    }

    private void ToggleDepotState(bool open)
    {
      GameObject lightObj = PlayerHouseHandler.Instance.GetStoredObjectByID("Kitchen Depot Light");
      if (lightObj) lightObj.GetComponent<Light>().enabled = open;

      InteractWithDoor("Kitchen Depot Door Lock");
    }

    private void InteractWithDoor(string doorID)
    {
      GameObject doorObj = PlayerHouseHandler.Instance.GetStoredObjectByID(doorID);
      if (doorObj)
      {
        var interactable = doorObj.GetComponent<IInteractable>();
        if (interactable != null) interactable.Interact(null);
      }
    }

    public void AllowNextActions(Component sender, object data) => allowNextAction = true;
    public void ResetAllowNextActions() => allowNextAction = false;
  }
}