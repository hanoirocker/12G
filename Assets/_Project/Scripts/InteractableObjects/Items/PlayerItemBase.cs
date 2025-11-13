using System.Collections;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public abstract class PlayerItemBase : MonoBehaviour
    {
        [Header("Common Item Settings")]
        private protected Animation anim;
        private protected bool canBeToogled = false;
        private protected bool itemIsShown = false;

        protected virtual void Awake()
        {
            anim = GetComponent<Animation>();
        }

        public IEnumerator ToggleItem(GameObject item)
        {
            if (anim == null)
                yield break;

            if (itemIsShown && canBeToogled)
            {
                // Si está visible, ejecuta animación para ocultar
                anim.Play("HideItem");
                yield return new WaitUntil(() => !anim.isPlaying);
                itemIsShown = false;
            }
            else if (!itemIsShown && canBeToogled)
            {
                // Si está oculto, ejecuta animación para mostrar
                anim.Play("ShowItem");
                yield return new WaitUntil(() => !anim.isPlaying);
                itemIsShown = true;
            }
            else
            {
                yield return null;
            }
        }

        public void AllowItemToBeToggled(bool allow)
        {
            canBeToogled = allow;
        }

        public bool CanBeToggled()
        {
            return canBeToogled;
        }

        public void ShowItem()
        {
            anim.Play("ShowItem");
            itemIsShown = true;
        }
    }
}