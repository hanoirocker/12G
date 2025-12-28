namespace TwelveG.Localization
{
  using System.Collections.Generic;
  using UnityEngine;

  [CreateAssetMenu(fileName = "UIOptionTextSO", menuName = "SO's/UI Texts/UIOptionTextSO", order = 0)]
  public class UIOptionsTextSO : ScriptableObject
  {
    public List<UIMenuTextStructure> uImenuTextStructure;

    [System.Serializable]
    public class UIMenuTextStructure
    {
      public LanguagesEnum language;
      public string value;
    }
  }
}