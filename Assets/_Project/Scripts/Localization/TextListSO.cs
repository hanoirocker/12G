namespace TwelveG.Localization
{
  using System.Collections.Generic;
  using UnityEngine;

  [CreateAssetMenu(fileName = "TextListSO", menuName = "SO's/UI Texts/TextListSO", order = 0)]
  public class TextListSO : ScriptableObject
  {
    public List<TextListSOStructure> textListSOStructures;

    [System.Serializable]
    public class TextListSOStructure
    {
      public LanguagesEnum language;
      public List<string> values;
    }
  }
}