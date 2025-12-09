using System.Collections;
using System.Collections.Generic;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.GameController
{
    public class HeadachesEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 0;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;

        [Header("Text event SO")]

        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [SerializeField] private DialogSO[] dialogOs;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private GameEventSO updateFallbackTexts;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onStartDialog;
        [SerializeField] private GameEventSO onSpawnVehicle;

        [Header("Other eventsSO references")]
        [SerializeField] private GameEventSO onLoadDialogForSpecificChannel;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ HEADACHES EVENT NOW -------->");
            updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);
            yield return new WaitForSeconds(initialTime);

            onSpawnVehicle.Raise(this, VehicleType.PoliceCarCrash);

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

        public void StopEvent()
        {
            StopAllCoroutines();
        }
    }
}
