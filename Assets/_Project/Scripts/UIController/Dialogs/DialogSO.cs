namespace TwelveG.DialogsController
{
  using System.Collections.Generic;
  using TwelveG.Localization;
    using UnityEngine;

    public enum CharacterName
    {
        Simon,
        Mica
    }

    [System.Serializable]
    public class DialogTextStructure
    {
        public LanguagesEnum language;
        [TextArea(3, 10)]
        public string dialogText;
    }

    [CreateAssetMenu(fileName = "NewDialogSO", menuName = "DialogSystem/Test1/Dialog")]
    public class DialogSO : ScriptableObject
    {
        public CharacterName characterName;

        public bool isSelfDialog = false;

        public List<DialogTextStructure> dialogTextStructure;
        public AudioClip spanishDialogClip;

        public DialogSO nextDialog;

        [Range(0f, 12f)]
        public float timeBeforeShowing = 0f;

        public List<DialogOptions> dialogOptions;
    }
}