namespace TwelveG.UIController
{
    using System.Collections;
    using UnityEngine;

    public class CinematicCanvasHandler : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Animator containerAnimator;

        [Header("Game Event SO's")]
        [SerializeField] private GameEventSO onCinematicBarsAnimationFinished;

        private IEnumerator ShowBars()
        {
            container.SetActive(true);
            yield return null; // Esperar un frame para que actualice el estado el animator

            AnimatorStateInfo stateInfo = containerAnimator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length);
            onCinematicBarsAnimationFinished.Raise(this, null);
        }

        private IEnumerator HideBars()
        {
            if (container.activeSelf)
            {
                containerAnimator.SetTrigger("HideBars");
                yield return null; // Esperar un frame para que actualice el estado el animator

                AnimatorStateInfo stateInfo = containerAnimator.GetCurrentAnimatorStateInfo(0);
                yield return new WaitForSeconds(stateInfo.length);
                container.SetActive(false);
                onCinematicBarsAnimationFinished.Raise(this, null);
            }
        }

        public void CinematicCanvasControls(Component sender, object data)
        {
            switch (data)
            {
                case ShowCinematicBars cmd:
                    if (cmd.Show)
                    {
                        StartCoroutine(ShowBars());
                    }
                    else
                    {
                        StartCoroutine(HideBars());
                    }
                    break;
                default:
                    Debug.LogWarning($"[CinematicCanvasHandler] Received unknown command: {data}");
                    break;
            }
        }
    }

}