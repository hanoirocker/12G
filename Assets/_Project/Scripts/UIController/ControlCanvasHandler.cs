namespace TwelveG.UIController
{
    using System.Collections;
    using TMPro;
    using TwelveG.GameController;
    using TwelveG.Localization;
    using TwelveG.PlayerController;
    using UnityEngine;

    public class ControlCanvasHandler : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI controlsHeadTitle;
        [SerializeField] private TextMeshProUGUI defaultOptions;
        [SerializeField] private TextMeshProUGUI specificOptions;

        [Header("Settings")]
        [SerializeField, Range(0f, 3f)] private float fadeDuration = 3f;

        [Header("Text SO")]
        [SerializeField] private EventsControlCanvasInteractionTextSO defaultOptionsTextsSO;
        [SerializeField] private EventsControlCanvasInteractionTextSO headTitleTextSO;

        private EventsControlCanvasInteractionTextSO lastEventControlCanvasInteractionTextSORecieved = null;
        private Canvas controlCanvas;
        private string defaultOptionText = "";

        void Awake()
        {
            controlCanvas = GetComponent<Canvas>();
            controlCanvas.enabled = false;
        }

        private void OnEnable()
        {
            UpdateCanvasTextOnLanguageChanged();
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
            string newLanguageSet = LocalizationManager.Instance?.GetCurrentLanguageCode();

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
                case EnableCanvas cmd:
                    StartCoroutine(ToggleControlCanvasCoroutine(cmd.Enabled));
                    break;
                case AlternateCanvasCurrentState:
                    StartCoroutine(ToggleControlCanvasCoroutine(!controlCanvas.enabled));
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

        private IEnumerator ToggleControlCanvasCoroutine(bool enableCanvas)
        {
            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(false));

            if (enableCanvas)
            {
                controlCanvas.enabled = true;
            }

            float elapsed = 0f;
            float target = enableCanvas ? 1f : 0f;
            float initialAlpha = canvasGroup.alpha;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(initialAlpha, target, elapsed / fadeDuration);
                canvasGroup.alpha = alpha;
                yield return null;
            }

            canvasGroup.alpha = target;

            if (!enableCanvas)
            {
                controlCanvas.enabled = false;
            }

            GameEvents.Common.onPlayerControls.Raise(this, new EnableControlCanvasAccess(true));
        }
    }
}