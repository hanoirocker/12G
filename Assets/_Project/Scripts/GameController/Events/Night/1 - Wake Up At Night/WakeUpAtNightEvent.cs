using System.Collections;
using TwelveG.AudioController;
using TwelveG.DialogsController;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.GameController
{
    public class WakeUpAtNightEvent : GameEventBase
    {
        [Header("Event options")]
        [SerializeField, Range(0f, 10f)] private float initialTime = 0f;

        [Space]
        [Header("Text event SO")]
        [SerializeField] private DialogSO dialogSO;
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;

        [Space]
        [Header("Audio Options")]
        [SerializeField] private AudioClip nightStandUpClip;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            AudioManager.Instance.FaderHandler.FadeAudioGroup(
                AudioGroup.inGameVol,
                0,
                AudioManager.Instance.GetCurrentChannelVolume("inGameVol"),
                7f
            );

            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            GameEvents.Common.onEnablePlayerHouseEnergy.Raise(this, true);
            GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.HardRainAndWind);
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Bed, true));
            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(false));

            yield return new WaitForSeconds(initialTime);

            // Empezar a hacer sonar el walkie-talkie (se desactiva solo al recoger el Walkie Talkie)
            FindAnyObjectByType<LightBeepHandler>().enabled = true;

            GameEvents.Common.onImageCanvasControls.Raise(this, new WakeUpBlinking());
            yield return new WaitForSeconds(4f);

            // Comentario sobre dolor de cabeza y algo asi
            GameEvents.Common.onStartDialog.Raise(this, dialogSO);
            yield return new WaitForSeconds(5f);

            // LEVANTARSE [E]
            GameEvents.Common.onEventInteractionCanvasShowText.Raise(
                this,
                eventsInteractionTextsSO
            );
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            GameEvents.Common.onInteractionCanvasControls.Raise(this, new HideText());

            AudioSource audioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
            if (audioSource != null && nightStandUpClip != null)
            {
                audioSource.PlayOneShot(nightStandUpClip);
            }

            GameEvents.Common.playWakeUpAtNightVCAnimation.Raise(this, null);

            // Unity Event (WakeUpVCHandler - onAnimationHasEnded):
            // Cuando termina la animación `incorporarse`, se pasa a lo próximo.
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));
            yield return new WaitForSeconds(1f);

            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Bed, false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerCameraZoom(true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            yield return new WaitForSeconds(1f);

            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));
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