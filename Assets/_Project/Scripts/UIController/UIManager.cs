using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

namespace TwelveG.UIController
{
  public enum CanvasHandlerType
  {
    Disclaimer,
    StudioInformation,
    MainMenu,
    Interaction,
    Contemplation,
    Observation,
    Dialog,
    Narrative,
    PauseMenu,
    Cinematic,
    Control,
    BlackBG,
    Settings,
    LoadScene,
    Examination
  }

  public class UIManager : MonoBehaviour, IDataPersistence
  {
    [System.Serializable]
    public class CanvasEntry
    {
      public CanvasHandlerType type;
      public GameObject canvasObject;
    }

    public static UIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Button continueBtn;
    [SerializeField] private LoadingSceneCanvasHandler loadingSceneCanvasHandler;

    [Header("Canvas mappings")]
    [SerializeField] private List<CanvasEntry> canvasMappings;
    [SerializeField] private GameObject inGameCanvas;
    [SerializeField] private List<GameObject> canvasesToHideWhilePaused;

    [Header("Children references")]
    [SerializeField] private ImageCanvasHandler imageCanvasHandler;
    [SerializeField] private MenuCanvasHandler menuCanvasHandler;
    [SerializeField] private InteractionCanvasHandler interactionCanvasHandler;
    [SerializeField] private ContemplationCanvasHandler contemplationCanvasHandler;
    [SerializeField] private ObservationCanvasHandler observationCanvasHandler;
    [SerializeField] private ControlCanvasHandler controlCanvasHandler;

    public ImageCanvasHandler ImageCanvasHandler => imageCanvasHandler;
    public MenuCanvasHandler MenuCanvasHandler => menuCanvasHandler;
    public InteractionCanvasHandler InteractionCanvasHandler => interactionCanvasHandler;
    public ContemplationCanvasHandler ContemplationCanvasHandler => contemplationCanvasHandler;
    public ObservationCanvasHandler ObservationCanvasHandler => observationCanvasHandler;
    public ControlCanvasHandler ControlCanvasHandler => controlCanvasHandler;

    private Dictionary<CanvasHandlerType, GameObject> canvasDict;
    private List<bool> originalStates = new List<bool>();

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

      MapCanvas();
    }

    private void MapCanvas()
    {
      canvasDict = new Dictionary<CanvasHandlerType, GameObject>();
      foreach (var entry in canvasMappings)
      {
        if (!canvasDict.ContainsKey(entry.type))
        {
          canvasDict.Add(entry.type, entry.canvasObject);
        }
        else
        {
          Debug.LogWarning($"[UIManager] Duplicado: {entry.type}");
        }
      }
    }

    // Llamado por GameManager luego de cargar y detectar índice de escena
    public void ToggleInGameCanvas(Component sender, object data)
    {
      if (data != null)
      {
        inGameCanvas.SetActive((bool)data);
      }
    }

    public void PauseGame(Component sender, object data)
    {
      bool isPaused = (bool)data;

      if (canvasDict.TryGetValue(CanvasHandlerType.PauseMenu, out var pauseCanvasGO))
      {
        // Leemos el estado de los canvas a apagar en la lista
        for (int i = 0; i < canvasesToHideWhilePaused.Count; i++)
        {
          originalStates.Add(canvasesToHideWhilePaused[i].GetComponent<Canvas>().enabled);
        }

        // Deshabilitamos los canvas de la lista
        if (isPaused)
        {
          foreach (var canvas in canvasesToHideWhilePaused)
          {
            canvas.GetComponent<Canvas>().enabled = false;
          }
        }
        // Si se reanuda el juego, restauramos los estados originales
        else
        {
          for (int i = 0; i < canvasesToHideWhilePaused.Count; i++)
          {
            canvasesToHideWhilePaused[i].GetComponent<Canvas>().enabled = originalStates[i];
          }
          originalStates.Clear();
        }

        // Habilitar o deshabilitar el canvas de pausa y script asociado
        pauseCanvasGO.GetComponent<Canvas>().enabled = isPaused;
        pauseCanvasGO.GetComponent<PauseMenuCanvasHandler>().enabled = isPaused;
      }
    }

    public void ActivateCanvas(Component sender, object data)
    {
      if (data is CanvasHandlerType canvasType)
      {
        if (canvasDict.TryGetValue(canvasType, out var canvasGO))
        {
          canvasGO.SetActive(true);
          // Debug.Log($"[UIManager] Activado canvas: {canvasType}");
        }
        else
        {
          Debug.LogWarning($"[UIManager] No se encontró canvas para tipo: {canvasType}");
        }
      }
      else
      {
        Debug.LogWarning("[UIManager] Tipo de datos inválido para activación de canvas");
      }
    }

    public void DeactivateCanvas(Component sender, object data)
    {
      if (data is CanvasHandlerType canvasType)
      {
        if (canvasDict.TryGetValue(canvasType, out var canvasGO))
        {
          canvasGO.SetActive(false);
        }
      }
      else
      {
        Debug.LogError("[UIManager] Tipo de datos inválido para activación de canvas");
      }
    }

    // Llamado desde cualquier botón del UI
    public void OnButtonPointed()
    {
      AudioManager.Instance.AudioUIHandler.PlayPointerEnterSound();
    }

    public void LoadData(GameData data)
    {
      if (data.savedSceneIndex <= 1) { continueBtn.interactable = false; }
    }

    public void SaveData(ref GameData data)
    {
      return;
    }
  }
}
