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
        [Header("Audio")]
        [SerializeField] private AudioClip wakeUpClip;
        [SerializeField] private AudioClip fallAsleepClip;

        [Header("Text event SO")]
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;

        [Header("Other eventsSO references")]
        public GameEventSO enableTVHandler;
        public GameEventSO tvAudioFadeOut;
        public GameEventSO allowPlayerToHandleTV;
        public GameEventSO activateRemoteController;


        [Header("Settings")]

        [SerializeField, Range(3f, 5f)] private float eventFadeOut = 5f;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ TV TIME EVENT NOW -------->");

            GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.SoftWind);
            AudioSource audioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Interaction);
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(false));
            enableTVHandler.Raise(this, null);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.WakeUp, true));
            yield return new WaitForSeconds(2f);
            GameEvents.Common.onImageCanvasControls.Raise(this, new WakeUpBlinking());

            if (audioSource != null && fallAsleepClip != null)
            {
                audioSource.PlayOneShot(wakeUpClip);
            }

            activateRemoteController.Raise(this, null);
            yield return new WaitForSeconds(5f);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(true));
            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(true));

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            allowPlayerToHandleTV.Raise(this, true);

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

            GameEvents.Common.onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.Cut, 0));
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

            if (audioSource != null && fallAsleepClip != null)
            {
                audioSource.PlayOneShot(fallAsleepClip);
            }

            GameEvents.Common.onInteractionCanvasControls.Raise(this, new VanishTextEffect());
            yield return new WaitForSeconds(2f);

            tvAudioFadeOut.Raise(this, eventFadeOut);

            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, eventFadeOut));
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
