namespace TwelveG.UIController
{
  using System.Collections;
  using TwelveG.Localization;
  using UnityEngine;

  public class MenuCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private CanvasGroup backGroundCanvasGroup;

    private Canvas menuCanvas;

    private void Awake()
    {
      menuCanvas = GetComponent<Canvas>();
    }

    private  void OnEnable()
    {
      UpdateCanvasTextOnLanguageChanged(LocalizationManager.Instance.GetCurrentLanguageCode());
    }

    // Llamar a cada TextMeshProUGUI anidado para actualizar sus textos
    // en relación a sus propios assets SO
    public void UpdateCanvasTextOnLanguageChanged(string languageCode)
    {
      foreach (UpdateTextHandler updateTextHandler in GetComponentsInChildren<UpdateTextHandler>())
      {
        updateTextHandler.UpdateText(languageCode);
      }
    }

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