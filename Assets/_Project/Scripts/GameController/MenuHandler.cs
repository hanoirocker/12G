namespace TwelveG.GameController
{
  using System.Collections;
  using TwelveG.AudioController;
  using TwelveG.UIController;
  using UnityEngine;

  public class MenuHandler : MonoBehaviour
  {
    [Header("Settings")]
    [SerializeField, Range(1f, 5f)] float blackFadeOutDuration;

    [Header("Game Event SO")]
    public GameEventSO onActivateCanvas;
    public GameEventSO onMenuBGFadeIn;

    private void Start()
    {
      // TODO?: quizas el mainAudioController debería por defector arrancar con el volumnen del 
      // master en 0 para TODAS las escenas .. y eventualmente esperar a ser llamado por el manager
      // luego de cargar los componentes de la escena. (ES BUENA)

      AudioManager.Instance.SetMasterVol(-88f);

      StartCoroutine(WaitForSceneToRender());
    }

    private IEnumerator WaitForSceneToRender()
    {
      onActivateCanvas.Raise(this, CanvasHandlerType.MainMenu);

      // TODO: Aprox? --> Basar en configs de video guardadas O async load desde Intro
      // `AsyncOperations loadOperations = SceneManager.LoadSceneAsync(sceneToLoad);`
      yield return new WaitForSeconds(4f);

      onMenuBGFadeIn.Raise(this, blackFadeOutDuration);

      AudioManager.Instance.FaderHandler.FadeAudioGroup(
        AudioGroup.masterVol,
        -88f,
        AudioManager.Instance.GetInitialChannelVolume(AudioGroup.masterVol),
        blackFadeOutDuration
      );
    }
  }
}