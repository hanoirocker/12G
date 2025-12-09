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

    public static float CalculateDurationWithPitch(AudioClip clip, float pitch, float duration = 0f)
    {
      if (clip == null && duration == 0f) return 0f;

      // Si no hay clip, pero se pasa una duración, devolvemos esa duración ajustada al pitch
      if (clip == null && duration > 0f)
      {
        if (Mathf.Approximately(pitch, 0f)) return 0f;

        return duration / Mathf.Abs(pitch);
      }
      if (clip != null && duration == 0f)
      {
        // Si hay clip, usamos su duración

        // Si el pitch es 0, el audio está pausado (duración infinita), 
        // pero devolvemos 0 o un valor seguro para no romper corrutinas.
        if (Mathf.Approximately(pitch, 0f)) return 0f;

        return clip.length / Mathf.Abs(pitch);
      }
      return 0f;
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

    public static void StopAndRestoreAudioSource(AudioSource source, AudioSourceState state)
    {
      if(source == null) return;
      source.Stop();
      source.RestoreSnapshot(state);
    }
  }
}