namespace TwelveG.UIController
{
  using System.Collections.Generic;
  using TwelveG.Localization;
  using UnityEngine;

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
    BlackBG
  }

  public class UIManager : MonoBehaviour
  {
    [System.Serializable]
    public class CanvasEntry
    {
      public CanvasHandlerType type;
      public GameObject canvasObject;
    }

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
  }
}
