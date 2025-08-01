namespace TwelveG.GameController
{
  using System.Collections;
  using TwelveG.UIController;
  using UnityEngine;
  using UnityEngine.SceneManagement;

  public class SceneLoaderHandler : MonoBehaviour
  {
    [Header("Settings")]
    [SerializeField, Range(1f, 5f)] private float delayTime = 2.5f;
    [Header("Game Event SO's")]
    [SerializeField] private GameEventSO onSceneLoaded;
    [SerializeField] private GameEventSO onActivateCanvas;

    private IEnumerator LoadNextScene(int sceneToLoadIndex)
    {
      // Escucha UIManager para activar el Loading Scene Canvas
      onActivateCanvas.Raise(this, CanvasHandlerType.LoadScene);

      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoadIndex);
      asyncLoad.allowSceneActivation = false;

      while (asyncLoad.progress < 0.9f)
      {
        yield return null;
      }

      yield return new WaitForSeconds(delayTime);

      // Escucha el Loading Scene Canvas para terminar de parpadear
      // y mostrar el texto "Press any button"
      onSceneLoaded.Raise(this, null);

      // Esperar input del jugador
      yield return new WaitUntil(() => Input.anyKeyDown);

      // Ahora sí, activar la escena cargada
      asyncLoad.allowSceneActivation = true;
    }

    public void LoadNextSceneSequence(int sceneToLoadIndex)
    {
      StartCoroutine(LoadNextScene(sceneToLoadIndex));
    }
  }
}