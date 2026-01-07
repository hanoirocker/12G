namespace TwelveG.InteractableObjects
{
    using System;
    using System.Collections;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class Flashlight : PlayerItemBase, IPlayerItem
    {
        [Header("References")]
        [SerializeField] private Light flashlightLight;

        [Header("Settings")]
        [SerializeField, Range(0f, 5f)] private float maxIntensity = 2.5f;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L) && canBeToogled)
            {
                ToggleItem();
            }
        }

        public void ToggleItem()
        {
            StartCoroutine(ToggleItemCoroutine());
        }

        private IEnumerator ToggleItemCoroutine()
        {
            if (anim == null)
                yield break;

            if (animationPlaying)
                yield break;

            if (itemIsShown && canBeToogled)
            {
                animationPlaying = true;
                // Si está visible, ejecuta animación para ocultar
                anim.Play("HideItem");
                StartCoroutine(LightCoroutine(maxIntensity, 0f, 1f));
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
                // Si está oculto, ejecuta animación para mostrar
                StartCoroutine(LightCoroutine(0f, maxIntensity, 1f));
                anim.Play("ShowItem");
                itemIsShown = true;
                yield return new WaitUntil(() => !anim.isPlaying);
                onItemToggled.Raise(this, itemIsShown);
                animationPlaying = false;
            }
            else
            {
                yield return null;
            }
        }

        private IEnumerator LightCoroutine(float from, float to, float duration)
        {
            canBeToogled = false;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float currentValue = Mathf.Lerp(from, to, elapsed / duration);
                flashlightLight.intensity = currentValue;
                yield return null;
            }

            canBeToogled = true;
        }

        public void ToogleFlashlightLight(int state)
        {
            if (flashlightLight != null)
            {
                if (state == 1)
                {
                    flashlightLight.enabled = true;
                }
                else if (state == 0)
                {
                    flashlightLight.enabled = false;
                }
            }
        }
    }
}