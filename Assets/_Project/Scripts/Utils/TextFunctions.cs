namespace TwelveG.Utils
{
  using TwelveG.DialogsController;
  using TwelveG.Localization;

    public static class TextFunctions
    {
        public static float CalculateTextDisplayDuration(string text)
        {
            float reason = 13;
            if (text.Length < 10) { reason = 4; }
            float calculatedTime = (text.Length) / reason;
            return calculatedTime;
        }

        public static string RetrieveInteractionText(string languageCode, InteractionTextSO interactionTextsSO)
        {
            if (interactionTextsSO == null)
            {
                return "InteractionTextSO is null";
            }

            var languageStructure = interactionTextsSO.interactionTextsStructure
                .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (languageStructure == null)
            {
                return "Language not found in InteractionTextSO";
            }

            return languageStructure.interactionText;
        }

        public static string RetrieveDialogText(string languageCode, DialogSO dialogSO)
        {
            if (dialogSO == null)
            {
                return "DialogSO is null";
            }

            var languageStructure = dialogSO.dialogTextStructure
                .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (languageStructure == null)
            {
                return "Language not found in DialogSO";
            }

            return languageStructure.dialogText;
        }

        public static string RetrieveDialogOptions(string languageCode, DialogOptions dialogOptions)
        {
            if (dialogOptions == null)
            {
                return "DialogOptions is null";
            }

            var languageStructure = dialogOptions.nextDialog.dialogTextStructure
                .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (languageStructure == null)
            {
                return "Language not found in DialogSO";
            }

            // Acorta el texto conseguido por una version reducida a las primeras 6 palabras mas "..."
            string optiontext = languageStructure.dialogText;
            string[] words = languageStructure.dialogText.Split(' ');

            if (words.Length <= 6)
            {
                return optiontext;
            }
            else
            {
                string shortenedText = string.Join(" ", words, 0, 6) + " ...";
                return shortenedText;
            }
        }

        public static string RetrieveEventControlCanvasInteractionsText(string languageCode, EventsControlCanvasInteractionTextSO eventsControlCanvasInteractionTextSO)
        {
            if (eventsControlCanvasInteractionTextSO == null)
            {
                return "eventsControlCanvasInteractionTextSO is null";
            }

            var languageStructure = eventsControlCanvasInteractionTextSO.eventControlCanvasInteractionStructures
                .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (languageStructure == null)
            {
                return "Language not found in eventControlCanvasInteractionStructures";
            }

            return languageStructure.eventControlCanvasInteractionText;
        }

        public static (string contemplationText, bool hasReachedMaxContemplations, int updatedIndex)
        RetrieveContemplationText(
            int currentContemplationIndex,
            bool repeatTexts,
            ContemplationTextSO contemplationTextsSO
        )
        {
            string languageCode = LocalizationManager.Instance.GetCurrentLanguageCode();
            if (contemplationTextsSO == null)
            {
                return ("ContemplationTextsSO is null", false, currentContemplationIndex);
            }

            var hasReachedMaxContemplations = false;

            // Encuentra el lenguaje correspondiente usando el código de idioma proporcionado
            var languageStructure = contemplationTextsSO.contemplationTextsStructure
                .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (languageStructure == null)
            {
                return ("Language not found in ContemplationTextsSO", false, currentContemplationIndex);
            }

            string contemplationTextToRetrieve = languageStructure.contemplationTexts[currentContemplationIndex];

            if (currentContemplationIndex + 1 <= languageStructure.contemplationTexts.Count)
            {
                currentContemplationIndex += 1;
            }

            if (repeatTexts && currentContemplationIndex >= languageStructure.contemplationTexts.Count)
            {
                return (contemplationTextToRetrieve, false, 0);
            }

            if (currentContemplationIndex >= languageStructure.contemplationTexts.Count)
            {
                hasReachedMaxContemplations = true;
            }

            return (contemplationTextToRetrieve, hasReachedMaxContemplations, currentContemplationIndex);
        }

        public static string RetrieveObservationText(string languageCode, ObservationTextSO observationTextSO)
        {
            if (observationTextSO == null)
            {
                return "observationTextSO is null";
            }

            // Encuentra el lenguaje correspondiente usando el código de idioma proporcionado
            var languageStructure = observationTextSO.observationTextsStructure
                .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (languageStructure == null)
            {
                return ("Language not found in languageStructure for observationTextsStructure");
            }

            string observationTextToRetrieve = languageStructure.observationText;

            return observationTextToRetrieve;
        }

        public static string RetrieveEventInteractionText(string languageCode, EventsInteractionTextsSO eventsInteractionTextsSO)
        {
            if (eventsInteractionTextsSO == null)
            {
                return "eventsInteractionTextsSO is null";
            }

            var languageStructure = eventsInteractionTextsSO.eventsInteractionTexts
                .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (languageStructure == null)
            {
                return ("Language not found in languageStructure for eventsInteractionTextsSO");
            }

            string eventInteractionTextToRetrieve = languageStructure.eventsInteractionsText;

            return eventInteractionTextToRetrieve;
        }

        public static string RetrieveExaminationText(string languageCode, ExaminationTextSO examinationTextSO)
        {
            if (examinationTextSO == null)
            {
                return "examinationTextSO is null";
            }

            var languageStructure = examinationTextSO.examinationTextStructure
                .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (languageStructure == null)
            {
                return ("Language not found in languageStructure for examinationTextSO");
            }

            string examinationTextToRetrieve = languageStructure.examinationText;

            return examinationTextToRetrieve;
        }

        public static (string fallbackText, bool hasReachedMaxContemplations, int updatedIndex)
        RetrieveEventFallbackText(
            string languageCode,
            int currentFallbackTextIndex,
            EventsFallbacksTextsSO eventsFallbacksTextsSO
            )
        {
            if (eventsFallbacksTextsSO == null)
            {
                return ("eventsFallbacksTextsSO is null", false, currentFallbackTextIndex);
            }

            var hasReachedMaxContemplations = false;

            // Encuentra el lenguaje correspondiente usando el código de idioma proporcionado
            var languageStructure = eventsFallbacksTextsSO.eventsFallbackTexts
                .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (languageStructure == null)
            {
                return ("Language not found in ContemplationTextsSO", false, currentFallbackTextIndex);
            }

            string eventFallbackTextToRetrieve = languageStructure.eventsFallbacksTexts[currentFallbackTextIndex];

            if (currentFallbackTextIndex + 1 <= languageStructure.eventsFallbacksTexts.Count)
            {
                currentFallbackTextIndex += 1;
            }

            if (currentFallbackTextIndex >= languageStructure.eventsFallbacksTexts.Count)
            {
                hasReachedMaxContemplations = true;
            }

            return (eventFallbackTextToRetrieve, hasReachedMaxContemplations, currentFallbackTextIndex);
        }
    }
}