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
    [SerializeField, Range(1, 10)] private int initialTime = 0;

    [Space]
    [Header("Text event SO")]
    [SerializeField] private UIOptionsTextSO[] playerHelperDataTextSO;
    [Space(5)]
    [SerializeField] private ObservationTextSO[] observationTextSOs;
    [Space(5)]
    [SerializeField] private DialogSO[] dialogSOs;

    [Space]
    [Header("Audio Options")]
    [SerializeField] private AudioClip track2Clip;
    [SerializeField, Range(0f, 1f)] private float track2Volume = 0.15f;
    [SerializeField] private AudioClip playerSurprisedClip;
    [SerializeField, Range(0f, 1f)] private float playerSurprisedClipVolume = 0.7f;

    [Space]
    [Header("Video Settings")]
    [SerializeField] private VideoClip subliminalJSClip1;
    [SerializeField, Range(0f, 1f)] private float jumpScareVolume = 1f;

    private PlayerHandler playerHandler;
    private CameraZoom cameraZoom;
    private Transform enemyTransform;

    private bool playerSpottedFromDownstairsAlready = false;
    private bool playerSpottedFromUpstairs = false;
    private bool allowNextAction = false;

    public override IEnumerator Execute()
    {
      AudioSource bgMusicSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
      AudioSource playerSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
      AudioSourceState playerSourceState = playerSource.GetSnapshot();
      PlayerHouseHandler playerHouseHandler = FindObjectOfType<PlayerHouseHandler>();
      playerHandler = FindObjectOfType<PlayerHandler>();
      cameraZoom = playerHandler.GetComponentInChildren<CameraZoom>();
      GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.ConstantThunders);
      GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

      yield return new WaitForSeconds(initialTime);

      // Simón decide llamar a Micaela
      GameEvents.Common.onStartDialog.Raise(this, dialogSOs[0]);

      // "conversationHasEnded"
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();

      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

      // Activar "Visions - Colliders"
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

      SpawnEnemy();

      yield return new WaitUntil(() => !cameraZoom.playerIsZooming());
      ResetAllowNextActions();

      // Desactivar controles del jugador y espera hasta que el zoom out termine
      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
      yield return new WaitForSeconds(0.5f);

      // Indica a la Virtual Camera activa que debe mirar hacia el enemigo
      GameEvents.Common.onVirtualCamerasControl.Raise(this, new LookAtTarget(enemyTransform));
      GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseIn, 1));

      if (playerSource != null)
      {
        playerSource.clip = playerSurprisedClip;
        playerSource.loop = false;
        playerSource.volume = playerSurprisedClipVolume;
        playerSource.Play();
      }
      // Espera a que termine la animación de la cámara y el clip sorprendido del jugador
      yield return new WaitForSeconds(1f);

      // Se dispara un video jumpscare
      GameEvents.Common.onVideoCanvasPlay.Raise(this, jumpScareVolume);

      // "onVideoCanvasFinished"
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();
      AudioUtils.StopAndRestoreAudioSource(playerSource, playerSourceState);
      GameEvents.Common.onVirtualCamerasControl.Raise(this, new LookAtTarget(null));
      GameEvents.Common.onMainCameraSettings.Raise(this, new ResetCinemachineBrain());
      GameEvents.Common.onShowEnemy.Raise(this, EnemyPositions.None);
      GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
      GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.CloseThunder);
      
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();
    }

    // Hace aparecer el enemigo dependiendo del lugar donde esté el jugador
    private void SpawnEnemy()
    {
      HouseArea currentHouseArea = playerHandler.GetCurrentHouseArea();

      switch (currentHouseArea)
      {
        case HouseArea.PlayerBedroom:
          GameEvents.Common.onShowEnemy.Raise(this, EnemyPositions.MiddleOfTheStreet);
          break;
        case HouseArea.GuestsBedroom:
          GameEvents.Common.onShowEnemy.Raise(this, EnemyPositions.PlayerHouseCorner);
          break;
        default:
          Debug.LogWarning("Player is not in a valid house area to spawn the enemy.");
          break;
      }

      EnvironmentHandler environmentHandler = FindObjectOfType<EnvironmentHandler>();
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

    private IEnumerator PlayerSpottedFromDownstairsCoroutine()
    {
      if (!playerSpottedFromDownstairsAlready)
      {
        // Cancela el movimiento del jugador para evitar que durante el dialogo el mismo
        // se desplace hacia un collider correcto.
        GameEvents.Common.onCinematicCanvasControls.Raise(this, new ShowCinematicBars(true));
        GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
        GameEvents.Common.onStartDialog.Raise(this, dialogSOs[1]);
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