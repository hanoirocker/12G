using System;
using System.Collections;
using TwelveG.AudioController;
using UnityEngine;

namespace TwelveG.VFXController
{
    public class ElectricFeelHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Range(0f, 15f)] private float effectFadeInDuration = 8f;
        [SerializeField, Range(0f, 15f)] private float effectFadeOutDuration = 8f;

        private bool isEffectEnabled = false;
        private float electricFeelMaxIntensity = 0f;
        private float effectMaxVolume = 0f;
        private PostProcessingHandler postProcessingHandler;
        private AudioSource electricFeelAudioSource;
        private AudioSourceState audioSourceState;
        private AudioClip electricFeelClip = null;

        private void OnEnable()
        {
            if (electricFeelAudioSource == null)
            {
                electricFeelAudioSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.VFX);
                audioSourceState = electricFeelAudioSource.GetSnapshot();
                electricFeelAudioSource.spatialBlend = 0f;
                electricFeelAudioSource.clip = electricFeelClip;
                electricFeelAudioSource.loop = true;
                electricFeelAudioSource.volume = 0f;
                electricFeelAudioSource.Play(); 
            }

            StartCoroutine(EffectUpdateCoroutine());
        }

        private void OnDisable()
        {
            if (electricFeelAudioSource != null)
            {
                electricFeelAudioSource.Stop();
                electricFeelAudioSource.RestoreSnapshot(audioSourceState);
                electricFeelAudioSource = null;
            }

            postProcessingHandler.SetElectricFeelWeight(0f);
        }

        private IEnumerator EffectUpdateCoroutine()
        {
            float fadeDuration = isEffectEnabled ? effectFadeInDuration : effectFadeOutDuration;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newWeight = Mathf.Lerp(0f, electricFeelMaxIntensity, elapsedTime / fadeDuration);
                float newVolume = Mathf.Lerp(0f, effectMaxVolume, elapsedTime / fadeDuration);
                postProcessingHandler.SetElectricFeelWeight(newWeight);
                electricFeelAudioSource.volume = newVolume;
                yield return null;
            }

            postProcessingHandler.SetElectricFeelWeight(electricFeelMaxIntensity);
            electricFeelAudioSource.volume = effectMaxVolume;
        }

        public void SetAudioSettings(float volume, AudioClip clip)
        {
            effectMaxVolume = volume;
            electricFeelClip = clip;
        }

        public void SetIntensity(float multiplier)
        {
            if (multiplier > 0f)
            {
                isEffectEnabled = true;
                electricFeelMaxIntensity = multiplier;
            }
            else
            {
                isEffectEnabled = false;
                electricFeelMaxIntensity = 0f;
            }
        }

        public void Initialize(PostProcessingHandler ppHandler)
        {
            this.postProcessingHandler = ppHandler;
        }

    }
}

