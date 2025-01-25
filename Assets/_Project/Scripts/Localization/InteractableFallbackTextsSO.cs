namespace TwelveG.Localization
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "InteractableFallbackTexts", menuName = "SO's/InteractableFallbackTextsSO", order = 0)]
    public class InteractableFallbackTextsSO : ScriptableObject
    {
        public List<InteractableFallbackStructure> interactableFallbackStructures;

        [System.Serializable]
        public class InteractableFallbackStructure
        {
            public LanguagesEnum language;
            public List<string> interactableFallbacksTexts;
        }
    }
}