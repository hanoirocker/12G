namespace TwelveG.GameManager
{
    using System.Collections;
    using TwelveG.Localization;
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
        public GameEventSO enablePC;
        [SerializeField] private GameEventSO updateFallbackTexts;

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

            onImageCanvasControls.Raise(this, "FadeOutImage");

            onPlayerControls.Raise(this, "DisablePlayerCapsule");

            yield return new WaitForSeconds(1f);

            onVirtualCamerasControl.Raise(this, "EnablePCVC");

            onPlayerControls.Raise(this, "EnableHeadLookAround");

            onImageCanvasControls.Raise(this, "FadeInImage");

            // Unity Event (PCHandler - onPC):
            // El jugador abandona la PC y vuelve a retomar control
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // onInteractionCanvasShowText.Raise(this, "LEVANTARSE [E]");
            onEventInteractionCanvasShowText.Raise(this, eventsInteractionTextsSO);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            onImageCanvasControls.Raise(this, "FadeOutImage2");
            yield return new WaitForSeconds(2f);

            onInteractionCanvasControls.Raise(this, "HideText");

            onControlCanvasControls.Raise(this, "HideControlCanvas");

            onVirtualCamerasControl.Raise(this, "DisablePCVC");

            onPlayerControls.Raise(this, "DisableHeadLookAround");

            onImageCanvasControls.Raise(this, "FadeInImage2");

            onPlayerControls.Raise(this, "EnablePlayerCapsule");

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