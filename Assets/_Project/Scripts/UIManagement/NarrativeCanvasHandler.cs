namespace TwelveG.UIManagement
{
    using System.Collections;
    using TMPro;
    using TwelveG.Localization;
    using UnityEngine;

    public class NarrativeCanvasHandler : MonoBehaviour
    {
        private TextMeshProUGUI narrativeCanvasText;
        private Canvas narrativeCanvas;
        private EventsInteractionTextsSO lastEventInteractionTextSORecieved = null;

        private void Awake()
        {
            narrativeCanvas = GetComponent<Canvas>();
            narrativeCanvasText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            narrativeCanvas.enabled = false;
        }

        public bool IsShowingText()
        {
            return narrativeCanvas.isActiveAndEnabled;
        }

        public void UpdateCanvasTextOnLanguageChanged()
        {
            if (lastEventInteractionTextSORecieved != null & narrativeCanvas.isActiveAndEnabled)
            {
                narrativeCanvas.enabled = false;
                string textToShow = Utils.TextFunctions.RetrieveEventInteractionText(
                    GetComponentInParent<UIHandler>().CurrentLanguage,
                    lastEventInteractionTextSORecieved
                );
                narrativeCanvasText.text = textToShow;
                narrativeCanvas.enabled = true;
            }
        }

        public void InteractionCanvasControls(Component sender, object data)
        {
            switch (data)
            {
                case HideText:
                    narrativeCanvas.enabled = false;
                    break;
                case VanishTextEffect:
                    StartCoroutine(VanishTextEffectCoroutine());
                    break;
                default:
                    Debug.LogWarning($"[NarrativeCanvasHandler] Received unknown command: {data}");
                    break;
            }
        }


        private IEnumerator VanishTextEffectCoroutine()
        {
            float duration = 2f;

            float startAlpha = 1;
            float endAlpha = 0f;

            float startFontSize = narrativeCanvasText.fontSize;
            float maxFontSize = narrativeCanvasText.fontSize * 1.25f;
            float textElapsedDuration = 0.15f * duration;

            float totalElapsedTime = 0f;
            while (totalElapsedTime < duration)
            {
                if (totalElapsedTime < textElapsedDuration)
                {
                    narrativeCanvasText.fontSize = Mathf.Lerp(startFontSize, maxFontSize, totalElapsedTime / textElapsedDuration);
                }
                else
                {
                    narrativeCanvasText.fontSize = Mathf.Lerp(maxFontSize, startFontSize, totalElapsedTime / duration);
                }

                narrativeCanvasText.alpha = Mathf.Lerp(startAlpha, endAlpha, totalElapsedTime / duration);
                totalElapsedTime += Time.deltaTime;
                yield return null;
            }

            narrativeCanvasText.alpha = endAlpha;
            narrativeCanvasText.text = "";
            narrativeCanvas.enabled = false;
        }

        public void ChangeText(string textGiven)
        {
            narrativeCanvasText.text = textGiven;
        }
    }
}