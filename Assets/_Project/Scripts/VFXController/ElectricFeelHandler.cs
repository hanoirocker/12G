using System;
using System.Collections;
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
        private PostProcessingHandler postProcessingHandler;

        private void OnEnable()
        {
            StartCoroutine(EffectUpdateCoroutine());
        }

        private IEnumerator EffectUpdateCoroutine()
        {
            float fadeDuration = isEffectEnabled ? effectFadeInDuration : effectFadeOutDuration;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newWeight = Mathf.Lerp(0f, electricFeelMaxIntensity, elapsedTime / fadeDuration);
                postProcessingHandler.SetElectricFeelWeight(newWeight);
                yield return null;
            }

            postProcessingHandler.SetElectricFeelWeight(electricFeelMaxIntensity);
        }

        private void OnDisable()
        {
            postProcessingHandler.SetElectricFeelWeight(0f);
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

