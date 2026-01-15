using System.Collections;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.UIController
{
  public class InformationCanvasHandler : MonoBehaviour
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

    public IEnumerator InformationFadeInSequence(float fadeInDuration)
    {
      informationCanvas.enabled = true;
      yield return UIUtils.FadeIntroCanvasGroup(elementsCanvasGroup, 0f, 1f, fadeInDuration, 8f); // All elements fade in
    }

    public IEnumerator InformationFadeOutSequence(float fadeOutDuration)
    {
      yield return UIUtils.FadeIntroCanvasGroup(elementsCanvasGroup, 1f, 0f, fadeOutDuration, 2f); // All elements fade out
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