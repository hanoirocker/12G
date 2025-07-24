
namespace TwelveG.UIController
{
  using System.Collections;
  using UnityEngine;

  public abstract class IntroCanvasBase : MonoBehaviour
  {
    protected IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration, float waitAfter = 0f, bool skipRequested = false)
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
  }
}