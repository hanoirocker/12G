using System.Collections;
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
    public AudioSource GetFreeSourceForInteractable(Transform transform, float clipsVolume)
    {
      AudioSource audioSource = AudioManager.Instance.
          PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Interaction);
      audioSource.transform.position = transform.position;
      audioSource.volume = clipsVolume;
      return audioSource;
    }

    // Obtiene fuente para ejecutar corrutina en su propio objeto (reset automático al finalizar el codigo invocado)
    public void GetFreeTemporarySourceByType(AudioPoolType poolType, float duration, System.Action<AudioSource> onSourceBorrowed)
    {
      AudioSource source = ReturnFreeAudioSource(poolType);

      if (source == null) return;

      var originalState = source.GetSnapshot();

      // Ejecución del callback con el AudioSource prestado
      onSourceBorrowed?.Invoke(source);

      StartCoroutine(ResetSourceRoutine(source, originalState, duration));
    }

    private IEnumerator ResetSourceRoutine(AudioSource source, AudioSourceState originalState, float duration)
    {
      yield return new WaitForSeconds(duration);

      // Limpieza de AudioSource prestado
      if (source.isPlaying) source.Stop();

      Animation anim = source.GetComponent<Animation>();

      if (anim != null)
      {
        Destroy(anim);
      }

      source.RestoreSnapshot(originalState);
    }

    public void AssignWeatherEvents(Component sender, object data)
    {
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
