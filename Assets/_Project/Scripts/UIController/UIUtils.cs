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
    // Usado para: Logo de carga y Texto "Press Key"
    public static IEnumerator BlinkAlphaContinuous(CanvasGroup canvasGroup, float speed, float minAlpha = 0f, float maxAlpha = 1f)
    {
      float t = 0f;
      while (true)
      {
        t += Time.deltaTime * speed;
        canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(t, 1f));
        yield return null;
      }
    }

    // Usado para: Ícono de guardado
    public static IEnumerator BlinkAlphaForDuration(CanvasGroup canvasGroup, float duration, int cycles)
    {
      float elapsed = 0f;
      float cycleTime = duration / cycles;

      while (elapsed < duration)
      {
        elapsed += Time.deltaTime;
        canvasGroup.alpha = Mathf.PingPong(elapsed, cycleTime) / cycleTime;
        yield return null;
      }

      canvasGroup.alpha = 0f;
    }

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