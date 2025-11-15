namespace TwelveG.UIController
{
    using System.Collections;
    using TMPro;
    using TwelveG.InteractableObjects;
    using TwelveG.Localization;
    using UnityEngine;

    public class ItemCanvasHandler : MonoBehaviour
    {
        [Header("References")]
        [Space]
        [SerializeField] private GameObject iconsPanel;
        [SerializeField] private GameObject alertPanel;
        [SerializeField] private TextMeshProUGUI alertText;
        [SerializeField] private GameObject WTImageObject;
        [SerializeField] private GameObject FlashlightImageObject;

        [Header("Text SO's References")]
        [SerializeField] private InteractionTextSO acceptCallTextSO;

        [Header("Testing Settings")]
        [Space]
        [SerializeField, Range(0.5f, 1f)] public float equippedAlpha = 0.8f;

        private float unequipiedItemAlpha = 0.15f;

        public void ToggleItemAlpha(Component sender, object data)
        {
            // Recibe onItemToggled event SO con el bool enviado en él
            string itemObject = sender.gameObject.name;
            bool itemIsShown = (bool)data;

            if (itemObject == ItemType.WalkieTalkie.ToString())
            {
                alertText.text = "";
                alertText.gameObject.SetActive(false);
                alertPanel.SetActive(false);
                WTImageObject.GetComponent<CanvasGroup>().alpha = itemIsShown ? equippedAlpha : unequipiedItemAlpha;
            }
            else if (itemObject == ItemType.Flashlight.ToString())
            {
                FlashlightImageObject.GetComponent<CanvasGroup>().alpha = itemIsShown ? equippedAlpha : unequipiedItemAlpha;
            }
        }

        public void IncomingDialogAlert()
        {
            if (acceptCallTextSO != null)
            {
                string textToShow = Utils.TextFunctions.RetrieveInteractionText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    acceptCallTextSO
                );
                alertText.text = textToShow;
            }

            if (!alertPanel.activeSelf)
            {
                alertPanel.SetActive(true);
                alertText.gameObject.SetActive(true);
            }
        }

        public void EnablePlayerItem(Component sender, object data)
        {
            Debug.Log($"[ItemCanvasHandler]: Habilitando item en canvas: {(ItemType)data}");

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
        }
    }
}