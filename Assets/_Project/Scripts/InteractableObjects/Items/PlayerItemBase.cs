using System.Collections;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public abstract class PlayerItemBase : MonoBehaviour
    {
        [Header("Common Item Settings")]
        private protected Animation anim;
        private protected bool canBeToogled = false;
        private bool animationPlaying = false;
        private protected bool itemIsShown = false;

        [Header("Event SO's references")]
        [SerializeField] private GameEventSO onItemToggled;

        protected virtual void Awake()
        {
            anim = GetComponent<Animation>();
        }

        public IEnumerator ToggleItem(GameObject item)
        {
            if (anim == null)
                yield break;

            if(animationPlaying)
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
            if (itemIsShown)
                return;

            anim.Play("ShowItem");
            itemIsShown = true;
            onItemToggled.Raise(this, itemIsShown);
        }
    }
}