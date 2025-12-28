namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "NarrativeTexts", menuName = "SO's/Event Texts/NarrativeTextsSO", order = 0)]
    public class NarrativeTextSO : ScriptableObject
    {
        public List<NarrativeTextsStructure> narrativeTextsStructure;

        [System.Serializable]
        public class NarrativeTextsStructure
        {
            public LanguagesEnum language;
            public string title;
            public string content;
            public string phrase;
        }
    }
}