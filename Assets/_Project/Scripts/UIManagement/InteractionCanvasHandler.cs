namespace TwelveG.UIManagement
{
    using System.Collections;
    using TMPro;
    using UnityEngine;

    public class InteractionCanvasHandler : MonoBehaviour
    {
        private TextMeshProUGUI interactionCanvasText;
        private Canvas interactionCavas;

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
            if((string)data != null)
            {
                interactionCavas.enabled = true;
                interactionCanvasText.text = (string)data;
            }
        }

        public void InteractionCanvasControls(Component sender, object data)
        {
            if((string)data == "HideText")
            {
                HideText();
            }
            if((string)data == "VanishTextEffect")
            {
                StartCoroutine(VanishTextEffect());
            }
        }

        private void HideText()
        {
            interactionCavas.enabled = false;
        }

        private IEnumerator VanishTextEffect()
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
            interactionCavas.enabled = false;
        }

        public void ChangeText(string textGiven)
        {
            interactionCanvasText.text = textGiven;
        }
    }
}