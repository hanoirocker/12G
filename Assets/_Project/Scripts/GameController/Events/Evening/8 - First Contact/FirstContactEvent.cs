namespace TwelveG.GameController
{
    using System.Collections;
    using UnityEngine;

    public class FirstContactEvent : GameEventBase
    {
        [Header("References")]
        // [SerializeField] private GameObject suicideTriggerColliders;

        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;
        // public Transform suicideViewTransform;

        [Header("Text event SO")]
        //[SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        //[SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        //[SerializeField] private GameEventSO updateFallbackTexts;

        [Header("EventsSO references")]
        // [SerializeField] private GameEventSO onObservationCanvasShowText;

        [Header("Other eventsSO references")]
        // [SerializeField] private GameEventSO drawerCanBeInteracted;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ FIRST CONTACT EVENT NOW -------->");

            yield return new WaitForSeconds(initialTime);

            // updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

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
