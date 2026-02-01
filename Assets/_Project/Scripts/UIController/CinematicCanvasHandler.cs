using System.Collections;
using System.Runtime.CompilerServices;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.UIController
{
    public class CinematicCanvasHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject container;
        [SerializeField] private Animation panelAnimation;

        [Space(10)]
        [SerializeField] private AnimationClip showBarsAnimationClip;
        [SerializeField] private AnimationClip hideBarsAnimationClip;


        private IEnumerator ToggleBars()
        {
            if (container.activeSelf)
            {
                panelAnimation.Play(hideBarsAnimationClip.name);
                yield return null; // Esperar un frame para que actualice el estado el animator
                yield return new WaitUntil(() => !panelAnimation.isPlaying);
                container.SetActive(false);
            }
            else
            {
                container.SetActive(true);
                panelAnimation.Play(showBarsAnimationClip.name);
                yield return null; // Esperar un frame para que actualice el estado el animator
                yield return new WaitUntil(() => !panelAnimation.isPlaying);
            }
            GameEvents.Common.onCinematicBarsAnimationFinished.Raise(this, null);
        }

        public void CinematicCanvasControls(Component sender, object data)
        {
            switch (data)
            {
                case ShowCinematicBars cmd:
                    StartCoroutine(ToggleBars());
                    break;
                default:
                    Debug.LogWarning($"[CinematicCanvasHandler] Received unknown command: {data}");
                    break;
            }
        }
    }

}