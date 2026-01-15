using System.Collections;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.EnvironmentController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.GameController
{
    public class LostSignalEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;

        [Header("Text event SO")]
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private UIOptionsTextSO playerHelperDataTextSO;
        [SerializeField] private ObservationTextSO eventsObservationTextSO;
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;
        [SerializeField] private DialogSO dialogSOs;

        [Header("Other eventsSO references")]
        [SerializeField] private GameEventSO enablePC;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            yield return new WaitForSeconds(initialTime);

            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(eventsObservationTextSO);

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.SlowCars);

            yield return new WaitForSeconds(TextFunctions.CalculateTextDisplayDuration(
                eventsObservationTextSO.observationTextsStructure[0].observationText
            ));

            GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO);

            // Habilita el interactuable de la pc y desactiva el contemplable
            enablePC.Raise(this, null);

            // Unity Event (PCHandler - onPC):
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);

            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeOut, 1f));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.PC, true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(true));
            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeIn, 1f));
            // Unity Event (PCHandler - onPC):
            // El jugador abandona la PC y vuelve a retomar control
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onSpawnVehicle.Raise(this, VehicleType.FastCars);
            GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.SoftWind);
            UIManager.Instance.InteractionCanvasHandler.ShowEventInteractionText(eventsInteractionTextsSO);

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            UIManager.Instance.InteractionCanvasHandler.HideInteractionText();
            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeOut, 2f));
            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(false));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.PC, false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(false));
            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeIn, 2f));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));

            yield return new WaitForSeconds(2f);
            // Simon expresa que le duele la cabeza al dejar la PC
            GameEvents.Common.onStartDialog.Raise(this, dialogSOs);

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
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