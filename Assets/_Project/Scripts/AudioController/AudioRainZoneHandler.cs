namespace TwelveG.AudioController
{
  using System.Collections.Generic;
  using UnityEngine;

  public class AudioRainZoneHandler : MonoBehaviour
  {
    // Necesita recibir la lista de fuentes
    private List<AudioSource> ambienceSource = new(); // Máximo 2 fuentes activas

    // Historial de zonas activas recientes (máx 2)
    private LinkedList<Transform> activeZoneHistory = new();


    // Se llama desde el AcousticZone script en OnTriggerEnter al colisionar
    // Ver: Acoustic Zones prefab en Player House prefab
    public void EnteredAcousticZone(Transform zoneTransform, AudioPoolType audioPoolType)
    {
      List<AudioSource> ambienceSource = AudioManager.Instance.PoolsHandler.ReturnAudioSourceByType(audioPoolType);


      // Si ya estaba en la lista, la movemos al final (más reciente)
      if (activeZoneHistory.Contains(zoneTransform))
      {
        activeZoneHistory.Remove(zoneTransform);
      }

      activeZoneHistory.AddLast(zoneTransform);

      // Mantener máximo 2 zonas en la lista
      if (activeZoneHistory.Count > 2)
      {
        activeZoneHistory.RemoveFirst();
      }

      // Reasignar posiciones de los audio sources a las zonas más recientes
      int i = 0;
      foreach (var zone in activeZoneHistory)
      {
        if (i >= ambienceSource.Count) break;

        ambienceSource[i].transform.position = zone.position;

        if (!ambienceSource[i].isPlaying)
          ambienceSource[i].Play();

        i++;
      }
    }

    public void ExitedAcousticZone(Transform zoneTransform, AudioPoolType audioPoolType)
    {
      if (activeZoneHistory.Contains(zoneTransform))
      {
        activeZoneHistory.Remove(zoneTransform);
      }
    }
  }
}