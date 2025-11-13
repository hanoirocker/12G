using System;
using System.Collections.Generic;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.Localization
{
    [CreateAssetMenu(fileName = "ContextualContemplation", menuName = "SO's/ContextualContemplationSO")]
    public class ContextualContemplationSO : ScriptableObject
    {
        [Serializable]
        public class EventRangeEntry
        {
            public SceneEnum sceneEnum;        // 2=Afternoon, 3=Evening, 4=Night
            public int minEventIndex;          // desde este evento inclusive
            public int maxEventIndex;          // hasta este evento inclusive
            public ContemplationTextSO texts;
        }

        public List<EventRangeEntry> entries;

        public ContemplationTextSO GetSOForContext(SceneEnum sceneEnum, int eventIndex)
        {
            foreach (var entry in entries)
            {
                if (entry.sceneEnum == sceneEnum &&
                    eventIndex >= entry.minEventIndex &&
                    eventIndex <= entry.maxEventIndex)
                {
                    return entry.texts;
                }
            }
            return null;
        }
    }
}


