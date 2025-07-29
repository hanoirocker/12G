namespace TwelveG.UIController
{
  using System.Collections.Generic;
  using TwelveG.SaveSystem;
  using UnityEngine;
  using UnityEngine.UI;

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
    LoadScene
  }

  public class UIManager : MonoBehaviour, IDataPersistence
  {
    [System.Serializable]
    public class CanvasEntry
    {
      public CanvasHandlerType type;
      public GameObject canvasObject;
    }
    [Header("References")]
    [SerializeField] private Button continueBtn;
    [SerializeField] private LoadingSceneCanvasHandler loadingSceneCanvasHandler;

    [Header("Canvas mappings")]
    public List<CanvasEntry> canvasMappings;
    public GameObject inGameCanvas;

    private Dictionary<CanvasHandlerType, GameObject> canvasDict;

    private void Awake()
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

    public void ActivateCanvas(Component sender, object data)
    {
      if (data is CanvasHandlerType canvasType)
      {
        if (canvasDict.TryGetValue(canvasType, out var canvasGO))
        {
          canvasGO.SetActive(true);
          Debug.Log($"[UIManager] Activado canvas: {canvasType}");
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

    // Llamado por GameManager luego de cargar y detectar índice de escena
    public void ToggleInGameCanvas(Component sender, object data)
    {
      if (data != null)
      {
        inGameCanvas.SetActive((bool)data);
      }
    }

    public void DeactivateCanvas(Component sender, object data)
    {
      if (data is CanvasHandlerType canvasType)
      {
        if (canvasDict.TryGetValue(canvasType, out var canvasGO))
        {
          canvasGO.SetActive(false);
          Debug.Log($"[UIManager] Desactivado canvas: {canvasType}");
        }
      }
      else
      {
        Debug.LogWarning("[UIManager] Tipo de datos inválido para activación de canvas");
      }

    }

    public void LoadData(GameData data)
    {
      loadingSceneCanvasHandler.sceneToLoadIndex = data.sceneIndex;
      if (data.sceneIndex < 2) { continueBtn.interactable = false; }
    }

    public void SaveData(ref GameData data)
    {
      // throw new System.NotImplementedException($"{gameObject.name}");
    }
  }
}
