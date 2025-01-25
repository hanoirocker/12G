namespace TwelveG.PlayerController
{
    using TwelveG.InteractableObjects;
    using TwelveG.Localization;
    using TwelveG.UIManagement;
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

        private LocalizationData localizationData;
        private bool canvasIsShowing;
        private string canvasText;

        private void Awake() 
        {
            localizationData = GetComponentInParent<LocalizationData>();
        }

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
                    var canvasText = itemObj.GetItemText(localizationData.CurrentLanguage);
                    ShowUI(canvasText);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        // Retorna el enum ItemType del objeto con el que se interactua
                        // para luego usarlo en el inventario y agregarlo a la lista de objetos
                        // recolectados (pickedUpItems)
                        ItemType itemType = itemObj.GetItemType();
                        // Destruir el prefab en escena (prefab tipo pickable)
                        itemObj.TakeItem();
                        // Agregar item a la lista de items del jugador y habilitarlo para su uso
                        playerInventory.AddItem(itemType);
                        //
                        ChangeUI(itemObj);
                    }
                }
            }
            else
            {
                HideUI();
            }
        }

        private void ShowUI(string givenText)
        {
            if (!canvasIsShowing)
            {
                onInteractionCanvasShowText.Raise(this, givenText);
                canvasIsShowing = true;
            }
        }

        private void ChangeUI(IItem itemObj)
        {
            canvasText = itemObj.GetItemText(localizationData.CurrentLanguage);
            onInteractionCanvasShowText.Raise(this, canvasText);
        }

        private void HideUI()
        {
            // Hide the canvas if not looking at an item object
            if (canvasIsShowing)
            {
                onInteractionCanvasControls.Raise(this, "HideText");
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
