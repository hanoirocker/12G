namespace TwelveG.PlayerController
{
    using TwelveG.InteractableObjects;
    using TwelveG.Localization;
    using TwelveG.UIController;
    using UnityEngine;

    public class PlayerAddItem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] PlayerInventory playerInventory;

        [Header("Raycast settings")]
        [SerializeField] private LayerMask addItemMask;
        [SerializeField, Range(0.5f, 2f)] private float addItemRange = 1.2f;
        public Transform interactorSource;
        public Color raycastColor;

        [Header("EventsSO references")]
        public GameEventSO onInteractionCanvasShowText;
        public GameEventSO onInteractionCanvasControls;

        private bool canvasIsShowing;
        private InteractionTextSO canvasText;

        private void Start()
        {
            canvasIsShowing = false;
        }

        private void Update()
        {
            Ray r = new Ray(interactorSource.position, interactorSource.forward);

            if (Physics.Raycast(r, out RaycastHit hitInfo, addItemRange, addItemMask))
            {
                HideUI();
                bool objectHasItemComponent = hitInfo.collider.gameObject.TryGetComponent(out IItem itemObj);

                if (objectHasItemComponent && itemObj.CanBePicked())
                {
                    canvasText = itemObj.RetrieveInteractionSO(null);
                    ShowUI(canvasText);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        ItemType itemType = itemObj.GetItemType();
                        // Destruir el prefab en escena (prefab tipo pickable)
                        itemObj.TakeItem();
                        // Agregar item a la lista de items del jugador y habilitarlo para su uso
                        playerInventory.AddItem(itemType);
                        ChangeUI(itemObj);
                    }
                }
            }
            else
            {
                HideUI();
            }
        }

        private void ShowUI(InteractionTextSO retrievedInteractionSO)
        {
            if (!canvasIsShowing)
            {
                onInteractionCanvasShowText.Raise(this, retrievedInteractionSO);
                canvasIsShowing = true;
            }
        }

        private void ChangeUI(IItem itemObj)
        {
            canvasText = itemObj.RetrieveInteractionSO(null);
            onInteractionCanvasShowText.Raise(this, canvasText);
        }

        private void HideUI()
        {
            // Hide the canvas if not looking at an item object
            if (canvasIsShowing)
            {
                onInteractionCanvasControls.Raise(this, new HideText());
                canvasIsShowing = false;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = raycastColor;
            Vector3 direction = interactorSource.TransformDirection(Vector3.forward) * addItemRange;
            Gizmos.DrawRay(interactorSource.position, direction);
        }
    }
}
