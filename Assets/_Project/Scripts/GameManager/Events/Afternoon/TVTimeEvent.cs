namespace TwelveG.GameManager
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.Collections;
    using TwelveG.Localization;

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

            onPlayerControls.Raise(this, "DisablePlayerShortcuts");

            onVirtualCamerasControl.Raise(this, "EnableWakeUpVC");

            enableTVHandler.Raise(this, null);

            onImageCanvasControls.Raise(this, "WakeUpBlinking");

            activateRemoteController.Raise(this, null);

            onImageCanvasControls.Raise(this, "FadeInImage");

            yield return new WaitForSeconds(10f);

            onPlayerControls.Raise(this, "EnableHeadLookAround");

            onControlCanvasControls.Raise(this, "ActivateControlCanvas");

            onControlCanvasControls.Raise(this, "ShowControlCanvas");

            onPlayerControls.Raise(this, "EnablePlayerShortcuts");

            allowPlayerToHandleTV.Raise(this, null);

            // Unity Event (TVHandler - allowNextAction):
            // Se recibe cuando el jugador alcanza el indice del canal principal
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onControlCanvasControls.Raise(this, "DeactivateControlCanvas");

            // onPlayerControls.Raise(this, "DisablePlayerShortcuts");

            onCinematicCanvasControls.Raise(this, "ShowBars");
            yield return new WaitForSeconds(3f);

            onPlayerControls.Raise(this, "DisableHeadLookAround");

            onPlayerControls.Raise(this, "DisableMainCamera");

            onPlayerDirectorControls.Raise(this, "EnableTimeline2Director");

            // TODO: reemplazar por el valor del timeline `TV focus timeline`.
            yield return new WaitForSeconds(30f);

            onCinematicCanvasControls.Raise(this, "HideBars");

            yield return new WaitForSeconds(3f);

            // DESCANSAR UN RATO [E]
            onEventInteractionCanvasShowText.Raise(
                this,
                eventsInteractionTextsSO
            );

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            onInteractionCanvasControls.Raise(this, "VanishTextEffect");
            yield return new WaitForSeconds(2f);


            onImageCanvasControls.Raise(this, "LongFadeOutImage");
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
