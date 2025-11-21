using System.Collections;
using System.Collections.Generic;
using TwelveG.DialogsController;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.GameController
{
    public class NoisesEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 0;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;

        [Header("Text event SO")]

        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [SerializeField] private DialogSO firstEventDialog;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private GameEventSO updateFallbackTexts;


        [Header("EventsSO references")]
        [SerializeField] private GameEventSO startDialog;

        [Header("Other eventsSO references")]

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ NOISES EVENT NOW -------->");

            updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

            yield return new WaitForSeconds(initialTime);

            // Desde cualquier ventana del primer piso deberia tener ...
            onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[0]
            );

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            startDialog.Raise(this, firstEventDialog);

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
