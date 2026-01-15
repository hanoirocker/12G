using System.Collections;
using TMPro;
using UnityEngine;

namespace TwelveG.UIController
{
  public struct UIColors
  {
    public static Color32 CONTROLS_ORANGE = new Color32(212, 126, 29, 255); // #D47E1D
    public static Color32 DEFAULT_RED = new Color32(255, 0, 0, 255); // #FF0000
  }

  public enum UIFormatingType
  {
    None,
    ControlsSpecificText,
    AlertColorText,
    ButtonHighlightColorText,
  }

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

    public static IEnumerator FadeIntroCanvasGroup(CanvasGroup group, float from, float to, float duration, float waitAfter = 0f, bool skipRequested = false)
    {
      float elapsed = 0f;

      if (!skipRequested)
      {
        while (elapsed < duration)
        {
          if (skipRequested)
            break;

          group.alpha = Mathf.Lerp(from, to, elapsed / duration);
          elapsed += Time.deltaTime;
          yield return null;
        }
      }
      group.alpha = to;

      if (waitAfter > 0f && !skipRequested)
        yield return new WaitForSeconds(waitAfter);
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