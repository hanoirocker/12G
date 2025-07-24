namespace TwelveG.AudioController
{
  using System.Collections.Generic;
  using UnityEngine;

  public enum AudioPoolType
  {
    Rain,
    BGMusic,
    UI,
  }

  public class AudioPoolsHandler : MonoBehaviour
  {
    [Header("Audio Source Pools")]
    [SerializeField] private List<AudioSource> rainSources;
    [SerializeField] private List<AudioSource> BGMusicSources;
    [SerializeField] private List<AudioSource> UISources;

    private Dictionary<AudioPoolType, List<AudioSource>> poolMap;

    private void Awake()
    {
      poolMap = new Dictionary<AudioPoolType, List<AudioSource>>
      {
        { AudioPoolType.Rain, rainSources },
        { AudioPoolType.BGMusic, BGMusicSources },
        { AudioPoolType.UI, UISources }
      };
    }

    public AudioSource ReturnFreeAudioSource(AudioPoolType audioPoolType)
    {
      if (!poolMap.TryGetValue(audioPoolType, out var sources))
      {
        Debug.LogError($"[AudioPoolsHandler]: No se encontró lista para pool '{audioPoolType}'");
        return null;
      }

      return CheckForFreeAudioSource(sources);
    }

    private AudioSource CheckForFreeAudioSource(List<AudioSource> sources)
    {
      foreach (AudioSource audioSource in sources)
      {
        if (!audioSource.isPlaying)
        {
          return audioSource;
        }
      }

      Debug.LogWarning($"[AudioPoolsHandler]: Todos los sources estaban ocupados.");
      return null;
    }
  }
}
