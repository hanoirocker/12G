namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "ObservationTextSO", menuName = "SO's/In Game Texts/ObservationTextSO", order = 0)]
    public class ObservationTextSO : ScriptableObject
    {
        public bool isEventText = false;
        public List<ObservationsStructure> observationTextsStructure;

        [System.Serializable]
        public class ObservationsStructure
        {
            public LanguagesEnum language;
            [TextArea(5, 50)]
            public string observationText;
        }
    }
}