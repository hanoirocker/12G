namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "InteractionText", menuName = "SO's/In Game Texts/InteractionTextSO", order = 0)]
    public class InteractionTextSO : ScriptableObject
    {
        public List<InteractionTextsStructure> interactionTextsStructure;

        [System.Serializable]
        public class InteractionTextsStructure
        {
            public LanguagesEnum language;
            public string interactionText;
        }
    }
}