namespace TwelveG.UIController
{
  using TMPro;
  using TwelveG.Localization;
  using UnityEngine;

  public class UpdateTextHandler : MonoBehaviour
  {
    [Header("Options")]
    [SerializeField] private bool isTMPSingleText;
    [SerializeField] private bool isTMPDropDownList;

    [Header("Singular Text SO's")]
    [SerializeField] private UIOptionsTextSO uIOptionTextSO;

    [Header("List Text SO's")]
    [SerializeField] private TextListSO textListSO;

    private void OnEnable()
    {
      UpdateText(LocalizationManager.Instance.GetCurrentLanguageCode());
    }

    public void UpdateText(string languageCode)
    {
      if (textListSO != null && isTMPDropDownList)
      {
        UpdateListTextSO(languageCode);
      }

      if (uIOptionTextSO != null && isTMPSingleText)
      {
        UpdateSingleTextSO(languageCode);
      }
    }

    private void UpdateSingleTextSO(string languageCode)
    {
      var languageStructure = uIOptionTextSO.uImenuTextStructure
        .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));
      // print("Texto del componente: " + GetComponent<TextMeshProUGUI>().text);
      // print("Texto de la estructure: " + languageStructure.value);
      GetComponent<TextMeshProUGUI>().text = languageStructure.value;
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
    }
  }
}