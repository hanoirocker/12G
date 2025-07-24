namespace TwelveG.GameController
{
  using UnityEditor;
  using UnityEngine;
  using UnityEngine.SceneManagement;

  public class GameManager : MonoBehaviour
  {
    public static GameManager Instance;

    public GameObject scene;

    private EventController eventController;
    private MenuHandler menuHandler;
    private int currentSceneIndex;

    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else
      {
        Destroy(gameObject);
      }

      eventController = scene.GetComponent<EventController>();
      menuHandler = scene.GetComponent<MenuHandler>();

      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void VerifySceneType()
    {
      currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

      if (currentSceneIndex == 1) // Main Menu Scene
      {
        menuHandler.enabled = true;
        return;
      }
      else // Intro / Afternoon / Evening / Night / Credits
      {
        eventController.enabled = true;
      }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      currentSceneIndex = scene.buildIndex;
      Debug.Log($"[GameManager] Escena cargada: {scene.name} (index: {currentSceneIndex})");
      VerifySceneType();
    }

    private void OnDestroy()
    {
      SceneManager.sceneLoaded -= OnSceneLoaded;
    }
  }
}