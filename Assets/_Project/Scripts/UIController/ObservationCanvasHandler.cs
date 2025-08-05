namespace TwelveG.UIController
{
    using System.Collections;
    using TMPro;
    using TwelveG.Localization;
    using UnityEngine;

    public class ObservationCanvasHandler : MonoBehaviour
    {
        private Canvas observationCanvas;
        private TextMeshProUGUI observationCanvasText;
        private ObservationTextSO lastObservationTextSORecieved = null;

        private void Awake()
        {
            observationCanvas = GetComponent<Canvas>();
            observationCanvasText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            UpdateCanvasTextOnLanguageChanged();
        }

        void Start()
        {
            observationCanvas.enabled = false;
        }

        public bool IsShowingText()
        {
            return observationCanvas.isActiveAndEnabled;
        }

        public void ShowObservationText(Component sender, object data)
        {
            if (data == null) { return; }

            StartCoroutine(ShowObservationTextCoroutine((ObservationTextSO)data));
        }

        public IEnumerator ShowObservationTextCoroutine(ObservationTextSO observationTextSO)
        {
            lastObservationTextSORecieved = observationTextSO;
            string textToShow = Utils.TextFunctions.RetrieveObservationText(
                LocalizationManager.Instance.GetCurrentLanguageCode(),
                lastObservationTextSORecieved
            );
            observationCanvasText.text = textToShow;
            observationCanvas.enabled = true;
            yield return new WaitForSeconds(Utils.TextFunctions.CalculateTextDisplayDuration(textToShow));
            observationCanvas.enabled = false;
        }

        public void UpdateCanvasTextOnLanguageChanged()
        {
            string textToShow = Utils.TextFunctions.RetrieveObservationText(
                LocalizationManager.Instance.GetCurrentLanguageCode(),
                lastObservationTextSORecieved
            );
            observationCanvasText.text = textToShow;
        }
    }

}