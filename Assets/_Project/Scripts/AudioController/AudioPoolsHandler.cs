namespace TwelveG.AudioController
{
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  public enum AudioPoolType
  {
    Rain,
    BGMusic,
    UI,
    Environment,
    Interaction,
    Wind
  }

  public enum WeatherSound
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

    [Header("Audio References")]
    [SerializeField] private AudioClip softWindClip;
    [SerializeField] private AudioClip softRainClip;

    private Dictionary<AudioPoolType, List<AudioSource>> poolMap;

    private void Awake()
    {
      poolMap = new Dictionary<AudioPoolType, List<AudioSource>>
      {
        { AudioPoolType.BGMusic, BGMusicSources },
        { AudioPoolType.Environment, EnvironmentSources },
        { AudioPoolType.Interaction, InteractionSources },
        { AudioPoolType.UI, UISources },
      };
    }

    public void PlayClipOnSpecificAudioSource(AudioPoolType audioPoolType, AudioClip clip)
    {
      AudioSource source = ReturnFreeAudioSource(audioPoolType);

      if (source.isPlaying)
      {
        Debug.Log($"[AudioPoolsHandler]: retrieved source {source.gameObject.name} was playing before playing new sound!");
        source.Stop();
      }
      source.PlayOneShot(clip);
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
        Debug.LogError($"[AudioPoolsHandler]: Non '{audioPoolType}' free source");
      }

      Debug.LogWarning($"[AudioPoolsHandler]: Todos los sources estaban ocupados.");
      return null;
    }

    public void AssignWeatherSounds(Component sender, object data)
    {
      AudioClip audioClip;

      switch ((WeatherSound)data)
      {
        case (WeatherSound.SoftRain):
          audioClip = softRainClip;
          break;
        case (WeatherSound.HardRain):
          audioClip = softRainClip;
          break;
        case (WeatherSound.SoftWind):
          audioClip = softWindClip;
          break;
        case (WeatherSound.HardWind):
          audioClip = softWindClip;
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
