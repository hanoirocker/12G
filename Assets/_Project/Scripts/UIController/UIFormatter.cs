using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TwelveG.UIController
{
  public enum UIFormatingType
  {
    None,
    ControlsDefaultText,
    ControlsSpecificText,
    AlertColorText,
    ButtonText,
    PlayerInteractionText,
    PlayerContemplationText,
    PlayerObservationText,
    DialogContentText,
    DialogCharacterText,
  }

  public class UIFormatter : MonoBehaviour
  {
    [Header("Formatting Data")]
    [SerializeField] private UIFormattingDataSO formatData;

    private void Awake()
    {
      if (formatData == null)
      {
        Debug.LogWarning("UIFormattingDataSO no asignada al UIFormatter.");
      }
    }

    public string UpdateTextColors(string inputText, UIFormatingType formatType, GameObject contextObject)
    {
      if (formatData == null) return inputText;

      switch (formatType)
      {
        case UIFormatingType.ControlsSpecificText:
          return FormatControlsText(inputText, formatData.GetColorByType(formatType));

        case UIFormatingType.AlertColorText:
          return FormatAlertText(inputText, formatData.GetColorByType(formatType));

        case UIFormatingType.PlayerInteractionText:
          return FormatControlsText(inputText, formatData.GetColorByType(formatType));

        case UIFormatingType.ButtonText:
          ConfigureParentButtonColor(contextObject, formatData.GetColorByType(formatType));
          return inputText;

        case UIFormatingType.None:
        default:
          return inputText;
      }
    }

    public void AssignFontByType(UIFormatingType formatType, TextMeshProUGUI textComponent)
    {
      if (formatData == null) return;

      TMP_FontAsset fontToAssign = formatData.GetFontByType(formatType);
      textComponent.font = fontToAssign;
    }

    private string FormatControlsText(string inputText, Color colorToUse)
    {
      if (string.IsNullOrEmpty(inputText)) return inputText;

      string hexColor = "#" + UnityEngine.ColorUtility.ToHtmlStringRGB(colorToUse);
      string pattern = @"~|\[.*?\]";

      return Regex.Replace(inputText, pattern, match =>
      {
        return $"<color={hexColor}>{match.Value}</color>";
      });
    }

    private string FormatAlertText(string inputText, Color colorToUse)
    {
      if (string.IsNullOrEmpty(inputText)) return inputText;

      string hexColor = "#" + UnityEngine.ColorUtility.ToHtmlStringRGB(colorToUse);
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