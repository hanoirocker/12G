namespace TwelveG.UIManagement
{
    using TMPro;
    using TwelveG.Localization;
    using UnityEngine;

    public class ControlCanvasHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI options;

        private EventsControlCanvasInteractionTextSO lastEventControlCanvasInteractionTextSORecieved = null;
        private Canvas controlCanvas;
        private string defaultOptionText = "";

        void Awake()
        {
            controlCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            HideControlCanvas();
        }

        private void ToogleControlCanvas()
        {
            controlCanvas.enabled = !controlCanvas.enabled;
        }

        private void ShowControlCanvas()
        {
            controlCanvas.enabled = true;
        }

        private void HideControlCanvas()
        {
            controlCanvas.enabled = false;
        }

        private void DeactivateControlCanvas()
        {
            controlCanvas.gameObject.SetActive(false);
        }

        private void ActivateControlCanvas()
        {
            controlCanvas.gameObject.SetActive(true);
        }

        public void SetInteractionOptions(Component sender, object data)
        {
            if (data != null)
            {
                lastEventControlCanvasInteractionTextSORecieved = (EventsControlCanvasInteractionTextSO)data;
                string actualLanguageCode = GetComponentInParent<LocalizationData>().CurrentLanguage;
                print("actualLanguageCode" + actualLanguageCode);

                string textToShow = Utils.TextFunctions.RetrieveEventControlCanvasInteractionsText(
                    actualLanguageCode,
                    lastEventControlCanvasInteractionTextSORecieved
                );
                options.text = textToShow;
            }
        }

        private void ResetEventControlsOptions()
        {
            options.text = defaultOptionText;
        }

        public void UpdateTextOnLanguageChanged(Component sender, object languageCode)
        {
            string textToShow = Utils.TextFunctions.RetrieveEventControlCanvasInteractionsText(
                (string)languageCode,
                lastEventControlCanvasInteractionTextSORecieved
            );
            options.text = textToShow;
        }

        public void ControlCanvasControls(Component sender, object data)
        {
            if ((string)data == "DeactivateControlCanvas")
            {
                DeactivateControlCanvas();
            }
            else if ((string)data == "ActivateControlCanvas")
            {
                ActivateControlCanvas();
            }
            else if ((string)data == "ShowControlCanvas")
            {
                ShowControlCanvas();
            }
            else if ((string)data == "ToogleControlCanvas")
            {
                ToogleControlCanvas();
            }
            else if ((string)data == "ResetEventControlsOptions")
            {
                ResetEventControlsOptions();
            }
            else if ((string)data == "HideControlCanvas")
            {
                HideControlCanvas();
            }
        }
    }
}