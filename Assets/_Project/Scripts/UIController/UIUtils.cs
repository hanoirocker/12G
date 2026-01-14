using System.Collections;
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
  }
}