using System.Collections;
using System.Collections.Generic;
using TMPro;
using TwelveG.AudioController;
using TwelveG.GameController;
using TwelveG.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace TwelveG.DialogsController
{
    public struct DialogForChannel
    {
        public int channelIndex;
        public DialogSO dialogSO;
    }

    public class DialogManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject dialogPanel;
        [SerializeField] private GameObject buttonPrefab;

        [SerializeField] private Canvas dialogCanvas;
        [SerializeField] private TextMeshProUGUI dialogCanvasText;
        [SerializeField] private TextMeshProUGUI characterNameCanvasText;

        [Header("Audio")]
        [SerializeField] private AudioClip WTBeepClip;
        [SerializeField, Range(0f, 1f)] private float WTBeepVolume = 0.7f;
        [SerializeField, Range(0f, 3f)] float charactersPitch = 1f;

        private AudioSource dialogSource;
        private AudioSourceState audioSourceState;
        private DialogSO currentDialog;
        private List<DialogOptions> currentOptions;

        private void OnEnable()
        {
            UpdateCanvasTextOnLanguageChanged();
        }

        void Start()
        {
            dialogCanvas.enabled = false;
        }

        public void HideDialog()
        {
            dialogCanvas.enabled = false;
        }

        public void UpdateCanvasTextOnLanguageChanged()
        {
            if (currentDialog == null) return;

            string textToShow = Utils.TextFunctions.RetrieveDialogText(
                LocalizationManager.Instance.GetCurrentLanguageCode(),
                currentDialog
            );

            dialogCanvasText.text = textToShow;
        }

        public bool DialogIsShowing()
        {
            return dialogCanvasText.gameObject.activeSelf;
        }

        public IEnumerator ShowDialogCoroutine(string charName, string dialog, float audioClipDuration)
        {
            characterNameCanvasText.text = charName + ":";
            dialogCanvasText.text = dialog;

            dialogCanvas.enabled = true;
            dialogPanel.SetActive(true);

            yield return new WaitForSeconds(audioClipDuration);

            dialogPanel.SetActive(false);
        }

        private IEnumerator StartDialogCoroutine(DialogSO currentDialog)
        {
            if(dialogSource == null)
            {
                dialogSource = AudioManager.Instance.PoolsHandler.ReturnFreeAudioSource(AudioPoolType.Dialogs);
                audioSourceState = dialogSource.GetSnapshot();
            }

            float timeBeforeShowing = currentDialog.timeBeforeShowing;

            currentOptions = currentDialog.dialogOptions;

            yield return new WaitForSeconds(timeBeforeShowing);

            if (currentDialog.characterName == CharacterName.Simon || currentDialog.characterName == CharacterName.Mica)
            {
                if (!currentDialog.isSelfDialog)
                {
                    GameEvents.Common.onDialogNodeRunning.Raise(this, true);
                }
            }

            string textToShow = Utils.TextFunctions.RetrieveDialogText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    currentDialog
                );

            if (currentDialog.characterName == CharacterName.Simon && !currentDialog.isSelfDialog)
            {
                AudioManager.Instance.AudioDialogsHandler.PlayBeepSound(dialogSource, WTBeepClip, WTBeepVolume);
            }

            float dialogTime = currentDialog.spanishDialogClip != null ? currentDialog.spanishDialogClip.length : Utils.TextFunctions.CalculateTextDisplayDuration(textToShow);

            if (currentDialog.spanishDialogClip != null)
            {
                if (currentDialog.characterName == CharacterName.Simon || currentDialog.characterName == CharacterName.Mica)
                {
                    AudioManager.Instance.AudioDialogsHandler.PlayDialogClip(dialogSource, currentDialog.spanishDialogClip, charactersPitch, 1f);
                }
                else
                {
                    AudioManager.Instance.AudioDialogsHandler.PlayDialogClip(dialogSource, currentDialog.spanishDialogClip, charactersPitch, 1f);
                }
            }

            if (currentDialog.startingEvent != null)
            {
                currentDialog.startingEvent.Raise(this, null);
            }

            CharacterName charName = currentDialog.characterName;
            string charNameString;

            if (charName == CharacterName.Simon || charName == CharacterName.Mica)
            {
                charNameString = currentDialog.characterName.ToString();
            }
            else
            {
                charNameString = " ... ";
            }

            yield return ShowDialogCoroutine(charNameString, textToShow, dialogTime);

            if (currentDialog.characterName == CharacterName.Simon && !currentDialog.isSelfDialog)
            {
                AudioManager.Instance.AudioDialogsHandler.PlayBeepSound(dialogSource, WTBeepClip, WTBeepVolume);
            }

            if (currentDialog.endingEvent != null)
            {
                currentDialog.endingEvent.Raise(this, null);
            }

            if (currentDialog.characterName == CharacterName.Simon || currentDialog.characterName == CharacterName.Mica)
            {
                if (!currentDialog.isSelfDialog)
                {
                    GameEvents.Common.onDialogNodeRunning.Raise(this, false);
                }
            }

            // Verifica si el diálogo tiene opciones
            if (currentOptions != null && currentOptions.Count > 0)
            {
                ShowOptions();
            }
            else
            {
                NextDialogOrEnd();
            }
        }

        public void StartDialog(Component sender, object data)
        {
            currentDialog = (DialogSO)data;
            if (currentDialog != null)
            {
                StartCoroutine(StartDialogCoroutine(currentDialog));
            }
        }

        void ShowOptions()
        {
            optionsPanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Elimina cualquier botón existente
            foreach (Transform child in optionsPanel.transform)
            {
                Destroy(child.gameObject);
            }

            // Instancia botones basados en la cantidad de opciones
            for (int i = 0; i < currentOptions.Count; i++)
            {
                GameObject optionButton = Instantiate(buttonPrefab, optionsPanel.transform);

                optionButton.GetComponentInChildren<TextMeshProUGUI>().text = Utils.TextFunctions.RetrieveDialogOptions(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    currentOptions[i]
                );

                int optionIndex = i;
                Button btnComponent = optionButton.GetComponent<Button>();
                btnComponent.onClick.RemoveAllListeners();
                btnComponent.onClick.AddListener(() => OnOptionSelected(optionIndex));
            }
        }

        void NextDialogOrEnd()
        {
            if (currentDialog.nextDialog != null)
            {
                StartDialog(this, currentDialog.nextDialog);
            }
            else
            {
                // Fin del diálogo
                // Debug.Log("conversationHasEnded Raised from DialogManager");
                GameEvents.Common.onConversationHasEnded.Raise(this, null);
                dialogCanvas.enabled = false;
                AudioUtils.StopAndRestoreAudioSource(dialogSource, audioSourceState);
            }
        }

        public void OnOptionSelected(int optionIndex)
        {
            DialogSO nextDialog = currentOptions[optionIndex].nextDialog;
            if (nextDialog != null)
            {
                HideOptions();
                StartDialog(this, nextDialog);
            }
            else
            {
                HideOptions();
            }
        }

        private void HideOptions()
        {
            optionsPanel.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}