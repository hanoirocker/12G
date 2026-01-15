using TMPro;
using TwelveG.InteractableObjects;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.UIController
{
    public class ItemCanvasHandler : MonoBehaviour
    {
        [Header("References")]
        [Space]
        [SerializeField] private GameObject iconsPanel;
        [SerializeField] private GameObject alertPanel;
        [SerializeField] private TextMeshProUGUI alertText;
        [SerializeField] private GameObject WTImageObject;
        [SerializeField] private GameObject FlashlightImageObject;

        [Header("Testing Settings")]
        [Space]
        [SerializeField, Range(0.5f, 1f)] public float equippedAlpha = 0.8f;

        private Canvas itemCanvas;
        private float unequipiedItemAlpha = 0.15f;

        private void Awake()
        {
            itemCanvas = GetComponent<Canvas>();
        }

        public void ToggleItemAlpha(Component sender, object data)
        {
            // Recibe onItemToggled event SO con el bool enviado en él
            string itemObject = sender.gameObject.name;
            bool itemIsShown = (bool)data;

            if (itemObject == ItemType.WalkieTalkie.ToString())
            {
                WTImageObject.GetComponent<CanvasGroup>().alpha = itemIsShown ? equippedAlpha : unequipiedItemAlpha;
            }
            else if (itemObject == ItemType.Flashlight.ToString())
            {
                FlashlightImageObject.GetComponent<CanvasGroup>().alpha = itemIsShown ? equippedAlpha : unequipiedItemAlpha;
            }
        }

        public void HideAlertPanel()
        {
            alertText.text = "";
            alertPanel.SetActive(false);
        }

        public void IncomingDialogAlert(Component sender, object data)
        {
            alertPanel.SetActive((bool)data);
        }

        public void EnablePlayerItem(Component sender, object data)
        {
            if (iconsPanel.activeSelf == false)
            {
                iconsPanel.SetActive(true);
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

            itemCanvas.enabled = true;
        }

        public void HandlePlayerDeath()
        {
            WTImageObject.SetActive(false);
            WTImageObject.GetComponent<CanvasGroup>().alpha = unequipiedItemAlpha;
            FlashlightImageObject.SetActive(false);
            FlashlightImageObject.GetComponent<CanvasGroup>().alpha = unequipiedItemAlpha;
            iconsPanel.SetActive(false);
            itemCanvas.enabled = false;
        }

        public void RemovePlayerItem(Component sender, object data)
        {
            Debug.Log($"ItemCanvasHandler: RemovePlayerItem called from {sender.name} with data {data}");
            ItemType itemType = (ItemType)data;

            if (itemType == ItemType.WalkieTalkie)
            {
                WTImageObject.SetActive(false);
            }
            else if (itemType == ItemType.Flashlight)
            {
                FlashlightImageObject.SetActive(false);
            }

            if (!WTImageObject.activeSelf && !FlashlightImageObject.activeSelf)
            {
                iconsPanel.SetActive(false);
                itemCanvas.enabled = false;
            }
        }
    }
}