namespace TwelveG.DialogsController
{
    using System.Collections.Generic;
    using TwelveG.Localization;
    using UnityEngine;

    public enum CharacterName
    {
        Simon,
        Mica,
        Cops
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
        [Range(0f, 100f)]
        public float timeBeforeShowing = 0f;
        public GameEventSO startingEvent = null;
        public GameEventSO endingEvent = null;
        public DialogSO nextDialog;
        public List<DialogOptions> dialogOptions;
    }
}