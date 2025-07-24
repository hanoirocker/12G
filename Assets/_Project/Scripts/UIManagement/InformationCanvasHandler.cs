namespace TwelveG.UIController
{
  using System.Collections;
  using TMPro;
  using TwelveG.Localization;
  using UnityEngine;
  using UnityEngine.Localization.Settings;

  [RequireComponent(typeof(GameEventListener))]
  public class InformationCanvasHandler : IntroCanvasBase
  {
    [Header("References")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI content;
    [SerializeField] private CanvasGroup elementsCanvasGroup;

    [Header("Game Event SO's")]
    [SerializeField] private GameEventSO onInformationFadeInFinished;
    [SerializeField] private GameEventSO onInformationFadeOutFinished;

    [Header("Narrative Text SO")]
    [SerializeField] private NarrativeTextSO disclaimerTextSO;

    private Canvas informationCanvas;

    private void Awake()
    {
      informationCanvas = GetComponent<Canvas>();
    }

    void Start()
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
      var currentLanguage = LocalizationSettings.SelectedLocale.Identifier.Code;
      var localizedText = disclaimerTextSO.narrativeTextsStructure
        .Find(entry => entry.language.ToString().Equals(currentLanguage, System.StringComparison.OrdinalIgnoreCase));

      if (localizedText != null)
      {
        title.text = localizedText.title;
        content.text = localizedText.content;
      }

      informationCanvas.enabled = true;

      yield return FadeCanvasGroup(elementsCanvasGroup, 0f, 1f, fadeInDuration, 8f); // All elements fade in

      onInformationFadeInFinished.Raise(this, null);
    }

    public IEnumerator InformationFadeOutSequence(float fadeOutDuration)
    {
      yield return FadeCanvasGroup(elementsCanvasGroup, 1f, 0f, fadeOutDuration, 2f); // All elements fade out

      onInformationFadeOutFinished.Raise(this, null);
    }
  }
}