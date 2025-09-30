namespace TwelveG.UIController
{
  using TMPro;
  using UnityEngine;
  using TwelveG.Localization;
  using System.Collections;

  public class ExaminationCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private CanvasGroup bgCanvasGroup;

    private TextMeshProUGUI examinationCanvasText;
    private Canvas examinationCanvas;

    private void Awake()
    {
      examinationCanvas = GetComponent<Canvas>();
      examinationCanvasText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
      examinationCanvas.enabled = false;
      examinationCanvasText.text = "";
    }

    private void OnDisable()
    {
      examinationCanvasText.text = "";
    }

    private IEnumerator ShowExaminationTextCoroutine(object data)
    {
      examinationCanvas.enabled = true;
      yield return StartCoroutine(FadeCanvasGroup(bgCanvasGroup, 0f, 0.7f, 0.5f));
      string textToShow = Utils.TextFunctions.RetrieveExaminationText(
      LocalizationManager.Instance.GetCurrentLanguageCode(),
      (ExaminationTextSO)data);
      examinationCanvasText.text = textToShow;
    }

    public void ShowExaminationText(Component sender, object data)
    {
      if ((ExaminationTextSO)data != null)
      {
        StartCoroutine(ShowExaminationTextCoroutine(data));
      }
    }

    public void ExaminationCanvasControls(Component sender, object data)
    {
      switch (data)
      {
        case EnableCanvas cmd:
          examinationCanvas.enabled = cmd.Enabled;
          break;
        default:
          Debug.LogWarning($"[ExaminationCanvasHandler] Received unknown command: {data}");
          break;
      }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
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