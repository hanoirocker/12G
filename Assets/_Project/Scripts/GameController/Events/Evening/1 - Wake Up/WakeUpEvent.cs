namespace TwelveG.GameController
{
    using System.Collections;
    using TwelveG.AudioController;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using TwelveG.UIController;
    using TwelveG.Utils;
    using UnityEngine;

    public class WakeUpEvent : GameEventBase
    {
        [Header("Event options")]
        [Space]
        [SerializeField, Range(1, 10)] private int initialTime = 3;

        [Header("Audio Options")]
        [Space]
        [SerializeField, Range(0f,1f)] private float maxbgMusicVol;
        [SerializeField, Range(0f,15f)] private float musicFadeTime;
        [SerializeField] private AudioClip bgMusic1;
        [SerializeField] private AudioClip standUpClip;
        

        [Header("Text event SO")]
        [Space]
        [SerializeField] private ObservationTextSO eventsObservationTextSO;
        [SerializeField] private EventsInteractionTextsSO eventsInteractionTextsSO;

        [Header("EventsSO references")]
        [Space]
        [SerializeField] private GameEventSO onImageCanvasControls;
        [SerializeField] private GameEventSO onControlCanvasControls;
        [SerializeField] private GameEventSO onObservationCanvasShowText;
        [SerializeField] private GameEventSO onEventInteractionCanvasShowText;
        [SerializeField] private GameEventSO onInteractionCanvasControls;
        [SerializeField] private GameEventSO onVirtualCamerasControl;
        [SerializeField] private GameEventSO onPlayerControls;

        [Header("Other eventsSO references")]
        [Space]
        [SerializeField] private GameEventSO playWakeUpVCAnimation;
        [SerializeField] private GameEventSO playCrashingWindowSound;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ WAKE UP EVENT NOW -------->");

            AudioSource bgMusicSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
            bgMusicSource.clip = bgMusic1;
            bgMusicSource.volume = 0;

            onControlCanvasControls.Raise(this, new EnableCanvas(false));

            yield return new WaitForSeconds(initialTime);

            onPlayerControls.Raise(this, new EnablePlayerControllers(false));
            onPlayerControls.Raise(this, new EnableControlCanvasAccess(false));

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.WakeUp, true));

            onPlayerControls.Raise(this, new EnablePlayerShortcuts(false));

            onPlayerControls.Raise(this, new TogglePlayerMainCamera(true));

            onPlayerControls.Raise(this, new EnablePlayerCameraZoom(false));

            playCrashingWindowSound.Raise(this, null);
            yield return new WaitForSeconds(3f);

            onImageCanvasControls.Raise(this, new WakeUpBlinking());
            yield return new WaitForSeconds(2f);

            // QUE MIERDA FUE ESO?
            onObservationCanvasShowText.Raise(
                this,
                eventsObservationTextSO
            );

            // TODO: quizas encontrar una forma de en vez de esperar 3 segundos,
            // ligarlo a cuando el canvas ya no esté mostrando el texto.
            yield return new WaitForSeconds(4f);

            onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            // LEVANTARSE [E]
            onEventInteractionCanvasShowText.Raise(
                this,
                eventsInteractionTextsSO
            );

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            bgMusicSource.Play();
            StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(bgMusicSource, 0f, maxbgMusicVol, musicFadeTime));

            AudioSource audioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Player);
            if (audioSource != null && standUpClip != null)
            {
                audioSource.PlayOneShot(standUpClip);
            }

            onInteractionCanvasControls.Raise(this, new HideText());

            playWakeUpVCAnimation.Raise(this, null);

            // Unity Event (WakeUpVCHandler - animationHasEnded):
            // Cuando termina la animación `incorporarse`, se pasa a lo próximo.
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, 1f));
            yield return new WaitForSeconds(1f);

            onVirtualCamerasControl.Raise(this, new ToggleVirtualCamera(VirtualCameraTarget.WakeUp, false));

            onPlayerControls.Raise(this, new EnablePlayerCameraZoom(true));

            onPlayerControls.Raise(this, new EnablePlayerShortcuts(true));

            yield return new WaitForSeconds(1f);

            onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));
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
