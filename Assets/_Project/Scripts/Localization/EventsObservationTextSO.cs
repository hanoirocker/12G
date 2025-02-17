namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EventObservationText", menuName = "SO's/Event Texts/EventsObservationTextSO", order = 0)]
    public class EventsObservationTextSO : ScriptableObject
    {
        public List<EventObservationStructure> eventsObservationTexts;

        [System.Serializable]
        public class EventObservationStructure
        {
            public LanguagesEnum language;
            [TextArea(5, 50)]
            public string eventObservationText;
        }
    }
}