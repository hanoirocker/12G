namespace TwelveG.GameController
{
  using UnityEngine;
  using System.Collections;
  using TwelveG.UIController;
  using TwelveG.AudioController;
  using UnityEngine.SceneManagement;

  // TODO?: Skip canvas logic
  public class IntroManager : MonoBehaviour
  {
    [SerializeField] public GameObject disclaimerCanvas;
    [SerializeField] public GameObject informationCanvas;
    [SerializeField] public GameObject audioGO;

    [Header("Settings:")]
    public bool allowSkip = false;

    // [Header("Game Event SO:")]
    // [SerializeField] private GameEventSO onIntroScreenSkip;

    private readonly bool userRequestedSkip = false;
    private AudioFaderHandler introAudioController;
    private DisclaimerCanvasHandler disclaimerHandler;
    private InformationCanvasHandler informationHandler;


    private void Awake()
    {
      disclaimerHandler = disclaimerCanvas.GetComponent<DisclaimerCanvasHandler>();
      informationHandler = informationCanvas.GetComponent<InformationCanvasHandler>();
      introAudioController = audioGO.GetComponent<AudioFaderHandler>();
    }

    private void Start()
    {
      disclaimerCanvas.SetActive(false);
      informationCanvas.SetActive(false);

      StartCoroutine(introAudioController.AudioFadeInSequence());
      StartCoroutine(RunIntroSequence());
    }

    // private void Update()
    // {
    //   if (allowSkip && Input.anyKeyDown)
    //   {
    //     onIntroScreenSkip.Raise(this, null);
    //   }
    // }

    private IEnumerator RunIntroSequence()
    {
      yield return StartCoroutine(RunCanvas(disclaimerCanvas, disclaimerHandler));
      yield return StartCoroutine(RunCanvas(informationCanvas, informationHandler));

      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator RunCanvas(GameObject canvasGO, MonoBehaviour controller)
    {
      canvasGO.SetActive(true);
      yield return StartCoroutine(controller.GetType()
          .GetMethod("RunSequence")
          .Invoke(controller, null) as IEnumerator);
      canvasGO.SetActive(false);
    }

    public bool UserRequestedSkip()
    {
      return userRequestedSkip;
    }
  }
}