namespace TwelveG.GameController
{
    using UnityEngine;
    using System.Collections;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using TwelveG.Utils;
    using TwelveG.UIController;
    using Cinemachine;

    public class TVTimeEvent : GameEventBase
    {
        [Header("Text event SO")]
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;

        [Header("EventsSO references")]
        public GameEventSO onControlCanvasControls;
        public GameEventSO onImageCanvasControls;
        public GameEventSO onDialogCanvasShowDialog;
        public GameEventSO onEventInteractionCanvasShowText;
        public GameEventSO onInteractionCanvasControls;
        public GameEventSO onCinematicCanvasControls;
        public GameEventSO onVirtualCamerasControl;
        public GameEventSO onPlayerControls;
        public GameEventSO onPlayerDirectorControls;

        [Header("Other eventsSO references")]
        public GameEventSO enableTVHandler;
        public GameEventSO onActivateCanvas;
        public GameEventSO allowPlayerToHandleTV;
        public GameEventSO activateRemoteController;
        public GameEventSO onMainCameraSettings;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ TV TIME EVENT NOW -------->");
            enableTVHandler.Raise(this, null);

            onPlayerControls.Raise(this, new TogglePlayerShortcuts(false));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.WakeUp, true));
            yield return new WaitForSeconds(2f);
            onImageCanvasControls.Raise(this, new WakeUpBlinking());

            activateRemoteController.Raise(this, null);
            yield return new WaitForSeconds(5f);

            onPlayerControls.Raise(this, new TogglePlayerHeadLookAround(true));

            onActivateCanvas.Raise(this, CanvasHandlerType.Control);
            onControlCanvasControls.Raise(this, new EnableCanvas(true));

            onPlayerControls.Raise(this, new TogglePlayerCameraZoom(true));
            onPlayerControls.Raise(this, new TogglePlayerShortcuts(true));

            allowPlayerToHandleTV.Raise(this, null);

            // Unity Event (TVHandler - allowNextAction):
            // Se recibe cuando el jugador alcana el indice del canal principal
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();


            onPlayerControls.Raise(this, new TogglePlayerShortcuts(false));
            onPlayerControls.Raise(this, new TogglePlayerHeadLookAround(false));
            onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, 4));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.TV, true));
            onCinematicCanvasControls.Raise(this, new ShowCinematicBars(true));
            yield return new WaitForSeconds(3f);

            // Activar TVTime - Main News timeline.
            onPlayerDirectorControls.Raise(this, new ToggleTimelineDirector(0, true));

            // TODO: reemplazar por el valor del timeline `TV focus timeline`.
            yield return new WaitForSeconds(30f);

            onCinematicCanvasControls.Raise(this, new ShowCinematicBars(false));

            yield return new WaitForSeconds(3f);

            // DESCANSAR UN RATO [E]
            onEventInteractionCanvasShowText.Raise(
                this,
                eventsInteractionTextsSO
            );

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            onInteractionCanvasControls.Raise(this, new VanishTextEffect());
            yield return new WaitForSeconds(2f);


            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 5f));
            yield return new WaitForSeconds(5f);
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
