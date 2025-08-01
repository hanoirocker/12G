namespace TwelveG.UIController
{
  using System.Collections;
  using UnityEngine;

  public class MenuCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private CanvasGroup backGroundCanvasGroup;

    [Header("Settings")]
    [SerializeField, Range(0.5f, 3f)] private float timeTillLoadScene = 1.5f;

    [Header("Game Event SO's")]
    [SerializeField] private GameEventSO onPlayGame;

    private Canvas menuCanvas;

    private void Awake()
    {
      menuCanvas = GetComponent<Canvas>();
    }

    private IEnumerator StartPlaying(bool isNewGame)
    {
      yield return StartCoroutine(FadeCanvasCoroutine(backGroundCanvasGroup, 0, 1, timeTillLoadScene));
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

    public void MenuBGFadeInCanvas(Component sender, object data)
    {
      if (data != null)
      {
        StartCoroutine(FadeCanvasCoroutine(backGroundCanvasGroup, 1f, 0f, (float)data));
      }
      else
      {
        Debug.LogError($"[MenuBGFadeInCanvas]: Duration param not passed on Raise at {sender.name} ");
      }
    }

    public void MenuBGFadeOutCanvas(Component sender, object data)
    {
      if (data != null)
      {
        StartCoroutine(FadeCanvasCoroutine(backGroundCanvasGroup, 0f, 1f, (float)data));
      }
      else
      {
        Debug.LogError($"[MenuBGFadeOutCanvas]: Duration param not passed on Raise at {sender.name} ");
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