namespace TwelveG.UIManagement
{
    using System.Collections;
    using TMPro;
    using TwelveG.Localization;
    using UnityEngine;

    public class NarrativeCanvasHandler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI phrase;
        [SerializeField] private CanvasGroup textCanvasGroup;
        [SerializeField] private CanvasGroup logoCanvasGroup;

        // TODO?: almacenar objeto NarrativeTextSO por si habilitamos el cambio de lenguage
        // mientras se muestra este canvas evento.

        [Header("Game Event SO's")]
        [SerializeField] private GameEventSO onFinishedNarrativeCouritine;

        public void ShowNarrativeCanvas(Component sender, object data)
        {


            if (data != null)
            {
                NarrativeTextSO narrativeTextSO = (NarrativeTextSO)data;
                string actualLanguageCode = GetComponentInParent<UIHandler>().CurrentLanguage;
                var languageStructure = narrativeTextSO.narrativeTextsStructure
                .Find(texts => texts.language.ToString().Equals(actualLanguageCode, System.StringComparison.OrdinalIgnoreCase));
                title.text = languageStructure.title;
                phrase.text = languageStructure.phrase;
                GetComponent<Canvas>().enabled = true;
                StartCoroutine(NarrativeCanvasCoroutine());
            }
        }

        private IEnumerator NarrativeCanvasCoroutine()
        {
            float startAlpha = 0f;
            float endAlpha = 1f;
            float elapsed = 0f;
            float duration = 1f;

            // Fade In
            while (elapsed < duration)
            {
                textCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            textCanvasGroup.alpha = endAlpha;

            // Esperar texto visible
            yield return new WaitForSeconds(8f);

            // Fade Out
            startAlpha = 1f;
            endAlpha = 0f;
            elapsed = 0f;
            duration = 3f;

            while (elapsed < duration)
            {
                textCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                logoCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            textCanvasGroup.alpha = endAlpha;
            logoCanvasGroup.alpha = endAlpha;

            // Desactivar canvas y emitir evento
            GetComponent<Canvas>().enabled  = false;
            onFinishedNarrativeCouritine.Raise(this, null);
        }
    }
}