namespace TwelveG.GameManager
{
    using System.Collections;
    using System.Collections.Generic;
    using TwelveG.Localization;
    using UnityEngine;

    public class WakeUpEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 3;

        [Header("Text event SO")]
        [SerializeField] private GameEventSO updateFallbackTexts;
        [SerializeField] private ObservationTextSO eventsObservationTextSO;
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;

        [Header("EventsSO references")]
        [SerializeField] private GameEventSO onControlCanvasControls;
        [SerializeField] private GameEventSO onImageCanvasControls;
        [SerializeField] private GameEventSO onObservationCanvasShowText;
        [SerializeField] private GameEventSO onEventInteractionCanvasShowText;
        [SerializeField] private GameEventSO onInteractionCanvasControls;
        [SerializeField] private GameEventSO onVirtualCamerasControl;
        [SerializeField] private GameEventSO onPlayerControls;

        [Header("Other eventsSO references")]
        [SerializeField] private GameEventSO playWakeUpVCAnimation;
        [SerializeField] private GameEventSO playCrashingWindowSound;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ WAKE UP EVENT NOW -------->");

            yield return new WaitForSeconds(initialTime);

            onPlayerControls.Raise(this, "DisablePlayerCapsule");

            onVirtualCamerasControl.Raise(this, "EnableWakeUpVC");

            onPlayerControls.Raise(this, "DisablePlayerShortcuts");

            onControlCanvasControls.Raise(this, "DeactivateControlCanvas");

            onPlayerControls.Raise(this, "EnableMainCamera");

            onPlayerControls.Raise(this, "DisableCameraZoom");

            playCrashingWindowSound.Raise(this, null);
            yield return new WaitForSeconds(3f);

            onImageCanvasControls.Raise(this, "WakeUpBlinking");
            yield return new WaitForSeconds(2f);

            // QUE MIERDA FUE ESO?
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO
            );

            // TODO: quizas encontrar una forma de en vez de esperar 3 segundos,
            // ligarlo a cuando el canvas ya no esté mostrando el texto.
            yield return new WaitForSeconds(4f);

            onPlayerControls.Raise(this, "EnablePlayerShortcuts");

            // LEVANTARSE [E]
            // TODO (FIX): si mientras se muestra el texto, se cambia de locale, aparece
            // el texto por defecto de interacion del canvas.
            // (no actualiza al nuevo texto)
            onEventInteractionCanvasShowText.Raise(
                this,
                eventsInteractionTextsSO
            );

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            onInteractionCanvasControls.Raise(this, "HideText");

            playWakeUpVCAnimation.Raise(this, null);

            // Unity Event (WakeUpVCHandler - animationHasEnded):
            // Cuando termina la animación `incorporarse`, se pasa a lo próximo.
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();


            onImageCanvasControls.Raise(this, "FadeOutImage");
            yield return new WaitForSeconds(1f);

            onVirtualCamerasControl.Raise(this, "DisableWakeUpVC");

            onPlayerControls.Raise(this, "EnableCameraZoom");

            onPlayerControls.Raise(this, "EnablePlayerShortcuts");

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
