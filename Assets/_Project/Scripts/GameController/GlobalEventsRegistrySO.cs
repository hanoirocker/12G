using UnityEngine;

namespace TwelveG.GameController
{
    [CreateAssetMenu(fileName = "GlobalEventsRegistry", menuName = "TwelveG/Global Events Registry")]
    public class GlobalEventsRegistrySO : ScriptableObject
    {
        [Header("Canvas & UI")]
        public GameEventSO onObservationCanvasShowText;
        public GameEventSO onActivateCanvas;
        public GameEventSO onImageCanvasControls;
        public GameEventSO updateFallbackTexts;
        public GameEventSO onLoadPlayerHelperData;
        public GameEventSO onControlCanvasSetInteractionOptions;
        public GameEventSO onControlCanvasControls;
        public GameEventSO onEventInteractionCanvasShowText;
        public GameEventSO onContemplationCanvasControls;
        public GameEventSO onContemplationCanvasShowText;
        public GameEventSO onInteractionCanvasControls;
        public GameEventSO onInteractionCanvasShowText;
        public GameEventSO onShowNarrativeIntro;
        public GameEventSO onToggleInGameCanvasAll;
        public GameEventSO onInformationFadeIn;
        public GameEventSO onInformationFadeOut;
        public GameEventSO onDeactivateCanvas;
        public GameEventSO onDisclaimerFadeIn;
        public GameEventSO onDisclaimerFadeInFinished;
        public GameEventSO onDisclaimerFadeOut;
        public GameEventSO onDisclaimerFadeOutFinished;
        public GameEventSO onInformationFadeInFinished;
        public GameEventSO onInformationFadeOutFinished;
        public GameEventSO onShowIncomingCallPanel; // WT
        public GameEventSO onExaminationCanvasControls;
        public GameEventSO onExaminationCanvasShowText;

        [Header("Cinematics and Animations")]
        public GameEventSO onAnimationHasEnded;
        public GameEventSO onCinematicCanvasControls;
        public GameEventSO onCinematicBarsAnimationFinished;
        public GameEventSO onPlayerDirectorControls;
        public GameEventSO onCutSceneFinished;
        public GameEventSO onTimelineFinished;

        [Header("Player & Controls")]
        public GameEventSO onEnablePlayerItem;
        public GameEventSO onPlayerControls;
        public GameEventSO onVirtualCamerasControl;
        public GameEventSO onMainCameraSettings;

        [Header("Inventory")]
        public GameEventSO onPizzaPickedUp;

        [Header("Localization")]
        public GameEventSO onLanguageChanged;

        [Header("Dialogs")]
        public GameEventSO onShowDialog; // WT
        public GameEventSO onStartDialog;
        public GameEventSO onLoadDialogForSpecificChannel;
        public GameEventSO onDialogNodeRunning;
        public GameEventSO onConversationHasEnded;

        [Header("Environment")]
        public GameEventSO onStartWeatherEvent;
        public GameEventSO onSpawnVehicle;

        [Header("Game Management")]
        public GameEventSO onResetEventDrivenTexts; // para defaultear textos de Player Data Helper y mainDoorsFallbacks
        public GameEventSO onNewEventBegun;
        public GameEventSO onSceneLoaded;
        public GameEventSO onAnyKeyPressed;
        public GameEventSO onPlayGame;
        public GameEventSO onPauseGame;
        public GameEventSO onFinishedNarrativeCouritine;
    }
}