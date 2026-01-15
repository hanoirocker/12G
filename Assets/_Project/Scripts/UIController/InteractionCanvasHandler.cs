using TMPro;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.UIController
{
    public class InteractionCanvasHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI interactionCanvasText;
        private Canvas interactionCavas;

        private InteractionTextSO currentSimpleTextSO;
        private EventsInteractionTextsSO currentEventTextSO;
        private bool isShowingEventText = false;

        private void Awake()
        {
            interactionCavas = GetComponent<Canvas>();
            if (interactionCanvasText == null) interactionCanvasText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            interactionCavas.enabled = false;
            UIManager.Instance.UIFormatter.AssignFontByType(UIFormatingType.PlayerInteractionText, interactionCanvasText);
        }

        private void OnEnable()
        {
            UpdateCanvasText();
        }

        private void OnValidate()
        {
            if (Application.isPlaying && isActiveAndEnabled && interactionCavas.enabled)
            {
                UpdateCanvasText();
                UIManager.Instance.UIFormatter.AssignFontByType(UIFormatingType.PlayerInteractionText, interactionCanvasText);
            }
        }

        public void ShowInteractionText(InteractionTextSO interactionTextSO)
        {
            if (interactionTextSO == null) return;

            currentSimpleTextSO = interactionTextSO;
            currentEventTextSO = null;
            isShowingEventText = false;

            UpdateCanvasText();

            interactionCavas.enabled = true;
        }

        public void ShowEventInteractionText(EventsInteractionTextsSO eventsInteractionTextsSO)
        {
            if (eventsInteractionTextsSO == null) return;

            currentEventTextSO = eventsInteractionTextsSO;
            currentSimpleTextSO = null;
            isShowingEventText = true;

            UpdateCanvasText();

            interactionCavas.enabled = true;
        }

        public void HideInteractionText()
        {
            interactionCavas.enabled = false;
        }

        public void UpdateCanvasTextOnLanguageChanged()
        {
            if (interactionCavas.isActiveAndEnabled)
            {
                UpdateCanvasText();
            }
        }

        private void UpdateCanvasText()
        {
            if (LocalizationManager.Instance == null || UIManager.Instance == null) return;

            string rawText = "";
            if (isShowingEventText && currentEventTextSO != null)
            {
                rawText = Utils.TextFunctions.RetrieveEventInteractionText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    currentEventTextSO
                );
            }
            else if (!isShowingEventText && currentSimpleTextSO != null)
            {
                rawText = Utils.TextFunctions.RetrieveInteractionText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    currentSimpleTextSO
                );
            }

            if (string.IsNullOrEmpty(rawText)) return;

            string formattedText = UIManager.Instance.UIFormatter.UpdateTextColors(
                rawText,
                UIFormatingType.PlayerInteractionText,
                this.gameObject
            );

            interactionCanvasText.text = formattedText;
        }

        public void VanishTextEffect()
        {
            StartCoroutine(UIUtils.VanishTextEffectCoroutine(interactionCanvasText, interactionCavas));
        }

        public void ChangeText(string textGiven)
        {
            interactionCanvasText.text = textGiven;
        }

        public bool IsShowingText()
        {
            return interactionCavas.isActiveAndEnabled;
        }
    }
}