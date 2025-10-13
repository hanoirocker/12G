namespace TwelveG.DialogsController
{
    using UnityEngine;

    [System.Serializable]
    public class DialogOption
    {
        [TextArea(3, 10)]
        public string optionText;

        public DialogSO nextDialog;
    }
}