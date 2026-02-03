using TMPro;
using TwelveG.Localization;
using UnityEngine;

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

    [Tooltip("Habilita el procesamiento de color y estilo del texto.")]
    [SerializeField] private bool formatText = false; // Nueva variable

    [Header("Formatting Settings")]
    [Space]
    [Tooltip("Define el comportamiento visual del texto según su tipo.")]
    public UIFormatingType uITextType = UIFormatingType.None;

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
        string finalValue = languageStructure.value;

        if (formatText)
        {
          finalValue = UIManager.Instance.UIFormatter.UpdateTextColors(finalValue, uITextType, this.gameObject);
        }

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
          string textValue = languageStructure.values[i];

          if (formatText)
          {
            textValue = UIManager.Instance.UIFormatter.UpdateTextColors(textValue, uITextType, this.gameObject);
          }

          dropdown.options[i].text = textValue;
        }

        dropdown.RefreshShownValue();
      }
    }

    public void ResetEventDrivenTextSO()
    {
      eventDrivenTextSO = null;
    }

    private void OnValidate()
    {
      if (Application.isPlaying && isActiveAndEnabled)
      {
        UpdateText(LocalizationManager.Instance?.GetCurrentLanguageCode());
      }
    }
  }
}