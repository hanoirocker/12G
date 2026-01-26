using System.Collections.Generic;
using TwelveG.DialogsController;
using TwelveG.GameController;
using UnityEngine;

namespace TwelveG.InteractableObjects
{
    [CreateAssetMenu(fileName = "WalkieTalkie Data - ", menuName = "SO's/Data Structures/WalkieTalkie Data", order = 0)]
    public class WalkieTalkieDataSO : ScriptableObject
    {
        public EventsEnum eventName;

        [SerializeField]
        private List<FrequencyDataStructure> frequencyData = new List<FrequencyDataStructure>(4)
        {
            new FrequencyDataStructure("Canal 1 (Aguirre)"),
            new FrequencyDataStructure("Canal 2 (Noticias)"),
            new FrequencyDataStructure("Canal 3 (Mica)"),
            new FrequencyDataStructure("Canal 4 (Policía)")
        };

        public List<FrequencyDataStructure> FrequencyData => frequencyData;

        [System.Serializable]
        public class FrequencyDataStructure
        {
            [HideInInspector] public string frequencyName;

            [Header("Atmosphere")]
            [Tooltip("El ruido de fondo/estática único de este canal.")]
            public AudioClip staticSignalClip;

            [Header("Lore Event (Optional)")]
            [Tooltip("Audio especial (Historia, interferencia, bebé). Se reproduce UNA vez.")]
            public AudioClip loreEventClip;

            [Tooltip("Diálogo opcional de Simon al terminar el clip de Lore.")]
            public DialogSO reactionDialog;

            public FrequencyDataStructure(string name)
            {
                frequencyName = name;
                staticSignalClip = null;
                loreEventClip = null;
                reactionDialog = null;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (frequencyData == null || frequencyData.Count != 4)
            {
                frequencyData = new List<FrequencyDataStructure>
                {
                    new FrequencyDataStructure("Canal 1 (Aguirre)"),
                    new FrequencyDataStructure("Canal 2 (Noticias)"),
                    new FrequencyDataStructure("Canal 3 (Mica)"),
                    new FrequencyDataStructure("Canal 4 (Policía)")
                };
            }
        }
#endif
    }
}