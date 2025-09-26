namespace TwelveG.GameController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Localization;
    using TwelveG.UIController;
    using TwelveG.PlayerController;

    public class WalkieTalkieQuestEvent : GameEventBase
    {
        [Header("References")]
        // [SerializeField] private GameObject suicideTriggerColliders;

        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;
        // public Transform suicideViewTransform;

        [Header("Text event SO")]
        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private GameEventSO updateFallbackTexts;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;
        // [SerializeField] private GameEventSO onCinematicCanvasControls;
        // [SerializeField] private GameEventSO onPlayerControls;
        // [SerializeField] private GameEventSO onPlayerDirectorControls;

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