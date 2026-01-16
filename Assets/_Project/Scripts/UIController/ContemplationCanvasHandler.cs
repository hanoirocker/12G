using System.Collections;
using TMPro;
using TwelveG.Utils;
using UnityEngine;

namespace TwelveG.UIController
{
    public class ContemplationCanvasHandler : MonoBehaviour
    {
        private Canvas contemplationCanvas;
        private TextMeshProUGUI contemplationCanvasText;

        private Coroutine currentDisplayCoroutine;

        private void Awake()
        {
            contemplationCanvas = GetComponent<Canvas>();
            contemplationCanvasText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            UIManager.Instance.UIFormatter.AssignFontByType(UIFormatingType.PlayerContemplationText, contemplationCanvasText);
        }

        void Start()
        {
            contemplationCanvas.enabled = false;
        }

        public IEnumerator ShowContemplationText(float delay, string contemplationText)
        {
            if (currentDisplayCoroutine != null) StopCoroutine(currentDisplayCoroutine);

            currentDisplayCoroutine = StartCoroutine(ShowTextSequence(delay, contemplationText));
            yield return currentDisplayCoroutine;
        }

        public void HideContemplationCanvas()
        {
            if (currentDisplayCoroutine != null) StopCoroutine(currentDisplayCoroutine);
            contemplationCanvas.enabled = false;
            currentDisplayCoroutine = null;
        }

        private IEnumerator ShowTextSequence(float delay, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                contemplationCanvas.enabled = false;
                contemplationCanvasText.text = text;

                if (delay > 0) yield return new WaitForSeconds(delay);

                contemplationCanvas.enabled = true;

                float displayDuration = TextFunctions.CalculateTextDisplayDuration(text);
                yield return new WaitForSeconds(displayDuration);

                contemplationCanvas.enabled = false;
                currentDisplayCoroutine = null;
            }
        }

        public bool IsShowingText()
        {
            return contemplationCanvas.isActiveAndEnabled;
        }
    }
}