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

    private UIOptionsTextSO eventDrivenTextSO;

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
      gameObject.GetComponent<TextMeshProUGUI>().text = languageStructure.value;
    }

    private void UpdateListTextSO(string languageCode)
    {
      var languageStructure = textListSO.textListSOStructures
        .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));
      TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();

      for (int i = 0; i < dropdown.options.Count && i < languageStructure.values.Count; i++)
      {
        dropdown.options[i].text = languageStructure.values[i];
      }
      dropdown.RefreshShownValue();
    }
  }
}