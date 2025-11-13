using System.Collections.Generic;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    [CreateAssetMenu(fileName = "WalkieTalkie Data - ", menuName = "SO's/WalkieTalkie Data", order = 0)]
    public class WalkieTalkieDataSO : ScriptableObject
    {
        public EventsEnum eventName;

        [SerializeField]
        private List<FrequencyDataStructure> frequencyData = new List<FrequencyDataStructure>(4)
        {
            new FrequencyDataStructure("Canal 1"),
            new FrequencyDataStructure("Canal 2"),
            new FrequencyDataStructure("Canal 3 (Mica)"),
            new FrequencyDataStructure("Canal 4")
        };

        public List<FrequencyDataStructure> FrequencyData => frequencyData;

        [System.Serializable]
        public class FrequencyDataStructure
        {
            [HideInInspector] public string frequencyName;
            public bool hasSpecialAudio;
            public List<AudioClip> clips;

            public FrequencyDataStructure(string name)
            {
                frequencyName = name;
                hasSpecialAudio = false;
                clips = new List<AudioClip>();
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            // Asegura que siempre haya 4 entradas con nombres fijos
            if (frequencyData == null || frequencyData.Count != 4)
            {
                frequencyData = new List<FrequencyDataStructure>
                {
                    new FrequencyDataStructure("Canal 1"),
                    new FrequencyDataStructure("Canal 2"),
                    new FrequencyDataStructure("Canal 3 (Mica)"),
                    new FrequencyDataStructure("Canal 4")
                };
            }
        }
#endif
    }
}
