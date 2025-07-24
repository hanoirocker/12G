namespace TwelveG.UIController
{
  using System.Collections;
  using System.Collections.Generic;
  using TMPro;
  using TwelveG.Localization;
  using UnityEngine;
  using UnityEngine.Localization.Settings;

  [RequireComponent(typeof(GameEventListener))]
  public class DisclaimerCanvasHandler : IntroCanvasBase
  {
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI content;
    [SerializeField] private TextMeshProUGUI phrase;

    [SerializeField] private List<CanvasGroup> canvasGroups = new();
    [SerializeField] private CanvasGroup textsCanvasGroup = new();

    [Header("Game Event SO's")]
    [SerializeField] private GameEventSO onDisclaimerFadeInFinished;
    [SerializeField] private GameEventSO onDisclaimerFadeOutFinished;

    [Header("Narrative Text SO")]
    [SerializeField] private NarrativeTextSO disclaimerTextSO;

    private Canvas disclaimerCanvas;
    private bool canvasGroupsReady = false;

    private void Awake()
    {
      disclaimerCanvas = GetComponent<Canvas>();
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

    public void DisclaimerFadeIn(Component sender, object data)
    {
      StartCoroutine(DisclaimerFadeInSequence());
    }

    public void DisclaimerFadeOut(Component sender, object data)
    {
      StartCoroutine(DisclaimerFadeOutSequence());
    }

    private IEnumerator DisclaimerFadeInSequence()
    {
      yield return new WaitUntil(() => canvasGroupsReady);

      var currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
      var localizedText = disclaimerTextSO.narrativeTextsStructure
        .Find(entry => entry.language.ToString().Equals(currentLanguage, System.StringComparison.OrdinalIgnoreCase));

      if (localizedText != null)
      {
        title.text = localizedText.title;
        content.text = localizedText.content;
        phrase.text = localizedText.phrase;
      }

      disclaimerCanvas.enabled = true;

      yield return FadeCanvasGroup(canvasGroups[0], 0f, 1f, 2f, 2f); // Title
      yield return FadeCanvasGroup(canvasGroups[1], 0f, 1f, 2f, 7f); // Content
      yield return FadeCanvasGroup(canvasGroups[2], 0f, 1f, 6f, 4f); // Phrase

      onDisclaimerFadeInFinished.Raise(this, null);
    }

    private IEnumerator DisclaimerFadeOutSequence()
    {
      yield return FadeCanvasGroup(textsCanvasGroup, 1f, 0f, 3f);

      onDisclaimerFadeOutFinished.Raise(this, null);
    }
  }
}
