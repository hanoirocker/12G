using System;
using System.Collections;
using Cinemachine;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using UnityEngine;
using UnityEngine.Video;

namespace TwelveG.GameController
{
  public class VisionsEvent : GameEventBase
  {
    [Header("Event options")]
    [SerializeField] private GameEventListener enemySpottedListener;

    [Space(10)]
    [Header("Event options")]
    [SerializeField, Range(1, 10)] private int initialTime = 0;

    [Space(10)]
    [Header("Text event SO")]
    [SerializeField] private UIOptionsTextSO[] playerHelperDataTextSO;
    [Space(5)]
    [SerializeField] private ObservationTextSO[] observationTextSOs;
    [Space(5)]
    [SerializeField] private DialogSO[] dialogSOs;
    [SerializeField] private DialogSO dialogFromDownstairsSO;

    [Space(10)]
    [Header("Audio Options")]
    [SerializeField] private AudioClip track2Clip;
    [SerializeField, Range(0f, 1f)] private float track2Volume = 0.15f;
    [SerializeField] private AudioClip[] forcingDoorClips;
    [SerializeField, Range(0f, 1f)] private float forcingDoorClipsVolume = 0.5f;

    [Space(10)]
    [Header("Video Settings")]
    [SerializeField] private VideoClip subliminalJSClip1;
    [SerializeField, Range(0f, 1f)] private float jumpScareVolume = 1f;

    private CameraZoom cameraZoom;
    private Transform enemyTransform;

    private bool playerSpottedFromDownstairsAlready = false;
    private bool playerSpottedFromUpstairs = false;
    private bool allowNextAction = false;
    private HouseArea currentHouseArea;

    public override IEnumerator Execute()
    {
      AudioSource bgMusicSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
      EnvironmentHandler environmentHandler = EnvironmentHandler.Instance;
      PlayerHouseHandler playerHouseHandler = PlayerHouseHandler.Instance;
      PlayerHandler playerHandler = PlayerHandler.Instance;
      cameraZoom = playerHandler.GetComponentInChildren<CameraZoom>();
      GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.ConstantThunders);
      GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

      // Desactiva el listener para que no interfiera con el evento
      enemySpottedListener.enabled = false;

      yield return new WaitForSeconds(initialTime);

      // Simón decide llamar a Micaela
      GameEvents.Common.onStartDialog.Raise(this, dialogSOs[0]);

      // "conversationHasEnded"
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();

      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

      // Activar "Visions - Colliders"
      // Nota: Estos colliders son dos y cada uno dispara un evento distinto para activar, o desactivar,
      // los 2 objetos con ZoneSpotterHandler de Mica Entrance house. Luego, según el spot area
      // activo al observar, se dispara un evento para distinguir si se observó desde arriba o desde abajo.
      playerHouseHandler.ToggleCheckpointPrefabs(new ObjectData("Visions - Colliders", true));

      // Comienza música ""
      if (track2Clip != null)
      {
        bgMusicSource.clip = track2Clip;
        bgMusicSource.volume = 0f;
        bgMusicSource.loop = true;
        bgMusicSource.Play();

        StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(bgMusicSource, 0f, track2Volume, 2f));
      }

      // Preparamos el video con las imágenes subliminales
      if (subliminalJSClip1 != null)
      {
        GameEvents.Common.onVideoCanvasLoadClip.Raise(this, subliminalJSClip1);
      }

      yield return new WaitUntil(() => allowNextAction && playerSpottedFromUpstairs);
      ResetAllowNextActions();

      SpawnEnemy(environmentHandler);

      yield return new WaitUntil(() => !cameraZoom.playerIsZooming());
      ResetAllowNextActions();

      // Desactivar controles del jugador y espera hasta que el zoom out termine
      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
      yield return new WaitForSeconds(0.5f);

