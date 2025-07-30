namespace TwelveG.GameController
{
  using TwelveG.SaveSystem;
  using UnityEngine;
  using UnityEngine.SceneManagement;

  public class GameManager : MonoBehaviour, IDataPersistence
  {
    public static GameManager Instance;

    [Header("References")]
    public GameObject scene;

    [Header("Game Event SO's")]
    public GameEventSO onToggleInGameCanvasAll;

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

      switch (currentSceneIndex)
      {
        case 0: // Intro
          eventController.enabled = true;
          onToggleInGameCanvasAll.Raise(this, false);
          eventController.BuildEvents();
          break;
        case 1: // Main Menu
          menuHandler.enabled = true;
          return;
        case 2: // Afternoon
          menuHandler.enabled = false;
          onToggleInGameCanvasAll.Raise(this, true);
          eventController.enabled = true;
          eventController.BuildEvents();
          break;
        case 3: // Evening
          menuHandler.enabled = false;
          onToggleInGameCanvasAll.Raise(this, true);
          eventController.enabled = true;
          eventController.BuildEvents();
          break;
        case 4: // Night
          menuHandler.enabled = false;
          onToggleInGameCanvasAll.Raise(this, true);
          eventController.enabled = true;
          eventController.BuildEvents();
          break;
        default:
          Debug.LogError("[InstantiateSceneEventsParent]: Index not found");
          break;
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

    public void LoadData(GameData data)
    {
      return;
    }

    public void SaveData(ref GameData data)
    {
      return;
    }
  }
}