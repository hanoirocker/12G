namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EventFallbackText", menuName = "SO's/Event Texts/EventsFallbackTextsSO", order = 0)]
    public class EventsFallbacksTextsSO : ScriptableObject
    {
        public string eventName;
        public List<EventFallbackStructure> eventsFallbackTexts;

        [System.Serializable]
        public class EventFallbackStructure
        {
            public LanguagesEnum language;
            public List<string> eventsFallbacksTexts;
        }
    }
}