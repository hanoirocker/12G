namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EventDialogTexts", menuName = "SO's/Event Texts/EventsDialogsTextsSO", order = 0)]
    public class EventsDialogsTextsSO : ScriptableObject
    {
        public string eventName;
        public List<EventFallbackStructure> eventsFallbackTexts;

        [System.Serializable]
        public class EventFallbackStructure
        {
            public LanguagesEnum language;
            public List<string> dialogs;
        }
    }
}