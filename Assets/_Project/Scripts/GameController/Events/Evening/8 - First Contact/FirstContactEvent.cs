using System.Collections;
using System.Collections.Generic;
using TwelveG.DialogsController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.GameController
{
    public class FirstContactEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;

        [Header("Text event SO")]

        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [SerializeField] private DialogSO firstEventDialog;

        [SerializeField] private DialogSO secondEventDialog;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private GameEventSO updateFallbackTexts;


        [Header("EventsSO references")]
        [SerializeField] private GameEventSO startDialog;

        [SerializeField] private GameEventSO enablePlayerItem;

        [Header("Other eventsSO references")]

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ FIRST CONTACT EVENT NOW -------->");

            updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

            yield return new WaitForSeconds(initialTime);

            // Primer dialogo cuando obtiene el walkie talkie y lo setea al canal de Mica
            startDialog.Raise(this, firstEventDialog);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            enablePlayerItem.Raise(this, ItemType.WalkieTalkie);
            Debug.Log("[FirstContactEvent] Walkie Talkie enabled");

            // Llamado a Mica
            startDialog.Raise(this, secondEventDialog);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // No hay chance que lo tenga encedido ... la puta madre.
            onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[0]
            );

            yield return new WaitForSeconds(12f);
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
