namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "ObservationTextSO", menuName = "SO's/ObservationTextSO", order = 0)]
    public class ObservationTextSO : ScriptableObject
    {
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