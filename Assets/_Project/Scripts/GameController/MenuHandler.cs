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

    // TODO: Quizas mover esto a un scriptable object de audio para menus
    [Header("Menu audio")]
    [SerializeField] private AudioClip afternoonMusic;
    [SerializeField, Range(0f, 1f)] private float afternoonMusicVolume = 0.7f;
    // [SerializeField] private AudioClip eveningMusic;
    // [SerializeField, Range(0f, 1f)] private float eveningMusicVolume = 0.7f;
    // [SerializeField] private AudioClip nightMusic;
    // [SerializeField, Range(0f, 1f)] private float nightMusicVolume = 0.7f;

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
      SceneEnum sceneEnum = SceneUtils.RetrieveCurrentSceneEnum();
      float targetVolume;

      // TODO: asignar variables de clip y volumen especificos en caso
      // de usar múltiples escenas de Menu el día de mañana.
      switch (sceneEnum)
      {
        case SceneEnum.MenuAfternoon:
          source.clip = afternoonMusic;
          targetVolume = afternoonMusicVolume;
          break;
        case SceneEnum.MenuEvening:
          source.clip = afternoonMusic;
          targetVolume = afternoonMusicVolume;
          break;
        case SceneEnum.MenuNight:
          source.clip = afternoonMusic;
          targetVolume = afternoonMusicVolume;
          break;
        default:
          Debug.LogWarning($"[MenuHandler]: No background music assigned for scene '{sceneEnum}'");
          return;
      }

      source.loop = true;
      source.volume = 0f;
      source.Play();
      StartCoroutine(
        AudioManager.Instance.FaderHandler.AudioSourceFadeIn(source, 0f, targetVolume, 2f)
      );
    }
  }
}