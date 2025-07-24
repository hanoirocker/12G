namespace TwelveG.GameController
{
    using UnityEngine;
    using System.Collections;
    using TwelveG.UIController;
    using TwelveG.AudioController;

    public class DisclaimerEvent : GameEventBase
    {
        [Header("Event references")]
        public AudioClip introTrack;

        [Header("Text event SO")]

        [Header("EventsSO references")]
        public GameEventSO onActivateCanvas;
        public GameEventSO onDeactivateCanvas;
        public GameEventSO onDisclaimerCanvasFadeIn;
        public GameEventSO onDisclaimerCanvasFadeOut;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ Disclaimer event -------->");

            // Activar Disclaimer canvas y correr corrutina
            onActivateCanvas.Raise(this, CanvasHandlerType.Disclaimer);

            // Correr corrutina de fade in del Disclaimer Canvas
            onDisclaimerCanvasFadeIn.Raise(this, null);

            // Cargar audio clip y hacer
            // introAudioController.AudioFadeInSequence()
            AudioSource source = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
            source.clip = introTrack;
            source.volume = 0f;
            GameManager.Instance.GetComponentInChildren<InformationEvent>().introSource = source;
            yield return StartCoroutine(AudioManager.Instance.FaderHandler.RunAudioFadeIn(source, 0f, 1f, 2f));

            // DisclaimerCanvasHandler: envia onDisclaimerFadeInFinished
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onDisclaimerCanvasFadeOut.Raise(this, null);

            // DisclaimerCanvasHandler: envia onDisclaimerFadeOutFinished
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onDeactivateCanvas.Raise(this, CanvasHandlerType.Disclaimer);
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
