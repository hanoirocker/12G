using System.Collections.Generic;
using UnityEngine;

namespace TwelveG.Localization
{
    [CreateAssetMenu(fileName = "ExaminationText", menuName = "SO's/In Game Texts/ExaminationTextSO", order = 0)]
    public class ExaminationTextSO : ScriptableObject
    {
        public List<ExaminationTextStructure> examinationTextStructure;

        [System.Serializable]
        public class ExaminationTextStructure
        {
            public LanguagesEnum language;
            [TextArea(6, 10)]
            public string examinationText;
        }
    }
}