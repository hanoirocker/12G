namespace TwelveG.DialogsController
{
    using UnityEngine;

    public enum CharacterName
    {
        Simon,
        Mica
    }

    [CreateAssetMenu(fileName = "NewDialogSO", menuName = "DialogSystem/Test1/Dialog")]
    public class DialogSO : ScriptableObject
    {
        public CharacterName characterName;

        [TextArea(3, 10)]
        public string dialogText;
        public AudioClip spanishDialogClip;

        public DialogSO nextDialog;

        [Range(0f, 12f)]
        public float timeBeforeShowing = 0f;

        public DialogOption[] options;
    }
}