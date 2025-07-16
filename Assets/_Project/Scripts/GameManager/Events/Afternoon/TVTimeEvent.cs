namespace TwelveG.GameManager
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.Collections;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using TwelveG.Utils;
    using TwelveG.UIManagement;

    public class TVTimeEvent : GameEventBase
    {
        [Header("Event references")]
        [SerializeField] private GameObject remoteControlContainer;

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
        public GameEventSO allowPlayerToHandleTV;
        public GameEventSO disableTVHandler;
        public GameEventSO activateRemoteController;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ TV TIME EVENT NOW -------->");

            onPlayerControls.Raise(this, new TogglePlayerShortcuts(false));

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.WakeUp, true));

            enableTVHandler.Raise(this, null);

            onImageCanvasControls.Raise(this, new WakeUpBlinking());

            activateRemoteController.Raise(this, null);

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));

            yield return new WaitForSeconds(10f);

            onPlayerControls.Raise(this, new TogglePlayerHeadLookAround(true));

            onControlCanvasControls.Raise(this, new ActivateCanvas(true));

            onControlCanvasControls.Raise(this, new EnableCanvas(true));

            onPlayerControls.Raise(this, new TogglePlayerShortcuts(true));

            allowPlayerToHandleTV.Raise(this, null);

            // Unity Event (TVHandler - allowNextAction):
            // Se recibe cuando el jugador alcanza el indice del canal principal
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onControlCanvasControls.Raise(this, new ActivateCanvas(false));

            // onPlayerControls.Raise(this, new TogglePlayerShortcuts(false));

            onCinematicCanvasControls.Raise(this, new ShowCinematicBars(true));
            yield return new WaitForSeconds(3f);

            onPlayerControls.Raise(this, new TogglePlayerHeadLookAround(false));

            onPlayerControls.Raise(this, new TogglePlayerMainCamera(false));

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

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void AllowNextActions(Component sender, object data)
        {
            print(gameObject.name + "recibió eventoSO desde " + sender.gameObject.name + " para continuar el evento");
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }
    }
}
