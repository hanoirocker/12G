using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace TwelveG.VFXController
{
  public enum HallucinationEffectType
  {
    None,
    NPRedDistortionFadeIn, // Sube y corta de golpe (No persistente)
    RedDistortionFadeIn,   // Sube y se queda (Persistente)
    RedDistortionFadeOut,  // Baja (Inteligente: Detecta si hay persistencia o arranca de golpe)
    RedDistortion,         // Autocontenido (Sube -> Espera -> Baja)
    FernandezHallucination // Autocontenido por Audio
    // Visions1 (Pendiente)
  }

  [CreateAssetMenu(fileName = "Hallucinations Profile", menuName = "SO's/Data Structures/Hallucinations Profile")]
  public class HallucinationsProfileSO : ScriptableObject
  {
    [System.Serializable]
    public struct HallucinationData
    {
      public HallucinationEffectType type;

      [Header("Visuals")]
      [Range(0f, 1f)] public float targetIntensity;
      public float effectDuration;

      [Header("Audio")]
      public AudioClip audioClip;
      [Range(0f, 1f)] public float audioVolume;

      [Header("Video")]
      public VideoClip videoClip; // Para Visions1 futuro
    }

    [SerializeField] private List<HallucinationData> hallucinationsList;
    private Dictionary<HallucinationEffectType, HallucinationData> lookupTable;

    public void Initialize()
    {
      lookupTable = new Dictionary<HallucinationEffectType, HallucinationData>();
      foreach (var h in hallucinationsList)
      {
        if (!lookupTable.ContainsKey(h.type)) lookupTable.Add(h.type, h);
      }
    }

    public HallucinationData GetSettingsByType(HallucinationEffectType type)
    {
      if (lookupTable == null) Initialize();
      if (lookupTable.TryGetValue(type, out HallucinationData data)) return data;

      Debug.LogWarning($"[HallucinationsProfileSO] Datos no encontrados para: {type}");
      return default;
    }
  }
}