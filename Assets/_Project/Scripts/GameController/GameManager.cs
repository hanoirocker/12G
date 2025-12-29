using System.Collections;
using TwelveG.SaveSystem;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwelveG.GameController
{
  public class GameManager : MonoBehaviour, IDataPersistence
  {
    public static GameManager Instance;

    [Header("References")]
    public GameObject handlers;

    [SerializeField] private EventsHandler eventsHandler;
    [SerializeField] private MenuHandler menuHandler;
    [SerializeField] private SceneLoaderHandler sceneLoaderHandler;

    public EventsHandler EventsHandler => eventsHandler;

    private int currentSceneIndex = 0;
    private int _savedSceneIndex = 0;

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
      switch (currentSceneIndex)
      {
        case 0: // Intro
          menuHandler.enabled = false;
          GameEvents.Common.onToggleInGameCanvasAll.Raise(this, false);
          eventsHandler.BuildEvents();
          break;
        case 1: // Main Menu
          menuHandler.enabled = true;
          return;
        case 2: // Afternoon
          menuHandler.enabled = false;
          GameEvents.Common.onToggleInGameCanvasAll.Raise(this, true);
          eventsHandler.BuildEvents();
          break;
        case 3: // Evening
          menuHandler.enabled = false;
          GameEvents.Common.onToggleInGameCanvasAll.Raise(this, true);
          eventsHandler.BuildEvents();
          break;
        case 4: // Night
          menuHandler.enabled = false;
          GameEvents.Common.onToggleInGameCanvasAll.Raise(this, true);
          eventsHandler.BuildEvents();
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
      // Debug.Log($"Escena cargada: {scene.name} (index: {currentSceneIndex})");

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
      VerifySceneType();
    }

    public void PlayGame(Component sender, object isNewGame)
    {
      sceneLoaderHandler.LoadNextSceneSequence((bool)isNewGame ? 2 : _savedSceneIndex);
    }

    public void LoadData(GameData data)
    {
      // Misma condición para hacer aparecer el botón de 'Continue'
      // en el Menu Canvas.
      // Sólo se considera el eventIndex si la última escena jugada y guardada
      // fue Afternoon.
      if (data.savedSceneIndex > 1)
      {
        _savedSceneIndex = data.savedSceneIndex;
        EventsHandler.SetSavedCheckpointList(data.checkpointEventsCompleted);
      }
    }

    public void SaveData(ref GameData data)
    {
      data.savedSceneIndex = currentSceneIndex;
      data.checkpointEventsCompleted = EventsHandler.UpdateCompletedCheckpointEvents(data.checkpointEventsCompleted);
    }
  }
}