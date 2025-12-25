using System.Collections;
using TwelveG.AudioController;
using TwelveG.DialogsController;
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
        [SerializeField, Range(1, 10)] private int initialTime = 3;

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
            GameEvents.Common.onEnablePlayerHouseEnergy.Raise(this, false);
            GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.HardRain);
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Bed, true));
            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(false));

            yield return new WaitForSeconds(initialTime);

            GameEvents.Common.onImageCanvasControls.Raise(this, new WakeUpBlinking());
            yield return new WaitForSeconds(5f);

            // Comentario sobre qué carajo hace en la cama
            GameEvents.Common.onStartDialog.Raise(this, dialogSO);
            yield return new WaitForSeconds(8f);

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

            // ----- ESTO IRIA EN EL PROXIMO EVENTO -----

            // // Esto es para actualizar los fallback texts para las Main Doors en particular
            // GameEvents.Common.updateFallbackTexts.Raise(this, mainDoorsFallbacksTextsSO[0]);
            // // Actualizar text de ayuda del canvas del menu de pausa al presionar ESC
            // GameEvents.Common.onLoadPlayerHelperData.Raise(this, playerHelperDataTextSO[0]);

            GameEvents.Common.onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, 1f));
            yield return new WaitForSeconds(1f);

            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(true));
            GameEvents.Common.onControlCanvasControls.Raise(this, new EnableCanvas(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));

            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();
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