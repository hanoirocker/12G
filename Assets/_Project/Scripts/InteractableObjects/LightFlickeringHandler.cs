using System.Collections;
using TwelveG.AudioController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
  [RequireComponent(typeof(Light))]
  public class LightFlickeringHandler : MonoBehaviour
  {
    [Header("Flickering Settings")]
    [Tooltip("Tiempo mínimo que la luz permanecerá en un estado antes de cambiar.")]
    [SerializeField, Range(0.01f, 0.2f)] private float minFlickerInterval = 0.05f;

    [Tooltip("Tiempo máximo que la luz permanecerá en un estado.")]
    [SerializeField, Range(0.05f, 0.5f)] private float maxFlickerInterval = 0.2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip sparkSoundClip;
    [Tooltip("Volumen base del chispazo.")]
    [SerializeField, Range(0f, 1f)] private float sparkSoundVolume = 0.7f;

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
    }

    public IEnumerator FlickerLightRoutine()
    {
      while (isFlickering)
      {
        bool lightState = Random.value > 0.4f;

        targetLight.enabled = lightState;

        if (lightState && sparkSoundClip != null)
        {
          audioSource.pitch = Random.Range(0.85f, 1.2f);
          audioSource.PlayOneShot(sparkSoundClip, sparkSoundVolume);
        }

        float randomWait = Random.Range(minFlickerInterval, maxFlickerInterval);
        yield return new WaitForSeconds(randomWait);
      }
    }
  }
}