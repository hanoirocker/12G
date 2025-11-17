using System.Collections;
using System.Collections.Generic;
using TMPro;
using TwelveG.AudioController;
using TwelveG.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace TwelveG.DialogsController
{
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
        [SerializeField] AudioClip WTBeepClip;

        [Header("Game Event SO's")]
        public GameEventSO conversationHasEnded;

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
            float timeBeforeShowing = currentDialog.timeBeforeShowing;

            currentOptions = currentDialog.dialogOptions;

            yield return new WaitForSeconds(timeBeforeShowing);

            string textToShow = Utils.TextFunctions.RetrieveDialogText(
                    LocalizationManager.Instance.GetCurrentLanguageCode(),
                    currentDialog
                );

            if (currentDialog.characterName == CharacterName.Simon && !currentDialog.isSelfDialog)
            {
                AudioManager.Instance.AudioDialogsHandler.PlayDialogClip(WTBeepClip);
            }

            float dialogTime = currentDialog.spanishDialogClip != null ? currentDialog.spanishDialogClip.length : Utils.TextFunctions.CalculateTextDisplayDuration(textToShow);

            if (currentDialog.spanishDialogClip != null)
            {
                AudioManager.Instance.AudioDialogsHandler.PlayDialogClip(currentDialog.spanishDialogClip);
            }

            yield return ShowDialogCoroutine(currentDialog.characterName.ToString(), textToShow, dialogTime);

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
                Debug.Log("conversationHasEnded Raised from DialogManager");
                conversationHasEnded.Raise(this, null);
                dialogCanvas.enabled = false;
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