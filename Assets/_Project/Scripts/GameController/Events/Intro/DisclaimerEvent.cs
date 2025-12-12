using UnityEngine;
using System.Collections;
using TwelveG.UIController;
using TwelveG.AudioController;
namespace TwelveG.GameController

{
    public class DisclaimerEvent : GameEventBase
    {
        [Header("Event references")]
        public AudioClip introTrack;

        private bool allowNextAction = false;

        public override IEnumerator Execute()
        {
            print("<------ Disclaimer event -------->");

            // Activar Disclaimer canvas y correr corrutina
            GameEvents.Common.onActivateCanvas.Raise(this, CanvasHandlerType.Disclaimer);

            // Correr corrutina de fade in del Disclaimer Canvas
            GameEvents.Common.onDisclaimerFadeIn.Raise(this, null);

            // Cargar audio clip y hacer
            // introAudioController.AudioFadeInSequence()
            AudioSource source = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
            source.clip = introTrack;
            source.volume = 0f;
            GameManager.Instance.GetComponentInChildren<InformationEvent>().introSource = source;
            yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(source, 0f, 1f, 2f));

            // DisclaimerCanvasHandler: envia onDisclaimerFadeInFinished
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onDisclaimerFadeOut.Raise(this, null);

            // DisclaimerCanvasHandler: envia onDisclaimerFadeOutFinished
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            GameEvents.Common.onDeactivateCanvas.Raise(this, CanvasHandlerType.Disclaimer);
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
