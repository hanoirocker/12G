namespace TwelveG.UIController
{
  using System.Collections;
  using UnityEngine;

  public class MenuCanvasHandler : MonoBehaviour
  {
    [Header("Settings")]
    [SerializeField, Range(0.5f, 3f)] private float fadeOutDuration = 1.5f;

    [Header("Game Event SO's")]
    [SerializeField] private GameEventSO onPlayGame;
    [SerializeField] private GameEventSO onImageCanvasControls;

    private Canvas menuCanvas;

    private void Awake()
    {
      menuCanvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
    }

  private void OnDisable()
    {
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator StartPlaying(bool isNewGame)
    {
      onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeOut, fadeOutDuration));
      yield return new WaitForSeconds(fadeOutDuration);
      // Escuchado por GameManager para luego llamar al SceneLoaderHandler.
      onPlayGame.Raise(this, isNewGame);
      gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvasCoroutine(CanvasGroup group, float from, float to, float duration)
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
    }

    public void QuitGameCanvasOption()
    {
      Debug.LogWarning($"[MenuCanvasHandler]: Quitting game");
      Application.Quit();
    }
  }
}