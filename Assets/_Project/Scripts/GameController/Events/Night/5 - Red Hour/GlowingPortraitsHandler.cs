using System;
using System.Collections;
using System.Collections.Generic;
using TwelveG.AudioController;
using TwelveG.EnvironmentController;
using TwelveG.PlayerController;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.GameController
{
  [Serializable]
  public struct PortraitGlowRule
  {
    public HouseArea[] validAreas;
    public AudioClip portraitClip;

    [Tooltip("Volumen máximo para el fade in")]
    [Range(0f, 1f)] public float maxVolume;

    [Tooltip("ID único del cuadro")]
    public string portraitID;
  }

  public class GlowingPortraitsHandler : MonoBehaviour
  {
    [Header("Glow Logic Configuration")]
    [SerializeField] private List<PortraitGlowRule> portraitGlowRules;
    [SerializeField] private float fadeDuration = 2f;

    private bool stopGlowingRoutine = false;
    private Coroutine portraitsGlowingCoroutine = null;

    private struct ActiveAudioData
    {
      public AudioSource source;
      public AudioSourceState originalState;
    }

    // Diccionario: ID_Cuadro -> DatosAudio (Para el manejo interno de audio)
    private Dictionary<string, ActiveAudioData> activeAudiosMap = new Dictionary<string, ActiveAudioData>();

    public void StopGlowingRoutine()
    {
      stopGlowingRoutine = true;
      StopAllCoroutines();
      ForceStopAllPortraits();
    }

    public void StartGlowingRoutine()
    {
      stopGlowingRoutine = false;
      activeAudiosMap.Clear();

      if (portraitsGlowingCoroutine == null)
      {
        portraitsGlowingCoroutine = StartCoroutine(HandlePortraitsGlowingRoutine());
      }
    }

    private IEnumerator HandlePortraitsGlowingRoutine()
    {
      HouseArea lastArea = HouseArea.None;

      // HashSet para rastrear qué IDs están activos lógicamente (Glow + Audio)
      // Usamos HashSet para búsquedas ultra rápidas
      HashSet<string> activePortraitIDs = new HashSet<string>();

      while (!stopGlowingRoutine)
      {
        HouseArea currentArea = PlayerHandler.Instance.GetCurrentHouseArea();

        // Retrasamos si estamos en None para evitar procesar zonas inválidas
        if (currentArea == HouseArea.None)
        {
          yield return new WaitForSeconds(0.2f);
          continue;
        }

        // Solo procesamos si cambiamos a una zona válida DIFERENTE a la anterior
        if (currentArea != lastArea)
        {
          // ID's con sus requisitos para la nueva área
          HashSet<string> requiredIDsForNewArea = new HashSet<string>();

          foreach (var rule in portraitGlowRules)
          {
            if (Array.Exists(rule.validAreas, area => area == currentArea))
            {
              requiredIDsForNewArea.Add(rule.portraitID);
            }
          }

          // APAGAR lo que ya no sirve
          // Estaba activo, pero NO está en los requeridos de la nueva zona
          List<string> toRemove = new List<string>();
          foreach (string id in activePortraitIDs)
          {
            if (!requiredIDsForNewArea.Contains(id))
            {
              SetPortraitGlow(id, false);
              DisablePortraitAudio(id);
              toRemove.Add(id);
            }
          }
          // Removemos de la lista de seguimiento
          foreach (string id in toRemove) activePortraitIDs.Remove(id);

          // ENCENDER lo nuevo
          // Es requerido, pero NO estaba activo antes
          foreach (string id in requiredIDsForNewArea)
          {
            if (!activePortraitIDs.Contains(id))
            {
              // Buscamos la regla para sacar los datos de config
              var rule = portraitGlowRules.Find(r => r.portraitID == id);

              SetPortraitGlow(id, true);

              if (rule.portraitClip != null)
              {
                EnablePortraitAudio(id, rule.portraitClip, rule.maxVolume > 0 ? rule.maxVolume : 1f);
              }

              activePortraitIDs.Add(id);
            }
            // NOTA: Si el ID ya estaba activo y sigue siendo requerido,
            // no entramos al if, por lo que el audio y glow siguen sin interrupción.
          }

          lastArea = currentArea;
        }

        yield return new WaitForSeconds(0.2f);
      }

      // Limpieza final al detener la rutina manualmente
      foreach (string id in activePortraitIDs) SetPortraitGlow(id, false);
      ForceStopAllPortraits();
      portraitsGlowingCoroutine = null;
    }

    private void SetPortraitGlow(string id, bool enable)
    {
      GameObject portrait = PlayerHouseHandler.Instance.GetStoredObjectByID(id);
      if (portrait == null) return;

      PulsingGlowHandler glowHandler = portrait.GetComponentInChildren<PulsingGlowHandler>();

      if (glowHandler != null)
      {
        if (enable) glowHandler.enabled = true;
        else glowHandler.TurnOffGlow();
      }
    }

    private void EnablePortraitAudio(string id, AudioClip clip, float targetVolume)
    {
      // Si ya está en el mapa, es que ya está sonando
      if (activeAudiosMap.ContainsKey(id)) return;

      GameObject portrait = PlayerHouseHandler.Instance.GetStoredObjectByID(id);
      if (portrait == null) return;

      var (source, state) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(portrait.transform, 0f);

      if (source != null)
      {
        source.clip = clip;
        source.maxDistance = 2.5f;
        source.rolloffMode = AudioRolloffMode.Custom;
        source.loop = true;
        source.volume = 0f;

        ActiveAudioData data = new ActiveAudioData { source = source, originalState = state };
        activeAudiosMap.Add(id, data);

        source.Play();
        StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(source, 0f, targetVolume, fadeDuration));
      }
    }

    private void DisablePortraitAudio(string id)
    {
      if (activeAudiosMap.TryGetValue(id, out ActiveAudioData data))
      {
        StartCoroutine(FadeOutAndReturnRoutine(data.source, data.originalState));
        activeAudiosMap.Remove(id);
      }
    }

    private IEnumerator FadeOutAndReturnRoutine(AudioSource source, AudioSourceState originalState)
    {
      if (source == null) yield break;

      yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(source, fadeDuration));

      AudioUtils.StopAndRestoreAudioSource(source, originalState);
    }

    private void ForceStopAllPortraits()
    {
      foreach (var kvp in activeAudiosMap)
      {
        if (kvp.Value.source != null)
        {
          AudioUtils.StopAndRestoreAudioSource(kvp.Value.source, kvp.Value.originalState);
        }
      }
      activeAudiosMap.Clear();
    }
  }
}