namespace TwelveG.Utils
{
    using TwelveG.Localization;

    public static class TextFunctions
    {
        public static float CalculateTextDisplayDuration(string text)
        {
            float reason = 14;
            if(text.Length < 10) { reason = 4; }
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
            string languageCode,
            int currentContemplationIndex,
            ContemplationTextSO contemplationTextsSO
            )
        {
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

            if (currentContemplationIndex >= languageStructure.contemplationTexts.Count)
            {
                hasReachedMaxContemplations = true;
            }

            return (contemplationTextToRetrieve, hasReachedMaxContemplations, currentContemplationIndex);
        }

        public static string RetrieveInteractableFallbackText(string languageCode, int index, InteractableFallbackTextsSO interactableFallbackTextsSO)
        {
            if (interactableFallbackTextsSO == null)
            {
                return "interactableFallbackTextsSO is null";
            }

            // Encuentra el lenguaje correspondiente usando el código de idioma proporcionado
            var languageStructure = interactableFallbackTextsSO.interactableFallbackStructures
                .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (languageStructure == null)
            {
                return "Language not found in languageStructure for interactableFallbackStructures";
            }

            string interactableFallbackTextToRetrieve = languageStructure.interactableFallbacksTexts[index];

            return interactableFallbackTextToRetrieve;
        }

        public static string RetrieveEventObservationText(string languageCode, int index, EventsObservationsTextsSO eventsObservationsTextsSO)
        {
            if (eventsObservationsTextsSO == null)
            {
                return "eventsObservationsTextsSO is null";
            }

            // Encuentra el lenguaje correspondiente usando el código de idioma proporcionado
            var languageStructure = eventsObservationsTextsSO.eventsObservations
                .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (languageStructure == null)
            {
                return ("Language not found in languageStructure for eventsObservationsTextsSO");
            }

            string eventContemplationTextToRetrieve = languageStructure.eventObservationTexts[index];

            return eventContemplationTextToRetrieve;
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