      // Indica a la Virtual Camera activa que debe mirar hacia el enemigo
      GameEvents.Common.onVirtualCamerasControl.Raise(this, new LookAtTarget(enemyTransform));
      GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseIn, 1));

      StartCoroutine(playerHandler.GetComponentInChildren<PlayerSoundsHandler>().
        PlayPlayerSound(PlayerSoundsType.Doubt));
      // Espera a que termine la animación de la cámara y el clip sorprendido del jugador
      yield return new WaitForSeconds(1f);

      // Se dispara un video jumpscare
      GameEvents.Common.onVideoCanvasPlay.Raise(this, jumpScareVolume);

      // "onVideoCanvasFinished"
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();
      playerHouseHandler.ToggleCheckpointPrefabs(new ObjectData("Visions - Colliders", false));
      GameEvents.Common.onVirtualCamerasControl.Raise(this, new LookAtTarget(null));
      GameEvents.Common.onMainCameraSettings.Raise(this, new ResetCinemachineBrain());
      environmentHandler.ShowEnemy(EnemyPositions.None);
      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
      GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.CloseThunder);

      // Self Dialog de simón después de la visión
      yield return new WaitForSeconds(0.25f);
      GameEvents.Common.onStartDialog.Raise(this, dialogSOs[1]);
      // "conversationHasEnded"
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();

      yield return new WaitForSeconds(2f);
      // Simon llama a Micaela despues de la vision (interferenciaaaaaaa)
      GameEvents.Common.onStartDialog.Raise(this, dialogSOs[2]);

      yield return new WaitForSeconds(6f);
      StartCoroutine(PlayDoorForcingSoundRoutine());

      // "conversationHasEnded"
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();

      yield return StartCoroutine(
        AudioManager.Instance.GetComponentInChildren<PlayerSoundsHandler>().
        PlayPlayerSound(PlayerSoundsType.VisionsNeckWhisper)
      );

      // Espera a que Simon no esté en las áreas de pasillo de abajo ni entrada
      // para hacer aparecer al enemigo
      yield return new WaitUntil(
        () => playerHandler.GetCurrentHouseArea() != HouseArea.DownstairsHall
        && playerHandler.GetCurrentHouseArea() != HouseArea.LivingRoom
        && playerHandler.GetCurrentHouseArea() != HouseArea.None
      );

      enemySpottedListener.enabled = true; // Vuelve a activar el listener para que detecte al jugador
      environmentHandler.ShowEnemy(EnemyPositions.DownstairsHallWindow);
      GameEvents.Common.onShowEnemy.Raise(this, EnemyPositions.DownstairsHallWindow);

      // "OnEnemySpotted" (el ZoneSpotterHandler del enemigo no precisa needsToBeZoomed)
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();
      GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.CloseThunder);
      // Se ejecuta animación del enemigo caminando hacia la entrada principal y desactiva al terminar
      environmentHandler.StartCoroutine(
        environmentHandler.PlayEnemyAnimation("Enemy - From DHall to Entrance", true)
      );

      // Espera a que termine de recuperarse del susto
      yield return StartCoroutine(playerHandler.GetComponentInChildren<PlayerSoundsHandler>().
        PlayPlayerSound(PlayerSoundsType.EnemySurpriseReaction));
    }

    // Hace aparecer el enemigo dependiendo del lugar donde esté el jugador
    private void SpawnEnemy(EnvironmentHandler environmentHandler)
    {
      currentHouseArea = PlayerHandler.Instance.GetCurrentHouseArea();

      switch (currentHouseArea)
      {
        case HouseArea.PlayerBedroom:
          environmentHandler.ShowEnemy(EnemyPositions.LivingRoomRightWindow);
          break;
        case HouseArea.GuestsBedroom:
          environmentHandler.ShowEnemy(EnemyPositions.PlayerHouseCorner);
          break;
        default:
          Debug.LogWarning("El jugador no está en una habitación válida para que aparezca el enemigo.");
          break;
      }

      enemyTransform = environmentHandler.GetCurrentEnemyTransform();
    }

    public void AllowNextActions(Component sender, object data)
    {
      allowNextAction = true;
    }

    public void ResetAllowNextActions()
    {
      allowNextAction = false;
    }

    public void OnPlayerSpottedFromUpstairs(Component sender, object data)
    {
      playerSpottedFromUpstairs = true;
      allowNextAction = true;
    }

    public void OnPlayerSpottedFromDownstairs(Component sender, object data)
    {
      StartCoroutine(PlayerSpottedFromDownstairsCoroutine());
    }

    private IEnumerator PlayDoorForcingSoundRoutine()
    {
      Transform entranceTransform = PlayerHouseHandler.Instance?.GetTransformByObject(HouseObjects.EntranceMainDoor);
      // Espera a que el jugador salga de las áreas de entrada, garage y pasillo de abajo
      yield return new WaitUntil(() =>
        PlayerHandler.Instance.GetCurrentHouseArea() != HouseArea.Entrance &&
        PlayerHandler.Instance.GetCurrentHouseArea() != HouseArea.Garage &&
        PlayerHandler.Instance.GetCurrentHouseArea() != HouseArea.DownstairsHall
        && PlayerHandler.Instance.GetCurrentHouseArea() != HouseArea.None
      );

      if (entranceTransform)
      {
        (AudioSource garageSource, AudioSourceState garageState) =
            AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
                entranceTransform, forcingDoorClipsVolume
        );

        if (garageSource != null && forcingDoorClips != null)
        {
          garageSource.clip = forcingDoorClips[0];
          garageSource.loop = false;
          garageSource.Play();
          yield return new WaitForSeconds(forcingDoorClips[0].length);
          garageSource.clip = forcingDoorClips[1];
          yield return new WaitForSeconds(1f);
          garageSource.Play();
          yield return new WaitForSeconds(forcingDoorClips[1].length);
        }

        AudioUtils.StopAndRestoreAudioSource(garageSource, garageState);
      }
    }

    private IEnumerator PlayerSpottedFromDownstairsCoroutine()
    {
      if (!playerSpottedFromDownstairsAlready)
      {
        // Cancela el movimiento del jugador para evitar que durante el dialogo el mismo
        // se desplace hacia un collider correcto.
        GameEvents.Common.onCinematicCanvasControls.Raise(this, new ShowCinematicBars(true));
        GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
        GameEvents.Common.onStartDialog.Raise(this, dialogFromDownstairsSO);
        playerSpottedFromDownstairsAlready = true;

        // Espera a que termine el diálogo para
        yield return new WaitUntil(() => allowNextAction);
        GameEvents.Common.onCinematicCanvasControls.Raise(this, new ShowCinematicBars(false));
        GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
        GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[1]);
      }
      else
      {
        GameEvents.Common.onObservationCanvasShowText.Raise(this, observationTextSOs[0]);
      }
    }
  }
}