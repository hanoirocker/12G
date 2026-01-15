using UnityEngine;

namespace TwelveG.UIController
{
  [CreateAssetMenu(fileName = "UIFormattingDataSO", menuName = "SO's/Data Structures/UIFormattingDataSO", order = 0)]
  public class UIFormattingDataSO : ScriptableObject
  {
    [Header("General Colors")]
    [Space(10)]
    public Color controlsHighlightColor;
    public Color alertColor;
    public Color buttonHighlightColor;
    public Color playerInteractionColor;

    public Color GetColorByType(UIFormatingType type)
    {
      switch (type)
      {
        case UIFormatingType.ControlsSpecificText:
          return controlsHighlightColor;

        case UIFormatingType.AlertColorText:
          return alertColor;

        case UIFormatingType.ButtonHighlightColorText:
          return buttonHighlightColor;

        default:
          return Color.white;
      }
    }
  }
}