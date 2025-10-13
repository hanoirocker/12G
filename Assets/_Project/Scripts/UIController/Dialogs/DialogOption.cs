namespace TwelveG.DialogsController
{
  using System.Collections.Generic;
  using TwelveG.Localization;
  using UnityEngine;

    [System.Serializable]
    public class DialogOptionsStructure
    {
        public LanguagesEnum language;
        [TextArea(2, 10)]
        public string optionText;
    }

    [System.Serializable]
    public class DialogOptions
    {
        public List<DialogOptionsStructure> dialogOptionsStructures;
        public DialogSO nextDialog;
    }
}