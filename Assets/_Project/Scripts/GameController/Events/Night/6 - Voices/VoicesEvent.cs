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

    [Header("Enemy Configuration")]
    [SerializeField] private string animGarageToEntrance = "Enemy_GarageToEntrance";
    [SerializeField] private string animEntranceToHall = "Enemy_EntranceToHall";

    [Header("Text & Dialogs")]
    [SerializeField] private UIOptionsTextSO[] playerHelperDataTextSO;

    [Header("Audio - Sequence")]
    [Tooltip("Música de tensión que incrementa mientras el enemigo avanza")]
    [SerializeField] private AudioClip tensionRampClip;

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

      // StartCoroutine(EnemyInvasionSequence());

      // "ALGUIEN HA INGRESADO A TU HOGAR"
      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

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
      enemyIsOmnipresent = false;

      // Apertura forzada de la puerta del garage
      DownstairsOfficeDoorHandler garageMainDoorHandler =
        PlayerHouseHandler.Instance.GetStoredObjectByID("Garage MainDoor Lock")
        .GetComponent<DownstairsOfficeDoorHandler>();
      garageMainDoorHandler.ForceOpenDoor();
      garageMainDoorHandler.isNightmare = false;

      // Posicionar en Garage (o donde empiece la animación)
      EnvironmentHandler.Instance.ShowEnemy(EnemyPositions.GarageMainDoor);

      // Garage -> Entrada
      StartCoroutine(
        EnvironmentHandler.Instance.PlayEnemyAnimation("Enemy_GarageToEntrance", true)
      );

      yield return new WaitForSeconds(3f); // Delay antes de abrir la puerta
      // Abrir Puerta Garage hacia la entrada de la casa
      InteractWithDoor("Garage Door Lock");

      // Pausa Dramática
      yield return new WaitForSeconds(1.5f);

      // Entrada -> Hall
      // if (enemyPrefab != null)
      // {
      //   var anim = enemyPrefab.GetComponent<Animation>();
      //   if (anim && !string.IsNullOrEmpty(animEntranceToHall))
      //   {
      //     anim.Play(animEntranceToHall);
      //     yield return new WaitForSeconds(anim[animEntranceToHall].length);
      //   }
      // }

      // Abrir Puerta Hall
      InteractWithDoor("Main Hall Door Lock");

      // El enemigo desaparece visualmente pero está "en todos lados"
      // if (enemyPrefab) enemyPrefab.SetActive(false);

      enemyIsOmnipresent = true;
    }

    // Monitor de supervivencia del jugador y audio de tensión
    private IEnumerator MonitorInvasionRoutine()
    {
      PlayerHandler player = PlayerHandler.Instance;

      // Configurar Música de Tensión (Loop)
      AudioSource tensionSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
      tensionSource.clip = tensionRampClip;
      tensionSource.loop = true;
      tensionSource.volume = 0.1f;
      tensionSource.Play();

      float estimatedDuration = 10f;
      float timer = 0f;

      HouseArea lastArea = player.GetCurrentHouseArea();

      while (!playerIsSafe)
      {
        if (player.GetCurrentHouseArea() == HouseArea.KitchenDepot)
        {
          playerIsSafe = true;
          HandleWinAudio();
          yield break;
        }

        // Lógica de audio para cuando el enemigo está avanzando
        if (!enemyIsOmnipresent)
        {
          timer += Time.deltaTime;
          float progress = Mathf.Clamp01(timer / estimatedDuration);
          tensionSource.volume = Mathf.Lerp(0.2f, 1f, progress);
          tensionSource.pitch = Mathf.Lerp(0.8f, 1.1f, progress);
        }
        // Lógica para cuando el enemigo completó su ingreso
        else
        {
          // TODO: Sonido de respiración del jugador constante 
          GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[1]); // "SIEMPRE FUISTE UNA VICTIMA"

          HouseArea currentArea = player.GetCurrentHouseArea();

          // A: Jugador cambia de cuarto -> MUERTE
          if (currentArea != lastArea && currentArea != HouseArea.None && currentArea != HouseArea.KitchenDepot)
          {
            yield return StartCoroutine(TriggerDeath());
          }

          // Jugador en Kitchen -> MUERTE INSTANTÁNEA
          if (currentArea == HouseArea.Kitchen)
          {
            yield return StartCoroutine(TriggerDeath());
          }

          // Jugador en Living -> MUERTE CON DELAY
          if (currentArea == HouseArea.LivingRoom)
          {
            yield return new WaitForSeconds(2.5f);
            // Chequeamos de nuevo por si se movió en esos 2.5s
            if (!playerIsSafe) yield return StartCoroutine(TriggerDeath());
          }

          // Actualizamos lastArea para el chequeo de movimiento
          if (currentArea != HouseArea.None) lastArea = currentArea;
        }

        yield return null;
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

      // Reproducir sonido de alivio
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