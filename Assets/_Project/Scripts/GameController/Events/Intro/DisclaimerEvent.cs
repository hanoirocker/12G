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

        public override IEnumerator Execute()
        {
            // Activar Disclaimer canvas y correr corrutina
            GameEvents.Common.onActivateCanvas.Raise(this, CanvasHandlerType.Disclaimer);

            // Correr corrutina de fade in del Disclaimer Canvas
            Coroutine fadeInSequence = StartCoroutine(UIManager.Instance.DisclaimerCanvasHandler.DisclaimerFadeInSequence());

            // Cargar audio clip y hacer
            // introAudioController.AudioFadeInSequence()
            AudioSource source = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
            source.clip = introTrack;
            source.volume = 0f;
            GameManager.Instance.GetComponentInChildren<InformationEvent>().introSource = source;

            yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(source, 0f, 1f, 2f));

            yield return fadeInSequence;

            yield return StartCoroutine(UIManager.Instance.DisclaimerCanvasHandler.DisclaimerFadeOutSequence());

            GameEvents.Common.onDeactivateCanvas.Raise(this, CanvasHandlerType.Disclaimer);
        }
    }
}
