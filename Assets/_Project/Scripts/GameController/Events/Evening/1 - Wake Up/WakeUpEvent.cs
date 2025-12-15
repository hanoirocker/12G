using System.Collections;
using TwelveG.AudioController;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.GameController
{
    public class WakeUpEvent : GameEventBase
    {
        [Header("Event options")]
        [Space]
        [SerializeField, Range(1, 10)] private int initialTime = 3;

        [Header("Audio Options")]
        [Space]
        [SerializeField] private AudioClip standUpClip;

        [Header("Text event SO")]
        [Space]
        [SerializeField] private ObservationTextSO eventsObservationTextSO;
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;

        [Header("Other eventsSO references")]
        [Space]
        [SerializeField] private GameEventSO playWakeUpVCAnimation;
        [SerializeField] private GameEventSO playCrashingWindowSound;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ WAKE UP EVENT NOW -------->");
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            yield return new WaitForSeconds(initialTime);

            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(false));

            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.WakeUp, true));

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));

            GameEvents.Common.onPlayerControls.Raise(this, new TogglePlayerMainCamera(true));

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(false));

            playCrashingWindowSound.Raise(this, null);
            yield return new WaitForSeconds(3f);

            GameEvents.Common.onImageCanvasControls.Raise(this, new WakeUpBlinking());
            yield return new WaitForSeconds(2f);

            // QUE MIERDA FUE ESO?
            GameEvents.Common.onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO
            );

            // TODO: quizas encontrar una forma de en vez de esperar 3 segundos,
            // ligarlo a cuando el canvas ya no esté mostrando el texto.
            yield return new WaitForSeconds(4f);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            // LEVANTARSE [E]
            GameEvents.Common.onEventInteractionCanvasShowText.Raise(
                this,
                eventsInteractionTextsSO
            );

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            AudioSource audioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
            if (audioSource != null && standUpClip != null)
            {
                audioSource.PlayOneShot(standUpClip);
            }

            GameEvents.Common.onInteractionCanvasControls.Raise(this, new HideText());

            playWakeUpVCAnimation.Raise(this, null);

            // Unity Event (WakeUpVCHandler - onAnimationHasEnded):
            // Cuando termina la animación `incorporarse`, se pasa a lo próximo.
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));
            yield return new WaitForSeconds(1f);

            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.WakeUp, false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            yield return new WaitForSeconds(1f);

            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));
        }

        public void AllowNextActions(Component sender, object data)
        {
            print(gameObject.name + " recieved event sent by: " + sender.gameObject.name);
            allowNextAction = true;
        }

        public void ResetAllowNextActions()
        {
            allowNextAction = false;
        }

    }
}
