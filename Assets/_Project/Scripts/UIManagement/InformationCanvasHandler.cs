namespace TwelveG.UIManagement
{
  using System.Collections;
  using TMPro;
  using TwelveG.Localization;
  using UnityEngine;
  using UnityEngine.Localization.Settings;

  // TODO?: Skip canvas logic
  [RequireComponent(typeof(GameEventListener))]
  public class InformationCanvasHandler : IntroCanvasBase
  {
    [Header("References")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI content;
    [SerializeField] private CanvasGroup elementsCanvasGroup;

    [Header("Settings")]
    [SerializeField, Range(1f, 7f)] float fadeDuration;

    [Header("Game Event SO's")]
    [SerializeField] GameEventSO onIntroAudioFadeOut;

    [Header("Narrative Text SO")]
    [SerializeField] private NarrativeTextSO disclaimerTextSO;

    private Canvas informationCanvas;
    // private bool skipRequested = false;

    private void Awake()
    {
      informationCanvas = GetComponent<Canvas>();
    }

    void Start()
    {
      elementsCanvasGroup.alpha = 0;
    }

    public IEnumerator RunSequence()
    {
      var currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
      var localizedText = disclaimerTextSO.narrativeTextsStructure
        .Find(entry => entry.language.ToString().Equals(currentLanguage, System.StringComparison.OrdinalIgnoreCase));

      if (localizedText != null)
      {
        title.text = localizedText.title;
        content.text = localizedText.content;
      }

      informationCanvas.enabled = true;

      yield return FadeCanvasGroup(elementsCanvasGroup, 0f, 1f, 3f, 8f); // All elements fade in

      onIntroAudioFadeOut.Raise(this, fadeDuration);

      yield return FadeCanvasGroup(elementsCanvasGroup, 1f, 0f, fadeDuration, 2f); // All elements fade out

      informationCanvas.enabled = false;
    }

    public void SkipSequence(Component sender, object data)
    {
      // skipRequested = true;
    }
  }
}