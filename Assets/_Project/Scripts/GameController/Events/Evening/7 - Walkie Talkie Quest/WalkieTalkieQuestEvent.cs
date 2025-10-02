namespace TwelveG.GameController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Localization;

    public class WalkieTalkieQuestEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;

        [Header("Text event SO")]
        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private GameEventSO updateFallbackTexts;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;
        [SerializeField] private GameEventSO finishCurrentEvent;


        [Header("Other eventsSO references")]
        [SerializeField] private GameEventSO drawerCanBeInteracted;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ WT QUEST EVENT NOW -------->");

            yield return new WaitForSeconds(initialTime);

            updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

            // Tengo que encontrar la forma de hablar con Mica como sea.
            onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[0]
            );

            yield return new WaitForSeconds(12f);
            // Mi Walkie Talkie! Si no mal recuerdo mi madre lo había escondido ...
            onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[1]
            );
            onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[2]
            );

            drawerCanBeInteracted.Raise(this, null);

            // TODO: Construir resto del evento
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

        public void WalkieTalkieQuestEventStop()
        {
            StopAllCoroutines();
            finishCurrentEvent.Raise(this, null);
        }
    }
}