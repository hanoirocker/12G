namespace TwelveG.UIController
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

        public void SetInteractionSpecificOptions(Component sender, object data)
        {
            if (data != null)
            {
                lastEventControlCanvasInteractionTextSORecieved = (EventsControlCanvasInteractionTextSO)data;
                string textToShow = Utils.TextFunctions.RetrieveEventControlCanvasInteractionsText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    lastEventControlCanvasInteractionTextSORecieved
                );
                specificOptions.text = textToShow;
            }
        }

        public void UpdateCanvasTextOnLanguageChanged()
        {
            string newLanguageSet = LocalizationManager.Instance.GetCurrentLanguageCode();

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
            if (lastEventControlCanvasInteractionTextSORecieved == null) { return; }

            string updatedSpecificOptionsText = Utils.TextFunctions.RetrieveEventControlCanvasInteractionsText(
                newLanguageSet,
                lastEventControlCanvasInteractionTextSORecieved
            );
            specificOptions.text = updatedSpecificOptionsText;
        }

        public void ControlCanvasControls(Component sender, object data)
        {
            switch (data)
            {
                case ActivateCanvas cmd:
                    controlCanvas.gameObject.SetActive(cmd.Activate);
                    break;
                case EnableCanvas cmd:
                    controlCanvas.enabled = cmd.Enabled;
                    break;
                case AlternateCanvasCurrentState:
                    controlCanvas.enabled = !controlCanvas.enabled;
                    break;
                case ResetControlCanvasSpecificOptions:
                    specificOptions.text = defaultOptionText;
                    // Previene que si el jugador cambia de idioma luego de eliminar las opciones extra
                    // aún se pueda imprimir lo último recibido en base al SO.
                    lastEventControlCanvasInteractionTextSORecieved = null;
                    break;
                default:
                    Debug.LogWarning($"[ControlCanvasHandler] Received unknown command: {data}");
                    break;
            }
        }
    }
}