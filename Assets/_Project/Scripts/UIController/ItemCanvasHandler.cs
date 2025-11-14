namespace TwelveG.UIController
{
    using TwelveG.InteractableObjects;
    using UnityEngine;

    public class ItemCanvasHandler : MonoBehaviour
    {
        [Header("References")]

        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject WTImageObject;
        [SerializeField] private GameObject FlashlightImageObject;

        private float unequipiedItemAlpha = 0.15f;

        private Canvas itemsCanvas;

        private void Awake()
        {
            itemsCanvas = GetComponent<Canvas>();
        }

        public void ToggleItemAlpha(Component sender, object data)
        {
            string itemObject = sender.gameObject.name;
            bool itemIsShown = (bool)data;

            if (itemObject == ItemType.WalkieTalkie.ToString())
            {
                WTImageObject.GetComponent<CanvasGroup>().alpha = itemIsShown? 1 : unequipiedItemAlpha;
            }
            else if (itemObject == ItemType.Flashlight.ToString())
            {
                FlashlightImageObject.GetComponent<CanvasGroup>().alpha = itemIsShown? 1 : unequipiedItemAlpha;
            }
        }

        public void EnablePlayerItem(Component sender, object data)
        {
            Debug.Log($"[ItemCanvasHandler]: Habilitando item en canvas: {(ItemType)data}");

            if (panel.activeSelf == false)
            {
                panel.SetActive(true);
            }

            ItemType itemType = (ItemType)data;
            if (itemType == ItemType.WalkieTalkie)
            {
                WTImageObject.SetActive(true);
            }
            else if (itemType == ItemType.Flashlight)
            {
                FlashlightImageObject.SetActive(true);
            }
        }
    }
}