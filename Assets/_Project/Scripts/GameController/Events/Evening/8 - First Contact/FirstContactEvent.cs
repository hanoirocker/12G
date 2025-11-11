using System.Collections;
using TwelveG.DialogsController;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.GameController
{
    public class FirstContactEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;

        [Header("Text event SO")]
        [SerializeField] private DialogSO firstEventDialog;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private GameEventSO updateFallbackTexts;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO startDialog;

        [Header("Other eventsSO references")]

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ FIRST CONTACT EVENT NOW -------->");

            updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

            yield return new WaitForSeconds(initialTime);

            startDialog.Raise(this, firstEventDialog);

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
