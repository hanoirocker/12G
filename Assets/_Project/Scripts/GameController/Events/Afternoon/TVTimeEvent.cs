namespace TwelveG.GameController
{
    using UnityEngine;
    using System.Collections;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using TwelveG.Utils;
    using TwelveG.UIController;
    using Cinemachine;
    using TwelveG.AudioController;

    public class TVTimeEvent : GameEventBase
    {
        [Header("Audio")]
        [SerializeField] private AudioClip wakeUpClip;
        [SerializeField] private AudioClip fallAsleepClip;

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
        public GameEventSO tvAudioFadeOut;
        public GameEventSO onActivateCanvas;
        public GameEventSO allowPlayerToHandleTV;
        public GameEventSO activateRemoteController;
        public GameEventSO onMainCameraSettings;
        public GameEventSO StartWeatherEvent;


        [Header("Settings")]

        [SerializeField, Range(3f, 5f)] private float eventFadeOut = 5f;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ TV TIME EVENT NOW -------->");

            StartWeatherEvent.Raise(this, WeatherEvent.SoftWind);
            AudioSource audioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Interaction);
            onPlayerControls.Raise(this, new EnablePlayerCameraZoom(false));
            enableTVHandler.Raise(this, null);

            onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.WakeUp, true));
            yield return new WaitForSeconds(2f);
            onImageCanvasControls.Raise(this, new WakeUpBlinking());

            if (audioSource != null && fallAsleepClip != null)
            {
                audioSource.PlayOneShot(wakeUpClip);
            }

            activateRemoteController.Raise(this, null);
            yield return new WaitForSeconds(5f);

            onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(true));
            onControlCanvasControls.Raise(this, new EnableCanvas(true));

            onPlayerControls.Raise(this, new EnablePlayerCameraZoom(true));
            onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            allowPlayerToHandleTV.Raise(this, true);

            // Unity Event (TVHandler - allowNextAction):
            // Se recibe cuando el jugador alcana el indice del canal principal
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onControlCanvasControls.Raise(this, new EnableCanvas(false));
            onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));
            onPlayerControls.Raise(this, new EnablePlayerHeadLookAround(false));
            onPlayerControls.Raise(this, new EnablePlayerCameraZoom(false));
            onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.EaseInOut, 4));
            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.TV, true));
            onCinematicCanvasControls.Raise(this, new ShowCinematicBars(true));
            yield return new WaitForSeconds(3f);

            onMainCameraSettings.Raise(this, new SetCameraBlend(CinemachineBlendDefinition.Style.Cut, 0));
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

            if (audioSource != null && fallAsleepClip != null)
            {
                audioSource.PlayOneShot(fallAsleepClip);
            }

            onInteractionCanvasControls.Raise(this, new VanishTextEffect());
            yield return new WaitForSeconds(2f);

            tvAudioFadeOut.Raise(this, eventFadeOut);

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, eventFadeOut));
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
