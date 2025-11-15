using System.Collections;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public abstract class PlayerItemBase : MonoBehaviour
    {
        [Header("Common Item Settings")]
        private protected Animation anim;
        private protected bool canBeToogled = false;
        private protected bool animationPlaying = false;
        private protected bool itemIsShown = false;

        [Header("Event SO's references")]

        // Se disparan para que los reciban el Item Canvas y el Dialog Manager
        [SerializeField] private protected GameEventSO onItemToggled;
        [SerializeField] private protected GameEventSO onShowingItem;

        protected virtual void Awake()
        {
            anim = GetComponent<Animation>();
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