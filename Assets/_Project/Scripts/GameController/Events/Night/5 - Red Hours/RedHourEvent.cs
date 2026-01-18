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
  public class RedHourEvent : GameEventBase
  {
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
    [SerializeField] private AudioClip track3Clip;
    [SerializeField, Range(0f, 1f)] private float track3Volume = 0.15f;

    private bool allowNextAction = false;

    public override IEnumerator Execute()
    {
      yield break;
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