using System;
using System.Collections;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.Utils;
using UnityEngine;

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

    private bool playerSpottedFromDownstairsAlready = false;
    private bool playerSpottedFromUpstairs = false;
    private bool allowNextAction = false;

    public override IEnumerator Execute()
    {
      AudioSource bgMusicSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
      PlayerHouseHandler playerHouseHandler = FindObjectOfType<PlayerHouseHandler>();
      GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
      yield return new WaitForSeconds(initialTime);

      // Simón decide llamar a Micaela
      GameEvents.Common.onStartDialog.Raise(this, dialogSOs[0]);

      // "conversationHasEnded"
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();

      GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

      // Activar "Visions - Colliders"
      if(playerHouseHandler != null)
      {
        playerHouseHandler?.ToggleCheckpointPrefabs(new ObjectData("Visions - Colliders", true));
      }

      // Comienza música ""
      if (bgMusicSource != null && track2Clip != null)
      {
        bgMusicSource.clip = track2Clip;
        bgMusicSource.volume = 0f;
        bgMusicSource.loop = true;
        bgMusicSource.Play();

        StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(bgMusicSource, 0f, track2Volume, 2f));
      }

      yield return new WaitUntil(() => allowNextAction && playerSpottedFromUpstairs);
      ResetAllowNextActions();

      Debug.Log("Player spotted from upstairs - Ending visions event");

      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();
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
        GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
        GameEvents.Common.onStartDialog.Raise(this, dialogSOs[1]);
        playerSpottedFromDownstairsAlready = true;

        // Espera a que termine el diálogo para
        yield return new WaitUntil(() => allowNextAction);
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