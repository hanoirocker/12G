namespace TwelveG.GameController
{
  using System.Collections;
  using TwelveG.AudioController;
  using TwelveG.UIController;
  using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MenuHandler : MonoBehaviour
  {
    [Header("Settings")]
    [SerializeField, Range(1f, 5f)] float blackFadeInDuration;

    [Header("Game Event SO")]
    public GameEventSO onActivateCanvas;
    public GameEventSO onImageCanvasControls;

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

      PlayBackgroundMusic();
    }

    private IEnumerator WaitForSceneToRender()
    {
      // Activar Canvas de Menu
      onActivateCanvas.Raise(this, CanvasHandlerType.MainMenu);

      yield return new WaitForSeconds(1f);

      onImageCanvasControls.Raise(this, new FadeImage(FadeType.FadeIn, blackFadeInDuration));

      AudioManager.Instance.FaderHandler.FadeAudioGroup(
        AudioGroup.masterVol,
        0,
        AudioManager.Instance.GetInitialChannelVolume(AudioGroup.masterVol),
        blackFadeInDuration
      );
    }

    private void PlayBackgroundMusic()
    {
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