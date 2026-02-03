using System;
using System.Collections;
using TwelveG.DialogsController;
using TwelveG.GameController;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class Flashlight : PlayerItemBase, IPlayerItem
    {
        [Header("References")]
        [SerializeField] private Light flashlightLight;

        [Space(10)]
        [Header("Settings")]
        [SerializeField, Range(0f, 5f)] private float maxIntensity = 2.5f;

        [Space(10)]
        [Header("Dialogs")]
        [SerializeField] private DialogSO outOfBatteriesHoldingFlashlight;
        [SerializeField] private DialogSO outOfBatteriesOnShowingFlashlight;

        Coroutine drainBatteriesCoroutine = null;
        private bool fullBatteries = true;
        bool playerNoticedBatteriesDrained = false;

        private void Update()
        {
            if (isActiveOnGame)
            {
                if (Input.GetKeyDown(KeyCode.L) && canBeToogled)
                {
                    ToggleItem();
                }
            }
        }

        public void ToggleItem()
        {
            StartCoroutine(ToggleItemCoroutine());
        }

        private IEnumerator ToggleItemCoroutine()
        {
            if (anim == null) yield break;
            if (animationPlaying) yield break;

            if (itemIsShown && canBeToogled)
            {
                animationPlaying = true;
                anim.Play("HideItem");

                if (fullBatteries && drainBatteriesCoroutine == null)
                {
                    StartCoroutine(LightCoroutine(maxIntensity, 0f, 1f));
                }

                itemIsShown = false;
                yield return new WaitUntil(() => !anim.isPlaying);
                GetComponentInParent<PlayerInventory>().HandleTogglingItemsHandState(itemType, false);
                onItemToggled.Raise(this, itemIsShown);
                animationPlaying = false;
            }
            else if (!itemIsShown && canBeToogled)
            {
                GetComponentInParent<PlayerInventory>().HandleTogglingItemsHandState(itemType, true);
                animationPlaying = true;

                if (fullBatteries && drainBatteriesCoroutine == null)
                {
                    StartCoroutine(LightCoroutine(0f, maxIntensity, 1f));
                }
                if (!fullBatteries && !playerNoticedBatteriesDrained)
                {
                    GameEvents.Common.onStartDialog.Raise(this, outOfBatteriesOnShowingFlashlight);
                    playerNoticedBatteriesDrained = true;
                }

                anim.Play("ShowItem");
                itemIsShown = true;

                yield return new WaitUntil(() => !anim.isPlaying);
                onItemToggled.Raise(this, itemIsShown);
                animationPlaying = false;
            }
        }

        private IEnumerator LightCoroutine(float from, float to, float duration)
        {
            if (drainBatteriesCoroutine != null) yield break;

            canBeToogled = false;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (drainBatteriesCoroutine != null)
                {
                    canBeToogled = true;
                    yield break;
                }

                elapsed += Time.deltaTime;
                float currentValue = Mathf.Lerp(from, to, elapsed / duration);
                flashlightLight.intensity = currentValue;
                yield return null;
            }

            flashlightLight.intensity = to;
            canBeToogled = true;
        }

        public void TriggerDrainBatteriesRoutine(float duration)
        {
            StopAllCoroutines();
            animationPlaying = false;
            canBeToogled = true;

            drainBatteriesCoroutine = StartCoroutine(DrainBatteriesRoutine(duration));
        }

        private IEnumerator DrainBatteriesRoutine(float duration)
        {
            if (flashlightLight != null)
            {
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;

                    float batteryPercent = Mathf.Clamp01(1f - (elapsed / duration));

                    if (itemIsShown)
                    {
                        flashlightLight.intensity = maxIntensity * batteryPercent;
                    }

                    if (!playerNoticedBatteriesDrained && itemIsShown && elapsed >= duration * 0.7f)
                    {
                        GameEvents.Common.onStartDialog.Raise(this, outOfBatteriesHoldingFlashlight);
                        playerNoticedBatteriesDrained = true;
                    }
                    yield return null;
                }

                fullBatteries = false;
                flashlightLight.intensity = 0f;
                drainBatteriesCoroutine = null;
            }
        }

        public void ToogleFlashlightLight(int state)
        {
            if (flashlightLight != null)
            {
                flashlightLight.enabled = (state == 1);
            }
        }
    }
}