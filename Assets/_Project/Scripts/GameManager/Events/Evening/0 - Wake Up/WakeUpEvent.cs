namespace TwelveG.GameManager
{
    using System.Collections;
    using TwelveG.Localization;
    using UnityEngine;

    public class WakeUpEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(1, 10)] private int initialTime = 3;

        [Header("Text event SO")]
        public GameEventSO updateFallbackTexts;
        [SerializeField] private EventsFallbacksTextsSO eventsFallbacksTextsSO;

        [Header("EventsSO references")]
        public GameEventSO onControlCanvasControls;
        public GameEventSO onImageCanvasControls;
        public GameEventSO onObservationCanvasShowText;
        public GameEventSO onInteractionCanvasShowText;
        public GameEventSO onInteractionCanvasControls;
        public GameEventSO onVirtualCamerasControl;
        public GameEventSO onPlayerControls;

        [Header("Other eventsSO references")]
        public GameEventSO playWakeUpVCAnimation;
        public GameEventSO playCrashingWindowSound;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ WAKE UP EVENT NOW -------->");

            onObservationCanvasShowText.Raise(this, "BLA");

            yield return new WaitForSeconds(initialTime);

            updateFallbackTexts.Raise(this , eventsFallbacksTextsSO);

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
            onObservationCanvasShowText.Raise(this, "BLA");
            // TODO: quizas encontrar una forma de en vez de esperar 3 segundos,
            // ligarlo a cuando el canvas ya no esté mostrando el texto.
            yield return new WaitForSeconds(4f);

            onPlayerControls.Raise(this, "EnablePlayerShortcuts");

            onInteractionCanvasShowText.Raise(this, "LEVANTARSE [E]");
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
