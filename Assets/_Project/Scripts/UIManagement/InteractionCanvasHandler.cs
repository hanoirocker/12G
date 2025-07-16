namespace TwelveG.UIManagement
{
    using System.Collections;
    using TMPro;
    using TwelveG.Localization;
    using UnityEngine;

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

        private void Start()
        {
            interactionCavas.enabled = false;
        }

        public bool IsShowingText()
        {
            return interactionCavas.isActiveAndEnabled;
        }

        public void ShowInteractionText(Component sender, object data)
        {
            if (data != null)
            {
                string actualLanguageCode = GetComponentInParent<UIHandler>().CurrentLanguage;
                string textToShow = Utils.TextFunctions.RetrieveInteractionText(
                    actualLanguageCode,
                    (InteractionTextSO)data
                );
                interactionCanvasText.text = textToShow;
                interactionCavas.enabled = true;
            }
        }

        public void ShowEventInteractionText(Component sender, object data)
        {
            if (data != null)
            {
                lastEventInteractionTextSORecieved = (EventsInteractionTextsSO)data;
                string textToShow = Utils.TextFunctions.RetrieveEventInteractionText(
                    GetComponentInParent<UIHandler>().CurrentLanguage,
                    lastEventInteractionTextSORecieved
                );
                interactionCanvasText.text = textToShow;
                interactionCavas.enabled = true;
            }
        }

        public void UpdateCanvasTextOnLanguageChanged()
        {
            if (lastEventInteractionTextSORecieved != null & interactionCavas.isActiveAndEnabled)
            {
                interactionCavas.enabled = false;
                string textToShow = Utils.TextFunctions.RetrieveEventInteractionText(
                    GetComponentInParent<UIHandler>().CurrentLanguage,
                    lastEventInteractionTextSORecieved
                );
                interactionCanvasText.text = textToShow;
                interactionCavas.enabled = true;
            }
        }

        public void InteractionCanvasControls(Component sender, object data)
        {
            switch (data)
            {
                case HideText:
                    interactionCavas.enabled = false;
                    break;
                case VanishTextEffect:
                    StartCoroutine(VanishTextEffectCoroutine());
                    break;
                default:
                    Debug.LogWarning($"[InteractionCanvasHandler] Received unknown command: {data}");
                    break;
            }
        }


        private IEnumerator VanishTextEffectCoroutine()
        {
            float duration = 2f;

            float startAlpha = 1;
            float endAlpha = 0f;

            float startFontSize = interactionCanvasText.fontSize;
            float maxFontSize = interactionCanvasText.fontSize * 1.25f;
            float textElapsedDuration = 0.15f * duration;

            float totalElapsedTime = 0f;
            while (totalElapsedTime < duration)
            {
                if (totalElapsedTime < textElapsedDuration)
                {
                    interactionCanvasText.fontSize = Mathf.Lerp(startFontSize, maxFontSize, totalElapsedTime / textElapsedDuration);
                }
                else
                {
                    interactionCanvasText.fontSize = Mathf.Lerp(maxFontSize, startFontSize, totalElapsedTime / duration);
                }

                interactionCanvasText.alpha = Mathf.Lerp(startAlpha, endAlpha, totalElapsedTime / duration);
                totalElapsedTime += Time.deltaTime;
                yield return null;
            }

            interactionCanvasText.alpha = endAlpha;
            interactionCanvasText.text = "";
            interactionCavas.enabled = false;
        }

        public void ChangeText(string textGiven)
        {
            interactionCanvasText.text = textGiven;
        }
    }
}