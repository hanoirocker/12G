
namespace TwelveG.UIManagement
{
  using System.Collections;
  using UnityEngine;

  public class MenuCanvasHandler : MonoBehaviour
  {
    [SerializeField] CanvasGroup backGroundCanvasGroup;


    public void FadeBackgroundCanvas(float from, float to, float duration, float waitAfter = 0f)
    {
      StartCoroutine(FadeCanvasCoroutine(backGroundCanvasGroup, from, to, duration, waitAfter));
    }

    private IEnumerator FadeCanvasCoroutine(CanvasGroup group, float from, float to, float duration, float waitAfter = 0f)
    {
      float elapsed = 0f;

      while (elapsed < duration)
      {
        group.alpha = Mathf.Lerp(from, to, elapsed / duration);
        elapsed += Time.deltaTime;
        yield return null;
      }

      group.alpha = to;
    }
  }
}