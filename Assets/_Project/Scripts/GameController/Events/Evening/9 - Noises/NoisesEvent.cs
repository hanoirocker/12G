using System.Collections;
using System.Collections.Generic;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
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
        [SerializeField] private DialogSO[] dialogSOs;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private GameEventSO updateFallbackTexts;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onStartDialog;
        [SerializeField] private GameEventSO onSpawnVehicle;

        [Header("Other eventsSO references")]
        [SerializeField] private GameEventSO activateMicaEntranceCollider;
        [SerializeField] private GameEventSO triggerHouseLightsFlickering;
        [SerializeField] private GameEventSO onLoadDialogForSpecificChannel;

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
            onSpawnVehicle.Raise(this, VehicleType.Helicopter1);

            // "Entrance - Spot" dispara el evento micaEntranceSpotted al ser chekeado por Simon
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Mica .. no veo nada raro en la entrada de tu casa ..
            // (Mica y Simon concuerdan en pedir ayuda a la policia en canal 4)
            onStartDialog.Raise(this, dialogSOs[0]);

            // Parpadean luces de la casa
            triggerHouseLightsFlickering.Raise(this, 5f);

            // Espera a que termine la conversacion con Micaela "conversationHasEnded"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Debo cambiar al canal 4 y pedir ayuda cuanto antes ..
            onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[1]
            );

            yield return new WaitForSeconds(2f);

            // Cargar dialogo. El mismo no inicia hasta que el jugador cambie al canal 4.
            onLoadDialogForSpecificChannel.Raise(this, new DialogForChannel
            {
                channelIndex = 3, // Canal 4 de la Policia
                dialogSO = dialogSOs[1]
            });

            // "conversationHasEnded"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(10f);
            // Self dialog: "................."
            onLoadDialogForSpecificChannel.Raise(this, new DialogForChannel
            {
                channelIndex = 3, // Canal 4 de la Policia
                dialogSO = dialogSOs[2]
            });

            // "conversationHasEnded"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(8f);

            // Dialogo interceptado de la policia
            onLoadDialogForSpecificChannel.Raise(this, new DialogForChannel
            {
                channelIndex = 3, // Canal 4 de la Policia
                dialogSO = dialogSOs[3]
            });

            // "conversationHasEnded"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(2f);
            // Parece que alguien ya dió aviso sobre el disparo ..
            onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[2]
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

        public void StopEvent()
        {
            StopAllCoroutines();
        }
    }
}
