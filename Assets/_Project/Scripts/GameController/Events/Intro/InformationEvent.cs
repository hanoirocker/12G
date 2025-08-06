namespace TwelveG.GameController
{
    using UnityEngine;
    using System.Collections;
    using TwelveG.UIController;
    using TwelveG.AudioController;
    using UnityEngine.SceneManagement;

    public class InformationEvent : GameEventBase
    {
        [Header("Event references")]
        public AudioClip introTrack;

        [Header("Event Settings")]
        [SerializeField, Range(1f, 10f)] float fadeOutDuration;
        [SerializeField, Range(1f, 10f)] float fadeInDuration;

        [Header("EventsSO references")]
        public GameEventSO onActivateCanvas;
        public GameEventSO onDeactivateCanvas;
        public GameEventSO onInformationFadeIn;
        public GameEventSO onInformationFadeOut;

        [HideInInspector]
        public AudioSource introSource = null;

        private bool allowNextAction = false;
        private Coroutine fadeInCoroutine = null;

        public override IEnumerator Execute()
        {
            print("<------ Disclaimer event -------->");

            if (introSource == null)
            {
                introSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
                introSource.clip = introTrack;
                introSource.volume = 0f;
                fadeInCoroutine = StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(introSource, 0f, 1f, fadeInDuration));
            }

            // Activar Information canvas y correr corrutina
            onActivateCanvas.Raise(this, CanvasHandlerType.StudioInformation);

            onInformationFadeIn.Raise(this, fadeInDuration);

            // DisclaimerCanvasHandler: envia onInformationFadeInFinished
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            // Fade out del Information Canvas
            onInformationFadeOut.Raise(this, fadeOutDuration);

            // Si el fade in del audio se estaba ejecutando (por ejemplo por probar
            // específicamente este evento, se detiene para comenzar el fade out
            // del audio)
            if (fadeInCoroutine != null)
            {
                print("Deteniendo coroutina!");
                StopCoroutine(fadeInCoroutine);
                fadeInCoroutine = null;
            }

            // Fade out del audio al mismo tiempo
            yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(introSource, fadeOutDuration));

            // DisclaimerCanvasHandler: envia onInformationFadeOutFinished
            yield return new WaitUntil(() => allowNextAction);
            ResetAllowNextActions();

            onDeactivateCanvas.Raise(this, CanvasHandlerType.StudioInformation);

            // TODO: Carga asincrónica del Menu, esperar hasta que termine
            yield return new WaitForSeconds(3f);
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
