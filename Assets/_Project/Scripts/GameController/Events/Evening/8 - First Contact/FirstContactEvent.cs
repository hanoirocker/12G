using System.Collections;
using System.Collections.Generic;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.UIController;
using UnityEngine;

namespace TwelveG.GameController
{
    public class FirstContactEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;

        [Space(10)]
        [Header("Text event SO")]
        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [Space(5)]
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private UIOptionsTextSO playerHelperDataTextSO;
        [Space(5)]
        [SerializeField] private DialogSO firstEventDialog;
        [SerializeField] private DialogSO secondEventDialog;
        [SerializeField] private DialogSO thirdEventDialog;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO);

            yield return new WaitForSeconds(initialTime);

            // Primer dialogo cuando obtiene el walkie talkie y lo setea al canal de Mica
            GameEvents.Common.onStartDialog.Raise(this, firstEventDialog);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.FastCars);

            GameEvents.Common.onEnablePlayerItem.Raise(this, ItemType.WalkieTalkie);

            // Llamado a Mica
            GameEvents.Common.onStartDialog.Raise(this, secondEventDialog);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();
            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.FastCars);

            // No hay chance que lo tenga encedido ... la puta madre.
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(
                eventObservationsTextsSOs[0]
            );

            // Tiempo de espera hasta que llame Mica
            yield return new WaitForSeconds(12f);
            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.FastCars);
            GameEvents.Common.onStartDialog.Raise(this, thirdEventDialog);

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
