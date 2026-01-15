using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace TwelveG.UIController
{
  public enum UIFormatingType
  {
    None,
    ControlsSpecificText,
    AlertColorText,
    ButtonHighlightColorText,
    PlayerInteractionText,
  }

  public  class UIFormatter: MonoBehaviour
  {
    [SerializeField] private UIFormattingDataSO palette;

    public string FormatTextByType(string inputText, UIFormatingType formatType, GameObject contextObject)
    {
      // Seguridad: Si no hay paleta, devolvemos el texto sin formato para evitar errores
      if (palette == null) return inputText;

      switch (formatType)
      {
        case UIFormatingType.ControlsSpecificText:
          return FormatInteractionInputText(inputText, palette.GetColorByType(formatType));

        case UIFormatingType.AlertColorText:
          return FormatAlertText(inputText, palette.GetColorByType(formatType));

        case UIFormatingType.PlayerInteractionText:
          return FormatInteractionInputText(inputText, palette.playerInteractionColor);

        case UIFormatingType.ButtonHighlightColorText:
          ConfigureParentButtonColor(contextObject, palette.GetColorByType(formatType));
          return inputText;

        case UIFormatingType.None:
        default:
          return inputText;
      }
    }

    private string FormatInteractionInputText(string inputText, Color colorToUse)
    {
      if (string.IsNullOrEmpty(inputText)) return inputText;

      string hexColor = "#" + ColorUtility.ToHtmlStringRGB(colorToUse);
      string pattern = @"~|\[.*?\]";

      return Regex.Replace(inputText, pattern, match =>
      {
        return $"<color={hexColor}>{match.Value}</color>";
      });
    }

    private string FormatAlertText(string inputText, Color colorToUse)
    {
      if (string.IsNullOrEmpty(inputText)) return inputText;

      string hexColor = "#" + ColorUtility.ToHtmlStringRGB(colorToUse);
      return $"<color={hexColor}>{inputText}</color>";
    }

    private void ConfigureParentButtonColor(GameObject contextObject, Color colorToUse)
    {
      if (contextObject == null) return;

      Button parentBtn = contextObject.GetComponentInParent<Button>();

      if (parentBtn != null)
      {
        ColorBlock colors = parentBtn.colors;
        colors.highlightedColor = colorToUse;
        parentBtn.colors = colors;
      }
    }
  }
}