using UnityEngine;
using System.Collections;
using TwelveG.UIController;
using TwelveG.AudioController;

namespace TwelveG.GameController
{
    public class InformationEvent : GameEventBase
    {
        [Header("Event references")]
        public AudioClip introTrack;

        [Header("Event Settings")]
        [SerializeField, Range(1f, 10f)] float fadeOutDuration;
        [SerializeField, Range(1f, 10f)] float fadeInDuration;

        [HideInInspector]
        private Coroutine fadeInCoroutine = null;

        public override IEnumerator Execute()
        {
            // Verificar si ya hay un audio source activo para evitar superposiciones
            AudioSource introSource = AudioManager.Instance.PoolsHandler.ReturnActiveSourceByType(AudioPoolType.BGMusic);
            if (introSource == null)
            {
                introSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
                introSource.clip = introTrack;
                introSource.volume = 0f;
                fadeInCoroutine = StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(introSource, 0f, 1f, fadeInDuration));
            }

            // Activar Information canvas y correr corrutina
            GameEvents.Common.onActivateCanvas.Raise(this, CanvasHandlerType.StudioInformation);
            yield return StartCoroutine(UIManager.Instance.InformationCanvasHandler.InformationFadeInSequence(fadeInDuration));

            // Fade out del Information Canvas
            yield return StartCoroutine(UIManager.Instance.InformationCanvasHandler.InformationFadeOutSequence(fadeOutDuration));

            // Si el fade in del audio se estaba ejecutando (por ejemplo por probar
            // específicamente este evento, se detiene para comenzar el fade out
            // del audio)
            if (fadeInCoroutine != null)
            {
                StopCoroutine(fadeInCoroutine);
                fadeInCoroutine = null;
            }

            // Fade out del audio al mismo tiempo
            yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(introSource, fadeOutDuration));
            introSource.Stop();
            AudioManager.Instance.PoolsHandler.ReleaseAudioSource(introSource);

            GameEvents.Common.onDeactivateCanvas.Raise(this, CanvasHandlerType.StudioInformation);

            // TODO: Carga asincrónica del Menu, esperar hasta que termine
            yield return new WaitForSeconds(1f);
        }
    }
}
