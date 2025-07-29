namespace TwelveG.UIController
{
  using System.Collections;
  using UnityEngine;
  using UnityEngine.SceneManagement;

  public class LoadingSceneCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private CanvasGroup logoCanvasGroup;
    [SerializeField] private GameObject continueText;

    [Header("Transition Settings")]
    public int sceneToLoadIndex;
    [SerializeField,  Range(0.25f, 2f)] private float blinkSpeed = 2f;
    [SerializeField, Range(1f, 3f)] private float delayBeforeText = 2f;
    [SerializeField, Range(0.25f, 1f)] private float blinkTime = 0.25f;

    [Header("Game Event SO's")]
    [SerializeField] private GameEventSO onActivateCanvas;

    private Canvas menuCanvas;

    private void Awake()
    {
      menuCanvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
      StartCoroutine(LoadSceneCoroutine());
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

    private IEnumerator LoadSceneCoroutine()
    {
      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoadIndex);
      asyncLoad.allowSceneActivation = false;

      // Parpadeo del logo mientras carga

      while (asyncLoad.progress < 0.9f)
      {
        blinkTime += Time.deltaTime * blinkSpeed;
        logoCanvasGroup.alpha = Mathf.PingPong(blinkTime, 1f);
        yield return null;
      }

      // Escena ya cargada al 90%, dejar el logo visible fijo
      logoCanvasGroup.alpha = 1f;

      // Esperar X segundos antes de mostrar el texto
      yield return new WaitForSeconds(delayBeforeText);

      // Mostrar texto para que el jugador presione cualquier tecla
      continueText.SetActive(true);
      Debug.Log("Scene loaded. Waiting for key press...");

      // Esperar input del jugador
      yield return new WaitUntil(() => Input.anyKeyDown);

      // Fade out del canvas acá si querés (por ejemplo animación o fade manual)
      yield return StartCoroutine(FadeCanvasCoroutine(panelCanvasGroup, 1, 0, 1f));

      // Ahora sí, activar la escena cargada
      asyncLoad.allowSceneActivation = true;

    }
  }
}