using System.Collections;
using TwelveG.AudioController;
using TwelveG.UIController;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TwelveG.GameController
{
  public class MenuHandler : MonoBehaviour
  {
    [Header("Settings")]
    [SerializeField, Range(1f, 5f)] float blackFadeInDuration;

    [Header("Menu audio")]
    public AudioClip afternoonMusic;
    public AudioClip eveningMusic;
    public AudioClip nightMusic;

    private void Start()
    {
      // TODO?: quizas el mainAudioController debería por defector arrancar con el volumnen del 
      // master en 0 para TODAS las escenas .. y eventualmente esperar a ser llamado por el manager
      // luego de cargar los componentes de la escena. (ES BUENA)

      AudioManager.Instance.SetMasterVol(0);

      StartCoroutine(WaitForSceneToRender());

      SetMenuSceneSettings();
    }

    private IEnumerator WaitForSceneToRender()
    {
      // Activar Canvas de Menu
      GameEvents.Common.onActivateCanvas.Raise(this, CanvasHandlerType.MainMenu);

      yield return new WaitForSeconds(1f);

      yield return StartCoroutine(
        UIManager.Instance.ImageCanvasHandler.FadeImageCanvas(FadeType.FadeIn, blackFadeInDuration)
      );

      // Esperamos al fade in del canvas de fondo negro para activar
      // el raycasting del main menu
      UIManager.Instance.MenuCanvasHandler.GetComponent<GraphicRaycaster>().enabled = true;

      AudioManager.Instance.FaderHandler.FadeAudioGroup(
        AudioGroup.masterVol,
        0,
        AudioManager.Instance.GetInitialChannelVolume(AudioGroup.masterVol),
        blackFadeInDuration
      );
    }

    private void SetMenuSceneSettings()
    {
      GameEvents.Common.onStartWeatherEvent.Raise(this, WeatherEvent.SoftWind);

      AudioSource source = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.BGMusic);
      string currentSceneName = SceneManager.GetActiveScene().name;

      switch (currentSceneName)
      {
        case "Menu Afternoon":
          source.clip = afternoonMusic;
          break;
        case "Menu Evening":
          source.clip = eveningMusic;
          break;
        case "Menu Night":
          source.clip = nightMusic;
          break;
        default:
          Debug.LogWarning($"[MenuHandler]: No background music assigned for scene '{currentSceneName}'");
          return;
      }

      source.loop = true;
      source.Play();
    }
  }
}