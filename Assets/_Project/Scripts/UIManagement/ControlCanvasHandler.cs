namespace TwelveG.UIManagement
{
    using TMPro;
    using TwelveG.Localization;
    using UnityEngine;

    public class ControlCanvasHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI controlsHeadTitle;
        [SerializeField] private TextMeshProUGUI defaultOptions;
        [SerializeField] private TextMeshProUGUI specificOptions;

        [Header("Text SO")]
        [SerializeField] private EventsControlCanvasInteractionTextSO defaultOptionsTextsSO;
        [SerializeField] private EventsControlCanvasInteractionTextSO headTitleTextSO;

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

        public void SetInteractionSpecificOptions(Component sender, object data)
        {
            if (data != null)
            {
                lastEventControlCanvasInteractionTextSORecieved = (EventsControlCanvasInteractionTextSO)data;
                string textToShow = Utils.TextFunctions.RetrieveEventControlCanvasInteractionsText(
                    GetComponentInParent<UIHandler>().CurrentLanguage,
                    lastEventControlCanvasInteractionTextSORecieved
                );
                specificOptions.text = textToShow;
            }
        }

        private void ResetControlCanvasSpecificOptions()
        {
            specificOptions.text = defaultOptionText;
            // Previene que si el jugador cambia de idioma luego de eliminar las opciones extra
            // aún se pueda imprimir lo último recibido en base al SO.
            lastEventControlCanvasInteractionTextSORecieved = null;
        }

        public void UpdateCanvasTextOnLanguageChanged()
        {
            string newLanguageSet = GetComponentInParent<UIHandler>().CurrentLanguage;

            string updatedHeadTitleText = Utils.TextFunctions.RetrieveEventControlCanvasInteractionsText(
                newLanguageSet,
                headTitleTextSO
            );
            controlsHeadTitle.text = updatedHeadTitleText;

            // Actualiza las opciones basicas del componente
            string updatedDefaultOptionsText = Utils.TextFunctions.RetrieveEventControlCanvasInteractionsText(
                newLanguageSet,
                defaultOptionsTextsSO
            );
            defaultOptions.text = updatedDefaultOptionsText;

            // Actualiza las opciones específicas envidas en el último
            // EventsControlCanvasInteractionTextSO recibido y guardado.
            // Si no se envió el evento pero se inició la escena y se ejecutó el seteo de lenguaje, retorna.
            if(lastEventControlCanvasInteractionTextSORecieved == null) { return; }

            string updatedSpecificOptionsText = Utils.TextFunctions.RetrieveEventControlCanvasInteractionsText(
                newLanguageSet,
                lastEventControlCanvasInteractionTextSORecieved
            );
            specificOptions.text = updatedSpecificOptionsText;
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
            else if ((string)data == "ResetControlCanvasSpecificOptions")
            {
                ResetControlCanvasSpecificOptions();
            }
            else if ((string)data == "HideControlCanvas")
            {
                HideControlCanvas();
            }
        }
    }
}