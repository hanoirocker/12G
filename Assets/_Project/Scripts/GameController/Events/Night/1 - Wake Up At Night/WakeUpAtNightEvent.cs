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
            GameEvents.Common.onTogglePlayerHouseEnergy.Raise(this, true);
            GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.HardRainAndWind);
            GameEvents.Common.onResetEventDrivenTexts.Raise(this, null);
            GameEvents.Common.onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.Bed, true));
            UIManager.Instance.ControlCanvasHandler.ToggleControlCanvas(false);
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(false));

            yield return new WaitForSeconds(initialTime);

            // Empezar a hacer sonar el walkie-talkie (se desactiva solo al recoger el Walkie Talkie)
            FindAnyObjectByType<LightBeepHandler>().enabled = true;

            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.WakeUpBlinkingCoroutine());

            // Comentario sobre dolor de cabeza y algo asi
            GameEvents.Common.onStartDialog.Raise(this, dialogSO);
            yield return new WaitForSeconds(5f);

            // LEVANTARSE [E]
            UIManager.Instance.InteractionCanvasHandler.ShowEventInteractionText(eventsInteractionTextsSO);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            UIManager.Instance.InteractionCanvasHandler.HideInteractionText();

            StartCoroutine(
                AudioManager.Instance.GetComponentInChildren<PlayerSoundsHandler>().
                PlayPlayerSound(PlayerSoundsType.StandUpNight)
            );

            GameEvents.Common.playWakeUpAtNightVCAnimation.Raise(this, null);

            // Tiempo de espera aproximado en el que la animación del jugador mira hacia la ventana
            yield return new WaitForSeconds(2.5f);
            GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.CloseThunder);

            // Unity Event (WakeUpVCHandler - onAnimationHasEnded):
            // Cuando termina la animación `incorporarse`, se pasa a lo próximo.
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeOut, 1f));

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