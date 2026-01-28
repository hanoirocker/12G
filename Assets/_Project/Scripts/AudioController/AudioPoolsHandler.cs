using System.Collections.Generic;
using UnityEngine;

namespace TwelveG.AudioController
{
  public enum AudioPoolType
  {
    RainAndWind, // Especial para lluvia y viento mediante las Acoustic Zones
    BGMusic,
    UI,
    Environment, // Eventos puntuales transicionales (como truenos)
    Interaction,
    Wind,
    Dialogs,
    Player,
    VFX,
    HouseStereoAmbient // Sonidos de ambiente stereo por zona de la casa
  }

  public enum AudioMixChannel
  {
    UI,
    Music,
    InGame,
  }

  public enum WeatherEvent
  {
    SoftRain,
    HardRain,
    SoftWind,
    HardWind,
    SoftRainAndWind,
    HardRainAndWind,
    None,
    ConstantThunders,
    CloseThunder,
  }

  public class AudioPoolsHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private AudioZoneHandler audioZoneHandler;

    [Space]
    [Header("Audio Source Pools")]
    [SerializeField] private List<AudioSource> BGMusicSources;
    [SerializeField] private List<AudioSource> EnvironmentSources;
    [SerializeField] private List<AudioSource> RainAndWindSources;
    [SerializeField] private List<AudioSource> InteractionSources;
    [SerializeField] private List<AudioSource> UISources;
    [SerializeField] private List<AudioSource> DialogsSources;
    [SerializeField] private List<AudioSource> PlayerSources;
    [SerializeField] private List<AudioSource> VFXSources;
    [SerializeField] private List<AudioSource> HouseStereoAmbientSources;

    [Space]
    [Header("Pause Settings")]
    [SerializeField]
    private AudioPoolType[] poolsToPause = new AudioPoolType[] {
            AudioPoolType.Dialogs,
            AudioPoolType.Interaction,
            AudioPoolType.BGMusic,
            AudioPoolType.Environment,
            AudioPoolType.RainAndWind,
            AudioPoolType.Player,
            AudioPoolType.VFX,
            AudioPoolType.HouseStereoAmbient
        };

    [Space]
    [Header("Audio References")]
    [SerializeField] private AudioClip softWindClip;
    [SerializeField] private AudioClip softRainClip;
    [SerializeField] private AudioClip hardRainClip;
    [SerializeField] private AudioClip hardRainAndWindClip;

    [Space(5)]
    [Header("Volumes")]
    [SerializeField, Range(0f, 1f)] private float softWindVolume = 0.225f;
    [SerializeField, Range(0f, 1f)] private float softRainVolume = 0.6f;
    [SerializeField, Range(0f, 1f)] private float hardWindVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float hardRainVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float hardRainAndWindVolume = 1f;

    [Space]
    [Header("Weather Settings")]
    [SerializeField, Range(1f, 4f)] private float softWeatherDefaultDistance = 4f;
    [SerializeField, Range(4f, 5.5f)] private float hardWeatherDistance = 4.5f;

    private Dictionary<AudioPoolType, List<AudioSource>> poolMap;
    private List<AudioSource> lastActiveAudioSources = new List<AudioSource>();

    // Fuentes "Prestadas". 
    // No se liberan hasta que el script que la pidió llame a ReleaseAudioSource.
    private HashSet<int> claimedSourcesIDs = new HashSet<int>();

    private void Awake()
    {
      poolMap = new Dictionary<AudioPoolType, List<AudioSource>>
            {
                { AudioPoolType.BGMusic, BGMusicSources },
                { AudioPoolType.Environment, EnvironmentSources },
                { AudioPoolType.RainAndWind, RainAndWindSources },
                { AudioPoolType.Interaction, InteractionSources },
                { AudioPoolType.HouseStereoAmbient, HouseStereoAmbientSources },
                { AudioPoolType.UI, UISources },
                { AudioPoolType.Dialogs, DialogsSources},
                { AudioPoolType.Player, PlayerSources},
                { AudioPoolType.VFX, VFXSources}
            };
    }

