using TMPro;
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

    [Header("Fonts")]
    [Space(10)]
    public TMP_FontAsset buttonsTextFont;
    public TMP_FontAsset controlCanvasTextFont;
    public TMP_FontAsset interactionTextFont;
    public TMP_FontAsset contemplationTextFont;
    public TMP_FontAsset observationTextFont;
    public TMP_FontAsset dialogsContentFont;
    public TMP_FontAsset dialogsCharacterNameFont;
    public TMP_FontAsset defaultFont;

    public Color GetColorByType(UIFormatingType type)
    {
      switch (type)
      {
        case UIFormatingType.ControlsSpecificText:
          return controlsHighlightColor;

        case UIFormatingType.AlertColorText:
          return alertColor;

        case UIFormatingType.ButtonText:
          return buttonHighlightColor;

        case UIFormatingType.PlayerInteractionText:
          return playerInteractionColor;

        default:
          return Color.white;
      }
    }

    public TMP_FontAsset GetFontByType(UIFormatingType type)
    {
      TMP_FontAsset fontToReturn;

      switch (type)
      {
        case UIFormatingType.ControlsSpecificText:
          fontToReturn = controlCanvasTextFont;
          break;
        case UIFormatingType.AlertColorText:
          fontToReturn = interactionTextFont;
          break;
        case UIFormatingType.ButtonText:
          fontToReturn = buttonsTextFont;
          break;
        case UIFormatingType.PlayerInteractionText:
          fontToReturn = interactionTextFont;
          break;
        case UIFormatingType.PlayerContemplationText:
          fontToReturn = contemplationTextFont;
          break;
        case UIFormatingType.PlayerObservationText:
          fontToReturn = observationTextFont;
          break;
        default:
          fontToReturn = defaultFont;
          break;
      }

      return fontToReturn != null ? fontToReturn : defaultFont;
    }
  }
}