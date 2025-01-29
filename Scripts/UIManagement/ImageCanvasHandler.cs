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

        private void BlackOutScreen()
        {
            canvasGroup.alpha = 1;
        }

        private IEnumerator WakeUpBlinking()
        {
            canvasIsShowing = true;
            yield return FadeInImage(0.5f);
            yield return FadeOutImage(0.5f);
            yield return FadeInImage(2f);
            canvasIsShowing = false;
        }

        public void ImageCanvasControls(Component sender, object data)
        {
            if ((string)data == "WakeUpBlinking")
            {
                StartCoroutine(WakeUpBlinking());
                return;
            }
            else if ((string)data == "FadeInImage")
            {
                StartCoroutine(FadeInImage(1f));
            }
            else if ((string)data == "FadeInImage2")
            {
                StartCoroutine(FadeInImage(2f));
            }
            else if ((string)data == "FadeOutImage")
            {
                StartCoroutine(FadeOutImage(1f));
            }
            else if ((string)data == "FadeOutImage2")
            {
                StartCoroutine(FadeOutImage(2f));
            }
            else if ((string)data == "LongFadeOutImage")
            {
                StartCoroutine(FadeOutImage(5f));
            }
        }
    }
}