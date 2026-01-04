using System.Collections;
using System.Collections.Generic;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.UIController;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.GameController
{
    public class NoisesEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 0;

        [Space]
        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;

        [Space]
        [Header("Text event SO")]
        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [SerializeField] private DialogSO[] dialogSOs;
        [SerializeField] private List<ObservationTextSO> mainDoorsFallbacksTextsSO;
        [SerializeField] private List<UIOptionsTextSO> playerHelperDataTextSO;

        [Space]
        [Header("Other eventsSO references")]
        [SerializeField] private GameEventSO activateMicaEntranceCollider;
        [SerializeField] private GameEventSO triggerHouseLightsFlickering;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            // TODO: borrar (solo de prueba)
            GameEvents.Common.onEnablePlayerItem.Raise(this, ItemType.WalkieTalkie);

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            yield return new WaitForSeconds(initialTime);

            // Desde cualquier ventana del primer piso deberia tener ...
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[0]
            );

            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventObservationsTextsSOs[0].observationTextsStructure[0].observationText
            ));

            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(true));

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[0]);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

            activateMicaEntranceCollider.Raise(this, null);
            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.Helicopter1);

            // "Entrance - Spot" dispara el evento micaEntranceSpotted al ser chekeado por Simon
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

            // Mica .. no veo nada raro en la entrada de tu casa ..
            // (Mica y Simon concuerdan en pedir ayuda a la policia en canal 4)
            GameEvents.Common.onStartDialog.Raise(this, dialogSOs[0]);

            // Parpadean luces de la casa
            GameEvents.Common.triggerHouseLightsFlickering.Raise(this, 5f);

            // Espera a que termine la conversacion con Micaela "conversationHasEnded"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Debo cambiar al canal 4 y pedir ayuda cuanto antes ..
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[1]
            );

            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventObservationsTextsSOs[1].observationTextsStructure[0].observationText
            ));

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[1]);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[1]);


            // Cargar dialogo. El mismo no inicia hasta que el jugador cambie al canal 4.
            GameEvents.Common.onLoadDialogForSpecificChannel.Raise(this, new DialogForChannel
            {
                channelIndex = 3, // Canal 4 de la Policia
                dialogSO = dialogSOs[1]
            });

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

            // "conversationHasEnded"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(10f);
            // Self dialog: "................."
            GameEvents.Common.onLoadDialogForSpecificChannel.Raise(this, new DialogForChannel
            {
                channelIndex = 3, // Canal 4 de la Policia
                dialogSO = dialogSOs[2]
            });

            // "conversationHasEnded"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[2]);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[2]);

            yield return new WaitForSeconds(8f);

            // Dialogo interceptado de la policia
            GameEvents.Common.onLoadDialogForSpecificChannel.Raise(this, new DialogForChannel
            {
                channelIndex = 3, // Canal 4 de la Policia
                dialogSO = dialogSOs[3]
            });

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

            // "conversationHasEnded"
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return new WaitForSeconds(2f);
            // Parece que alguien ya dió aviso sobre el disparo ..
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[2]
            );

            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventObservationsTextsSOs[2].observationTextsStructure[0].observationText
            ));
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
