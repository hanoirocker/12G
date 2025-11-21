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
        [SerializeField] private GameEventSO activateMicaEntranceCollider;
        [SerializeField] private GameEventSO triggerHouseLightsFlickering;

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

            activateMicaEntranceCollider.Raise(this, null);

            // "Entrance - Spot" dispara el evento micaEntranceSpotted al ser chekeado por Simon
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Mica .. no veo nada en la entrada raro en la entrada de tu casa ..
            startDialog.Raise(this, firstEventDialog);

            // Parpadean luces de la casa
            triggerHouseLightsFlickering.Raise(this, 5f);

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
