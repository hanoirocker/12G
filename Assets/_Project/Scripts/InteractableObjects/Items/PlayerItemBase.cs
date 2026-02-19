using TwelveG.GameController;
using TwelveG.PlayerController;
using TwelveG.UIController;
using TwelveG.VFXController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public abstract class PlayerItemBase : MonoBehaviour
    {
        [Header("Common Item Settings")]
        public ItemType itemType;

        [Header("Event SO's references")]
        // Se disparan para que los reciban el Item Canvas y el Dialog Manager
        [SerializeField] private protected GameEventSO onItemToggled;
        [SerializeField] private protected GameEventSO onShowingItem;

        private protected Animation anim;
        private protected bool canBeToogled = false;
        private protected bool animationPlaying = false;
        private protected bool itemIsShown = false;
        private protected bool isActiveOnGame = false;

        protected virtual void Awake()
        {
            anim = GetComponent<Animation>();
        }

        public void AllowItemToBeToggled(bool allow)
        {
            canBeToogled = allow;
        }

        // Llamada al hacer habilitar el item mediante GameEvent o al recogerlo
        // También al deshabilitarlo.
        public void ActivateItem(bool activate)
        {
            isActiveOnGame = activate;

            // Ocultamos el item si se desactiva y avisamos al Item Canvas para que oculte sus íconos
            if (!activate)
            {
                anim.Play("HideItem");
                itemIsShown = false;
                onItemToggled.Raise(this, itemIsShown);
            }

            // Avisarle al Control Canvas para que muestre/oculte las opciones del item
            InteractionObjectType interactionType = itemType switch
            {
                ItemType.Flashlight => InteractionObjectType.Flashlight,
                ItemType.WalkieTalkie => InteractionObjectType.WalkieTalkie,
                _ => InteractionObjectType.None,
            };

            // Si se activa el WT, inicializar en el VFXManager para actualizar efecto ElectricFeel
            // con los settings del evento actual
            if (gameObject.TryGetComponent(out WalkieTalkie walkieTalkie) && activate)
            {
                VFXManager.Instance.Initialize(walkieTalkie);
            }

            UIManager.Instance.ControlCanvasHandler.SetInteractionSpecificOptions(interactionType, activate);
        }

        public bool IsItemActiveInGame()
        {
            return isActiveOnGame;
        }

        public bool ItemIsEquipped()
        {
            return itemIsShown;
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
            GetComponentInParent<PlayerInventory>().HandleTogglingItemsHandState(itemType, true);
        }
    }
}