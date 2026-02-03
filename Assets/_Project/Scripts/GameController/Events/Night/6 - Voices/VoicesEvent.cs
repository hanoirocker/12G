using System;
using System.Collections;
using Cinemachine;
using TwelveG.AudioController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
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
    [SerializeField, Range(0f, 5f)] private float GARAGEMAIN_OPEN_WAIT = 6.7f; // Tiempo que dura aprox el clip "Garage Door Foced by Enemy"
    [SerializeField, Range(0f, 5f)] private float GARAGE_OPEN_WAIT = 3f;
    [SerializeField, Range(0f, 5f)] private float HALL_OPEN_WAIT = 3f;
    [SerializeField, Range(0f, 5f)] private float DRAMATIC_PAUSE = 1.5f;

    [Space(5)]
    [Header("Audio")]
    [SerializeField] private AudioClip jumpscareClip;
    [Space(2)]
    [SerializeField] private AudioClip kitchenKnifeSoundClip;
    [SerializeField, Range(0f, 1f)] private float kitchenKnifeSoundVolume = 0.7f;
    [Space(2)]
    [SerializeField] private AudioClip doorKnockingClip;
    [SerializeField, Range(0f, 1f)] private float doorKnockingSoundVolume = 0.7f;

    private bool allowNextAction = false;
    private bool enemyIsOmnipresent = false;
    private bool playerIsSafe = false;

    private AudioSource tensionSource;

    // Estas se pueden detener si el jugador ve al enemigo
    // o se acerca demasiado a él sin mirarlo
    private Coroutine enemyInvasionCoroutine;
    private Coroutine enemyWalkingRoutine;
    private Coroutine enemyAnimationRoutine;

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

      // Comenzar a drenar baterías de la linterna (Los dialogos están en la linterna)
      PlayerHandler.Instance.GetComponentInChildren<Flashlight>()
        .GetComponent<Flashlight>().TriggerDrainBatteriesRoutine(7f);

      // Luz y Puerta Depot
      StartCoroutine(ToggleDepotDoor(true));

      // Posicionar en Garage (o donde empiece la animación)
      EnvironmentHandler.Instance.EnemyHandler.ShowEnemy(EnemyPositions.GarageMainDoor);

      // Apertura forzada de la puerta del garage
      DownstairsOfficeDoorHandler garageMainDoorHandler =
        PlayerHouseHandler.Instance.GetStoredObjectByID("Garage MainDoor Lock")
        .GetComponent<DownstairsOfficeDoorHandler>();
      garageMainDoorHandler.ForceOpenDoor();
      garageMainDoorHandler.isNightmare = false;

      // Esperar hasta que se abra la puerta del garage
      yield return new WaitForSeconds(GARAGEMAIN_OPEN_WAIT);

      // Rutina del enemigo
      enemyInvasionCoroutine = StartCoroutine(EnemyInvasionSequence());

      // Monitor de estado (Controla Audio, Muerte y Exito)
      yield return StartCoroutine(MonitorInvasionRoutine());

      // ------------------------------------------------------------
      // SI LLEGAMOS HASTA ACÁ, EL JUGADOR SUPERO LA INVASIÓN
      // ------------------------------------------------------------

      // Aca continúa la rutian del enemigo pero en la cocina, esperamos a que termine
      // para terminar con PlayerInsideDepotRoutine
      yield return StartCoroutine(PlayerInsideDepotRoutine());

      // BLA
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();
    }

    // --- RUTINA DEL ENEMIGO ---
    private IEnumerator EnemyInvasionSequence()
    {
      enemyIsOmnipresent = false;

      // "ALGUIEN HA INGRESADO A TU HOGAR"
      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

      // Garage -> Entrada
      enemyWalkingRoutine = StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyWalkingRoutine(FSMaterial.MosaicGarage)
      );
      enemyAnimationRoutine = StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyAnimation(EnemyAnimations.VoicesMainGarageToEntrance, false)
      );
      yield return enemyAnimationRoutine;

      StopCoroutine(enemyWalkingRoutine);

      // Esperar hasta que se abra la puerta del garage al hall
      InteractWithDoor("Garage Door Lock");
      yield return new WaitForSeconds(GARAGE_OPEN_WAIT);

      // Entrada --> DownstairsHall
      enemyWalkingRoutine = StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyWalkingRoutine(FSMaterial.MosaicGarage)
      );
      enemyAnimationRoutine = StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyAnimation(EnemyAnimations.VoicesGarageToDhall, false)
      );
      yield return enemyAnimationRoutine;
      StopCoroutine(enemyWalkingRoutine);

      // Abrir Puerta Hall
      InteractWithDoor("Main Hall Door Lock");
      yield return new WaitForSeconds(HALL_OPEN_WAIT);

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
          GARAGEMAIN_OPEN_WAIT + GARAGE_OPEN_WAIT + HALL_OPEN_WAIT + DRAMATIC_PAUSE
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
      HouseArea currentArea = lastArea;

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
          currentArea = player.GetCurrentHouseArea();

          // A: Jugador cambia de cuarto -> MUERTE
          if (currentArea != lastArea && !PlayerIsInNeutralZone())
          {
            Debug.Log("A: Jugador cambia de cuarto -> MUERTE");
            yield return StartCoroutine(TriggerDeath());
            yield break;
          }

          // B: Jugador en Kitchen -> MUERTE INSTANTÁNEA
          if (currentArea == HouseArea.Kitchen)
          {
            Debug.Log("B: Jugador en Kitchen -> MUERTE INSTANTÁNEA");
            yield return StartCoroutine(TriggerDeath());
            yield break;
          }

          // C: Jugador en Living -> MUERTE CON DELAY
          if (currentArea == HouseArea.LivingRoom)
          {
            Debug.Log("C: Jugador en Living -> MUERTE CON DELAY");
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

        currentArea = player.GetCurrentHouseArea();
        if (currentArea != HouseArea.None) lastArea = currentArea;

        yield return null; // Esperar al siguiente frame
      }
    }

    private IEnumerator TriggerDeath()
    {
      if (tensionSource)
      {
        tensionSource.Stop();
        AudioManager.Instance.PoolsHandler.ReleaseAudioSource(tensionSource);
        tensionSource = null;
      }

      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));

      // Metodo para detener todas las UI activas

      if (jumpscareClip)
      {
        AudioSource vfxSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
        vfxSource.PlayOneShot(jumpscareClip);

        // TODO: Animación de muerte del jugador (pantalla, zoom, etc.)
      }

      Debug.Log("<color=red><b>JUMPSCARE! YOU DIED.</b></color>");
      yield return new WaitForSeconds(2f);

      StopAllCoroutines();

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

    private IEnumerator ToggleDepotDoor(bool open)
    {
      GameObject lightObj = PlayerHouseHandler.Instance.GetStoredObjectByID("Kitchen Depot Light");
      GameObject doorObj = PlayerHouseHandler.Instance.GetStoredObjectByID("Kitchen Depot Door Lock");
      SlideDrawerHandler doorHandler = doorObj.GetComponent<SlideDrawerHandler>();

      if (doorObj == null) yield break;
      if (open)
      {
        if (lightObj) lightObj.GetComponent<Light>().enabled = open;
        if (!doorHandler.IsDoorOpen()) doorHandler.Interact(null);
        yield break;
      }
      else
      {
        if (doorHandler.IsDoorOpen()) doorHandler.Interact(null);
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(doorHandler.PlayLockSound());
      }
    }

    private void InteractWithDoor(string doorID)
    {
      GameObject doorObj = PlayerHouseHandler.Instance.GetStoredObjectByID(doorID);
      if (doorObj)
      {
        var interactable = doorObj.GetComponent<RotativeDoorHandler>();
        if (interactable != null && !interactable.IsDoorOpen()) interactable.Interact(null);
      }
    }

    private IEnumerator PlayerInsideDepotRoutine()
    {
      UIManager.Instance.ControlCanvasHandler.ToggleControlCanvas(false);
      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));
      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
      GameEvents.Common.onPlayerControls.Raise(this, new EnableInteractionModules(false));
      GameEvents.Common.onCinematicCanvasControls.Raise(this, new ShowCinematicBars(true));
      GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, 3));
      GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.KitchenDepot, true));

      yield return new WaitForSeconds(3f);
      yield return StartCoroutine(ToggleDepotDoor(false));

      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(true));
      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(true));

      // Esperamos a que haya termiando la rutina del enemigo antes de continuar
      // hacia la rutina de la cocina ..
      if (enemyInvasionCoroutine != null)
      {
        yield return enemyInvasionCoroutine;
      }

      yield return StartCoroutine(EnemyInsideKitchenRoutine());
    }

    private IEnumerator EnemyInsideKitchenRoutine()
    {
      AudioSource audioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Interaction);
      AudioSourceState audioState = audioSource.GetSnapshot();

      PlayerHouseHandler playerHouseHandler = PlayerHouseHandler.Instance;

      // Dhall --> Kitchen
      Coroutine enemyWalkingCoroutine = StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyWalkingRoutine(FSMaterial.Wood)
      );
      yield return StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyAnimation(EnemyAnimations.VoicesDhallToKichen, false)
      );
      StopCoroutine(enemyWalkingCoroutine);

      // Sonido de que agarra un cuchillo de la cocina
      audioSource.transform.position = playerHouseHandler.GetTransformByObject(HouseObjects.KitchenKnifeStack).position;
      audioSource.PlayOneShot(kitchenKnifeSoundClip, kitchenKnifeSoundVolume);
      yield return new WaitForSeconds(kitchenKnifeSoundClip.length + 0.5f);

      // Kitchen --> Kitchen Depot
      enemyWalkingCoroutine = StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyWalkingRoutine(FSMaterial.Wood)
      );
      yield return StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyAnimation(EnemyAnimations.VoicesKichenToKichenDepot, false)
      );
      StopCoroutine(enemyWalkingCoroutine);
      yield return new WaitForSeconds(2f);

      // Enemigo llama a la puerta a Simon
      audioSource.transform.position = playerHouseHandler.GetTransformByObject(HouseObjects.KitchenDepotDoor).position;
      audioSource.PlayOneShot(doorKnockingClip, doorKnockingSoundVolume);
      yield return new WaitForSeconds(1f);
      StartCoroutine(AudioManager.Instance.PlayerSoundsHandler.PlayPlayerSound(PlayerSoundsType.ScaredReactionLong));
      yield return new WaitForSeconds(doorKnockingClip.length + 0.5f);

      // Kitchen Depot --> Kitchen Table
      enemyWalkingCoroutine = StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyWalkingRoutine(FSMaterial.Wood)
      );
      yield return StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyAnimation(EnemyAnimations.VoicesKichenDepotToKichenTable, false)
      );
      StopCoroutine(enemyWalkingCoroutine);

      yield return new WaitForSeconds(doorKnockingClip.length + 2f);

      // Kitchen --> Garage (ocultar enemigo al terminar)
      enemyWalkingCoroutine = StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyWalkingRoutine(FSMaterial.Wood)
      );
      yield return StartCoroutine(
        EnvironmentHandler.Instance.EnemyHandler.PlayEnemyAnimation(EnemyAnimations.VoicesKitchenTableToGarage, true)
      );
      StopCoroutine(enemyWalkingCoroutine);

      AudioUtils.StopAndRestoreAudioSource(audioSource, audioState);
    }

    public void OnEnemySpotted(Component sender, object data)
    {
      StartCoroutine(EnemySpottedRoutine(sender));
    }

    // Recibe "onEnemySpotted" desde ZoneSpotterHandler del enemigo
    private IEnumerator EnemySpottedRoutine(Component sender)
    {
      sender.gameObject.GetComponent<ZoneSpotterHandler>().canBeSpotted = false;
      GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.CloseThunder);

      if (enemyInvasionCoroutine != null)
      {
        StopCoroutine(enemyInvasionCoroutine);
        enemyInvasionCoroutine = null;
      }
      if (enemyWalkingRoutine != null)
      {
        StopCoroutine(enemyWalkingRoutine);
        enemyWalkingRoutine = null;
      }
      if (enemyAnimationRoutine != null)
      {
        StopCoroutine(enemyAnimationRoutine);
        enemyAnimationRoutine = null;
      }

      // El jugador llega a visualizar levemente al enemigo antes de hacerlo desaparecer
      yield return new WaitForSeconds(0.1f);

      EnvironmentHandler.Instance.EnemyHandler.HideEnemy();
      enemyInvasionCoroutine = null;
      enemyIsOmnipresent = true;
      Debug.Log("Enemy Spotted! Now Omnipresent.");
    }

    private bool PlayerIsInNeutralZone()
    {
      return
        PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.UpstairsHall
        || PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.UpperStairs
        || PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.MiddleStairs
        || PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.LowerStairs
        || PlayerHandler.Instance.GetCurrentHouseArea() == HouseArea.None;
    }

    public void AllowNextActions(Component sender, object data) => allowNextAction = true;
    public void ResetAllowNextActions() => allowNextAction = false;
  }
}