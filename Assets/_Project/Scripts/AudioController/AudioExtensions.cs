namespace TwelveG.AudioController
{
  using UnityEngine;
  public static class AudioUtilities
  {
    public static void StopIfPlaying(this AudioSource source)
    {
      if (source.isPlaying)
        source.Stop();
    }
  }
}