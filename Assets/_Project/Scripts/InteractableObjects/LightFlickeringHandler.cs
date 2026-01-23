using System.Collections;
using TwelveG.AudioController;
using TwelveG.Utils; // Asumo AudioUtils aquí
using UnityEngine;

namespace TwelveG.InteractableObjects
{
  [RequireComponent(typeof(Light))]
  public class LightFlickeringHandler : MonoBehaviour
  {
    // ... (Tus Settings siguen igual) ...
    [Header("Flickering Settings")]
    [SerializeField, Range(0.01f, 0.2f)] private float minFlickerInterval = 0.05f;
    [SerializeField, Range(0.05f, 0.5f)] private float maxFlickerInterval = 0.2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip sparkSoundClip;
    [SerializeField, Range(0f, 10f)] private float sparkSoundMaxDistance = 4f;
    [SerializeField, Range(0f, 1f)] private float sparkSoundVolume = 0.07f;

    private Light targetLight;
    private AudioSource audioSource;
    private AudioSourceState audioSourceState;
    private bool isFlickering = false;

    void Awake()
    {
      targetLight = GetComponent<Light>();
    }

    public void StartFlickering()
    {
      // Pedimos la fuente (Ahora el Pool la marca como OCUPADA para siempre)
      (audioSource, audioSourceState) =
        AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
          transform,
          sparkSoundVolume
        );

      if (!isFlickering)
      {
        isFlickering = true;
        StartCoroutine(FlickerLightRoutine());
      }
    }

    public void StopFlickering(bool stayOn = true)
    {
      isFlickering = false;
      if (targetLight != null) targetLight.enabled = stayOn;

      AudioUtils.StopAndRestoreAudioSource(audioSource, audioSourceState);
      audioSource = null;
    }

    public IEnumerator FlickerLightRoutine()
    {
      while (isFlickering)
      {
        bool lightState = Random.value > 0.4f;

        targetLight.enabled = lightState;

        if (audioSource == null) yield break;

        if (lightState && sparkSoundClip != null)
        {
          audioSource.transform.position = transform.position;
          audioSource.pitch = Random.Range(0.85f, 1.2f);
          audioSource.maxDistance = sparkSoundMaxDistance;
          audioSource.clip = sparkSoundClip;
          audioSource.rolloffMode = AudioRolloffMode.Custom;
          audioSource.volume = sparkSoundVolume;
          if (!audioSource.isPlaying) audioSource.Play();
        }

        float randomWait = Random.Range(minFlickerInterval, maxFlickerInterval);
        yield return new WaitForSeconds(randomWait);
      }
    }
  }
}