using UnityEngine;

namespace TwelveG.AudioController
{
  public static class AudioUtils
  {
    private const float MIN_DB = -88f;
    private const float MAX_DB = 0f;


    public static float NormalizedToDecibels(float normalizedValue)
    {
      if (normalizedValue <= 0f)
      {
        return MIN_DB;
      }
      else if (normalizedValue >= 1)
      {
        return MAX_DB;
      }

      float dB = Mathf.Log10(normalizedValue) * 20f;

      dB = Mathf.Clamp(dB, MIN_DB, MAX_DB);

      return dB;
    }

    public static float DecibelsToNormalized(float decibels)
    {
      if (decibels <= MIN_DB)
      {
        return 0f;
      }
      else if (decibels >= MAX_DB)
      {
        return 1f;
      }

      float normalizedValue = Mathf.Pow(10f, decibels / 20f);

      // Asegurarnos que esté entre 0 y 1
      return Mathf.Clamp01(normalizedValue);
    }

    public static AudioSource GetAudioSourceForInteractable(Transform transform, float clipsVolume)
    {
      AudioSource audioSource = AudioManager.Instance.
          PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Interaction);
      audioSource.transform.position = transform.position;
      audioSource.volume = clipsVolume;
      return audioSource;
    }
  }
}