using System.Collections;
using TwelveG.AudioController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
  public class LightBeepHandler : MonoBehaviour
  {
    [Header("Light Beep Settings")]
    [SerializeField] private float maxLightIntensity = 1f;
    [SerializeField] private AudioClip alertClip;
    [SerializeField, Range(0f, 1f)] private float alertClipVolume = 0.5f;

    private Light walkieTalkieLight;
    private AudioSource beepAudioSource;
    private AudioSourceState beepAudioSourceState;
    private float beepOnDuration = 0.5f;
    private float beepOffDuration = 0.5f;

    private void OnEnable()
    {
      walkieTalkieLight = GetComponentInChildren<Light>();

      if (alertClip)
      {
        beepOnDuration = alertClip.length * 0.11169f;
        beepOffDuration = alertClip.length * 0.88831f;
      }

      if (walkieTalkieLight == null)
        return;

      StartCoroutine(LightBeepRoutine());
    }

    private void OnDisable()
    {
      StopAllCoroutines();

      if (beepAudioSource)
      {
        beepAudioSource.Stop(); ;
        beepAudioSource.RestoreSnapshot(beepAudioSourceState);
      }
    }

    private IEnumerator LightBeepRoutine()
    {
      walkieTalkieLight.enabled = true;

      yield return new WaitForFixedUpdate();

      (beepAudioSource, beepAudioSourceState) = AudioManager.Instance.PoolsHandler.GetFreeSourceForInteractable(
          transform,
          alertClipVolume
      );

      if (beepAudioSource)
      {
        beepAudioSource.clip = alertClip;
        beepAudioSource.loop = true;
        beepAudioSource.Play();
      }

      if (walkieTalkieLight == null)
        yield break;

      float initialIntensity = walkieTalkieLight.intensity;
      float targetIntensity = maxLightIntensity;

      while (gameObject.activeInHierarchy)
      {
        if (beepOnDuration <= 0f)
        {
          walkieTalkieLight.intensity = targetIntensity;
        }
        else
        {
          float t = 0f;
          while (t < beepOnDuration && gameObject.activeInHierarchy)
          {
            if (walkieTalkieLight == null) yield break;
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / beepOnDuration);
            walkieTalkieLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, p);
            yield return null;
          }
        }

        // Fall phase
        if (beepOffDuration <= 0f)
        {
          walkieTalkieLight.intensity = initialIntensity;
        }
        else
        {
          float t = 0f;
          while (t < beepOffDuration && gameObject.activeInHierarchy)
          {
            if (walkieTalkieLight == null) yield break;
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / beepOffDuration);
            walkieTalkieLight.intensity = Mathf.Lerp(targetIntensity, initialIntensity, p);
            yield return null;
          }
        }
      }
    }
  }
}