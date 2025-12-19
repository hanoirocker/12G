using System.Collections;
using System.Collections.Generic;
using TwelveG.Localization;
using TwelveG.UIController;
using Cinemachine;
using TwelveG.PlayerController;
using TwelveG.Utils;
using UnityEngine;
using TwelveG.EnvironmentController;

namespace TwelveG.GameController
{
    public class LostSignal2Event : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;

        [Header("Text event SO")]
        [SerializeField] private List<ObservationTextSO> eventObservationsTextsSOs;
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private UIOptionsTextSO playerHelperDataTextSO;

        [Header("Other eventsSO references")]
        [SerializeField] private GameEventSO enableBackpack;
        [SerializeField] private GameEventSO disableBackpack;
        [SerializeField] private GameEventSO enablePhone;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            yield return new WaitForSeconds(initialTime);

            // Esto sera recibido por Backpack para activar el interactuable
            // y desactivar el contemplable.
            enableBackpack.Raise(this, null);
            enablePhone.Raise(this, null);

            // ¡Mi teléfono! Necesito hablar con Mica y ver si soy el único con esta suerte. Pero .. ¿Dónde lo habré dejado?
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventObservationsTextsSOs[0]
            );

            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventObservationsTextsSOs[0].observationTextsStructure[0].observationText
            ));

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO);

            yield return new WaitForSeconds(3f);
            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.FastCars);

            // Unity Event (PhonePrefabHandler - finishedUsingPhone):
            // Se recibe cuando el jugador deja de usar el teléfono
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // (El onResetEventDrivenTexts hace en PhonePrefabHandler)

            // Retorna a la camara del player desde la del sofá
            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));
            yield return new WaitForSeconds(1f);
            GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.Cut, 0));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Sofa, false));
            yield return new WaitForSeconds(1f);
            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.FastCars);

            // Nos aseguramos que el interactuable del Backpack se destruya
            // si no fue revisada hasta este punto.
            disableBackpack.Raise(this, null);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
            yield return new WaitForSeconds(2f);
        }

        public void AllowNextActions(Component sender, object data)
        {
            print(gameObject.name + "recieved event sent by: " + sender.gameObject.name);
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }
    }
}