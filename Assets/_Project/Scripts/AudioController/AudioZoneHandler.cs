using System.Collections.Generic;
using UnityEngine;
namespace TwelveG.AudioController
{
  public class AudioZoneHandler : MonoBehaviour
  {
    // Necesita recibir la lista de fuentes
    private List<AudioSource> ambienceSources = new(); // Máximo 2 fuentes activas

    // Historial de zonas activas recientes (máx 2)
    private LinkedList<Transform> activeZoneHistory = new();

    // Se llama desde el AcousticZone script en OnTriggerEnter al colisionar
    // Ver: Acoustic Zones prefab en Player House prefab
    public void EnteredAcousticZone(Transform zoneTransform)
    {
      if (activeZoneHistory.Contains(zoneTransform))
      {
        activeZoneHistory.Remove(zoneTransform);
      }

      activeZoneHistory.AddLast(zoneTransform);

      // Mantener máximo 2 zonas
      if (activeZoneHistory.Count > 2)
      {
        activeZoneHistory.RemoveFirst();
      }

      UpdateAudioSources();
    }

    public void ResetAcousticSources()
    {
      ambienceSources = null;
    }

    public void ExitedAcousticZone(Transform zoneTransform)
    {
      if (activeZoneHistory.Contains(zoneTransform))
      {
        activeZoneHistory.Remove(zoneTransform);
      }

      UpdateAudioSources();
    }

    public void GetAudioSourcesFormPool()
    {
      ambienceSources = AudioManager.Instance.PoolsHandler.ReturnAudioSourceByType(AudioPoolType.Environment);
    }

    private void UpdateAudioSources()
    {
      // Caso sin zonas → resetear
      if (activeZoneHistory.Count == 0)
      {
        foreach (var source in ambienceSources)
        {
          source.transform.position = Vector3.zero;
          source.Stop();
        }
        return;
      }

      int i = 0;
      foreach (var zone in activeZoneHistory)
      {
        if (i >= ambienceSources.Count) break;

        ambienceSources[i].transform.position = zone.position;

        if (!ambienceSources[i].isPlaying)
          ambienceSources[i].Play();

        i++;
      }

      // Si sobra alguna fuente no usada, resetearla
      for (; i < ambienceSources.Count; i++)
      {
        ambienceSources[i].transform.position = Vector3.zero;
        ambienceSources[i].Stop();
      }
    }
  }
}