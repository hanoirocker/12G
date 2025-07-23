namespace TwelveG.GameController
{
    using System.Collections;
    using TwelveG.AudioController;
    using TwelveG.UIController;
    using UnityEngine;

    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private MainAudioController mainAudioController;
        [SerializeField] private GameObject menuEnvironment;
        [SerializeField] private MenuCanvasHandler menuCanvasHandler;

        private void Start()
        {
            // TODO?: quizas el mainAudioController debería por defector arrancar con el volumnen del 
            // master en 0 para TODAS las escenas .. y eventualmente esperar a ser llamado por el manager
            // luego de cargar los componentes de la escena. (ES BUENA)

            mainAudioController.SetMasterVoume(-88f);

            // Aca PODRIA haber picos de audio.

            menuEnvironment.SetActive(true);

            StartCoroutine(WaitForSceneToRender());
        }

        // `AsyncOperations loadOperations = SceneManager.LoadSceneAsync(sceneToLoad);`
        private IEnumerator WaitForSceneToRender()
        {

            yield return new WaitForSeconds(3f); // TODO: Aprox? --> Basar en configs de video guardadas O async load desde Intro

            menuCanvasHandler.FadeBackgroundCanvas(1f, 0f, 1f);
            mainAudioController.FadeAudioGroup(AudioGroup.masterVol, -88f, 0f, 1f);
        }
    }
}