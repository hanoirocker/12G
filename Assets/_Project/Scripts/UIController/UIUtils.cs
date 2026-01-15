using System.Collections;
using TMPro;
using UnityEngine;

namespace TwelveG.UIController
{
  public enum FadeType
  {
    FadeIn,
    FadeOut
  }

  public static class UIUtils
  {
    public static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float from, float to, float duration)
    {
      float elapsed = 0f;

      while (elapsed < duration)
      {
        canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
        elapsed += Time.deltaTime;
        yield return null;
      }
      canvasGroup.alpha = to;
    }

    public static IEnumerator VanishTextEffectCoroutine(TextMeshProUGUI canvasText, Canvas cavas)
    {
      float duration = 2f;

      float startAlpha = 1f;
      float endAlpha = 0f;

      float startFontSize = canvasText.fontSize;
      float maxFontSize = canvasText.fontSize * 1.25f;
      float textElapsedDuration = 0.15f * duration;

      float totalElapsedTime = 0f;
      while (totalElapsedTime < duration)
      {
        if (totalElapsedTime < textElapsedDuration)
        {
          canvasText.fontSize = Mathf.Lerp(startFontSize, maxFontSize, totalElapsedTime / textElapsedDuration);
        }
        else
        {
          canvasText.fontSize = Mathf.Lerp(maxFontSize, startFontSize, totalElapsedTime / duration);
        }

        canvasText.alpha = Mathf.Lerp(startAlpha, endAlpha, totalElapsedTime / duration);
        totalElapsedTime += Time.deltaTime;
        yield return null;
      }

      canvasText.alpha = endAlpha;
      canvasText.text = "";
      cavas.enabled = false;
      canvasText.alpha = 1f;
    }
  }
}