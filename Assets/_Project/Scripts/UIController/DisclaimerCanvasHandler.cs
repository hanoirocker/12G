using System.Collections;
using System.Collections.Generic;
using TwelveG.GameController;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.UIController
{
  [RequireComponent(typeof(GameEventListener))]
  public class DisclaimerCanvasHandler : IntroCanvasBase
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

    private IEnumerator DisclaimerFadeInSequence()
    {
      yield return new WaitUntil(() => canvasGroupsReady);

      disclaimerCanvas.enabled = true;

      yield return FadeCanvasGroup(canvasGroups[0], 0f, 1f, 2f, 2f); // Title
      yield return FadeCanvasGroup(canvasGroups[1], 0f, 1f, 2f, 7f); // Content
      yield return FadeCanvasGroup(canvasGroups[2], 0f, 1f, 6f, 4f); // Phrase

      GameEvents.Common.onDisclaimerFadeInFinished.Raise(this, null);
    }

    private IEnumerator DisclaimerFadeOutSequence()
    {
      yield return FadeCanvasGroup(textsCanvasGroup, 1f, 0f, 3f);

      GameEvents.Common.onDisclaimerFadeOutFinished.Raise(this, null);
    }

    public void DisclaimerFadeIn(Component sender, object data)
    {
      StartCoroutine(DisclaimerFadeInSequence());
    }

    public void DisclaimerFadeOut(Component sender, object data)
    {
      StartCoroutine(DisclaimerFadeOutSequence());
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
