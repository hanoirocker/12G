namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "EventsControlCanvasInteractionTextSO", menuName = "SO's/Event Texts/EventsControlCanvasInteractionTextSO", order = 0)]
    public class EventsControlCanvasInteractionTextSO : ScriptableObject
    {
        public string eventName;
        public List<EventControlCanvasInteractionStructure> eventControlCanvasInteractionStructures;

        [System.Serializable]
        public class EventControlCanvasInteractionStructure
        {
            public LanguagesEnum language;
            [TextArea(3, 10)]
            public string eventControlCanvasInteractionText;
        }
    }
}