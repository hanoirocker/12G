using System.Collections;
using TwelveG.GameController;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.UIController
{
  [RequireComponent(typeof(GameEventListener))]
  public class InformationCanvasHandler : IntroCanvasBase
  {
    [Header("References")]
    [SerializeField] private CanvasGroup elementsCanvasGroup;

    private Canvas informationCanvas;

    private void Awake()
    {
      informationCanvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
      UpdateCanvasTextOnLanguageChanged(LocalizationManager.Instance.GetCurrentLanguageCode());
    }

    private void Start()
    {
      elementsCanvasGroup.alpha = 0;
    }

    public void InformationCanvasFadeIn(Component sender, object data)
    {
      if (data == null)
      {
        Debug.LogError($"[InformationCanvasFadeOut]: empty fadeInDuration param");
        return;
      }

      StartCoroutine(InformationFadeInSequence((float)data));
    }

    public void InformationCanvasFadeOut(Component sender, object data)
    {
      if (data == null)
      {
        Debug.LogError($"[InformationCanvasFadeOut]: empty fadeOutDuration param");
        return;
      }

      StartCoroutine(InformationFadeOutSequence((float)data));
    }

    public IEnumerator InformationFadeInSequence(float fadeInDuration)
    {

      informationCanvas.enabled = true;

      yield return FadeCanvasGroup(elementsCanvasGroup, 0f, 1f, fadeInDuration, 8f); // All elements fade in

      GameEvents.Common.onInformationFadeInFinished.Raise(this, null);
    }

    public IEnumerator InformationFadeOutSequence(float fadeOutDuration)
    {
      yield return FadeCanvasGroup(elementsCanvasGroup, 1f, 0f, fadeOutDuration, 2f); // All elements fade out

      GameEvents.Common.onInformationFadeOutFinished.Raise(this, null);
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
  }
}