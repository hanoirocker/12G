namespace TwelveG.UIController
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "DialogDataBase", menuName = "SO's/DialogDataBaseSO", order = 0)]
    public class DialogDataBaseSO : ScriptableObject
    {
        [SerializeField] private List<EventList> eventsList = null;

        public List<EventList> EventsList => eventsList;

        Dictionary<Characters, List<Dialogs>> lookupTable = null;

        [System.Serializable]
        public class Events
        {
            public Characters character;
            [TextArea(3, 10)]
            public string dialog;
        }

        [System.Serializable]
        public class Dialogs
        {
            public string dialog;
        }

        [System.Serializable]
        public class EventList
        {
            public string eventName; // To identify the event
            public List<Events> eventDialogs;
        }

        public enum Characters
        {
            Simon,
            Micaela
        }

        // Method to build the lookup table
        public void BuildLookupTable()
        {
            lookupTable = new Dictionary<Characters, List<Dialogs>>();

            foreach (var eventList in eventsList)
            {
                foreach (var eventDialog in eventList.eventDialogs)
                {
                    if (!lookupTable.ContainsKey(eventDialog.character))
                    {
                        lookupTable[eventDialog.character] = new List<Dialogs>();
                    }
                    lookupTable[eventDialog.character].Add(new Dialogs { dialog = eventDialog.dialog });
                }
            }
        }
    }
}
