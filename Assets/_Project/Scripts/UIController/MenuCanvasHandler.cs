
namespace TwelveG.UIController
{
  using System.Collections;
  using UnityEngine;

  public class MenuCanvasHandler : MonoBehaviour
  {
    [SerializeField] CanvasGroup backGroundCanvasGroup;

    public void MenuBGFadeInCanvas(Component sender, object data)
    {
      if (data != null)
      {
        StartCoroutine(FadeCanvasCoroutine(backGroundCanvasGroup, 1f, 0f, (float)data));
      }
      else
      {
        Debug.LogError($"[MenuBGFadeInCanvas]: Duration param not passed on Raise at {sender.name} ");
      }
    }

    public void MenuBGFadeOutCanvas(Component sender, object data)
    {
      if (data != null)
      {
        StartCoroutine(FadeCanvasCoroutine(backGroundCanvasGroup, 0f, 1f, (float)data));
      }
      else
      {
        Debug.LogError($"[MenuBGFadeOutCanvas]: Duration param not passed on Raise at {sender.name} ");
      }
    }

    private IEnumerator FadeCanvasCoroutine(CanvasGroup group, float from, float to, float duration)
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