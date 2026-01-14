namespace TwelveG.UIController
{
    using System.Collections;
    using UnityEngine;

    public class ImageCanvasHandler : MonoBehaviour
    {
        [Header("Image Canvas Settings")]
        public static bool canvasIsShowing;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeImageCanvas(FadeType fadeType, float duration)
        {
            if (fadeType == FadeType.FadeOut)
            {
                yield return UIUtils.FadeCanvasGroup(canvasGroup, 0f, 1f, duration);
            }
            else if (fadeType == FadeType.FadeIn)
            {
                yield return UIUtils.FadeCanvasGroup(canvasGroup, 1f, 0f, duration);
            }
        }

        public IEnumerator WakeUpBlinkingCoroutine()
        {
            canvasIsShowing = true;
            yield return FadeImageCanvas(FadeType.FadeIn, 0.5f);
            yield return FadeImageCanvas(FadeType.FadeOut, 0.5f);
            yield return FadeImageCanvas(FadeType.FadeIn, 2f);
            canvasIsShowing = false;
        }
    }
}