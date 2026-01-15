using UnityEngine;
using System.Collections;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.Utils;
using TwelveG.UIController;
using Cinemachine;
using TwelveG.AudioController;

namespace TwelveG.GameController
{
    public class TVTimeEvent : GameEventBase
    {
        [Header("Settings")]
        [SerializeField, Range(3f, 5f)] private float eventFadeOut = 5f;

        [Space(10)]
        [Header("Text event SO")]
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;
        [SerializeField] private UIOptionsTextSO playerHelperDataTextSO;

        [Space(10)]
        [Header("Other eventsSO references")]
        public GameEventSO enableTVHandler;
        public GameEventSO tvAudioFadeOut;
        public GameEventSO allowPlayerToHandleTV;
        public GameEventSO activateRemoteController;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.None);
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(false));
            enableTVHandler.Raise(this, null);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.WakeUp, true));
            yield return new WaitForSeconds(2f);
            StartCoroutine(UIManager.Instance.ImageCanvasHandler.WakeUpBlinkingCoroutine());

            StartCoroutine(
                AudioManager.Instance.GetComponentInChildren<PlayerSoundsHandler>().
                PlayPlayerSound(PlayerSoundsType.WakeUpAfternoon)
            );

            activateRemoteController.Raise(this, null);
            yield return new WaitForSeconds(5f);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(true));
            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(true));

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            allowPlayerToHandleTV.Raise(this, true);
            GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO);

            // Unity Event (TVHandler - allowNextAction):
            // Se recibe cuando el jugador alcana el indice del canal principal
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(false));
            GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, 4));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.TV, true));
            GameEvents.Common.onCinematicCanvasControls.Raise(this, new ShowCinematicBars(true));
            yield return new WaitForSeconds(3f);

            GameEvents.Common.onMainCameraSettings.Raise(this, new ResetCinemachineBrain());
            // Activar TVTime - Main News timeline.
            GameEvents.Common.onPlayerDirectorControls.Raise(this, new ToggleTimelineDirector(0, true));

            // TODO: reemplazar por el valor del timeline `TV focus timeline`.
            yield return new WaitForSeconds(30f);

            GameEvents.Common.onCinematicCanvasControls.Raise(this, new ShowCinematicBars(false));

            yield return new WaitForSeconds(3f);

            // DESCANSAR UN RATO [E]
            GameEvents.Common.onEventInteractionCanvasShowText.Raise(
                this,
                eventsInteractionTextsSO
            );

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            StartCoroutine(
                AudioManager.Instance.GetComponentInChildren<PlayerSoundsHandler>().
                PlayPlayerSound(PlayerSoundsType.FallAsleepAfternoon)
            );

            UIManager.Instance.InteractionCanvasHandler.VanishTextEffect();
            yield return new WaitForSeconds(2f);

            tvAudioFadeOut.Raise(this, eventFadeOut);

            StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeOut, eventFadeOut));
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
