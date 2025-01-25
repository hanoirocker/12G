namespace TwelveG.UIManagement
{
    using System.Collections;
    using TMPro;
    using UnityEngine;

    public class ObservationCanvasHandler : MonoBehaviour
    {
        private Canvas observationCanvas;
        private TextMeshProUGUI observationCanvasText;

        private void Awake()
        {
            observationCanvas = GetComponent<Canvas>();
            observationCanvasText = GetComponentInChildren<TextMeshProUGUI>();
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
            StartCoroutine(ShowObservationTextCoroutine(data.ToString()));
        }

        public IEnumerator ShowObservationTextCoroutine(string fallbacktext)
        {
            observationCanvasText.text = "[ " + fallbacktext + " ]";
            observationCanvas.enabled = true;
            yield return new WaitForSeconds(Utils.TextFunctions.CalculateTextDisplayDuration(fallbacktext));
            observationCanvas.enabled = false;
        }
    }

}