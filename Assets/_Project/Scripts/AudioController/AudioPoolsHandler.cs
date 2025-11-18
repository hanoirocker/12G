namespace TwelveG.AudioController
{
  using System.Collections.Generic;
  using TwelveG.GameController;
  using UnityEngine;
  using UnityEngine.SceneManagement;

  public enum AudioPoolType
  {
    Rain,
    BGMusic,
    UI,
    Environment,
    Interaction,
    Wind,
    Dialogs,
    Player
  }

  public enum WeatherEvent
  {
    SoftRain,
    HardRain,
    SoftWind,
    HardWind,
    SoftRainAndWind,
    HardRainAndWind
  }

  public class AudioPoolsHandler : MonoBehaviour
  {
    [Header("Audio Source Pools")]
    [SerializeField] private List<AudioSource> BGMusicSources;
    [SerializeField] private List<AudioSource> EnvironmentSources;
    [SerializeField] private List<AudioSource> InteractionSources;
    [SerializeField] private List<AudioSource> UISources;
    [SerializeField] private List<AudioSource> DialogsSources;
    [SerializeField] private List<AudioSource> PlayerSources;

    [Header("Pause Settings")]
    [SerializeField]
    private AudioPoolType[] poolsToPause = new AudioPoolType[] {
    AudioPoolType.Dialogs,
    AudioPoolType.Interaction,
    AudioPoolType.BGMusic,
    AudioPoolType.Environment,
    AudioPoolType.Player
    };

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
        { AudioPoolType.Player, PlayerSources}
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

    public void AssignWeatherEvents(Component sender, object data)
    {
      string currentSceneName = SceneManager.GetActiveScene().name;

      if (currentSceneName.Contains("Menu"))
      {
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
    }
  }
}
