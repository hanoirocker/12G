namespace TwelveG.GameController
{
    using System.Collections;
  using TwelveG.AudioController;
  using TwelveG.Localization;
    using TwelveG.PlayerController;
    using TwelveG.UIController;
    using TwelveG.Utils;
    using UnityEngine;

    public class LostSignalEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 1;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onObservationCanvasShowText;
        [SerializeField] private GameEventSO onImageCanvasControls;
        [SerializeField] private GameEventSO onEventInteractionCanvasShowText;
        [SerializeField] private GameEventSO onInteractionCanvasControls;
        [SerializeField] private GameEventSO onControlCanvasControls;
        [SerializeField] private GameEventSO onPlayerControls;
        [SerializeField] private GameEventSO onVirtualCamerasControl;

        [Header("Text event SO")]
        [SerializeField] private ObservationTextSO mainDoorsFallbacksTextsSO;
        [SerializeField] private ObservationTextSO eventsObservationTextSO;
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;

        [Header("Other eventsSO references")]
        [SerializeField] private GameEventSO enablePC;
        [SerializeField] private GameEventSO updateFallbackTexts;
        [SerializeField] private GameEventSO StartWeatherSound;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ LOST SIGNAL 1 EVENT NOW -------->");

            updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO);

            yield return new WaitForSeconds(initialTime);
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO
            );

            // Habilita el interactuable de la pc y desactiva el contemplable
            enablePC.Raise(this, null);

            // Unity Event (PCHandler - onPC):
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));

            onPlayerControls.Raise(this, new EnablePlayerControllers(false));

            yield return new WaitForSeconds(1f);

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.PC, true));

            onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(true));

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));

            // Unity Event (PCHandler - onPC):
            // El jugador abandona la PC y vuelve a retomar control
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            StartWeatherSound.Raise(this, WeatherSound.SoftWind);

            // onInteractionCanvasShowText.Raise(this, "LEVANTARSE [E]");
            onEventInteractionCanvasShowText.Raise(this, eventsInteractionTextsSO);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            onInteractionCanvasControls.Raise(this, new HideText());

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 2f));
            yield return new WaitForSeconds(2f);

            onControlCanvasControls.Raise(this, new EnableCanvas(false));

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.PC, false));

            onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(false));

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 2f));

            yield return new WaitForSeconds(2f);

            onPlayerControls.Raise(this, new EnablePlayerControllers(true));

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