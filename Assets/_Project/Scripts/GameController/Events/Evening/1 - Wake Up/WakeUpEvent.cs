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
        [SerializeField, Range(1, 10)] private int initialTime = 3;

        [Space]
        [Header("Text event SO")]
        [SerializeField] private ObservationTextSO eventsObservationTextSO;
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            yield return new WaitForSeconds(initialTime);

            GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.SoftWind);
            UIManager.Instance.ControlCanvasHandler.ToggleControlCanvas(false);
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(false));

            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.WakeUp, true));

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));

            GameEvents.Common.onPlayerControls.Raise(this, new TogglePlayerMainCamera(true));

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(false));

            GameEvents.Common.playCrashingWindowSound.Raise(this, null);
            yield return new WaitForSeconds(3f);

            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.WakeUpBlinkingCoroutine());

            // QUE MIERDA FUE ESO?
            UIManager.Instance.ObservationCanvasHandler.ShowObservationText(eventsObservationTextSO);

            // TODO: quizas encontrar una forma de en vez de esperar 3 segundos,
            // ligarlo a cuando el canvas ya no esté mostrando el texto.
            yield return new WaitForSeconds(4f);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            // LEVANTARSE [E]
            UIManager.Instance.InteractionCanvasHandler.ShowEventInteractionText(eventsInteractionTextsSO);

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            StartCoroutine(
                AudioManager.Instance.GetComponentInChildren<PlayerSoundsHandler>().
                PlayPlayerSound(PlayerSoundsType.StandUpEvening)
            );

            UIManager.Instance.InteractionCanvasHandler.HideInteractionText();
            GameEvents.Common.playWakeUpVCAnimation.Raise(this, null);

            // Unity Event (WakeUpVCHandler - onAnimationHasEnded):
            // Cuando termina la animación `incorporarse`, se pasa a lo próximo.
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeOut, 1f));

            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.WakeUp, false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            yield return new WaitForSeconds(1f);

            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));
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
