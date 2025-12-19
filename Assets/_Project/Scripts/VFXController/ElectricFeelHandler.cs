using System;
using System.Collections;
using TwelveG.AudioController;
using UnityEngine;

namespace TwelveG.VFXController
{
    public class ElectricFeelHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Range(0f, 180)] private float effectFadeInDuration = 60f;
        [SerializeField, Range(0f, 180)] private float effectFadeOutDuration = 12f;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip electricFeelClip;


        private bool isEffectEnabled = false;
        private float lastEffectIntensity = 0f;
        private float lastEffectVolume = 0f;
        private float electricFeelMaxIntensity = 0f;
        private float effectMaxVolume = 0f;
        private PostProcessingHandler postProcessingHandler;
        private AudioSource electricFeelAudioSource;
        private AudioSourceState audioSourceState;

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

            StartCoroutine(UpdateEffectCoroutine());
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

        private IEnumerator UpdateEffectCoroutine()
        {
            float fadeDuration = isEffectEnabled ? effectFadeInDuration : effectFadeOutDuration;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newWeight = Mathf.Lerp(lastEffectIntensity, electricFeelMaxIntensity, elapsedTime / fadeDuration);
                float newVolume = Mathf.Lerp(lastEffectVolume, effectMaxVolume, elapsedTime / fadeDuration);
                postProcessingHandler.SetElectricFeelWeight(newWeight);
                electricFeelAudioSource.volume = newVolume;
                yield return null;
            }

            lastEffectIntensity = electricFeelMaxIntensity;
            lastEffectVolume = effectMaxVolume;
            postProcessingHandler.SetElectricFeelWeight(electricFeelMaxIntensity);
            electricFeelAudioSource.volume = effectMaxVolume;
        }

        public void SetAudioSettings(float volume)
        {
            effectMaxVolume = volume;
        }

        public void UpdateEffect()
        {
            StartCoroutine(UpdateEffectCoroutine());
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

