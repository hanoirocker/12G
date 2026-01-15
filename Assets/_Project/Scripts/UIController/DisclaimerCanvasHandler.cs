using System.Collections;
using System.Collections.Generic;
using TwelveG.GameController;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.UIController
{
  public class DisclaimerCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private List<CanvasGroup> canvasGroups = new();
    [SerializeField] private CanvasGroup textsCanvasGroup = new();

    private Canvas disclaimerCanvas;
    private bool canvasGroupsReady = false;

    private void Awake()
    {
      disclaimerCanvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
      UpdateCanvasTextOnLanguageChanged(LocalizationManager.Instance.GetCurrentLanguageCode());
    }

    private void Start()
    {
      SetInitialCanvasGroupsAlpha();
    }

    private void SetInitialCanvasGroupsAlpha()
    {
      foreach (var group in canvasGroups)
      {
        group.alpha = 0;
      }
      canvasGroupsReady = true;
    }

    public IEnumerator DisclaimerFadeInSequence()
    {
      yield return new WaitUntil(() => canvasGroupsReady);

      disclaimerCanvas.enabled = true;

      yield return StartCoroutine(UIUtils.FadeIntroCanvasGroup(canvasGroups[0], 0f, 1f, 2f, 2f)); // Title
      yield return StartCoroutine(UIUtils.FadeIntroCanvasGroup(canvasGroups[1], 0f, 1f, 2f, 7f)); // Content
      yield return StartCoroutine(UIUtils.FadeIntroCanvasGroup(canvasGroups[2], 0f, 1f, 6f, 4f)); // Phrase
    }

    public IEnumerator DisclaimerFadeOutSequence()
    {
      yield return StartCoroutine(UIUtils.FadeIntroCanvasGroup(textsCanvasGroup, 1f, 0f, 3f));
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
