using System.Text.RegularExpressions;
using TMPro;
using TwelveG.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace TwelveG.UIController
{
  public class UpdateTextHandler : MonoBehaviour
  {
    [Header("Options")]
    [Space]
    [SerializeField] private bool isTMPSingleText;
    [SerializeField] private UIOptionsTextSO uIOptionTextSO;
    [SerializeField] private bool isTMPDropDownList;
    [SerializeField] private TextListSO textListSO;
    [SerializeField] private bool textDependsOnEvents = false;

    [Header("Formatting Settings")]
    [Space]
    [Tooltip("Define el comportamiento visual del texto según su tipo.")]
    [SerializeField] private UIFormatingType uITextType = UIFormatingType.None;

    [Header("Text SO's")]
    [Space]
    public UIOptionsTextSO eventDrivenTextSO;

    private void OnEnable()
    {
      UpdateText(LocalizationManager.Instance?.GetCurrentLanguageCode());
    }

    public void UpdateText(string languageCode)
    {
      if (textListSO != null && isTMPDropDownList)
      {
        UpdateListTextSO(languageCode);
      }

      if (uIOptionTextSO != null && isTMPSingleText)
      {
        UpdateSingleTextSO(languageCode, uIOptionTextSO);
      }

      if (textDependsOnEvents && eventDrivenTextSO != null)
      {
        UpdateSingleTextSO(languageCode, eventDrivenTextSO);
      }
    }

    public void LoadEventDrivenTextSO(Component sender, object data)
    {
      eventDrivenTextSO = data as UIOptionsTextSO;
    }

    private void UpdateSingleTextSO(string languageCode, UIOptionsTextSO optionTextSO)
    {
      var languageStructure = optionTextSO.uImenuTextStructure
        .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

      if (languageStructure != null)
      {
        string finalValue = ProcessTextByType(languageStructure.value);
        gameObject.GetComponent<TextMeshProUGUI>().text = finalValue;
      }
    }

    private void UpdateListTextSO(string languageCode)
    {
      var languageStructure = textListSO.textListSOStructures
        .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

      if (languageStructure != null)
      {
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();

        for (int i = 0; i < dropdown.options.Count && i < languageStructure.values.Count; i++)
        {
          string textValue = ProcessTextByType(languageStructure.values[i]);
          dropdown.options[i].text = textValue;
        }

        dropdown.RefreshShownValue();
      }
    }

    private string ProcessTextByType(string inputText)
    {
      switch (uITextType)
      {
        case UIFormatingType.ControlsSpecificText:
          return FormatControlsText(inputText);

        case UIFormatingType.AlertColorText:
          return FormatAlertText(inputText);

        case UIFormatingType.ButtonHighlightColorText:
          ConfigureParentButtonColor();
          return inputText;

        case UIFormatingType.None:
        default:
          return inputText;
      }
    }

    // Lógica para Controles
    private string FormatControlsText(string inputText)
    {
      if (string.IsNullOrEmpty(inputText)) return inputText;

      string hexColor = "#" + ColorUtility.ToHtmlStringRGB(UIColors.CONTROLS_ORANGE);
      string pattern = @"~|\[.*?\]";

      return Regex.Replace(inputText, pattern, match =>
      {
        return $"<color={hexColor}>{match.Value}</color>";
      });
    }

    // Lógica para ItemAlert (Todo el texto coloreado)
    private string FormatAlertText(string inputText)
    {
      if (string.IsNullOrEmpty(inputText)) return inputText;

      string hexColor = "#" + ColorUtility.ToHtmlStringRGB(UIColors.CONTROLS_ORANGE);
      return $"<color={hexColor}>{inputText}</color>";
    }

    // Lógica para Botón (Modificar componente en el padre)
    private void ConfigureParentButtonColor()
    {
      Button parentBtn = GetComponentInParent<Button>();

      if (parentBtn != null)
      {
        ColorBlock colors = parentBtn.colors;
        colors.highlightedColor = UIColors.CONTROLS_ORANGE;
        parentBtn.colors = colors;
      }
    }
  }
}