    public void PauseActiveAudioSources(bool pauseSources)
    {
      if (pauseSources)
      {
        lastActiveAudioSources.Clear();

        foreach (AudioPoolType poolType in poolsToPause)
        {
          if (poolMap.TryGetValue(poolType, out List<AudioSource> sources))
          {
            foreach (AudioSource audioSource in sources)
            {
              if (audioSource != null && audioSource.isPlaying)
              {
                lastActiveAudioSources.Add(audioSource);
                audioSource.Pause();
              }
            }
          }
        }
      }
      else
      {
        foreach (AudioSource audioSource in lastActiveAudioSources)
        {
          if (audioSource != null)
          {
            audioSource.UnPause();
          }
        }
      }
    }

    public void StopActivePoolSources(AudioPoolType audioPoolType)
    {
      if (!poolMap.TryGetValue(audioPoolType, out var sources))
      {
        Debug.LogError($"[AudioPoolsHandler]: No se encontró lista para pool '{audioPoolType}'");
        return;
      }

      foreach (AudioSource audioSource in sources)
      {
        if (audioSource.isPlaying)
        {
          audioSource.Stop();
        }
      }
    }

    public void StopActiveSourcesOnMixChannel(AudioMixChannel audioMixChannel)
    {
      switch (audioMixChannel)
      {
        case AudioMixChannel.UI:
          StopActivePoolSources(AudioPoolType.UI);
          break;
        case AudioMixChannel.Music:
          StopActivePoolSources(AudioPoolType.BGMusic);
          break;
        case AudioMixChannel.InGame:
          StopActivePoolSources(AudioPoolType.RainAndWind);
          StopActivePoolSources(AudioPoolType.Environment);
          StopActivePoolSources(AudioPoolType.Interaction);
          StopActivePoolSources(AudioPoolType.Dialogs);
          StopActivePoolSources(AudioPoolType.Player);
          StopActivePoolSources(AudioPoolType.VFX);
          StopActivePoolSources(AudioPoolType.HouseStereoAmbient);
          break;
        default:
          Debug.LogWarning($"[AudioPoolsHandler]: Canal de mezcla desconocido '{audioMixChannel}'");
          break;
      }
    }

    public AudioSource ReturnActiveSourceByType(AudioPoolType audioPoolType)
    {
      if (!poolMap.TryGetValue(audioPoolType, out var sources))
      {
        Debug.LogError($"[AudioPoolsHandler]: No se encontró lista para pool '{audioPoolType}'");
        return null;
      }

      foreach (AudioSource audioSource in sources)
      {
        if (audioSource.isPlaying)
        {
          return audioSource;
        }
      }

      Debug.LogWarning($"[AudioPoolsHandler]: Non '{audioPoolType}' active source!");
      return null;
    }

    public List<AudioSource> ReturnAudioSourceByType(AudioPoolType audioPoolType)
    {
      if (!poolMap.TryGetValue(audioPoolType, out var sources))
      {
        Debug.LogError($"[AudioPoolsHandler]: No se encontró lista para pool '{audioPoolType}'");
        return null;
      }

      return sources;
    }

    public AudioSource ReturnFreeAudioSource(AudioPoolType audioPoolType)
    {
      if (!poolMap.TryGetValue(audioPoolType, out var sources))
      {
        Debug.LogError($"[AudioPoolsHandler]: Couldn't find '{audioPoolType}' sources list");
        return null;
      }
      return CheckForFreeAudioSource(sources, audioPoolType);
    }

    // Lógica de chequeo con doble validación (!isPlaying y !Reserved)
    private AudioSource CheckForFreeAudioSource(List<AudioSource> sources, AudioPoolType audioPoolType)
    {
      foreach (AudioSource audioSource in sources)
      {
        if (audioSource != null)
        {
          int id = audioSource.GetInstanceID();

          // CONDICIÓN ESTRICTA:
          // Debe no estar sonando Y ADEMÁS no estar en la lista de "Prestados" (claimed)
          if (!audioSource.isPlaying && !claimedSourcesIDs.Contains(id))
          {
            claimedSourcesIDs.Add(id); // La marcamos como Ocupada permanentemente
            return audioSource;
          }
        }
      }
      Debug.LogWarning($"[AudioPoolsHandler]: No hay fuentes libres del tipo '{audioPoolType}'");
      return null;
    }

