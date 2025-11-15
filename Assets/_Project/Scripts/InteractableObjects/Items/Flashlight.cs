namespace TwelveG.InteractableObjects
{
    using System.Collections;
    using UnityEngine;

    public class Flashlight : PlayerItemBase, IPlayerItem
    {
        [Header("References")]
        [SerializeField] private Light flashlightLight;

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
                itemIsShown = false;
                yield return new WaitUntil(() => !anim.isPlaying);
                onItemToggled.Raise(this, itemIsShown);
                animationPlaying = false;
            }
            else if (!itemIsShown && canBeToogled)
            {
                animationPlaying = true;
                // Si está oculto, ejecuta animación para mostrar
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