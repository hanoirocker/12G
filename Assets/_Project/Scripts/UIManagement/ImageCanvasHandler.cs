namespace TwelveG.UIManagement
{
    using System.Collections;
    using UnityEngine;

    public class ImageCanvasHandler : MonoBehaviour
    {
        public static bool canvasIsShowing;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private IEnumerator FadeInImage(float duration)
        {
            float startAlpha = 1;
            float endAlpha = 0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
        }

        private IEnumerator FadeOutImage(float duration)
        {
            float startAlpha = 0;
            float endAlpha = 1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
        }

        private IEnumerator WakeUpBlinkingCoroutine()
        {
            canvasIsShowing = true;
            yield return FadeInImage(0.5f);
            yield return FadeOutImage(0.5f);
            yield return FadeInImage(2f);
            canvasIsShowing = false;
        }

        public void ImageCanvasControls(Component sender, object data)
        {
            switch (data)
            {
                case WakeUpBlinking:
                    StartCoroutine(WakeUpBlinkingCoroutine());
                    break;

                case FadeImage fade:
                    if (fade.FadeType == FadeType.FadeIn)
                    {
                        StartCoroutine(FadeInImage(fade.Duration));
                    }
                    else if (fade.FadeType == FadeType.FadeOut)
                    {
                        StartCoroutine(FadeOutImage(fade.Duration));
                    }
                    break;

                default:
                    Debug.LogWarning($"[ImageCanvasHandler] Received unknown command: {data?.GetType().Name}");
                    break;
            }
        }
    }
}