namespace TwelveG.UIController
{
  using System.Collections;
  using TwelveG.GameController;
  using UnityEngine;
  using UnityEngine.SceneManagement;

  public class LoadingSceneCanvasHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private CanvasGroup logoCanvasGroup;
    [SerializeField] private GameObject continueText;

    [Header("Transition Settings")]

    [SerializeField, Range(0.25f, 2f)] private float blinkSpeed = 2f;
    [SerializeField, Range(1f, 3f)] private float delayBeforeText = 2f;
    [SerializeField, Range(0.25f, 1f)] private float blinkTime = 0.25f;

    private Canvas menuCanvas;

    private void Awake()
    {
      menuCanvas = GetComponent<Canvas>();
    }

    private IEnumerator LoadSceneCoroutine(int sceneToLoadIndex)
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

      // Ahora sí, activar la escena cargada
      asyncLoad.allowSceneActivation = true;
    }

    public void LoadSceneSequence(Component sender, object data)
    {
      if (data != null)
      {
        StartCoroutine(LoadSceneCoroutine((int)data));
      }
    }
  }
}