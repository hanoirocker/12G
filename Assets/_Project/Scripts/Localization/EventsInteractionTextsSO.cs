namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EventInteractionText", menuName = "SO's/Event Texts/EventsInteractionTextsSO", order = 0)]
    public class EventsInteractionTextsSO : ScriptableObject
    {
        public string eventName;
        public List<EventInteractionsStructure> eventsInteractionTexts;

        [System.Serializable]
        public class EventInteractionsStructure
        {
            public LanguagesEnum language;
            public string eventsInteractionsText;
        }
    }
}