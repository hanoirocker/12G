using TwelveG.Localization;
using TwelveG.PlayerController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    public class PickableItem : MonoBehaviour, IItem
    {
        [Header("Item settings")]
        [SerializeField] private ItemType itemType;
        public bool canBePicked;
        public bool triggerEventWhenPicked = false;

        [Header("Interaction Texts SO")]
        [SerializeField] private InteractionTextSO interactionTextsSO;

        [Header("Event SO references")]
        [SerializeField] private GameEventSO eventToTriggerWhenItemPicked;


        public void AllowToBePicked(Component sender, object data)
        {
            canBePicked = (bool)data;
        }

        public bool CanBePicked()
        {
            return canBePicked;
        }

        public InteractionTextSO RetrieveInteractionSO(PlayerInteraction playerCamera)
        {
            return interactionTextsSO;
        }

        public ItemType GetItemType()
        {
            return itemType;
        }

        // Funcion principal
        public void TakeItem()
        {
            if (triggerEventWhenPicked)
            {
                eventToTriggerWhenItemPicked.Raise(this, null);

            }
            Destroy(gameObject);
        }
    }
}