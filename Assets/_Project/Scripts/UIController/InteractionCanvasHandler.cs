using TMPro;
using TwelveG.Localization;
using UnityEngine;

namespace TwelveG.UIController
{
    public class InteractionCanvasHandler : MonoBehaviour
    {
        private TextMeshProUGUI interactionCanvasText;
        private Canvas interactionCavas;
        private EventsInteractionTextsSO lastEventInteractionTextSORecieved = null;

        private void Awake()
        {
            interactionCavas = GetComponent<Canvas>();
            interactionCanvasText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            UpdateCanvasTextOnLanguageChanged();
        }

        private void OnDisable()
        {
            interactionCavas.enabled = false;
        }

        private void Start()
        {
            interactionCavas.enabled = false;
        }

        public bool IsShowingText()
        {
            return interactionCavas.isActiveAndEnabled;
        }

        public void ShowInteractionText(InteractionTextSO interactionTextSO)
        {
            if (interactionTextSO != null)
            {
                string textToShow = Utils.TextFunctions.RetrieveInteractionText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    interactionTextSO
                );
                interactionCanvasText.text = textToShow;
                interactionCavas.enabled = true;
            }
        }

        public void ShowEventInteractionText(EventsInteractionTextsSO eventsInteractionTextsSO)
        {
            if (eventsInteractionTextsSO != null)
            {
                lastEventInteractionTextSORecieved = eventsInteractionTextsSO;
                string textToShow = Utils.TextFunctions.RetrieveEventInteractionText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    lastEventInteractionTextSORecieved
                );
                interactionCanvasText.text = textToShow;
                interactionCavas.enabled = true;
            }
        }

        public void UpdateCanvasTextOnLanguageChanged()
        {
            if (lastEventInteractionTextSORecieved != null && interactionCavas.isActiveAndEnabled)
            {
                interactionCavas.enabled = false;
                string textToShow = Utils.TextFunctions.RetrieveEventInteractionText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    lastEventInteractionTextSORecieved
                );
                interactionCanvasText.text = textToShow;
                interactionCavas.enabled = true;
            }
        }

        // TODO: Refactor to use HideInteractionText in all places
        public void HideInteractionText()
        {
            interactionCavas.enabled = false;
        }

        // TODO: Refactor to use HideInteractionText in all places
        public void VanishTextEffect()
        {
            StartCoroutine(UIUtils.VanishTextEffectCoroutine(interactionCanvasText, interactionCavas));
        }

        public void ChangeText(string textGiven)
        {
            interactionCanvasText.text = textGiven;
        }
    }
}