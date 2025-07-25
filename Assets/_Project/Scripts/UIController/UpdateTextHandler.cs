namespace TwelveG.UIController
{
  using TMPro;
  using TwelveG.Localization;
  using UnityEngine;

  public class UpdateTextHandler : MonoBehaviour
  {
    public UIOptionsTextSO uIOptionTextSO;

    public void UpdateText(string languageCode)
    {
      if (uIOptionTextSO == null)
      {
        Debug.LogError($"[UpdateTextHandler]: null uIOptionTextSO in object {gameObject.name}");
        GetComponent<TextMeshProUGUI>().text = "ERROR";
        return;
      }

        var languageStructure = uIOptionTextSO.uImenuTextStructure
                  .Find(texts => texts.language.ToString().Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));
      GetComponent<TextMeshProUGUI>().text = languageStructure.value;
    }
  }
}