    // Los scripts deben llamar a esto cuando terminan de usar la fuente
    public void ReleaseAudioSource(AudioSource source)
    {
      if (source == null) return;

      source.clip = null;

      int id = source.GetInstanceID();
      if (claimedSourcesIDs.Contains(id))
      {
        claimedSourcesIDs.Remove(id);
      }
    }

    // Método para limpiar el HashSet y no bloquear fuentes eternamente
    private void CleanUpReservations(List<AudioSource> sources)
    {
      foreach (var source in sources)
      {
        if (source == null) continue;

        int id = source.GetInstanceID();

        // Si está reservada PERO ya está sonando (isPlaying = true), 
        // significa que la "reserva" cumplió su función y podemos quitarla del HashSet.
        // Ahora la protección natural de 'isPlaying' es suficiente.
        if (source.isPlaying && claimedSourcesIDs.Contains(id))
        {
          claimedSourcesIDs.Remove(id);
        }
      }
    }

    public void ResetWeatherSources()
    {
      foreach (AudioSource source in EnvironmentSources)
      {
        source.clip = null;
      }
    }

    // Obtiene fuente para incluir en corrutinas de objeto ajeno (no restea valores alterados de la fuente)
    public (AudioSource, AudioSourceState) GetFreeSourceForInteractable(Transform transform, float clipsVolume)
    {
      AudioSource audioSource = AudioManager.Instance.
          PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Interaction);

      // Protección contra nulos si el pool se agotó
      if (audioSource != null)
      {
        AudioSourceState originalSourceParams = audioSource.GetSnapshot();
        audioSource.transform.position = transform.position;
        audioSource.volume = clipsVolume;
        return (audioSource, originalSourceParams);
      }

      return (null, default(AudioSourceState));
    }

    public void AssignWeatherEvents(Component sender, object data)
    {
      if ((WeatherEvent)data == WeatherEvent.None)
      {
        // Resetea clips de audio
        ResetWeatherSources();
        return;
      }

      AudioClip audioClip;
      float sourceMaxDistance;

      switch ((WeatherEvent)data)
      {
        case (WeatherEvent.SoftWind):
          sourceMaxDistance = softWeatherDefaultDistance;
          audioClip = softWindClip;
          SetAudioZoneSources(audioClip, sourceMaxDistance, softWindVolume);
          break;
        case (WeatherEvent.HardWind):
          sourceMaxDistance = hardWeatherDistance;
          audioClip = softWindClip;
          SetAudioZoneSources(audioClip, sourceMaxDistance, hardWindVolume);
          break;
        case (WeatherEvent.SoftRain):
          audioClip = softRainClip;
          sourceMaxDistance = softWeatherDefaultDistance;
          SetAudioZoneSources(audioClip, sourceMaxDistance, softRainVolume);
          break;
        case (WeatherEvent.HardRain):
          audioClip = hardRainClip;
          sourceMaxDistance = hardWeatherDistance;
          SetAudioZoneSources(audioClip, sourceMaxDistance, hardRainVolume);
          break;
        case (WeatherEvent.HardRainAndWind):
          audioClip = hardRainAndWindClip;
          sourceMaxDistance = hardWeatherDistance;
          SetAudioZoneSources(audioClip, sourceMaxDistance, hardRainAndWindVolume);
          break;
        case (WeatherEvent.ConstantThunders):
          // No hace nada, el rayo usa su propia fuente de audio
          break;
        case (WeatherEvent.CloseThunder):
          // No hace nada, el rayo usa su propia fuente de audio
          break;
        default:
          break;
      }
    }

    private void SetAudioZoneSources(AudioClip audioClip, float sourceMaxDistance, float volume)
    {
      foreach (AudioSource source in RainAndWindSources)
      {
        source.clip = audioClip;
        source.volume = volume;
        source.maxDistance = sourceMaxDistance;
      }

      // Le asigna las fuentes al AudioZoneHandler para que las posicione en zonas acústicas
      audioZoneHandler?.AssignWeatherSourcesFromPool(RainAndWindSources);
    }
  }
}
