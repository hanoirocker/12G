using System;
using System.Collections;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
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
    [SerializeField] private UIOptionsTextSO playerHelperDataTextSO;
    [SerializeField] private ObservationTextSO[] observationTextSOs;
    [SerializeField] private DialogSO[] dialogSOs;

    [Space]
    [Header("Audio Options")]
    [SerializeField] private AudioClip hauntingSoundClip;
    [SerializeField, Range(0f, 1f)] private float hauntingSoundVolume = 0.15f;

    private bool allowNextAction = false;

    public override IEnumerator Execute()
    {
      AudioSource bgMusicSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
      GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
      yield return new WaitForSeconds(initialTime);

      // TODO: borrar (solo de prueba)
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();

      // Simón decide llamar a Micaela
      GameEvents.Common.onStartDialog.Raise(this, dialogSOs[0]);

      // "conversationHasEnded"
      yield return new WaitUntil(() => allowNextAction);
      ResetAllowNextActions();

      // Comienza música ""
      if (bgMusicSource != null && hauntingSoundClip != null)
      {
        bgMusicSource.clip = hauntingSoundClip;
        bgMusicSource.volume = 0f;
        bgMusicSource.loop = true;
        bgMusicSource.Play();

        StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(bgMusicSource, 0f, hauntingSoundVolume, 2f));
      }

      // bla
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
  }
}