namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EventObservationsText", menuName = "SO's/Event Texts/EventsObservationsTextsSO", order = 0)]
    public class EventsObservationsTextsSO : ScriptableObject
    {
        public List<EventObservationsStructure> eventsObservations;

        [System.Serializable]
        public class EventObservationsStructure
        {
            public LanguagesEnum language;
            public List<string> eventObservationTexts;
        }
    }
}