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

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ HEADACHES EVENT NOW -------->");
            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);
            yield return new WaitForSeconds(initialTime);

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.PoliceCarCrash);

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
