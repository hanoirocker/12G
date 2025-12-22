using TMPro;
using UnityEngine;
using TwelveG.Localization;
using System.Collections;

namespace TwelveG.UIController
{
  public struct ExaminationData
  {
    public ExaminationTextSO ExaminationTextSO;
  }

  public class ExaminationCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private CanvasGroup examinationCanvasGroup;
    [SerializeField] private CanvasGroup optionsCanvasGroup;
    [SerializeField] private TextMeshProUGUI examinationCanvasText;
    [SerializeField] private GameObject examineOption;

    private Canvas examinationCanvas;
    private ExaminationTextSO currentExaminationTextSO;

    private Coroutine activeTextCoroutine;
    private Coroutine activeFadeOptionsCoroutine;
    private Coroutine activeFadeExamCoroutine;

    private UpdateTextHandler[] cachedUpdateTextHandlers;

    private void Awake()
    {
      examinationCanvas = GetComponent<Canvas>();
      cachedUpdateTextHandlers = GetComponentsInChildren<UpdateTextHandler>(true);
    }

    private void Start()
    {
      UpdateCanvasTextOnLanguageChanged();
      examineOption.SetActive(false);

      // Inicialización segura de alphas
      if (examinationCanvasGroup != null) examinationCanvasGroup.alpha = 0f;
      if (optionsCanvasGroup != null) optionsCanvasGroup.alpha = 1f;
    }

    private IEnumerator ShowExaminationTextCoroutine()
    {
      examinationCanvasText.text = "";

      if (activeFadeOptionsCoroutine != null) StopCoroutine(activeFadeOptionsCoroutine);
      activeFadeOptionsCoroutine = StartCoroutine(FadeCanvasGroup(optionsCanvasGroup, optionsCanvasGroup.alpha, 0f, 0.5f));

      if (activeFadeExamCoroutine != null) StopCoroutine(activeFadeExamCoroutine);
      activeFadeExamCoroutine = StartCoroutine(FadeCanvasGroup(examinationCanvasGroup, examinationCanvasGroup.alpha, 0.7f, 0.5f));

      yield return new WaitForSeconds(0.1f);

      if (currentExaminationTextSO != null)
      {
        string textToShow = Utils.TextFunctions.RetrieveExaminationText(
            LocalizationManager.Instance.GetCurrentLanguageCode(),
            currentExaminationTextSO);
        examinationCanvasText.text = textToShow;
      }
    }

    private IEnumerator HideExaminationTextCoroutine()
    {
      examinationCanvasText.text = "";

      if (activeFadeOptionsCoroutine != null) StopCoroutine(activeFadeOptionsCoroutine);
      activeFadeOptionsCoroutine = StartCoroutine(FadeCanvasGroup(optionsCanvasGroup, optionsCanvasGroup.alpha, 1f, 0.5f));

      if (activeFadeExamCoroutine != null) StopCoroutine(activeFadeExamCoroutine);

      yield return activeFadeExamCoroutine = StartCoroutine(FadeCanvasGroup(examinationCanvasGroup, examinationCanvasGroup.alpha, 0f, 0.5f));
    }

    public void ShowExaminationText(Component sender, object data)
    {
      // Detenemos cualquier proceso anterior de mostrar/ocultar
      if (activeTextCoroutine != null) StopCoroutine(activeTextCoroutine);

      if ((bool)data)
      {
        activeTextCoroutine = StartCoroutine(ShowExaminationTextCoroutine());
      }
      else
      {
        activeTextCoroutine = StartCoroutine(HideExaminationTextCoroutine());
      }
    }

    public void ExaminationCanvasControls(Component sender, object data)
    {
      switch (data)
      {
        case ExaminationTextSO examTextSO:
          if (examTextSO != null)
          {
            currentExaminationTextSO = examTextSO;
            examineOption.SetActive(true);
          }
          break;

        case EnableCanvas cmd:
          if (activeTextCoroutine != null) StopCoroutine(activeTextCoroutine);
          if (activeFadeOptionsCoroutine != null) StopCoroutine(activeFadeOptionsCoroutine);
          if (activeFadeExamCoroutine != null) StopCoroutine(activeFadeExamCoroutine);

          examinationCanvasGroup.alpha = 0f;
          optionsCanvasGroup.alpha = 1f;
          examinationCanvasText.text = "";

          examinationCanvas.enabled = cmd.Enabled;
          break;

        default:
          // Caso por defecto para limpiar datos si llega null o data desconocida
          currentExaminationTextSO = null;
          examineOption.SetActive(false);
          break;
      }
    }

    public void UpdateCanvasTextOnLanguageChanged()
    {
      if (LocalizationManager.Instance == null) return;

      string languageCode = LocalizationManager.Instance.GetCurrentLanguageCode();

      // Usamos el array cacheado
      if (cachedUpdateTextHandlers != null)
      {
        for (int i = 0; i < cachedUpdateTextHandlers.Length; i++)
        {
          if (cachedUpdateTextHandlers[i] != null)
            cachedUpdateTextHandlers[i].UpdateText(languageCode);
        }
      }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
      float elapsed = 0f;
      group.alpha = from; // Aseguramos punto de partida

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