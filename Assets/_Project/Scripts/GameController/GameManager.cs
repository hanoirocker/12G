namespace TwelveG.GameController
{
  using System.Collections;
  using TwelveG.SaveSystem;
  using TwelveG.UIController;
  using UnityEngine;
  using UnityEngine.SceneManagement;

  public class GameManager : MonoBehaviour, IDataPersistence
  {
    public static GameManager Instance;

    [Header("References")]
    public GameObject scene;

    [Header("Game Event SO's")]
    public GameEventSO onToggleInGameCanvasAll;
    public GameEventSO onPlayGame;
    public GameEventSO onActivateCanvas;

    private EventController eventController;
    private MenuHandler menuHandler;
    private int currentSceneIndex = 0;
    private int _savedSceneIndex = 0;
    private int currentEventIndex = 0;

    // Evita que los GameManager temporales ejecuten la lógica
    // Solo la instancia persistente procesa la carga de escena!
    private bool isPersistentInstance = false;

    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        isPersistentInstance = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
      }
      else
      {
        Destroy(gameObject);
        return;
      }
    }

    private void VerifySceneType()
    {
      if (eventController == null || menuHandler == null)
      {
        Debug.LogWarning("Componentes de escena no asignados. Reintentando...");
        StartCoroutine(InitializeSceneComponents());
        return;
      }

      switch (currentSceneIndex)
      {
        case 0: // Intro
          menuHandler.enabled = false;
          eventController.enabled = true;
          onToggleInGameCanvasAll.Raise(this, false);
          eventController.BuildEvents();
          break;
        case 1: // Main Menu
          eventController.enabled = false;
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
          Debug.LogError("currentSceneIndex not found");
          break;
      }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      if (!isPersistentInstance) return;

      currentSceneIndex = scene.buildIndex;
      Debug.Log($"Escena cargada: {scene.name} (index: {currentSceneIndex})");

      StartCoroutine(InitializeSceneComponents());
    }

    private void OnDestroy()
    {
      SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator InitializeSceneComponents()
    {
      // Esperar un frame a que todos los componentes estén listos.
      yield return null;

      GameObject currentSceneObj = GetComponentInChildren<EventController>().gameObject;
      if (currentSceneObj != null)
      {
        eventController = currentSceneObj.GetComponent<EventController>();
        menuHandler = currentSceneObj.GetComponent<MenuHandler>();
        VerifySceneType();
      }
    }

    public int GetSavedSceneIndex()
    {
      return _savedSceneIndex;
    }

    // 'currentEventIndex' se actualizará mediante el EventController antes de iniciar
    // cada corrutina dentro de la lista.
    public void UpdateEventIndex(int index)
    {
      currentEventIndex = index;
    }

    public void LoadNextScene()
    {
      if (currentSceneIndex + 1 <= 4)
      {
        // Activa Loading Scene Canvas (Escucha UI Manager)
        onActivateCanvas.Raise(this, CanvasHandlerType.LoadScene);
        // Cargar siguiente escena (Escucha Loading Scene Canvas)
        onPlayGame.Raise(this, currentSceneIndex + 1);
      }
      if (currentSceneIndex + 1 == 5)
      {
        // Cargar créditos
      }
      if (currentSceneIndex + 1 == 6)
      {
        // Volver desde créditos a Menú principal
      }
    }

    public void LoadData(GameData data)
    {
      // Misma condición para hacer aparecer el botón de 'Continue'
      // en el Menu Canvas.
      // Sólo se considera el eventIndex si la última escena jugada y guardada
      // fue Afternoon.
      if (data.savedSceneIndex > 1)
      {
        currentEventIndex = data.savedEventIndex;
        _savedSceneIndex = data.savedSceneIndex;
      }
    }

    public void SaveData(ref GameData data)
    {
      data.savedSceneIndex = currentSceneIndex;

      if (currentSceneIndex > 1)
      {
        data.savedEventIndex = currentEventIndex;
      }
    }
  }
}