using System;
using System.Collections;
using TMPro;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.UIController
{
  public class MenuCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private CanvasGroup menuCanvasGroup;
    [Space(5)]
    [SerializeField] private UpdateTextHandler[] updateTextHandlers;

    [Space(10)]
    [Header("Settings")]
    [SerializeField, Range(0.5f, 3f)] private float fadeOutDuration = 1.5f;

    private Canvas menuCanvas;

    private void Awake()
    {
      menuCanvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;

      foreach (var textHandler in updateTextHandlers)
      {
        UIManager.Instance.UIFormatter.AssignFontByType(
        textHandler.uITextType,
        textHandler.GetComponent<TextMeshProUGUI>()
        );
      }
    }

    private void OnDisable()
    {
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator StartPlaying(bool isNewGame)
    {
      yield return StartCoroutine(UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeOut, fadeOutDuration));

      GameManager.Instance.PlayGame(isNewGame);

      gameObject.SetActive(false);
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

    public void PlayOption(bool isNewGame)
    {
      StartCoroutine(StartPlaying(isNewGame));
      StartCoroutine(FadeMenuCanvasRoutine());
    }

    private IEnumerator FadeMenuCanvasRoutine()
    {
      float originalAlpha = menuCanvasGroup.alpha;

      yield return StartCoroutine(UIUtils.FadeCanvasGroup(
        menuCanvasGroup,
        originalAlpha,
        0f,
        fadeOutDuration
      ));

      menuCanvas.enabled = false;
      menuCanvasGroup.alpha = originalAlpha;
    }

    public void QuitGameCanvasOption()
    {
      Debug.LogWarning($"[MenuCanvasHandler]: Quitting game");
      Application.Quit();
    }
  }
}