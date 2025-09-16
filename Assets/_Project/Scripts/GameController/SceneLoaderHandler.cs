namespace TwelveG.GameController
{
  using System;
  using System.Collections;
  using TwelveG.AudioController;
  using TwelveG.UIController;
  using UnityEngine;
  using UnityEngine.SceneManagement;

  public class SceneLoaderHandler : MonoBehaviour
  {
    [Header("Settings")]
    [SerializeField, Range(1f, 5f)] private float delayTime = 2.5f;

    [Header("References")]
    [SerializeField] private AudioClip loadingClip;
    [SerializeField, Range(0f, 1f)] private float clipVolume = 0.6f;

    [Header("Game Event SO's")]
    [SerializeField] private GameEventSO onSceneLoaded;
    [SerializeField] private GameEventSO onActivateCanvas;
    [SerializeField] private GameEventSO onDeactivateAcanvas;

    private IEnumerator BasicLoadScene(int sceneToLoadIndex)
    {
      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoadIndex);
      asyncLoad.allowSceneActivation = false;

      while (asyncLoad.progress < 0.9f)
      {
        yield return null;
      }

      yield return new WaitForSeconds(delayTime);

      asyncLoad.allowSceneActivation = true;
    }

    private IEnumerator LoadNextScene(int sceneToLoadIndex)
    {
      // Escucha UIManager para activar el Loading Scene Canvas
      onActivateCanvas.Raise(this, CanvasHandlerType.LoadScene);

      AudioSource audioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.UI);
      audioSource.PlayOneShot(loadingClip, clipVolume);

      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoadIndex);
      asyncLoad.allowSceneActivation = false;

      while (asyncLoad.progress < 0.9f)
      {
        yield return null;
      }

      yield return new WaitForSeconds(delayTime);
      audioSource.Stop();

      // Escucha el LoadingSceneCanvasHandler para terminar de parpadear
      // y mostrar el texto "Press any button"
      onSceneLoaded.Raise(this, null);

      // Esperar input del jugador
      yield return new WaitUntil(() => Input.anyKeyDown);

      // Ahora sí, activar la escena cargada
      asyncLoad.allowSceneActivation = true;

      onDeactivateAcanvas.Raise(this, CanvasHandlerType.LoadScene);
    }

    public void LoadNextSceneSequence(int sceneToLoadIndex)
    {
      // Debug.Log($"Escena actual: {SceneManager.GetActiveScene().buildIndex}");
      // Debug.Log($"Escena a cargar {sceneToLoadIndex}");
  
      if (SceneManager.GetActiveScene().buildIndex == 0)
      {
        StartCoroutine(BasicLoadScene(sceneToLoadIndex));
      }
      else
      {
        StartCoroutine(LoadNextScene(sceneToLoadIndex));
      }
    }
  }
}