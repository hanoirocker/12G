using System;
using System.Collections;
using TwelveG.AudioController;
using TwelveG.UIController;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwelveG.GameController
{
  public class SceneLoaderHandler : MonoBehaviour
  {
    [Header("Settings")]
    [Space]
    [SerializeField, Range(1f, 5f)] private float delayTime = 2.5f;
    [SerializeField, Range(1f, 5f)] private float bgMusicFadeOut = 3f;
    [SerializeField, Range(1f, 5f)] private float extraTimeBeforeSceneActivation = 4f;

    [Header("References")]
    [Space]
    [SerializeField] private AudioClip loadingClip;
    [SerializeField, Range(0f, 1f)] private float clipVolume = 0.6f;

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
      GameEvents.Common.onActivateCanvas.Raise(this, CanvasHandlerType.LoadScene);

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
      GameEvents.Common.onSceneLoaded.Raise(this, null);

      // Esperar input del jugador
      yield return new WaitUntil(() => Input.anyKeyDown);

      // Escucha el LoadingSceneCanvasHandler para ocultar el texto "Press any Button"
      GameEvents.Common.onAnyKeyPressed.Raise(this, bgMusicFadeOut);

      AudioSource bgMusicSource = AudioManager.Instance.PoolsHandler.ReturnActiveSourceByType(AudioPoolType.BGMusic);

      if (bgMusicSource)
      {
        StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(bgMusicSource, bgMusicFadeOut));
      }

      yield return new WaitForSeconds(bgMusicFadeOut);

      yield return new WaitForSeconds(extraTimeBeforeSceneActivation);

      // Ahora sí, activar la escena cargada
      asyncLoad.allowSceneActivation = true;

      GameEvents.Common.onDeactivateCanvas.Raise(this, CanvasHandlerType.LoadScene);

      // Escucha AudioManager, ... , para volver a los estados originales
      GameEvents.Common.onSceneActivated.Raise(this, null);
    }

    public void LoadNextSceneSequence(int sceneToLoadIndex)
    {
      Debug.Log($"Escena actual: {SceneManager.GetActiveScene().buildIndex}");
      Debug.Log($"Escena a cargar {sceneToLoadIndex}");

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