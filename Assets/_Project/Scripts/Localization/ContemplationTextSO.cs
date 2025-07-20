namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ContemplationTexts", menuName = "SO's/ContemplationTextsSO", order = 0)]
    public class ContemplationTextSO : ScriptableObject
    {
        public List<ContemplationTextsStructure> contemplationTextsStructure;

        [System.Serializable]
        public class ContemplationTextsStructure
        {
            public LanguagesEnum language;
            public List<string> contemplationTexts;
        }
    }
}