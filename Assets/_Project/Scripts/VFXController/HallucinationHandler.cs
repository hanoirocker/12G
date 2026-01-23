using System.Collections;
using TwelveG.AudioController;
using UnityEngine;

namespace TwelveG.VFXController
{
  public class HallucinationHandler : MonoBehaviour
  {
    [Header("SO Reference")]
    [SerializeField] private HallucinationsProfileSO hallucinationsProfileSO;

    private PostProcessingHandler postProcessingHandler;

    // Variable PERSISTENTE: Solo se usa para conectar FadeIn con FadeOut
    private AudioSource currentPersistentSource = null;
    private AudioSourceState currentPersistentState;

    public void Initialize(PostProcessingHandler ppHandler)
    {
      this.postProcessingHandler = ppHandler;
      hallucinationsProfileSO.Initialize();
    }

    public IEnumerator TriggerHallucinationRoutine(HallucinationEffectType type)
    {
      if (type == HallucinationEffectType.None) yield break;

      var data = hallucinationsProfileSO.GetSettingsByType(type);

      if (data.effectDuration < 0) data.effectDuration = 0.5f;

      switch (type)
      {
        // FADE IN NO PERSISTENTE (Sube y corta)
        case HallucinationEffectType.NPRedDistortionFadeIn:
          yield return StartCoroutine(HandleNPFadeIn(data));
          break;

        // FADE IN PERSISTENTE (Sube y se queda)
        case HallucinationEffectType.RedDistortionFadeIn:
          yield return StartCoroutine(HandlePersistentFadeIn(data));
          break;

        // (Baja suave o Golpe+Baja)
        case HallucinationEffectType.RedDistortionFadeOut:
          yield return StartCoroutine(HandleSmartFadeOut(data));
          break;

        // RED DISTORTION (Autocontenido: Sube, espera, baja)
        case HallucinationEffectType.RedDistortion:
          yield return StartCoroutine(HandleRedDistortionAutocontained(data));
          break;

        // FERNANDEZ (La duración del audio manda)
        case HallucinationEffectType.FernandezHallucination:
          yield return StartCoroutine(HandleFernandezAutocontained(data));
          break;
      }
    }


    private IEnumerator HandleNPFadeIn(HallucinationsProfileSO.HallucinationData data)
    {
      AudioSource source = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
      AudioSourceState state = default;

      if (source != null)
      {
        state = source.GetSnapshot();
        source.clip = data.audioClip;
        source.loop = true;
        source.spatialBlend = 0f;
        source.Play();

        StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(source, 0f, data.audioVolume, data.effectDuration));
      }

      yield return StartCoroutine(postProcessingHandler.SetRedDistortionWeight(data.targetIntensity, data.effectDuration));

      if (source != null)
      {
        AudioUtils.StopAndRestoreAudioSource(source, state);
      }

      StartCoroutine(postProcessingHandler.SetRedDistortionWeight(0f, 0f));
    }

    private IEnumerator HandlePersistentFadeIn(HallucinationsProfileSO.HallucinationData data)
    {
      if (currentPersistentSource != null)
      {
        AudioUtils.StopAndRestoreAudioSource(currentPersistentSource, currentPersistentState);
      }

      currentPersistentSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);

      if (currentPersistentSource != null)
      {
        currentPersistentState = currentPersistentSource.GetSnapshot();
        currentPersistentSource.clip = data.audioClip;
        currentPersistentSource.loop = true;
        currentPersistentSource.spatialBlend = 0f;
        currentPersistentSource.Play();

        StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeIn(currentPersistentSource, 0f, data.audioVolume, data.effectDuration));
      }

      yield return StartCoroutine(postProcessingHandler.SetRedDistortionWeight(data.targetIntensity, data.effectDuration));
    }

    private IEnumerator HandleSmartFadeOut(HallucinationsProfileSO.HallucinationData data)
    {
      // Si hay un efecto persistente activo .. (Persistencia)
      if (currentPersistentSource != null)
      {
        StartCoroutine(postProcessingHandler.SetRedDistortionWeight(0f, data.effectDuration));

        yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(currentPersistentSource, data.effectDuration));

        AudioUtils.StopAndRestoreAudioSource(currentPersistentSource, currentPersistentState);

        currentPersistentSource = null;
      }
      // Si no hay nada previo (Arranca de golpe y baja)
      else
      {
        StartCoroutine(postProcessingHandler.SetRedDistortionWeight(data.targetIntensity, 0f));

        AudioSource tempSource = null;
        AudioSourceState tempState = default;

        if (data.audioClip != null)
        {
          tempSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
          if (tempSource != null)
          {
            tempState = tempSource.GetSnapshot();
            tempSource.clip = data.audioClip;
            tempSource.loop = true;
            tempSource.spatialBlend = 0f;
            tempSource.volume = data.audioVolume;
            tempSource.Play();
          }
        }
  
        yield return null;

        StartCoroutine(postProcessingHandler.SetRedDistortionWeight(0f, data.effectDuration));

        if (tempSource != null)
        {
          yield return StartCoroutine(AudioManager.Instance.FaderHandler.AudioSourceFadeOut(tempSource, data.effectDuration));

          AudioUtils.StopAndRestoreAudioSource(tempSource, tempState);
          AudioManager.Instance.PoolsHandler.ReleaseAudioSource(tempSource);
        }
        else
        {
          yield return new WaitForSeconds(data.effectDuration);
        }
      }
    }

    private IEnumerator HandleRedDistortionAutocontained(HallucinationsProfileSO.HallucinationData data)
    {
      AudioSource source = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
      AudioSourceState state = default;

      if (source != null)
      {
        state = source.GetSnapshot();
        source.clip = data.audioClip;
        source.loop = true;
        source.spatialBlend = 0f;
        source.volume = data.audioVolume;
        source.Play();
      }

      StartCoroutine(postProcessingHandler.SetRedDistortionWeight(data.targetIntensity, 0.1f));

      yield return new WaitForSeconds(data.effectDuration);

      StartCoroutine(postProcessingHandler.SetRedDistortionWeight(0f, 0.1f));

      if (source != null)
      {
        AudioUtils.StopAndRestoreAudioSource(source, state);
      }
    }

    private IEnumerator HandleFernandezAutocontained(HallucinationsProfileSO.HallucinationData data)
    {
      AudioSource source = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
      AudioSourceState state = default;
      float waitTime = 2f;

      if (source != null)
      {
        state = source.GetSnapshot();
        source.clip = data.audioClip;
        source.loop = false;
        source.spatialBlend = 0f;
        source.volume = data.audioVolume;
        source.Play();

        if (data.audioClip != null) waitTime = data.audioClip.length;
      }

      StartCoroutine(postProcessingHandler.SetRedDistortionWeight(data.targetIntensity, 0.1f));

      yield return new WaitForSeconds(waitTime);

      StartCoroutine(postProcessingHandler.SetRedDistortionWeight(0f, 0.5f));

      if (source != null)
      {
        AudioUtils.StopAndRestoreAudioSource(source, state);
      }
    }
  }
}