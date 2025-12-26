using System.Collections.Generic;
using UnityEngine;
namespace TwelveG.AudioController
{
  public enum AudioPoolType
  {
    Rain,
    BGMusic,
    UI,
    Environment,
    Interaction,
    Wind,
    Dialogs,
    Player,
    VFX
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
  }

  public class AudioPoolsHandler : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private AudioZoneHandler audioZoneHandler;

    [Space]
    [Header("Audio Source Pools")]
    [SerializeField] private List<AudioSource> BGMusicSources;
    [SerializeField] private List<AudioSource> EnvironmentSources;
    [SerializeField] private List<AudioSource> InteractionSources;
    [SerializeField] private List<AudioSource> UISources;
    [SerializeField] private List<AudioSource> DialogsSources;
    [SerializeField] private List<AudioSource> PlayerSources;
    [SerializeField] private List<AudioSource> VFXSources;

    [Space]
    [Header("Pause Settings")]
    [SerializeField]
    private AudioPoolType[] poolsToPause = new AudioPoolType[] {
    AudioPoolType.Dialogs,
    AudioPoolType.Interaction,
    AudioPoolType.BGMusic,
    AudioPoolType.Environment,
    AudioPoolType.Player,
    AudioPoolType.VFX
    };

    [Space]
    [Header("Audio References")]
    [SerializeField] private AudioClip softWindClip;
    [SerializeField] private AudioClip softRainClip;

    private Dictionary<AudioPoolType, List<AudioSource>> poolMap;
    private List<AudioSource> lastActiveAudioSources = new List<AudioSource>();

    private void Awake()
    {
      poolMap = new Dictionary<AudioPoolType, List<AudioSource>>
      {
        { AudioPoolType.BGMusic, BGMusicSources },
        { AudioPoolType.Environment, EnvironmentSources },
        { AudioPoolType.Interaction, InteractionSources },
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
          StopActivePoolSources(AudioPoolType.Environment);
          StopActivePoolSources(AudioPoolType.Interaction);
          StopActivePoolSources(AudioPoolType.Dialogs);
          StopActivePoolSources(AudioPoolType.Player);
          StopActivePoolSources(AudioPoolType.VFX);
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

    private AudioSource CheckForFreeAudioSource(List<AudioSource> sources, AudioPoolType audioPoolType)
    {
      foreach (AudioSource audioSource in sources)
      {
        if (!audioSource.isPlaying)
        {
          return audioSource;
        }
      }

      Debug.LogWarning($"[AudioPoolsHandler]: Non '{audioPoolType}' free source");
      return null;
    }

    public void ResetWeatherClips()
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
      AudioSourceState originalSourceParams = AudioExtensions.GetSnapshot(audioSource);
      audioSource.transform.position = transform.position;
      audioSource.volume = clipsVolume;
      return (audioSource, originalSourceParams);
    }

    public void AssignWeatherEvents(Component sender, object data)
    {
      if((WeatherEvent)data == WeatherEvent.None)
      {
        // Resetea clips de audio
        ResetWeatherClips();
        return;
      }

      AudioClip audioClip;

      switch ((WeatherEvent)data)
      {
        case (WeatherEvent.SoftRain):
          audioClip = softRainClip;
          break;
        case (WeatherEvent.HardRain):
          audioClip = softRainClip; // TODO: cargar audio de lluvia fuerte a futuro
          break;
        case (WeatherEvent.SoftWind):
          audioClip = softWindClip;
          break;
        case (WeatherEvent.HardWind):
          audioClip = softWindClip; // TODO: cargar audio de viento fuerte a futuro
          break;
        default:
          audioClip = null;
          break;
      }

      foreach (AudioSource source in EnvironmentSources)
      {
        source.clip = audioClip;
      }

      // Le asigna las fuentes al AudioZoneHandler para que las posicione en zonas acústicas
      audioZoneHandler?.AssignAudioSourcesFromPool(EnvironmentSources);
    }
  }
}
